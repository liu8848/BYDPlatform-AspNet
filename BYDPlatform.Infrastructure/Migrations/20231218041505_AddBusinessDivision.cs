using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BYDPlatform.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBusinessDivision : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "business_division",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false, comment: "主键-事业部编号")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    buname = table.Column<string>(name: "bu_name", type: "longtext", nullable: false, comment: "事业部名称")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    createdby = table.Column<string>(name: "created_by", type: "longtext", nullable: true, comment: "创建人")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    lastmodified = table.Column<DateTime>(name: "last_modified", type: "datetime(6)", nullable: true, comment: "修改时间"),
                    lastmodifiedby = table.Column<string>(name: "last_modified_by", type: "longtext", nullable: true, comment: "上次修改时间")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_business_division", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "business_division");
        }
    }
}
