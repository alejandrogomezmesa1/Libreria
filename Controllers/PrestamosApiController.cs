using BibliotecaEFCore.Datos;
using BibliotecaEFCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaEFCore.Controllers;

[ApiController]
[Route("api/prestamos")]
public class PrestamosApiController(ApplicationDbContext contexto) : ControllerBase
{
    private readonly ApplicationDbContext _contexto = contexto;

    [HttpGet]
    public async Task<IActionResult> GetPrestamos()
    {
        var prestamos = await _contexto.Prestamos
            .Include(p => p.Usuario)
            .Include(p => p.Libro)
            .ToListAsync();

        return Ok(prestamos);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPrestamo(int id)
    {
        var prestamo = await _contexto.Prestamos
            .Include(p => p.Usuario)
            .Include(p => p.Libro)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (prestamo == null)
        {
            return NotFound(new { mensaje = $"Préstamo con ID {id} no encontrado." });
        }

        return Ok(prestamo);
    }

    [HttpPost]
    public async Task<IActionResult> CrearPrestamo(Prestamo prestamo)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var usuarioExiste = await _contexto.Usuarios.AnyAsync(u => u.Id == prestamo.UsuarioId);
        if (!usuarioExiste)
        {
            return BadRequest(new { mensaje = $"No existe ningún usuario con el ID {prestamo.UsuarioId}." });
        }

        var libroExiste = await _contexto.Libros.AnyAsync(l => l.Id == prestamo.LibroId);
        if (!libroExiste)
        {
            return BadRequest(new { mensaje = $"No existe ningún libro con el ID {prestamo.LibroId}." });
        }

        _contexto.Prestamos.Add(prestamo);
        await _contexto.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetPrestamo),
            new { id = prestamo.Id },
            prestamo
        );
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> EditarPrestamo(int id, Prestamo prestamo)
    {
        if (id != prestamo.Id)
        {
            return BadRequest(new { mensaje = "El ID de la ruta no coincide con el ID del préstamo." });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var prestamoDb = await _contexto.Prestamos.FindAsync(id);
        if (prestamoDb == null)
        {
            return NotFound(new { mensaje = $"Préstamo con ID {id} no encontrado." });
        }

        var usuarioExiste = await _contexto.Usuarios.AnyAsync(u => u.Id == prestamo.UsuarioId);
        if (!usuarioExiste)
        {
            return BadRequest(new { mensaje = $"No existe ningún usuario con el ID {prestamo.UsuarioId}." });
        }

        var libroExiste = await _contexto.Libros.AnyAsync(l => l.Id == prestamo.LibroId);
        if (!libroExiste)
        {
            return BadRequest(new { mensaje = $"No existe ningún libro con el ID {prestamo.LibroId}." });
        }

        prestamoDb.UsuarioId = prestamo.UsuarioId;
        prestamoDb.LibroId = prestamo.LibroId;
        prestamoDb.FechaPrestamo = prestamo.FechaPrestamo;
        prestamoDb.FechaDevolucion = prestamo.FechaDevolucion;

        await _contexto.SaveChangesAsync();

        return Ok(prestamoDb);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> EliminarPrestamo(int id)
    {
        var prestamo = await _contexto.Prestamos.FindAsync(id);

        if (prestamo == null)
        {
            return NotFound(new { mensaje = $"Préstamo con ID {id} no encontrado." });
        }

        _contexto.Prestamos.Remove(prestamo);
        await _contexto.SaveChangesAsync();

        return NoContent();
    }
}
