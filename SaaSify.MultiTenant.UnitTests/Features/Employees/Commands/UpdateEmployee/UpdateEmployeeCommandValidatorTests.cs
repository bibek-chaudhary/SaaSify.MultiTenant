using FluentValidation.TestHelper;
using SaaSify.MultiTenant.Application.Features.Employees.Commands.UpdateEmployee;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SaaSify.MultiTenant.UnitTests.Features.Employees.Commands.UpdateEmployee
{
    public class UpdateEmployeeCommandValidatorTests
    {
        private readonly UpdateEmployeeCommandValidator _validator = new();

        [Fact]
        public void Should_Pass_When_Command_Is_Valid() 
        {
            var command = new UpdateEmployeeCommand(Guid.Parse("9f7231a2-4a1c-4262-a2c8-5d130118079c"), "Bibek Chaudhary", "email@domain.com");

            var result = _validator.TestValidate(command);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Should_Have_Error_When_Id_Is_Empty()
        {
            var command = new UpdateEmployeeCommand(Guid.Empty, "John Cena", "john@email.com");

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Id);
        }

        [Fact]
        public void Should_Have_Error_When_Fullname_And_EmailAddress_Are_Both_Empty()
        {
            var command = new UpdateEmployeeCommand(Guid.Parse("9f7231a2-4a1c-4262-a2c8-5d130118079c"), null, null);

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrors();
        }

        [Fact]
        public void Should_Have_Error_When_EmailAddress_Is_Invalid()
        {
            var command = new UpdateEmployeeCommand(Guid.Parse("9f7231a2-4a1c-4262-a2c8-5d130118079c"), "Roman Reigns", "not-an-email");

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.EmailAddress);
        }
    }
}
