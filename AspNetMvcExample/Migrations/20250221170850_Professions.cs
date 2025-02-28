using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AspNetMvcExample.Migrations
{
    /// <inheritdoc />
    public partial class Professions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Profession",
                table: "UserInfos");

            migrationBuilder.AddColumn<int>(
                name: "ProfessionId",
                table: "UserInfos",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Profession",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profession", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserInfos_ProfessionId",
                table: "UserInfos",
                column: "ProfessionId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserInfos_Profession_ProfessionId",
                table: "UserInfos",
                column: "ProfessionId",
                principalTable: "Profession",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserInfos_Profession_ProfessionId",
                table: "UserInfos");

            migrationBuilder.DropTable(
                name: "Profession");

            migrationBuilder.DropIndex(
                name: "IX_UserInfos_ProfessionId",
                table: "UserInfos");

            migrationBuilder.DropColumn(
                name: "ProfessionId",
                table: "UserInfos");

            migrationBuilder.AddColumn<string>(
                name: "Profession",
                table: "UserInfos",
                type: "TEXT",
                nullable: true);
        }
    }
}
