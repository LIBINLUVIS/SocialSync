using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialSyncData.Migrations
{
    /// <inheritdoc />
    public partial class Altered_table_relations_20251101 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
        }
    }
}
