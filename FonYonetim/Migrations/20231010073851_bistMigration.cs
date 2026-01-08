using Microsoft.EntityFrameworkCore.Migrations;

namespace FonYonetim.Migrations
{
    public partial class bistMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Bists",
                columns: table => new
                {
                    BistMarketID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BistMarketSymbol = table.Column<string>(nullable: true),
                    BistMarketPrice = table.Column<double>(nullable: false),
                    BistMarketTargetPrice = table.Column<double>(nullable: false),
                    BistMarketTargetPricePercent = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bists", x => x.BistMarketID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bists");
        }
    }
}
