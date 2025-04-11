using System.Reflection;
using System.Text;
using CorrelationId;
using CorrelationId.DependencyInjection;
using GHLearning.CleanArchitecture.Application.Abstractions.Authentication;
using GHLearning.CleanArchitecture.Infrastructure;
using GHLearning.CleanArchitecture.Infrastructure.Authentication;
using GHLearning.CleanArchitecture.Infrastructure.Authorization;
using GHLearning.CleanArchitecture.Infrastructure.Correlations;
using GHLearning.CleanArchitecture.Infrastructure.Entities;
using GHLearning.CleanArchitecture.SharedKernel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TokenOptions = GHLearning.CleanArchitecture.Infrastructure.Authentication.TokenOptions;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddInfrastructure(
		this IServiceCollection services,
		Action<IServiceProvider, DbContextOptionsBuilder> dbContextOptionsBuilder,
		Action<TokenOptions, IServiceProvider> tokenOptions)
		=> services
		.AddOptions<TokenOptions>()
		.Configure(tokenOptions)
		.Services
		.AddServices()
		.AddDatabase(dbContextOptionsBuilder)
		.AddHealth()
		.AddAuthenticationInternal()
		.AddAuthorizationInternal()
		.AddCorrelation();

	private static IServiceCollection AddServices(this IServiceCollection services)
		=> services.AddSingleton(TimeProvider.System)
		.AddSingleton<ISequentialGuidGenerator, SequentialGuidGenerator>();

	private static IServiceCollection AddDatabase(this IServiceCollection services, Action<IServiceProvider, DbContextOptionsBuilder> dbContextOptionsBuilder)
	=> services
		.AddDbContextFactory<SampleContext>(dbContextOptionsBuilder)
		.Scan(scan => scan
		.FromAssemblies(Assembly.GetExecutingAssembly())
		.AddClasses(
			filter => filter.Where(x => x.Name.EndsWith("repository", StringComparison.OrdinalIgnoreCase)),
			publicOnly: false)
		.AsImplementedInterfaces()
		.WithTransientLifetime());

	private static IServiceCollection AddHealth(this IServiceCollection services)
		=> services
		.AddHealthChecks()
		.AddCheck("self", () => HealthCheckResult.Healthy(), tags: ["live"])
		.AddDbContextCheck<SampleContext>()
		.Services;

	private static IServiceCollection AddAuthenticationInternal(
		this IServiceCollection services)
	{
		var tokenOptions = services.BuildServiceProvider().GetRequiredService<IOptions<TokenOptions>>().Value;
		services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
			.AddJwtBearer(o =>
			{
				o.RequireHttpsMetadata = false;
				o.TokenValidationParameters = new TokenValidationParameters
				{
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenOptions.Secret)),
					ValidIssuer = tokenOptions.Issuer,
					ValidAudience = tokenOptions.Audience,
					ClockSkew = TimeSpan.Zero
				};
			});

		services.AddHttpContextAccessor();
		services.AddScoped<IUserContext, UserContext>();
		services.AddSingleton<IPasswordHasher, PasswordHasher>();
		services.AddSingleton<ITokenProvider, TokenProvider>();

		return services;
	}

	private static IServiceCollection AddAuthorizationInternal(this IServiceCollection services)
	{
		services.AddAuthorization();

		services.AddScoped<PermissionProvider>();

		services.AddTransient<IAuthorizationHandler, PermissionAuthorizationHandler>();

		services.AddTransient<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();

		return services;
	}

	private static IServiceCollection AddCorrelation(this IServiceCollection services)
	{
		//Learn more about configuring CorrelationId at https://github.com/stevejgordon/CorrelationId/wiki
		services.AddCorrelationId<CustomCorrelationIdProvider>(options =>
		{
			options.AddToLoggingScope = true;
			options.LoggingScopeKey = CorrelationIdOptions.DefaultHeader;
		});

		return services;
	}
}
