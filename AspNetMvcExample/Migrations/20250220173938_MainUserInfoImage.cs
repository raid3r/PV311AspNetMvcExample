using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AspNetMvcExample.Migrations
{
    /// <inheritdoc />
    public partial class MainUserInfoImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MainImageFileId",
                table: "UserInfos",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserInfos_MainImageFileId",
                table: "UserInfos",
                column: "MainImageFileId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserInfos_ImageFiles_MainImageFileId",
                table: "UserInfos",
                column: "MainImageFileId",
                principalTable: "ImageFiles",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserInfos_ImageFiles_MainImageFileId",
                table: "UserInfos");

            migrationBuilder.DropIndex(
                name: "IX_UserInfos_MainImageFileId",
                table: "UserInfos");

            migrationBuilder.DropColumn(
                name: "MainImageFileId",
                table: "UserInfos");
        }
    }
}
