using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Npgsql;
using SaaSify.MultiTenant.Application.Abstractions.Database;
using SaaSify.MultiTenant.Infrastructure.Configurations;
using SaaSify.MultiTenant.Infrastructure.Persistence.Contexts;
using System.Text.RegularExpressions;

namespace SaaSify.MultiTenant.Infrastructure.Database;

public class TenantDatabaseService : ITenantDatabaseService
{
    private static readonly Regex _safeIdentifier = new(@"^[a-zA-Z0-9]+$", RegexOptions.Compiled);

    private readonly DatabaseSettings _databaseSettings;

    public TenantDatabaseService(
        IOptions<DatabaseSettings> databaseOptions)
    {
        _databaseSettings = databaseOptions.Value;
    }

    public async Task<string> CreateTenantDatabaseAsync(string tenantIdentifier)
    {
        if (!_safeIdentifier.IsMatch(tenantIdentifier))
            throw new ArgumentException($"Invalid tenant identifier: {tenantIdentifier}");

        var masterConnection = _databaseSettings.MasterConnection;

        var builder = new NpgsqlConnectionStringBuilder(masterConnection);

        var databaseName = $"tenant_{tenantIdentifier}";

        builder.Database = "postgres";

        await using var connection = new NpgsqlConnection(builder.ConnectionString);

        await connection.OpenAsync();

        var command = connection.CreateCommand();

        command.CommandText = $"CREATE DATABASE \"{databaseName}\"";

        await command.ExecuteNonQueryAsync();

        builder.Database = databaseName;

        var tenantConnectionString = builder.ConnectionString;

        var options = new DbContextOptionsBuilder<TenantDbContext>()
            .UseNpgsql(tenantConnectionString)
            .Options;

        await using var context = new TenantDbContext(options);

        await context.Database.MigrateAsync();

        return tenantConnectionString;
    }

    public async Task DeleteTenantDatabaseAsync(string tenantIdentifier)
    {
        if (!_safeIdentifier.IsMatch(tenantIdentifier))
            return;

        var builder = new NpgsqlConnectionStringBuilder(_databaseSettings.MasterConnection);

        builder.Database = "postgres";

        await using var connection = new NpgsqlConnection(builder.ConnectionString);

        await connection.OpenAsync();

        var command = connection.CreateCommand();

        command.CommandText =
            $"SELECT pg_terminate_backend(pid) FROM pg_stat_activity WHERE datname = 'tenant_{tenantIdentifier}'";

        await command.ExecuteNonQueryAsync();

        command.CommandText = $"DROP DATABASE IF EXISTS \"tenant_{tenantIdentifier}\"";

        await command.ExecuteNonQueryAsync();
    }
}