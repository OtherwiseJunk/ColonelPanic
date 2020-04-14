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
                    TurnipSellPrice = table.Column<int>(nullable: false),
                    TurnipBuyPrice = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AC_Towns", x => x.TownId);
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

            migrationBuilder.CreateIndex(
                name: "IX_AC_Fruits_TownId",
                table: "AC_Fruits",
                column: "TownId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AC_Fruits");

            migrationBuilder.DropTable(
                name: "AC_Towns");
        }
    }
}
