using BibliotecaEFCore.Datos;
using BibliotecaEFCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaEFCore.Controllers;

[ApiController]
[Route("api/tarjetas")]
public class TarjetasBibliotecaApiController(ApplicationDbContext contexto) : ControllerBase
{
    private readonly ApplicationDbContext _contexto = contexto;

    [HttpGet]
    public async Task<IActionResult> GetTarjetas()
    {
        var tarjetas = await _contexto.TarjetasBiblioteca
            .Include(t => t.Usuario)
            .ToListAsync();

        return Ok(tarjetas);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTarjeta(int id)
    {
        var tarjeta = await _contexto.TarjetasBiblioteca
            .Include(t => t.Usuario)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (tarjeta == null)
        {
            return NotFound(new { mensaje = $"Tarjeta con ID {id} no encontrada." });
        }

        return Ok(tarjeta);
    }

    [HttpPost]
    public async Task<IActionResult> CrearTarjeta(TarjetaBiblioteca tarjeta)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var usuarioExiste = await _contexto.Usuarios.AnyAsync(u => u.Id == tarjeta.UsuarioId);
        if (!usuarioExiste)
        {
            return BadRequest(new { mensaje = $"No existe ningún usuario con el ID {tarjeta.UsuarioId}." });
        }

        var tarjetaExistente = await _contexto.TarjetasBiblioteca.AnyAsync(t => t.UsuarioId == tarjeta.UsuarioId);
        if (tarjetaExistente)
        {
            return Conflict(new { mensaje = "El usuario especificado ya cuenta con una tarjeta de biblioteca." });
        }

        _contexto.TarjetasBiblioteca.Add(tarjeta);
        await _contexto.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetTarjeta),
            new { id = tarjeta.Id },
            tarjeta
        );
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> EditarTarjeta(int id, TarjetaBiblioteca tarjeta)
    {
        if (id != tarjeta.Id)
        {
            return BadRequest(new { mensaje = "El ID de la ruta no coincide con el ID de la tarjeta." });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var tarjetaDb = await _contexto.TarjetasBiblioteca.FindAsync(id);
        if (tarjetaDb == null)
        {
            return NotFound(new { mensaje = $"Tarjeta con ID {id} no encontrada." });
        }

        if (tarjetaDb.UsuarioId != tarjeta.UsuarioId)
        {
            var usuarioExiste = await _contexto.Usuarios.AnyAsync(u => u.Id == tarjeta.UsuarioId);
            if (!usuarioExiste)
            {
                return BadRequest(new { mensaje = $"No existe ningún usuario con el ID {tarjeta.UsuarioId}." });
            }

            var otraTarjeta = await _contexto.TarjetasBiblioteca.AnyAsync(t => t.UsuarioId == tarjeta.UsuarioId && t.Id != id);
            if (otraTarjeta)
            {
                return Conflict(new { mensaje = "El nuevo usuario especificado ya tiene otra tarjeta de biblioteca." });
            }

            tarjetaDb.UsuarioId = tarjeta.UsuarioId;
        }

        tarjetaDb.Codigo = tarjeta.Codigo;

        await _contexto.SaveChangesAsync();

        return Ok(tarjetaDb);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> EliminarTarjeta(int id)
    {
        var tarjeta = await _contexto.TarjetasBiblioteca.FindAsync(id);

        if (tarjeta == null)
        {
            return NotFound(new { mensaje = $"Tarjeta con ID {id} no encontrada." });
        }

        _contexto.TarjetasBiblioteca.Remove(tarjeta);
        await _contexto.SaveChangesAsync();

        return NoContent();
    }
}
