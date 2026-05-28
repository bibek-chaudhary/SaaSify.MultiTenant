using Microsoft.AspNetCore.Identity;
using SaaSify.MultiTenant.Api.Extensions;
using SaaSify.MultiTenant.Infrastructure;
using SaaSify.MultiTenant.Infrastructure.Identity.Entities;
using SaaSify.MultiTenant.Infrastructure.Persistence.Contexts;
using SaaSify.MultiTenant.Infrastructure.Persistence.Seed;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddPresentation();

builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context =
        services.GetRequiredService<MasterDbContext>();

    var roleManager =
        services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

    var userManager =
        services.GetRequiredService<UserManager<IdentityApplicationUser>>();

    await ApplicationDbSeeder.SeedAsync(
        context,
        roleManager,
        userManager);
}

app.Run();