using BibliotecaEFCore.Datos;
using BibliotecaEFCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaEFCore.Controllers;

[ApiController]
[Route("api/autores")]
public class AutoresApiController(ApplicationDbContext contexto) : ControllerBase
{
    private readonly ApplicationDbContext _contexto = contexto;

    [HttpGet]
    public async Task<IActionResult> GetAutores()
    {
        var autores = await _contexto.Autores
            .Include(a => a.Libros)
            .ToListAsync();

        return Ok(autores);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAutor(int id)
    {
        var autor = await _contexto.Autores
            .Include(a => a.Libros)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (autor == null)
        {
            return NotFound(new { mensaje = $"Autor con ID {id} no encontrado." });
        }

        return Ok(autor);
    }

    [HttpPost]
    public async Task<IActionResult> CrearAutor(Autor autor)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _contexto.Autores.Add(autor);
        await _contexto.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetAutor),
            new { id = autor.Id },
            autor
        );
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> EditarAutor(int id, Autor autor)
    {
        if (id != autor.Id)
        {
            return BadRequest(new { mensaje = "El ID de la ruta no coincide con el ID del autor." });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var autorDb = await _contexto.Autores.FindAsync(id);
        if (autorDb == null)
        {
            return NotFound(new { mensaje = $"Autor con ID {id} no encontrado." });
        }

        autorDb.Nombre = autor.Nombre;

        await _contexto.SaveChangesAsync();

        return Ok(autorDb);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> EliminarAutor(int id)
    {
        var autor = await _contexto.Autores
            .Include(a => a.Libros)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (autor == null)
        {
            return NotFound(new { mensaje = $"Autor con ID {id} no encontrado." });
        }

        if (autor.Libros.Count != 0)
        {
            return Conflict(new { mensaje = "No se puede eliminar el autor porque tiene libros asociados." });
        }

        _contexto.Autores.Remove(autor);
        await _contexto.SaveChangesAsync();

        return NoContent();
    }
}
