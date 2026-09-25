using BibliotecaEFCore.Datos;
using BibliotecaEFCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaEFCore.Controllers;

[ApiController]
[Route("api/libros")]
public class LibrosApiController(ApplicationDbContext contexto) : ControllerBase
{
    private readonly ApplicationDbContext _contexto = contexto;

    [HttpGet]
    public async Task<IActionResult> GetLibros()
    {
        var libros = await _contexto.Libros
            .Include(l => l.Autor)
            .Include(l => l.Categorias)
            .ToListAsync();

        return Ok(libros);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetLibro(int id)
    {
        var libro = await _contexto.Libros
            .Include(l => l.Autor)
            .Include(l => l.Categorias)
            .FirstOrDefaultAsync(l => l.Id == id);

        if (libro == null)
        {
            return NotFound(new { mensaje = $"Libro con ID {id} no encontrado." });
        }

        return Ok(libro);
    }

    [HttpPost]
    public async Task<IActionResult> CrearLibro(Libro libro)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var autorExiste = await _contexto.Autores.AnyAsync(a => a.Id == libro.AutorId);
        if (!autorExiste)
        {
            return BadRequest(new { mensaje = $"No existe ningún autor con el ID {libro.AutorId}." });
        }

        _contexto.Libros.Add(libro);
        await _contexto.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetLibro),
            new { id = libro.Id },
            libro
        );
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> EditarLibro(int id, Libro libro)
    {
        if (id != libro.Id)
        {
            return BadRequest(new { mensaje = "El ID de la ruta no coincide con el ID del libro." });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var libroDb = await _contexto.Libros.FindAsync(id);
        if (libroDb == null)
        {
            return NotFound(new { mensaje = $"Libro con ID {id} no encontrado." });
        }

        var autorExiste = await _contexto.Autores.AnyAsync(a => a.Id == libro.AutorId);
        if (!autorExiste)
        {
            return BadRequest(new { mensaje = $"No existe ningún autor con el ID {libro.AutorId}." });
        }

        libroDb.Titulo = libro.Titulo;
        libroDb.AutorId = libro.AutorId;

        await _contexto.SaveChangesAsync();

        return Ok(libroDb);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> EliminarLibro(int id)
    {
        var libro = await _contexto.Libros.FindAsync(id);
        if (libro == null)
        {
            return NotFound(new { mensaje = $"Libro con ID {id} no encontrado." });
        }

        _contexto.Libros.Remove(libro);
        await _contexto.SaveChangesAsync();

        return NoContent();
    }
}
