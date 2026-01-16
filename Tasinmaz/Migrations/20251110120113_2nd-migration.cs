using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Tasinmaz.Migrations
{
    /// <inheritdoc />
    public partial class _2ndmigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Mahalle",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Ad = table.Column<string>(type: "text", nullable: false),
                    IlceId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mahalle", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Mahalle_Ilceler_IlceId",
                        column: x => x.IlceId,
                        principalTable: "Ilceler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tasinmaz",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Ada = table.Column<string>(type: "text", nullable: false),
                    Parsel = table.Column<string>(type: "text", nullable: false),
                    Nitelik = table.Column<string>(type: "text", nullable: true),
                    MahalleId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tasinmaz", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tasinmaz_Mahalle_MahalleId",
                        column: x => x.MahalleId,
                        principalTable: "Mahalle",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Mahalle_IlceId",
                table: "Mahalle",
                column: "IlceId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasinmaz_MahalleId",
                table: "Tasinmaz",
                column: "MahalleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tasinmaz");

            migrationBuilder.DropTable(
                name: "Mahalle");
        }
    }
}
