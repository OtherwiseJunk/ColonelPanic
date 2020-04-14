using Microsoft.EntityFrameworkCore.Migrations;

namespace ColonelPanic.Database.Migrations
{
    public partial class AddHemisphereString : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "NorthernHempisphere",
                table: "AC_Towns",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "NorthernHempisphere",
                table: "AC_Towns",
                type: "bit",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
