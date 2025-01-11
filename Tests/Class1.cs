using Itmo.Dev.Platform.Common.Extensions;
using Itmo.Dev.Platform.Postgres.Extensions;
using Itmo.Dev.Platform.Postgres.Models;
using Microsoft.Extensions.DependencyInjection;
using Migrations;
using NativeConsole;
using Outcoming;
using Persistance;
using Services;

namespace Tests;

#pragma warning disable SA1402

public class Class1
{
    public static async Task Main()
    {
        var services = new ServiceCollection();
        var cts = new CancellationTokenSource();
        ConfigureServices(services);

        services.AddSingleton(new CancellationTokenProvider(cts.Token));

        Something(services, options =>
            {
                options.Host = "localhost";
                options.Port = 6432;
                options.Username = "postgres";
                options.Password = "postgres";
                options.Database = "postgres";
                options.SslMode = "Prefer";
            });
        ServiceProvider serviceProvider = services.BuildServiceProvider();

        // using IServiceScope scope = serviceProvider.CreateScope();
        // scope.UsePlatformMigrationsAsync(default).GetAwaiter().GetResult();
        ConsoleInterface console = serviceProvider.GetRequiredService<ConsoleInterface>();
        try
        {
            await console.Run(cts.Token).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Canceled");
        }
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddTransient<ConsoleInterface>();
        services.AddTransient<IUserConsoleController, UserConsoleController>();
        services.AddTransient<IAdminConsoleController, AdminConsoleController>();

        services.AddTransient<IAccountService, AccountService>();

        services.AddTransient<IAccountRepository, PostgresAccountRepository>();
        services.AddTransient<ITransactionRepository, PostgresTransactionRepository>();
    }

    private static void Something(ServiceCollection services, Action<PostgresConnectionConfiguration> configure)
    {
        services.AddPlatform();
        services.AddPlatformPostgres(builder => builder.Configure(configure));
        services.AddPlatformMigrations(typeof(InitialMigrations.InitialMigration).Assembly);
    }
}

public class CancellationTokenProvider
{
    public CancellationToken Token { get; }

    public CancellationTokenProvider(CancellationToken token)
    {
        Token = token;
    }
}
