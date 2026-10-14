using Microsoft.AspNetCore.Identity;
using SaaSify.MultiTenant.Api.Extensions;
using SaaSify.MultiTenant.Api.Middlewares;
using SaaSify.MultiTenant.Application;
using SaaSify.MultiTenant.Infrastructure;
using SaaSify.MultiTenant.Infrastructure.Identity.Entities;
using SaaSify.MultiTenant.Infrastructure.Persistence.Contexts;
using SaaSify.MultiTenant.Infrastructure.Persistence.Initialization;
using SaaSify.MultiTenant.Infrastructure.Settings;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddPresentation();

builder.Services.AddApplication();

builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context =
        services.GetRequiredService<MasterDbContext>();

    var roleManager =
        services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

    var userManager =
        services.GetRequiredService<UserManager<IdentityApplicationUser>>();

    var superAdmin =
        builder.Configuration.GetSection("SuperAdmin")
            .Get<SuperAdminSettings>() ?? new SuperAdminSettings();

    await ApplicationDbSeeder.SeedAsync(
        context,
        roleManager,
        userManager,
        superAdmin);

    await TenantDatabaseMigrator.MigrateAllAsync(
        context,
        userManager,
        services.GetRequiredService<ILogger<Program>>());
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseRateLimiter();

app.UseAuthentication();

app.UseMiddleware<TenantMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();