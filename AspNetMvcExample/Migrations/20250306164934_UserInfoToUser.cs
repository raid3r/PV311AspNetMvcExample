using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AspNetMvcExample.Migrations
{
    /// <inheritdoc />
    public partial class UserInfoToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AuthorId",
                table: "UserInfos",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserInfos_AuthorId",
                table: "UserInfos",
                column: "AuthorId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserInfos_AspNetUsers_AuthorId",
                table: "UserInfos",
                column: "AuthorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserInfos_AspNetUsers_AuthorId",
                table: "UserInfos");

            migrationBuilder.DropIndex(
                name: "IX_UserInfos_AuthorId",
                table: "UserInfos");

            migrationBuilder.DropColumn(
                name: "AuthorId",
                table: "UserInfos");
        }
    }
}
