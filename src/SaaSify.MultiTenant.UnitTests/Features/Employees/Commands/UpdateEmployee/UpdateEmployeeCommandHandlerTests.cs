using FluentAssertions;
using Moq;
using SaaSify.MultiTenant.Application.Abstractions.Authentication;
using SaaSify.MultiTenant.Application.Abstractions.Persistence;
using SaaSify.MultiTenant.Application.Exceptions;
using SaaSify.MultiTenant.Application.Features.Employees.Commands.UpdateEmployee;
using SaaSify.MultiTenant.Core.Entities;
using Xunit;

namespace SaaSify.MultiTenant.UnitTests.Features.Employees.Commands.UpdateEmployee;

public class UpdateEmployeeCommandHandlerTests
{
    private readonly Mock<IEmployeeRepository> _employeeRepository = new();
    private readonly Mock<IIdentityService> _identityService = new();
    private readonly UpdateEmployeeCommandHandler _handler;

    public UpdateEmployeeCommandHandlerTests()
    {
        _handler = new UpdateEmployeeCommandHandler(
            _employeeRepository.Object,
            _identityService.Object);
    }

    private Employee SetupExistingEmployee()
    {
        var employee = new Employee
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            FullName = "Old Name",
            EmailAddress = "old@example.com"
        };

        _employeeRepository
            .Setup(x => x.GetByIdAsync(employee.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(employee);

        return employee;
    }

    [Fact]
    public async Task Should_Update_FullName_And_EmailAddress_When_Employee_Exists()
    {
        var employee = SetupExistingEmployee();
        var command = new UpdateEmployeeCommand(employee.Id, "New Name", "new@example.com");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeTrue();
        employee.FullName.Should().Be("New Name");
        employee.EmailAddress.Should().Be("new@example.com");
        employee.UpdatedAtUtc.Should().NotBeNull();
        _employeeRepository.Verify(
            x => x.UpdateAsync(employee, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Should_Update_Identity_Email_When_EmailAddress_Changes()
    {
        var employee = SetupExistingEmployee();
        var command = new UpdateEmployeeCommand(employee.Id, null, "new@example.com");

        await _handler.Handle(command, CancellationToken.None);

        _identityService.Verify(
            x => x.UpdateEmailAsync(employee.UserId, "new@example.com"),
            Times.Once);
    }

    [Fact]
    public async Task Should_Not_Touch_Identity_When_EmailAddress_Is_Unchanged()
    {
        var employee = SetupExistingEmployee();
        var command = new UpdateEmployeeCommand(employee.Id, "New Name", "OLD@example.com");

        await _handler.Handle(command, CancellationToken.None);

        _identityService.Verify(
            x => x.UpdateEmailAsync(It.IsAny<Guid>(), It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task Should_Not_Change_Employee_When_Identity_Email_Update_Fails()
    {
        var employee = SetupExistingEmployee();
        var command = new UpdateEmployeeCommand(employee.Id, "New Name", "taken@example.com");

        _identityService
            .Setup(x => x.UpdateEmailAsync(employee.UserId, "taken@example.com"))
            .ThrowsAsync(new ConflictException("User already exists."));

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<ConflictException>();
        employee.EmailAddress.Should().Be("old@example.com");
        _employeeRepository.Verify(
            x => x.UpdateAsync(It.IsAny<Employee>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Should_Revert_Identity_Email_When_Saving_Employee_Fails()
    {
        var employee = SetupExistingEmployee();
        var command = new UpdateEmployeeCommand(employee.Id, null, "new@example.com");

        _employeeRepository
            .Setup(x => x.UpdateAsync(employee, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("db down"));

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>();
        _identityService.Verify(
            x => x.UpdateEmailAsync(employee.UserId, "old@example.com"),
            Times.Once);
    }

    [Fact]
    public async Task Should_Only_Update_FullName_When_EmailAddress_Is_Not_Provided()
    {
        var employee = SetupExistingEmployee();
        var command = new UpdateEmployeeCommand(employee.Id, "New Name", null);

        await _handler.Handle(command, CancellationToken.None);

        employee.FullName.Should().Be("New Name");
        employee.EmailAddress.Should().Be("old@example.com");
    }

    [Fact]
    public async Task Should_Only_Update_EmailAddress_When_FullName_Is_Not_Provided()
    {
        var employee = SetupExistingEmployee();
        var command = new UpdateEmployeeCommand(employee.Id, null, "new@example.com");

        await _handler.Handle(command, CancellationToken.None);

        employee.FullName.Should().Be("Old Name");
        employee.EmailAddress.Should().Be("new@example.com");
    }

    [Fact]
    public async Task Should_Throw_NotFoundException_When_Employee_Does_Not_Exist()
    {
        var command = new UpdateEmployeeCommand(Guid.NewGuid(), "New Name", "new@example.com");

        _employeeRepository
            .Setup(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Employee?)null);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
        _employeeRepository.Verify(
            x => x.UpdateAsync(It.IsAny<Employee>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
