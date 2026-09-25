using BibliotecaEFCore.Datos;
using BibliotecaEFCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaEFCore.Controllers;

[ApiController]
[Route("api/usuarios")]
public class UsuariosApiController(ApplicationDbContext contexto) : ControllerBase
{
    private readonly ApplicationDbContext _contexto = contexto;

    [HttpGet]
    public async Task<IActionResult> GetUsuarios()
    {
        var usuarios = await _contexto.Usuarios
            .Include(u => u.Tarjeta)
            .Include(u => u.Prestamos)
            .ToListAsync();

        return Ok(usuarios);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUsuario(int id)
    {
        var usuario = await _contexto.Usuarios
            .Include(u => u.Tarjeta)
            .Include(u => u.Prestamos)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (usuario == null)
        {
            return NotFound(new { mensaje = $"Usuario con ID {id} no encontrado." });
        }

        return Ok(usuario);
    }

    [HttpPost]
    public async Task<IActionResult> CrearUsuario(Usuario usuario)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _contexto.Usuarios.Add(usuario);
        await _contexto.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetUsuario),
            new { id = usuario.Id },
            usuario
        );
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> EditarUsuario(int id, Usuario usuario)
    {
        if (id != usuario.Id)
        {
            return BadRequest(new { mensaje = "El ID de la ruta no coincide con el ID del usuario." });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var usuarioDb = await _contexto.Usuarios.FindAsync(id);
        if (usuarioDb == null)
        {
            return NotFound(new { mensaje = $"Usuario con ID {id} no encontrado." });
        }

        usuarioDb.Nombre = usuario.Nombre;

        await _contexto.SaveChangesAsync();

        return Ok(usuarioDb);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> EliminarUsuario(int id)
    {
        var usuario = await _contexto.Usuarios.FindAsync(id);

        if (usuario == null)
        {
            return NotFound(new { mensaje = $"Usuario con ID {id} no encontrado." });
        }

        _contexto.Usuarios.Remove(usuario);
        await _contexto.SaveChangesAsync();

        return NoContent();
    }
}
