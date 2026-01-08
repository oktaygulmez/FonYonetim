using Microsoft.EntityFrameworkCore.Migrations;

namespace FonYonetim.Migrations
{
    public partial class addBistDollarTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BistDolars",
                columns: table => new
                {
                    BistMarketID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BistMarketSymbol = table.Column<string>(nullable: true),
                    BistMarketPriceDolar = table.Column<double>(nullable: false),
                    BistMarketTargetPriceDolar = table.Column<double>(nullable: false),
                    BistMarketTargetPricePercent = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BistDolars", x => x.BistMarketID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BistDolars");
        }
    }
}
