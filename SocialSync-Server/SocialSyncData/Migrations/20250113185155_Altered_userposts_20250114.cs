using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialSyncData.Migrations
{
    /// <inheritdoc />
    public partial class Altered_userposts_20250114 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_userposts_PostSchedulers_PostSchedulerId",
                table: "userposts");

            migrationBuilder.DropIndex(
                name: "IX_userposts_PostSchedulerId",
                table: "userposts");

            migrationBuilder.AlterColumn<int>(
                name: "PostSchedulerId",
                table: "userposts",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_userposts_PostSchedulerId",
                table: "userposts",
                column: "PostSchedulerId",
                unique: true,
                filter: "[PostSchedulerId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_userposts_PostSchedulers_PostSchedulerId",
                table: "userposts",
                column: "PostSchedulerId",
                principalTable: "PostSchedulers",
                principalColumn: "PostSchedulerId");
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

            migrationBuilder.AlterColumn<int>(
                name: "PostSchedulerId",
                table: "userposts",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

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
    }
}
