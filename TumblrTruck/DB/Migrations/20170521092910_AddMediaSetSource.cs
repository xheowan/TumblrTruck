using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TumblrTruck.DB.Migrations
{
    public partial class AddMediaSetSource : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "SourceID",
                table: "MediaSet",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "SourceUrl",
                table: "MediaSet",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SourceID",
                table: "MediaSet");

            migrationBuilder.DropColumn(
                name: "SourceUrl",
                table: "MediaSet");
        }
    }
}
