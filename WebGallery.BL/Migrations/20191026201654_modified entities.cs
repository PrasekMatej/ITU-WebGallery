using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace WebGallery.BL.Migrations
{
    public partial class modifiedentities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                table: "ItemEntity",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDate",
                table: "ItemEntity",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "CreatedDate",
                table: "ItemEntity",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Metadata",
                table: "ItemEntity",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedDate",
                table: "ItemEntity");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "ItemEntity");

            migrationBuilder.DropColumn(
                name: "Metadata",
                table: "ItemEntity");

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                table: "ItemEntity",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);
        }
    }
}
