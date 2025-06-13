using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FaceApi.Migrations
{
    /// <inheritdoc />
    public partial class AddGeolocalizationAndPreCheckin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AwsFaceId",
                table: "PresenceRecords",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "GeoLocalization",
                table: "PresenceRecords",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AwsFaceId",
                table: "PresenceRecords");

            migrationBuilder.DropColumn(
                name: "GeoLocalization",
                table: "PresenceRecords");
        }
    }
}
