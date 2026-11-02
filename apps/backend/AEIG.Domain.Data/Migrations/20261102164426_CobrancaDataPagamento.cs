using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AEIG.Domain.Data.Migrations
{
    /// <inheritdoc />
    public partial class CobrancaDataPagamento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "DataPagamento",
                table: "Cobranca",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataPagamento",
                table: "Cobranca");
        }
    }
}
