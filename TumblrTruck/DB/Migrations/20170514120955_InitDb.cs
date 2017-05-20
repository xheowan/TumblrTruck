using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TumblrTruck.DB.Migrations
{
    public partial class InitDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Blog",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    Url = table.Column<string>(maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Blog", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "LastActiveLog",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreateTime = table.Column<DateTime>(nullable: false),
                    LikedCount = table.Column<int>(nullable: false),
                    Status = table.Column<byte>(nullable: false),
                    Timestamp = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LastActiveLog", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "MediaSet",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    Cover = table.Column<string>(maxLength: 200, nullable: true),
                    Key = table.Column<string>(maxLength: 50, nullable: true),
                    Layout = table.Column<string>(maxLength: 50, nullable: true),
                    Type = table.Column<byte>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MediaSet", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Media",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FileName = table.Column<string>(maxLength: 200, nullable: false),
                    MediaSetID = table.Column<Guid>(nullable: false),
                    Size = table.Column<string>(maxLength: 50, nullable: false),
                    Status = table.Column<byte>(nullable: false),
                    Url = table.Column<string>(maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Media", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Media_MediaSet_MediaSetID",
                        column: x => x.MediaSetID,
                        principalTable: "MediaSet",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Post",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BlogName = table.Column<string>(maxLength: 100, nullable: false),
                    Content = table.Column<string>(nullable: true),
                    CreateTime = table.Column<DateTime>(nullable: false),
                    MediaSetID = table.Column<Guid>(nullable: false),
                    Slug = table.Column<string>(maxLength: 500, nullable: true),
                    SourceID = table.Column<long>(nullable: false),
                    SourceName = table.Column<string>(nullable: true),
                    SourceUrl = table.Column<string>(maxLength: 50, nullable: true),
                    Timestamp = table.Column<long>(nullable: false),
                    Type = table.Column<byte>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Post", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Post_MediaSet_MediaSetID",
                        column: x => x.MediaSetID,
                        principalTable: "MediaSet",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Media_MediaSetID",
                table: "Media",
                column: "MediaSetID");

            migrationBuilder.CreateIndex(
                name: "IX_Post_MediaSetID",
                table: "Post",
                column: "MediaSetID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Blog");

            migrationBuilder.DropTable(
                name: "LastActiveLog");

            migrationBuilder.DropTable(
                name: "Media");

            migrationBuilder.DropTable(
                name: "Post");

            migrationBuilder.DropTable(
                name: "MediaSet");
        }
    }
}
