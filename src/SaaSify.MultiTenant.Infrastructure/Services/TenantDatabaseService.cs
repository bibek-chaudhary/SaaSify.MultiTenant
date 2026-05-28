using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql;
using SaaSify.MultiTenant.Application.Interfaces;
using SaaSify.MultiTenant.Infrastructure.Persistence.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaaSify.MultiTenant.Infrastructure.Services;
public class TenantDatabaseService : ITenantDatabaseService
{
    private readonly IConfiguration _configuration;

    public TenantDatabaseService(
        IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<string> CreateTenantDatabaseAsync( string tenantIdentifier)
    {
        var masterConnection = _configuration.GetConnectionString("MasterDatabase");

        var builder = new NpgsqlConnectionStringBuilder(masterConnection);

        var databaseName = $"tenant_{tenantIdentifier}";

        var postgreDatabase = builder.Database;

        builder.Database = "postgres";

        var postgreConnection = builder.ConnectionString;

        await using var connection = new NpgsqlConnection(postgreConnection);

        await connection.OpenAsync();

        var command = connection.CreateCommand();

        command.CommandText = $"CREATE DATABASE \"{databaseName}\"";

        await command.ExecuteNonQueryAsync();

        var tenantConnectionString = builder.ConnectionString;

        var options = new DbContextOptionsBuilder<TenantDbContext>()
            .UseNpgsql(tenantConnectionString)
            .Options;

        await using var context = new TenantDbContext(options);

        await context.Database.MigrateAsync();

        return tenantConnectionString;
    }
}