using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AEIG.Domain.Data.Migrations
{
    /// <inheritdoc />
    public partial class AvisoPagamento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AvisoPagamento",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IdEvento = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    NossoNumero = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    DataPagamento = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    NomeStatus = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    ValorPago = table.Column<decimal>(type: "TEXT", precision: 10, scale: 2, nullable: false),
                    Mensagem = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    NavigationId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    Active = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AvisoPagamento", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AvisoPagamento_IdEvento",
                table: "AvisoPagamento",
                column: "IdEvento",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AvisoPagamento_NavigationId",
                table: "AvisoPagamento",
                column: "NavigationId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AvisoPagamento");
        }
    }
}
