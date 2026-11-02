using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AEIG.Domain.Data.Migrations
{
    /// <inheritdoc />
    public partial class CobrancaValorDataVencimento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "DataVencimento",
                table: "Cobranca",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<decimal>(
                name: "Valor",
                table: "Cobranca",
                type: "TEXT",
                precision: 10,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ValorPago",
                table: "Cobranca",
                type: "TEXT",
                precision: 10,
                scale: 2,
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataVencimento",
                table: "Cobranca");

            migrationBuilder.DropColumn(
                name: "Valor",
                table: "Cobranca");

            migrationBuilder.DropColumn(
                name: "ValorPago",
                table: "Cobranca");
        }
    }
}
