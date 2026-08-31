using FluentValidation.TestHelper;
using SaaSify.MultiTenant.Application.Features.Employees.Commands.CreateEmployee;
using Xunit;

namespace SaaSify.MultiTenant.UnitTests.Features.Employees.Commands.CreateEmployee;

public class CreateEmployeeCommandValidatorTests
{
    private readonly CreateEmployeeCommandValidator _validator = new();

    [Fact]
    public void Should_Pass_When_Command_Is_Valid()
    {
        var command = new CreateEmployeeCommand("Jane Doe", "jane.doe@example.com", "P@ssword1");

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Have_Error_When_FullName_Is_Empty()
    {
        var command = new CreateEmployeeCommand("", "jane.doe@example.com", "P@ssword1");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.FullName);
    }

    [Theory]
    [InlineData("not-an-email")]
    [InlineData("")]
    public void Should_Have_Error_When_EmailAddress_Is_Invalid(string email)
    {
        var command = new CreateEmployeeCommand("Jane Doe", email, "P@ssword1");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.EmailAddress);
    }

    [Fact]
    public void Should_Have_Error_When_Password_Is_Too_Short()
    {
        var command = new CreateEmployeeCommand("Jane Doe", "jane.doe@example.com", "short");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Password);
    }
}
