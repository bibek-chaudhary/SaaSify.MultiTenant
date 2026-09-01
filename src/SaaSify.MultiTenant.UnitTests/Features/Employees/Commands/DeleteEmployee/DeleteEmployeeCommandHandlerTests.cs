using FluentAssertions;
using Moq;
using SaaSify.MultiTenant.Application.Abstractions.Authentication;
using SaaSify.MultiTenant.Application.Abstractions.Persistence;
using SaaSify.MultiTenant.Application.Exceptions;
using SaaSify.MultiTenant.Application.Features.Employees.Commands.DeleteEmployee;
using SaaSify.MultiTenant.Application.Features.Employees.Commands.UpdateEmployee;
using SaaSify.MultiTenant.Core.Entities;
using Xunit;

namespace SaaSify.MultiTenant.UnitTests.Features.Employees.Commands.DeleteEmployee;

public class DeleteEmployeeCommandHandlerTests
{
    private readonly Mock<IEmployeeRepository> _employeeRepository = new();
    private readonly Mock<IIdentityService> _identityService = new();
    private readonly DeleteEmployeeCommandHandler _handler;

    public DeleteEmployeeCommandHandlerTests()
    {
        _handler = new DeleteEmployeeCommandHandler(
            _employeeRepository.Object,
            _identityService.Object);
    }

    [Fact]
    public async Task Should_Delete_Employee_When_Found()
    {
        var employee = new Employee
        {
            Id = Guid.NewGuid(),
            FullName = "Jane Doe",
            EmailAddress = "jane.doe@example.com"
        };
        var command = new DeleteEmployeeCommand(employee.Id);

        _employeeRepository
            .Setup(x => x.GetByIdAsync(employee.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(employee);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeTrue();
        _employeeRepository.Verify(
            x => x.DeleteAsync(employee, It.IsAny<CancellationToken>()),
            Times.Once);
        _identityService.Verify(
            x => x.DeleteUserAsync(employee.EmailAddress),
            Times.Once);
    }

    [Fact]
    public async Task Should_Throw_NotFoundException_When_Employee_Does_Not_Exist()
    {
        var command = new DeleteEmployeeCommand(Guid.NewGuid());

        _employeeRepository
            .Setup(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Employee?)null);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
        _employeeRepository.Verify(
            x => x.DeleteAsync(It.IsAny<Employee>(), It.IsAny<CancellationToken>()),
            Times.Never);
        _identityService.Verify(
            x => x.DeleteUserAsync(It.IsAny<string>()),
            Times.Never);
    }
}
