using Microsoft.EntityFrameworkCore.Migrations;

namespace ColonelPanic.Database.Migrations
{
    public partial class AddNativeFruitBorderStatusAndWishlist : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "BorderOpen",
                table: "AC_Towns",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DodoCode",
                table: "AC_Towns",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NativeFruit",
                table: "AC_Towns",
                nullable: true);

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
                name: "IX_AC_Wishlist_Items_TownId",
                table: "AC_Wishlist_Items",
                column: "TownId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AC_Wishlist_Items");

            migrationBuilder.DropColumn(
                name: "BorderOpen",
                table: "AC_Towns");

            migrationBuilder.DropColumn(
                name: "DodoCode",
                table: "AC_Towns");

            migrationBuilder.DropColumn(
                name: "NativeFruit",
                table: "AC_Towns");
        }
    }
}
