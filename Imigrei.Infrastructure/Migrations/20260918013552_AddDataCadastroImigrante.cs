using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Imigrei.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDataCadastroImigrante : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DataCadastro",
                table: "imigrantes",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            // Cadastros anteriores a esta coluna não têm data de origem conhecida e
            // ficariam em 0001-01-01. Passam a valer a partir de agora, que é quando
            // o sistema de fato começou a registrar o dado.
            migrationBuilder.Sql(
                "UPDATE imigrantes SET DataCadastro = UTC_TIMESTAMP(6) " +
                "WHERE DataCadastro <= '0001-01-02 00:00:00';");

            migrationBuilder.CreateIndex(
                name: "IX_imigrantes_DataCadastro",
                table: "imigrantes",
                column: "DataCadastro");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_imigrantes_DataCadastro",
                table: "imigrantes");

            migrationBuilder.DropColumn(
                name: "DataCadastro",
                table: "imigrantes");
        }
    }
}
