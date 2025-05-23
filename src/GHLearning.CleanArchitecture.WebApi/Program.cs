using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using CorrelationId;
using GHLearning.CleanArchitecture.SharedKernel;
using GHLearning.CleanArchitecture.WebApi.Filters;
using GHLearning.CleanArchitecture.WebApi.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
	.AddRouting(options => options.LowercaseUrls = true)
	.AddControllers(options =>
	{
		options.Filters.Add(new ProducesAttribute(MediaTypeNames.Application.Json));
		options.Filters.Add(new ConsumesAttribute(MediaTypeNames.Application.Json));
		options.Filters.Add<FormatResponseExceptionFilterAttribute>();
		options.Filters.Add<FormatResponseResultFilterAttribute>();
	})
	.AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
	var securityScheme = new OpenApiSecurityScheme
	{
		Name = "JWT Authentication",
		Description = "Enter your JWT token in this field",
		In = ParameterLocation.Header,
		Type = SecuritySchemeType.Http,
		Scheme = JwtBearerDefaults.AuthenticationScheme,
		BearerFormat = "JWT"
	};

	options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, securityScheme);

	var securityRequirement = new OpenApiSecurityRequirement
	{
		{
			new OpenApiSecurityScheme
			{
				Reference = new OpenApiReference
				{
					Type = ReferenceType.SecurityScheme,
					Id = JwtBearerDefaults.AuthenticationScheme
				}
			},
			[]
		}
	};
	options.AddSecurityRequirement(securityRequirement);

	options.EnableAnnotations();

	options.CustomSchemaIds(type =>
	{
		var pattern = @"^GHLearning\.\w+\.(Core|WebApi(\.Controllers)?)\.";
		var namespaceName = Regex.Replace(type.Namespace!, pattern, string.Empty);

		var name = type.Name;
		if (type.IsGenericType)
		{
			name = $"{type.Name.Split('`')[0]}<{string.Join(",", type.GetGenericArguments().Select(t => t.Name))}>";
		}

		return string.Concat(namespaceName, ".", name);
	});
});

// Add services to the container.
builder.Services
	.AddApplication()
	.AddInfrastructure(
	(services, options) => options.UseMySql(
		connectionString: builder.Configuration.GetConnectionString("MySql"),
		serverVersion: ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("MySql"))),
	(options, sp) =>
	{
		var configuration = sp.GetRequiredService<IConfiguration>();
		configuration.GetSection("Jwt").Bind(options);
	});

//Learn more about configuring HttpLogging at https://learn.microsoft.com/en-us/aspnet/core/fundamentals/http-logging/?view=aspnetcore-8.0
builder.Services.AddHttpLogging(logging =>
{
	logging.LoggingFields = HttpLoggingFields.All;
	logging.RequestHeaders.Add(CorrelationIdOptions.DefaultHeader);
	logging.ResponseHeaders.Add(CorrelationIdOptions.DefaultHeader);
	logging.RequestHeaders.Add(TraceHeaders.TraceParent);
	logging.ResponseHeaders.Add(TraceHeaders.TraceParent);
	logging.RequestHeaders.Add(TraceHeaders.TraceId);
	logging.ResponseHeaders.Add(TraceHeaders.TraceId);
	logging.RequestHeaders.Add(TraceHeaders.ParentId);
	logging.ResponseHeaders.Add(TraceHeaders.ParentId);
	logging.RequestHeaders.Add(TraceHeaders.TraceFlag);
	logging.ResponseHeaders.Add(TraceHeaders.TraceFlag);
	logging.RequestBodyLogLimit = 4096;
	logging.ResponseBodyLogLimit = 4096;
	logging.CombineLogs = true;
});

//AddOpenTelemetry
builder.Services.AddOpenTelemetry()
	.ConfigureResource(resource => resource
	.AddService(
		serviceName: builder.Configuration["ServiceName"]!.ToLower(),
		serviceNamespace: typeof(Program).Assembly.GetName().Name,
		serviceInstanceId: Environment.MachineName))
	.UseOtlpExporter(OtlpExportProtocol.Grpc, new Uri(builder.Configuration["OtlpEndpointUrl"]!))
	.WithMetrics(metrics => metrics
		.AddMeter("GHLearning.")
		.AddAspNetCoreInstrumentation()
		.AddRuntimeInstrumentation()
		.AddProcessInstrumentation()
		.AddPrometheusExporter())
	.WithTracing(tracing => tracing
		.AddEntityFrameworkCoreInstrumentation()
		.AddHttpClientInstrumentation()
		.AddAspNetCoreInstrumentation(options => options.Filter = (httpContext) => !httpContext.Request.Path.StartsWithSegments("/swagger", StringComparison.OrdinalIgnoreCase) &&
				!httpContext.Request.Path.StartsWithSegments("/live", StringComparison.OrdinalIgnoreCase) &&
				!httpContext.Request.Path.StartsWithSegments("/healthz", StringComparison.OrdinalIgnoreCase) &&
				!httpContext.Request.Path.StartsWithSegments("/metrics", StringComparison.OrdinalIgnoreCase) &&
				!httpContext.Request.Path.StartsWithSegments("/favicon.ico", StringComparison.OrdinalIgnoreCase) &&
				!httpContext.Request.Path.Value!.Equals("/api/events/raw", StringComparison.OrdinalIgnoreCase) &&
				!httpContext.Request.Path.Value!.EndsWith(".js", StringComparison.OrdinalIgnoreCase) &&
				!httpContext.Request.Path.StartsWithSegments("/_vs", StringComparison.OrdinalIgnoreCase)));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCorrelationId();

app.UseMiddleware<TraceMiddleware>();

app.UseMiddleware<CorrelationMiddleware>();

app.UseHttpLogging();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.UseHealthChecks("/live", new HealthCheckOptions
{
	Predicate = check => check.Tags.Contains("live"),
	ResultStatusCodes =
	{
		[HealthStatus.Healthy] = StatusCodes.Status200OK,
		[HealthStatus.Degraded] = StatusCodes.Status200OK,
		[HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
	}
});
app.UseHealthChecks("/healthz", new HealthCheckOptions
{
	Predicate = _ => true,
	ResponseWriter = (context, report) =>
	{
		context.Response.ContentType = "application/json; charset=utf-8";

		var options = new JsonWriterOptions { Indented = true };

		using var memoryStream = new MemoryStream();
		using (var jsonWriter = new Utf8JsonWriter(memoryStream, options))
		{
			jsonWriter.WriteStartObject();
			jsonWriter.WriteString("Status", report.Status.ToString());
			jsonWriter.WriteString("TotalDuration", report.TotalDuration.ToString());
			jsonWriter.WriteStartObject("Entries");

			foreach (var healthReportEntry in report.Entries)
			{
				jsonWriter.WriteStartObject(healthReportEntry.Key);
				jsonWriter.WriteString("Status", healthReportEntry.Value.Status.ToString());
				jsonWriter.WriteString("Duration", healthReportEntry.Value.Duration.ToString());
				jsonWriter.WriteString("Description", healthReportEntry.Value.Description ?? null);
				jsonWriter.WriteEndObject();
			}

			jsonWriter.WriteEndObject();
			jsonWriter.WriteEndObject();
		}

		return context.Response.WriteAsync(
			Encoding.UTF8.GetString(memoryStream.ToArray()));
	},
	ResultStatusCodes =
	{
		[HealthStatus.Healthy] = StatusCodes.Status200OK,
		[HealthStatus.Degraded] = StatusCodes.Status200OK,
		[HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
	}
});

// Prometheus 提供服務數據資料源
app.MapMetrics();
app.UseHttpMetrics();

app.Run();
