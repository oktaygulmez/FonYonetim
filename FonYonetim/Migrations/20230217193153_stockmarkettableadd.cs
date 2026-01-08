using Microsoft.EntityFrameworkCore.Migrations;

namespace FonYonetim.Migrations
{
    public partial class stockmarkettableadd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Portfolios");

            migrationBuilder.DropTable(
                name: "WatchLists");

            migrationBuilder.CreateTable(
                name: "StockMarkets",
                columns: table => new
                {
                    StockMarketID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StockMarketSymbol = table.Column<string>(nullable: true),
                    StockMarketPrice = table.Column<double>(nullable: false),
                    StockMarketTargetPrice = table.Column<double>(nullable: false),
                    StockMarketTargetPricePercent = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockMarkets", x => x.StockMarketID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StockMarkets");

            migrationBuilder.CreateTable(
                name: "Portfolios",
                columns: table => new
                {
                    ProductID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductPrice = table.Column<double>(type: "float", nullable: false),
                    ProductSymbol = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductTargetPrice = table.Column<double>(type: "float", nullable: false),
                    ProductTargetPricePercent = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Portfolios", x => x.ProductID);
                });

            migrationBuilder.CreateTable(
                name: "WatchLists",
                columns: table => new
                {
                    ProductID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductPrice = table.Column<double>(type: "float", nullable: false),
                    ProductSymbol = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductTargetPrice = table.Column<double>(type: "float", nullable: false),
                    ProductTargetPricePercent = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WatchLists", x => x.ProductID);
                });
        }
    }
}
