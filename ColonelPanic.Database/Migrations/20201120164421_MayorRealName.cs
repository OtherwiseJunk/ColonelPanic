using Microsoft.EntityFrameworkCore.Migrations;

namespace ColonelPanic.Database.Migrations
{
    public partial class MayorRealName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MayorRealName",
                table: "AC_Towns",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MayorRealName",
                table: "AC_Towns");
        }
    }
}
