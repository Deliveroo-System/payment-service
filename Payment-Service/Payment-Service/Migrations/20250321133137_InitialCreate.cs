using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Payment_Service.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PaymentId = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Currency = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false),
                    Status = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Payments__3214EC07D22DF512", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Payments");
        }
    }
}
