using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BibliotecaEFCore.Datos;
using BibliotecaEFCore.Models;

namespace BibliotecaEFCore.Controllers;

public class HomeController(ApplicationDbContext contexto) : Controller
{
    private readonly ApplicationDbContext _contexto = contexto;

    public async Task<IActionResult> Index()
    {
        ViewBag.TotalLibros = await _contexto.Libros.CountAsync();
        ViewBag.TotalAutores = await _contexto.Autores.CountAsync();
        ViewBag.TotalCategorias = await _contexto.Categorias.CountAsync();
        ViewBag.TotalUsuarios = await _contexto.Usuarios.CountAsync();
        ViewBag.PrestamosPendientes = await _contexto.Prestamos.CountAsync(p => p.FechaDevolucion == null);
        ViewBag.PrestamosDevueltos = await _contexto.Prestamos.CountAsync(p => p.FechaDevolucion != null);

        var ultimosPrestamos = await _contexto.Prestamos
            .Include(p => p.Usuario)
            .Include(p => p.Libro)
            .OrderByDescending(p => p.FechaPrestamo)
            .Take(5)
            .ToListAsync();

        return View(ultimosPrestamos);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
