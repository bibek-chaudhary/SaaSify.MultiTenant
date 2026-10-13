using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SaaSify.MultiTenant.Infrastructure.Persistence.TenantMigrations
{
    /// <inheritdoc />
    public partial class InitialEmployeeTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Employees_EmailAddress",
                table: "Employees",
                column: "EmailAddress",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Employees_EmailAddress",
                table: "Employees");
        }
    }
}
