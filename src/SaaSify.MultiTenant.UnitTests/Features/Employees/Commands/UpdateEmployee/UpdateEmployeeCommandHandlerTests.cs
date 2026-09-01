using FluentAssertions;
using Moq;
using SaaSify.MultiTenant.Application.Abstractions.Persistence;
using SaaSify.MultiTenant.Application.Exceptions;
using SaaSify.MultiTenant.Application.Features.Employees.Commands.UpdateEmployee;
using SaaSify.MultiTenant.Core.Entities;
using Xunit;

namespace SaaSify.MultiTenant.UnitTests.Features.Employees.Commands.UpdateEmployee;

public class UpdateEmployeeCommandHandlerTests
{
    private readonly Mock<IEmployeeRepository> _employeeRepository = new();
    private readonly UpdateEmployeeCommandHandler _handler;

    public UpdateEmployeeCommandHandlerTests()
    {
        _handler = new UpdateEmployeeCommandHandler(_employeeRepository.Object);
    }

    [Fact]
    public async Task Should_Update_FullName_And_EmailAddress_When_Employee_Exists()
    {
        var employee = new Employee
        {
            Id = Guid.NewGuid(),
            FullName = "Old Name",
            EmailAddress = "old@example.com"
        };
        var command = new UpdateEmployeeCommand(employee.Id, "New Name", "new@example.com");

        _employeeRepository
            .Setup(x => x.GetByIdAsync(employee.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(employee);

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
    public async Task Should_Only_Update_FullName_When_EmailAddress_Is_Not_Provided()
    {
        var employee = new Employee
        {
            Id = Guid.NewGuid(),
            FullName = "Old Name",
            EmailAddress = "old@example.com"
        };
        var command = new UpdateEmployeeCommand(employee.Id, "New Name", null);

        _employeeRepository
            .Setup(x => x.GetByIdAsync(employee.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(employee);

        await _handler.Handle(command, CancellationToken.None);

        employee.FullName.Should().Be("New Name");
        employee.EmailAddress.Should().Be("old@example.com");
    }

    [Fact]
    public async Task Should_Only_Update_EmailAddress_When_FullName_Is_Not_Provided()
    {
        var employee = new Employee
        {
            Id = Guid.NewGuid(),
            FullName = "Old Name",
            EmailAddress = "old@example.com"
        };
        var command = new UpdateEmployeeCommand(employee.Id, null, "new@example.com");

        _employeeRepository
            .Setup(x => x.GetByIdAsync(employee.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(employee);

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
