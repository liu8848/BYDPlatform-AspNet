using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BYDPlatform.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFactory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "register_factory",
                columns: table => new
                {
                    id = table.Column<int>(type: "int(11)", nullable: false, comment: "主键")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    buid = table.Column<int>(name: "bu_id", type: "int(11)", nullable: false, comment: "所属事业部编号"),
                    factoryname = table.Column<string>(name: "factory_name", type: "varchar(50)", nullable: false, comment: "备案工厂名称")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    factorylevel = table.Column<int>(name: "factory_level", type: "int(2)", nullable: false, comment: "工厂等级"),
                    created = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    createdby = table.Column<string>(name: "created_by", type: "longtext", nullable: true, comment: "创建人")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    lastmodified = table.Column<DateTime>(name: "last_modified", type: "datetime(6)", nullable: true, comment: "修改时间"),
                    lastmodifiedby = table.Column<string>(name: "last_modified_by", type: "longtext", nullable: true, comment: "上次修改时间")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_register_factory", x => x.id);
                },
                comment: "备案工厂表")
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "register_factory");
        }
    }
}
