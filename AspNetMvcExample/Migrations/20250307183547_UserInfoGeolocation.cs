using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AspNetMvcExample.Migrations
{
    /// <inheritdoc />
    public partial class UserInfoGeolocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "Lat",
                table: "UserInfos",
                type: "REAL",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "Lng",
                table: "UserInfos",
                type: "REAL",
                nullable: false,
                defaultValue: 0f);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Lat",
                table: "UserInfos");

            migrationBuilder.DropColumn(
                name: "Lng",
                table: "UserInfos");
        }
    }
}
