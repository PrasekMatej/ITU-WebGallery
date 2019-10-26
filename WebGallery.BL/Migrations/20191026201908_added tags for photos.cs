using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace WebGallery.BL.Migrations
{
    public partial class addedtagsforphotos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TagEntity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Owner = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PhotoEntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TagEntity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TagEntity_ItemEntity_PhotoEntityId",
                        column: x => x.PhotoEntityId,
                        principalTable: "ItemEntity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TagEntity_PhotoEntityId",
                table: "TagEntity",
                column: "PhotoEntityId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TagEntity");
        }
    }
}
