using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BYDPlatform.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ModifiedUserEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreateBy",
                table: "user");

            migrationBuilder.RenameColumn(
                name: "Created",
                table: "user",
                newName: "created");

            migrationBuilder.RenameColumn(
                name: "LastModifiedBy",
                table: "user",
                newName: "last_modified_by");

            migrationBuilder.RenameColumn(
                name: "LastModified",
                table: "user",
                newName: "last_modified");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created",
                table: "user",
                type: "datetime(6)",
                nullable: false,
                comment: "创建时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AlterColumn<string>(
                name: "last_modified_by",
                table: "user",
                type: "longtext",
                nullable: true,
                comment: "上次修改时间",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<DateTime>(
                name: "last_modified",
                table: "user",
                type: "datetime(6)",
                nullable: true,
                comment: "修改时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "created_by",
                table: "user",
                type: "longtext",
                nullable: true,
                comment: "创建人")
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "created_by",
                table: "user");

            migrationBuilder.RenameColumn(
                name: "created",
                table: "user",
                newName: "Created");

            migrationBuilder.RenameColumn(
                name: "last_modified_by",
                table: "user",
                newName: "LastModifiedBy");

            migrationBuilder.RenameColumn(
                name: "last_modified",
                table: "user",
                newName: "LastModified");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Created",
                table: "user",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldComment: "创建时间");

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                table: "user",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true,
                oldComment: "上次修改时间")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastModified",
                table: "user",
                type: "datetime(6)",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true,
                oldComment: "修改时间");

            migrationBuilder.AddColumn<string>(
                name: "CreateBy",
                table: "user",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
