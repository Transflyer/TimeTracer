using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TimeTracker.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    UserName = table.Column<string>(maxLength: 128, nullable: false),
                    Email = table.Column<string>(nullable: false),
                    DisplayName = table.Column<string>(nullable: true),
                    Notes = table.Column<string>(nullable: true),
                    Type = table.Column<int>(nullable: false),
                    Flags = table.Column<int>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    LastModifiedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NodeElements",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    Title = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Text = table.Column<string>(nullable: true),
                    Notes = table.Column<string>(nullable: true),
                    Type = table.Column<int>(nullable: false),
                    Flags = table.Column<int>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    ParentId = table.Column<int>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    LastModifiedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NodeElements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NodeElements_NodeElements_ParentId",
                        column: x => x.ParentId,
                        principalTable: "NodeElements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NodeElements_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NodeElements_ParentId",
                table: "NodeElements",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_NodeElements_UserId",
                table: "NodeElements",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NodeElements");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
