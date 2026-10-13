using Microsoft.Extensions.Configuration;

namespace SaaSify.MultiTenant.Infrastructure.Persistence.Factories;

// Mirrors the Api's configuration sources so `dotnet ef` sees the same
// settings (including user-secrets) as the running app.
internal static class DesignTimeConfiguration
{
    // Keep in sync with <UserSecretsId> in SaaSify.MultiTenant.Api.csproj.
    private const string UserSecretsId = "saasify-multitenant-api";

    public static IConfiguration Build()
    {
        return new ConfigurationBuilder()
            .SetBasePath(
                Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "../SaaSify.MultiTenant.Api"))
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddUserSecrets(UserSecretsId)
            .AddEnvironmentVariables()
            .Build();
    }

    public static string GetMasterConnection()
    {
        var connectionString =
            Build()["DatabaseSettings:MasterConnection"];

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "DatabaseSettings:MasterConnection is not configured. " +
                "Set it with `dotnet user-secrets` in the Api project or the " +
                "DatabaseSettings__MasterConnection environment variable.");
        }

        return connectionString;
    }
}
