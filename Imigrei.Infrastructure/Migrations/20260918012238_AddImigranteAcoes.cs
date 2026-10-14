using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Imigrei.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddImigranteAcoes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "imigrante_acoes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ImigranteId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Acao = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TipoAcao = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DataRegistro = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_imigrante_acoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_imigrante_acoes_imigrantes_ImigranteId",
                        column: x => x.ImigranteId,
                        principalTable: "imigrantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_imigrante_acoes_ImigranteId_DataRegistro",
                table: "imigrante_acoes",
                columns: new[] { "ImigranteId", "DataRegistro" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "imigrante_acoes");
        }
    }
}
