using Microsoft.EntityFrameworkCore.Migrations;

namespace Application.Migrations;

public partial class AddsTodo : Migration
{
    protected override void Down(MigrationBuilder migrationBuilder) => migrationBuilder.DropTable(name: "Todo");

    protected override void Up(MigrationBuilder migrationBuilder) => migrationBuilder.CreateTable(
            name: "Todo",
            columns: table => new
            {
                Id = table.Column<string>(type: "varchar(36)", unicode: false, maxLength: 36, nullable: false),
                Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                IsDone = table.Column<bool>(type: "bit", nullable: false),
            },
            constraints: table => table.PrimaryKey("PK_Todo", x => x.Id));
}
