using FluentValidation.TestHelper;
using SaaSify.MultiTenant.Application.Features.Employees.Commands.DeleteEmployee;
using Xunit;

namespace SaaSify.MultiTenant.UnitTests.Features.Employees.Commands.DeleteEmployee;

public class DeleteEmployeeCommandValidatorTests
{
    private readonly DeleteEmployeeCommandValidator _validator = new();

    [Fact]
    public void Should_Pass_When_Id_Is_Valid()
    {
        var command = new DeleteEmployeeCommand(Guid.NewGuid());

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Have_Error_When_Id_Is_Empty()
    {
        var command = new DeleteEmployeeCommand(Guid.Empty);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Id);
    }
}
