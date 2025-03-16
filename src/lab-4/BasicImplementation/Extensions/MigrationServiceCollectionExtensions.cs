using BasicImplementation.Migrations;
using BasicImplementation.Models;
using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace BasicImplementation.Extensions;

public static class MigrationServiceCollectionExtensions
{
    public static void AddMigration(this IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddFluentMigratorCore()
            .ConfigureRunner(runner => runner
                .AddPostgres()
                .WithGlobalConnectionString(provider => provider.GetRequiredService<IOptions<PostgresConnectionOptions>>().Value.ToConnectionString())
                .WithMigrationsIn(typeof(IMigrationAssemblyMarker).Assembly));
    }

    public static void RunMigration(this IServiceProvider serviceProvider)
    {
        using IServiceScope scope = serviceProvider.CreateAsyncScope();
        IMigrationRunner runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
        runner.MigrateUp();
    }
}