using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Imigrei.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddHashArquivoImigrante : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImigranteArquivos_imigrantes_ImigranteId",
                table: "ImigranteArquivos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ImigranteArquivos",
                table: "ImigranteArquivos");

            migrationBuilder.RenameTable(
                name: "ImigranteArquivos",
                newName: "imigrante_arquivos");

            migrationBuilder.RenameIndex(
                name: "IX_ImigranteArquivos_ImigranteId",
                table: "imigrante_arquivos",
                newName: "IX_imigrante_arquivos_ImigranteId");

            migrationBuilder.AlterColumn<string>(
                name: "TipoArquivo",
                table: "imigrante_arquivos",
                type: "varchar(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "NomeOriginal",
                table: "imigrante_arquivos",
                type: "varchar(260)",
                maxLength: 260,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "NomeArmazenado",
                table: "imigrante_arquivos",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "ContentType",
                table: "imigrante_arquivos",
                type: "varchar(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "CaminhoRelativo",
                table: "imigrante_arquivos",
                type: "varchar(300)",
                maxLength: 300,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "HashSha256",
                table: "imigrante_arquivos",
                type: "varchar(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddPrimaryKey(
                name: "PK_imigrante_arquivos",
                table: "imigrante_arquivos",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_imigrante_arquivos_HashSha256",
                table: "imigrante_arquivos",
                column: "HashSha256");

            migrationBuilder.AddForeignKey(
                name: "FK_imigrante_arquivos_imigrantes_ImigranteId",
                table: "imigrante_arquivos",
                column: "ImigranteId",
                principalTable: "imigrantes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_imigrante_arquivos_imigrantes_ImigranteId",
                table: "imigrante_arquivos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_imigrante_arquivos",
                table: "imigrante_arquivos");

            migrationBuilder.DropIndex(
                name: "IX_imigrante_arquivos_HashSha256",
                table: "imigrante_arquivos");

            migrationBuilder.DropColumn(
                name: "HashSha256",
                table: "imigrante_arquivos");

            migrationBuilder.RenameTable(
                name: "imigrante_arquivos",
                newName: "ImigranteArquivos");

            migrationBuilder.RenameIndex(
                name: "IX_imigrante_arquivos_ImigranteId",
                table: "ImigranteArquivos",
                newName: "IX_ImigranteArquivos_ImigranteId");

            migrationBuilder.AlterColumn<int>(
                name: "TipoArquivo",
                table: "ImigranteArquivos",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(30)",
                oldMaxLength: 30)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "NomeOriginal",
                table: "ImigranteArquivos",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(260)",
                oldMaxLength: 260)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "NomeArmazenado",
                table: "ImigranteArquivos",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "ContentType",
                table: "ImigranteArquivos",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(150)",
                oldMaxLength: 150)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "CaminhoRelativo",
                table: "ImigranteArquivos",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(300)",
                oldMaxLength: 300)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ImigranteArquivos",
                table: "ImigranteArquivos",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ImigranteArquivos_imigrantes_ImigranteId",
                table: "ImigranteArquivos",
                column: "ImigranteId",
                principalTable: "imigrantes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
