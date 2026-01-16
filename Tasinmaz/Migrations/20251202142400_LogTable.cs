using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tasinmaz.Migrations
{
    /// <inheritdoc />
    public partial class LogTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // YENİ LOG TABLOSUNU OLUŞTURMA BAŞLANGIÇ
            migrationBuilder.CreateTable(
                name: "Log",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", Npgsql.EntityFrameworkCore.PostgreSQL.Metadata.NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    OperationType = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserIp = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Log", x => x.Id);
                    // Kullanıcıya Foreign Key eklenmesi (User tablosu ile ilişki)
                    table.ForeignKey(
                        name: "FK_Log_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User", // Kullanıcı tablosunun adı
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Index eklenmesi (Foreign Key için iyi bir pratiktir)
            migrationBuilder.CreateIndex(
                name: "IX_Log_UserId",
                table: "Log",
                column: "UserId");
            // YENİ LOG TABLOSUNU OLUŞTURMA SON
        }
    }
}
