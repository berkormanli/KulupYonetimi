using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KulupYonetimi.Migrations
{
    /// <inheritdoc />
    public partial class AddDuyuruGecerlilik : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "GecerlilikBitis",
                table: "Duyurular",
                type: "datetime(6)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GecerlilikBitis",
                table: "Duyurular");
        }
    }
}
