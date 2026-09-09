using BibliotecaEFCore.Datos;
using BibliotecaEFCore.Models;

namespace BibliotecaEFCore.Data;

public static class SeedData
{
    public static void Poblar(ApplicationDbContext context)
    {
        // Evitar duplicados si ya existen datos
        if (context.Autores.Any())
        {
            return;
        }

        // 1. Crear Autores
        var autor1 = new Autor { Nombre = "Gabriel García Márquez" };
        var autor2 = new Autor { Nombre = "Mario Vargas Llosa" };
        var autor3 = new Autor { Nombre = "Isabel Allende" };
        var autor4 = new Autor { Nombre = "Jorge Luis Borges" };

        context.Autores.AddRange(autor1, autor2, autor3, autor4);

        // 2. Crear Categorías
        var catNovela = new Categoria { Nombre = "Novela" };
        var catRealismo = new Categoria { Nombre = "Realismo Mágico" };
        var catFiccion = new Categoria { Nombre = "Ciencia Ficción" };
        var catHistoria = new Categoria { Nombre = "Historia" };
        var catFantasia = new Categoria { Nombre = "Fantasía" };

        context.Categorias.AddRange(catNovela, catRealismo, catFiccion, catHistoria, catFantasia);

        // 3. Crear Libros y asociar con Autor y Categorías (N:N)
        var libro1 = new Libro
        {
            Titulo = "Cien años de soledad",
            Autor = autor1,
            Categorias = [catNovela, catRealismo]
        };

        var libro2 = new Libro
        {
            Titulo = "El amor en los tiempos del cólera",
            Autor = autor1,
            Categorias = [catNovela]
        };

        var libro3 = new Libro
        {
            Titulo = "La ciudad y los perros",
            Autor = autor2,
            Categorias = [catNovela, catFiccion]
        };

        var libro4 = new Libro
        {
            Titulo = "La casa de los espíritus",
            Autor = autor3,
            Categorias = [catNovela, catRealismo]
        };

        var libro5 = new Libro
        {
            Titulo = "Ficciones",
            Autor = autor4,
            Categorias = [catFiccion, catFantasia]
        };

        context.Libros.AddRange(libro1, libro2, libro3, libro4, libro5);

        // 4. Crear Usuarios con Tarjetas de Biblioteca (1:1)
        var usuario1 = new Usuario
        {
            Nombre = "Carlos Gómez",
            Tarjeta = new TarjetaBiblioteca { Codigo = "TB-001" }
        };

        var usuario2 = new Usuario
        {
            Nombre = "María Rodríguez",
            Tarjeta = new TarjetaBiblioteca { Codigo = "TB-002" }
        };

        var usuario3 = new Usuario
        {
            Nombre = "Juan Pérez",
            Tarjeta = new TarjetaBiblioteca { Codigo = "TB-003" }
        };

        context.Usuarios.AddRange(usuario1, usuario2, usuario3);

        // 5. Crear Préstamos
        var prestamo1 = new Prestamo
        {
            Usuario = usuario1,
            Libro = libro1,
            FechaPrestamo = DateTime.Today.AddDays(-7),
            FechaDevolucion = null // Pendiente
        };

        var prestamo2 = new Prestamo
        {
            Usuario = usuario2,
            Libro = libro3,
            FechaPrestamo = DateTime.Today.AddDays(-14),
            FechaDevolucion = DateTime.Today.AddDays(-2) // Devuelto
        };

        var prestamo3 = new Prestamo
        {
            Usuario = usuario3,
            Libro = libro5,
            FechaPrestamo = DateTime.Today.AddDays(-2),
            FechaDevolucion = null // Pendiente
        };

        context.Prestamos.AddRange(prestamo1, prestamo2, prestamo3);

        // Guardar cambios en la base de datos
        context.SaveChanges();
    }
}
