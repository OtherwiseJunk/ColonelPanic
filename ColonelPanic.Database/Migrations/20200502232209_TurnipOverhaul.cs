using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ColonelPanic.Database.Migrations
{
    public partial class TurnipOverhaul : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TurnipBuyPrice",
                table: "AC_Towns");

            migrationBuilder.DropColumn(
                name: "TurnipSellPrice",
                table: "AC_Towns");

            migrationBuilder.CreateTable(
                name: "BuyPrices",
                columns: table => new
                {
                    BuyPriceId = table.Column<double>(nullable: false),
                    Price = table.Column<int>(nullable: false),
                    Timestamp = table.Column<DateTime>(nullable: false),
                    TownId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuyPrices", x => x.BuyPriceId);
                    table.ForeignKey(
                        name: "FK_BuyPrices_AC_Towns_TownId",
                        column: x => x.TownId,
                        principalTable: "AC_Towns",
                        principalColumn: "TownId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SellPrices",
                columns: table => new
                {
                    SellPriceId = table.Column<double>(nullable: false),
                    Price = table.Column<int>(nullable: false),
                    Timestamp = table.Column<DateTime>(nullable: false),
                    IsMorningPrice = table.Column<bool>(nullable: false),
                    TownId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SellPrices", x => x.SellPriceId);
                    table.ForeignKey(
                        name: "FK_SellPrices_AC_Towns_TownId",
                        column: x => x.TownId,
                        principalTable: "AC_Towns",
                        principalColumn: "TownId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BuyPrices_TownId",
                table: "BuyPrices",
                column: "TownId");

            migrationBuilder.CreateIndex(
                name: "IX_SellPrices_TownId",
                table: "SellPrices",
                column: "TownId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BuyPrices");

            migrationBuilder.DropTable(
                name: "SellPrices");

            migrationBuilder.AddColumn<int>(
                name: "TurnipBuyPrice",
                table: "AC_Towns",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TurnipSellPrice",
                table: "AC_Towns",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
