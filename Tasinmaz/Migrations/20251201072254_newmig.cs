using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tasinmaz.Migrations
{
    /// <inheritdoc />
    public partial class newmig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PropertyType",
                table: "Tasinmaz");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PropertyType",
                table: "Tasinmaz",
                type: "text",
                nullable: false, // KRİTİK: Zorunlu (NOT NULL)
                defaultValue: "");
        }
    }
}
