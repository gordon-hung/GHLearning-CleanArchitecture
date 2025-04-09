using GHLearning.CleanArchitecture.Migrations.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GHLearning.CleanArchitecture.Migrations;

public class MigrationsContextFactory : IDesignTimeDbContextFactory<SampleContext>
{
	public SampleContext CreateDbContext(string[] args)
	{
		var host = Host.CreateDefaultBuilder(args)
			.ConfigureHostConfiguration(builder => builder
				.AddJsonFile("appsettings.json")
				.AddEnvironmentVariables())
			.ConfigureServices(services => services
				.AddDbContext<SampleContext>(
				dbOptions => dbOptions
				.UseMySql(
					connectionString: services.BuildServiceProvider().GetRequiredService<IConfiguration>().GetConnectionString("MySql"),
					serverVersion: ServerVersion.AutoDetect(services.BuildServiceProvider().GetRequiredService<IConfiguration>().GetConnectionString("MySql")),
					mySqlOptionsAction: option => option
					.MigrationsAssembly("GHLearning.CleanArchitecture.Migrations"))))
			.Build();

		var dbContext = host.Services.GetRequiredService<SampleContext>();

		return dbContext;
	}
}
