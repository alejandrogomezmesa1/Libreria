using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BibliotecaEFCore.Migrations;

/// <inheritdoc />
public partial class MigracionInicial : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Autores",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_Autores", x => x.Id));

        migrationBuilder.CreateTable(
            name: "Categorias",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_Categorias", x => x.Id));

        migrationBuilder.CreateTable(
            name: "Usuarios",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_Usuarios", x => x.Id));

        migrationBuilder.CreateTable(
            name: "Libros",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Titulo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                AutorId = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Libros", x => x.Id);
                table.ForeignKey(
                    name: "FK_Libros_Autores_AutorId",
                    column: x => x.AutorId,
                    principalTable: "Autores",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "TarjetasBiblioteca",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Codigo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                UsuarioId = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_TarjetasBiblioteca", x => x.Id);
                table.ForeignKey(
                    name: "FK_TarjetasBiblioteca_Usuarios_UsuarioId",
                    column: x => x.UsuarioId,
                    principalTable: "Usuarios",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "CategoriaLibro",
            columns: table => new
            {
                CategoriasId = table.Column<int>(type: "int", nullable: false),
                LibrosId = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CategoriaLibro", x => new { x.CategoriasId, x.LibrosId });
                table.ForeignKey(
                    name: "FK_CategoriaLibro_Categorias_CategoriasId",
                    column: x => x.CategoriasId,
                    principalTable: "Categorias",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_CategoriaLibro_Libros_LibrosId",
                    column: x => x.LibrosId,
                    principalTable: "Libros",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "Prestamos",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                UsuarioId = table.Column<int>(type: "int", nullable: false),
                LibroId = table.Column<int>(type: "int", nullable: false),
                FechaPrestamo = table.Column<DateTime>(type: "datetime2", nullable: false),
                FechaDevolucion = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Prestamos", x => x.Id);
                table.ForeignKey(
                    name: "FK_Prestamos_Libros_LibroId",
                    column: x => x.LibroId,
                    principalTable: "Libros",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_Prestamos_Usuarios_UsuarioId",
                    column: x => x.UsuarioId,
                    principalTable: "Usuarios",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_CategoriaLibro_LibrosId",
            table: "CategoriaLibro",
            column: "LibrosId");

        migrationBuilder.CreateIndex(
            name: "IX_Libros_AutorId",
            table: "Libros",
            column: "AutorId");

        migrationBuilder.CreateIndex(
            name: "IX_Prestamos_LibroId",
            table: "Prestamos",
            column: "LibroId");

        migrationBuilder.CreateIndex(
            name: "IX_Prestamos_UsuarioId",
            table: "Prestamos",
            column: "UsuarioId");

        migrationBuilder.CreateIndex(
            name: "IX_TarjetasBiblioteca_UsuarioId",
            table: "TarjetasBiblioteca",
            column: "UsuarioId",
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "CategoriaLibro");

        migrationBuilder.DropTable(
            name: "Prestamos");

        migrationBuilder.DropTable(
            name: "TarjetasBiblioteca");

        migrationBuilder.DropTable(
            name: "Categorias");

        migrationBuilder.DropTable(
            name: "Libros");

        migrationBuilder.DropTable(
            name: "Usuarios");

        migrationBuilder.DropTable(
            name: "Autores");
    }
}
