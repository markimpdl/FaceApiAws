using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FaceApi.Migrations
{
    /// <inheritdoc />
    public partial class AjusteRelacional : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AzurePersonId",
                table: "Users");

            migrationBuilder.AddColumn<string>(
                name: "AzurePersonId",
                table: "UserSchools",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AzurePersonId",
                table: "UserSchools");

            migrationBuilder.AddColumn<string>(
                name: "AzurePersonId",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
