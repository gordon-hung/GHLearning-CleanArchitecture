using System.Reflection;
using FluentValidation;
using GHLearning.CleanArchitecture.Application.Abstractions.Behaviors;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddApplication(this IServiceCollection services)
	{
		services.AddMediatR(config =>
		{
			config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());

			config.AddOpenBehavior(typeof(HandleTracingPipelineBehavior<,>));
			config.AddOpenBehavior(typeof(RequestLoggingPipelineBehavior<,>));
			config.AddOpenBehavior(typeof(ValidationPipelineBehavior<,>));
		});

		services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly(), includeInternalTypes: true);
		return services;
	}
}
