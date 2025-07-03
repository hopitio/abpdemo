using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AbpAngular.Migrations
{
    /// <inheritdoc />
    public partial class AddSuppliersColumnToBooks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Suppliers",
                table: "AppBooks",
                type: "varchar(512)",
                maxLength: 512,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Suppliers",
                table: "AppBooks");
        }
    }
}
