using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FaceApi.Migrations
{
    /// <inheritdoc />
    public partial class AjusteAzureToAws : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AzurePersonId",
                table: "UserSchools",
                newName: "AwsFaceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AwsFaceId",
                table: "UserSchools",
                newName: "AzurePersonId");
        }
    }
}
