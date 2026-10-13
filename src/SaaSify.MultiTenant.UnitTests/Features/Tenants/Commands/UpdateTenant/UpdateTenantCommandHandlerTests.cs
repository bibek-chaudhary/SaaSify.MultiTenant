using FluentAssertions;
using Moq;
using SaaSify.MultiTenant.Application.Abstractions.Authentication;
using SaaSify.MultiTenant.Application.Abstractions.Persistence;
using SaaSify.MultiTenant.Application.Exceptions;
using SaaSify.MultiTenant.Application.Features.Tenants.Commands.UpdateTenant;
using SaaSify.MultiTenant.Core.Entities;
using Xunit;

namespace SaaSify.MultiTenant.UnitTests.Features.Tenants.Commands.UpdateTenant;

public class UpdateTenantCommandHandlerTests
{
    private readonly Mock<ITenantRepository> _tenantRepository = new();
    private readonly Mock<IIdentityService> _identityService = new();
    private readonly UpdateTenantCommandHandler _handler;

    public UpdateTenantCommandHandlerTests()
    {
        _handler = new UpdateTenantCommandHandler(
            _tenantRepository.Object,
            _identityService.Object);
    }

    private Tenant SetupExistingTenant()
    {
        var tenant = new Tenant
        {
            Id = Guid.NewGuid(),
            Name = "Old Co",
            EmailAddress = "admin@old.com",
            TenantId = "1234",
            DbConnStr = "Host=localhost"
        };

        _tenantRepository
            .Setup(x => x.GetByIdAsync(tenant.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tenant);

        return tenant;
    }

    [Fact]
    public async Task Should_Update_Admin_Identity_Email_When_EmailAddress_Changes()
    {
        var tenant = SetupExistingTenant();
        var adminUserId = Guid.NewGuid();
        var command = new UpdateTenantCommand(tenant.Id, null, "admin@new.com");

        _identityService
            .Setup(x => x.FindUserIdByEmailAsync("admin@old.com"))
            .ReturnsAsync(adminUserId);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeTrue();
        tenant.EmailAddress.Should().Be("admin@new.com");
        _identityService.Verify(
            x => x.UpdateEmailAsync(adminUserId, "admin@new.com"),
            Times.Once);
        _tenantRepository.Verify(
            x => x.UpdateAsync(tenant, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Should_Not_Touch_Identity_When_Only_Name_Changes()
    {
        var tenant = SetupExistingTenant();
        var command = new UpdateTenantCommand(tenant.Id, "New Co", null);

        await _handler.Handle(command, CancellationToken.None);

        tenant.Name.Should().Be("New Co");
        _identityService.Verify(
            x => x.UpdateEmailAsync(It.IsAny<Guid>(), It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task Should_Throw_NotFoundException_When_Admin_User_Is_Missing()
    {
        var tenant = SetupExistingTenant();
        var command = new UpdateTenantCommand(tenant.Id, null, "admin@new.com");

        _identityService
            .Setup(x => x.FindUserIdByEmailAsync("admin@old.com"))
            .ReturnsAsync((Guid?)null);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
        tenant.EmailAddress.Should().Be("admin@old.com");
        _tenantRepository.Verify(
            x => x.UpdateAsync(It.IsAny<Tenant>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
