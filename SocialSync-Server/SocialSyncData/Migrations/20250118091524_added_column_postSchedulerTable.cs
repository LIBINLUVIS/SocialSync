﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialSyncData.Migrations
{
    /// <inheritdoc />
    public partial class added_column_postSchedulerTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UseraccountId",
                table: "PostSchedulers",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UseraccountId",
                table: "PostSchedulers");
        }
    }
}
