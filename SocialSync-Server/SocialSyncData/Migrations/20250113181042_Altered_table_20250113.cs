using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialSyncData.Migrations
{
    /// <inheritdoc />
    public partial class Altered_table_20250113 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PostSchedulers_userposts_UserPostId",
                table: "PostSchedulers");

            migrationBuilder.DropIndex(
                name: "IX_PostSchedulers_UserPostId",
                table: "PostSchedulers");

            migrationBuilder.DropColumn(
                name: "UserPostId",
                table: "PostSchedulers");

            migrationBuilder.AddColumn<int>(
                name: "PostSchedulerId",
                table: "userposts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_userposts_PostSchedulerId",
                table: "userposts",
                column: "PostSchedulerId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_userposts_PostSchedulers_PostSchedulerId",
                table: "userposts",
                column: "PostSchedulerId",
                principalTable: "PostSchedulers",
                principalColumn: "PostSchedulerId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_userposts_PostSchedulers_PostSchedulerId",
                table: "userposts");

            migrationBuilder.DropIndex(
                name: "IX_userposts_PostSchedulerId",
                table: "userposts");

            migrationBuilder.DropColumn(
                name: "PostSchedulerId",
                table: "userposts");

            migrationBuilder.AddColumn<int>(
                name: "UserPostId",
                table: "PostSchedulers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PostSchedulers_UserPostId",
                table: "PostSchedulers",
                column: "UserPostId",
                unique: true,
                filter: "[UserPostId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_PostSchedulers_userposts_UserPostId",
                table: "PostSchedulers",
                column: "UserPostId",
                principalTable: "userposts",
                principalColumn: "Id");
        }
    }
}
