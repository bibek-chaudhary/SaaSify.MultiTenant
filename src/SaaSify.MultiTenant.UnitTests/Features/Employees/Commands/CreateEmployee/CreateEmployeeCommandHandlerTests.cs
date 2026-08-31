using FluentAssertions;
using Moq;
using SaaSify.MultiTenant.Application.Abstractions.Authentication;
using SaaSify.MultiTenant.Application.Abstractions.Persistence;
using SaaSify.MultiTenant.Application.Exceptions;
using SaaSify.MultiTenant.Application.Features.Employees.Commands.CreateEmployee;
using SaaSify.MultiTenant.Core.Constants;
using SaaSify.MultiTenant.Core.Entities;
using Xunit;

namespace SaaSify.MultiTenant.UnitTests.Features.Employees.Commands.CreateEmployee;

public class CreateEmployeeCommandHandlerTests
{
    private readonly Mock<IEmployeeRepository> _employeeRepository = new();
    private readonly Mock<IIdentityService> _identityService = new();
    private readonly Mock<ICurrentUserService> _currentUserService = new();
    private readonly CreateEmployeeCommandHandler _handler;

    public CreateEmployeeCommandHandlerTests()
    {
        _handler = new CreateEmployeeCommandHandler(
            _employeeRepository.Object,
            _identityService.Object,
            _currentUserService.Object);
    }

    [Fact]
    public async Task Should_Create_Employee_When_Request_Is_Valid()
    {
        var tenantId = Guid.NewGuid();
        var command = new CreateEmployeeCommand("Jane Doe", "jane.doe@example.com", "P@ssword1");

        _currentUserService.Setup(x => x.TenantId).Returns(tenantId);
        _employeeRepository
            .Setup(x => x.EmailExistsAsync(command.EmailAddress, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.FullName.Should().Be(command.FullName);
        result.EmailAddress.Should().Be(command.EmailAddress);

        _identityService.Verify(
            x => x.RegisterUserAsync(command.EmailAddress, command.Password, Roles.Employee, tenantId),
            Times.Once);
        _employeeRepository.Verify(
            x => x.AddAsync(It.Is<Employee>(e => e.EmailAddress == command.EmailAddress), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Should_Throw_UnauthorizedException_When_TenantId_Is_Missing()
    {
        var command = new CreateEmployeeCommand("Jane Doe", "jane.doe@example.com", "P@ssword1");

        _currentUserService.Setup(x => x.TenantId).Returns((Guid?)null);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedException>();
        _employeeRepository.Verify(
            x => x.EmailExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Should_Throw_ConflictException_When_Email_Already_Exists()
    {

        var command = new CreateEmployeeCommand("Jane Doe", "jane.doe@example.com", "P@ssword1");

        _currentUserService.Setup(x => x.TenantId).Returns(Guid.NewGuid());
        _employeeRepository
            .Setup(x => x.EmailExistsAsync(command.EmailAddress, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<ConflictException>();
        _identityService.Verify(
            x => x.RegisterUserAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>()),
            Times.Never);
    }
}
