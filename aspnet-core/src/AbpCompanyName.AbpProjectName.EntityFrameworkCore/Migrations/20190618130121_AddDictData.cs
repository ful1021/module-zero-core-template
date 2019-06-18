using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AbpCompanyName.AbpProjectName.Migrations
{
    public partial class AddDictData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Core_DataDictionaries",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    Code = table.Column<string>(maxLength: 128, nullable: true),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    FullName = table.Column<string>(nullable: true),
                    NameTextType = table.Column<int>(nullable: false),
                    Sort = table.Column<int>(nullable: false),
                    ExtensionData = table.Column<string>(nullable: true),
                    Level = table.Column<int>(nullable: false),
                    ParentId = table.Column<int>(nullable: true),
                    TypeCode = table.Column<string>(maxLength: 128, nullable: true),
                    TypeName = table.Column<string>(maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Core_DataDictionaries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Core_DataDictionaries_Core_DataDictionaries_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Core_DataDictionaries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Core_ExtendColumns",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TableName = table.Column<int>(nullable: false),
                    Key = table.Column<string>(maxLength: 128, nullable: true),
                    Title = table.Column<string>(maxLength: 256, nullable: true),
                    Width = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Core_ExtendColumns", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Core_DataDictionaries_ParentId",
                table: "Core_DataDictionaries",
                column: "ParentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Core_DataDictionaries");

            migrationBuilder.DropTable(
                name: "Core_ExtendColumns");
        }
    }
}
