using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ColonelPanic.Database.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AC_Towns",
                columns: table => new
                {
                    TownId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MayorDiscordId = table.Column<decimal>(nullable: false),
                    TownName = table.Column<string>(nullable: true),
                    NativeFruit = table.Column<string>(nullable: true),
                    DodoCode = table.Column<string>(nullable: true),
                    BorderOpen = table.Column<bool>(nullable: false),
                    NorthernHempisphere = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AC_Towns", x => x.TownId);
                });

            migrationBuilder.CreateTable(
                name: "AC_BuyPrices",
                columns: table => new
                {
                    BuyPriceId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Price = table.Column<int>(nullable: false),
                    Timestamp = table.Column<DateTime>(nullable: false),
                    TownId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AC_BuyPrices", x => x.BuyPriceId);
                    table.ForeignKey(
                        name: "FK_AC_BuyPrices_AC_Towns_TownId",
                        column: x => x.TownId,
                        principalTable: "AC_Towns",
                        principalColumn: "TownId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AC_Fruits",
                columns: table => new
                {
                    FruitId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FruitName = table.Column<string>(nullable: true),
                    TownId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AC_Fruits", x => x.FruitId);
                    table.ForeignKey(
                        name: "FK_AC_Fruits_AC_Towns_TownId",
                        column: x => x.TownId,
                        principalTable: "AC_Towns",
                        principalColumn: "TownId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AC_SellPrices",
                columns: table => new
                {
                    SellPriceId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Price = table.Column<int>(nullable: false),
                    Timestamp = table.Column<DateTime>(nullable: false),
                    IsMorningPrice = table.Column<bool>(nullable: false),
                    TownId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AC_SellPrices", x => x.SellPriceId);
                    table.ForeignKey(
                        name: "FK_AC_SellPrices_AC_Towns_TownId",
                        column: x => x.TownId,
                        principalTable: "AC_Towns",
                        principalColumn: "TownId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AC_Wishlist_Items",
                columns: table => new
                {
                    ItemNum = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemName = table.Column<string>(nullable: true),
                    TownId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AC_Wishlist_Items", x => x.ItemNum);
                    table.ForeignKey(
                        name: "FK_AC_Wishlist_Items_AC_Towns_TownId",
                        column: x => x.TownId,
                        principalTable: "AC_Towns",
                        principalColumn: "TownId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AC_BuyPrices_TownId",
                table: "AC_BuyPrices",
                column: "TownId");

            migrationBuilder.CreateIndex(
                name: "IX_AC_Fruits_TownId",
                table: "AC_Fruits",
                column: "TownId");

            migrationBuilder.CreateIndex(
                name: "IX_AC_SellPrices_TownId",
                table: "AC_SellPrices",
                column: "TownId");

            migrationBuilder.CreateIndex(
                name: "IX_AC_Wishlist_Items_TownId",
                table: "AC_Wishlist_Items",
                column: "TownId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AC_BuyPrices");

            migrationBuilder.DropTable(
                name: "AC_Fruits");

            migrationBuilder.DropTable(
                name: "AC_SellPrices");

            migrationBuilder.DropTable(
                name: "AC_Wishlist_Items");

            migrationBuilder.DropTable(
                name: "AC_Towns");
        }
    }
}
