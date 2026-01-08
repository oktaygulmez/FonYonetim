using Microsoft.EntityFrameworkCore.Migrations;

namespace FonYonetim.Migrations
{
    public partial class nasdaqAddMig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Nasdaqs",
                columns: table => new
                {
                    NasdaqMarketID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NasdaqMarketSymbol = table.Column<string>(nullable: true),
                    NasdaqMarketPrice = table.Column<double>(nullable: false),
                    NasdaqMarketTargetPrice = table.Column<double>(nullable: false),
                    NasdaqMarketTargetPricePercent = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Nasdaqs", x => x.NasdaqMarketID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Nasdaqs");
        }
    }
}
