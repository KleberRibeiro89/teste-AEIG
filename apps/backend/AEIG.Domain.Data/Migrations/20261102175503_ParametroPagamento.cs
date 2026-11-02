using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AEIG.Domain.Data.Migrations
{
    /// <inheritdoc />
    public partial class ParametroPagamento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ParametroPagamento",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MultaPercentual = table.Column<decimal>(type: "TEXT", precision: 10, scale: 6, nullable: false),
                    JurosDiarioPercentual = table.Column<decimal>(type: "TEXT", precision: 10, scale: 6, nullable: false),
                    NavigationId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    Active = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParametroPagamento", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ParametroPagamento_NavigationId",
                table: "ParametroPagamento",
                column: "NavigationId",
                unique: true);

            migrationBuilder.InsertData(
                table: "ParametroPagamento",
                columns: new[]
                {
                    "MultaPercentual",
                    "JurosDiarioPercentual",
                    "NavigationId",
                    "CreatedAt",
                    "CreatedBy",
                    "Active"
                },
                values: new object[]
                {
                    2m,
                    0.033m,
                    new Guid("8f4e2c1a-6b3d-4a9e-9c71-2d5b7e0a4f18"),
                    new DateTime(2026, 11, 2, 0, 0, 0, DateTimeKind.Utc),
                    Guid.Empty,
                    true
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ParametroPagamento");
        }
    }
}
