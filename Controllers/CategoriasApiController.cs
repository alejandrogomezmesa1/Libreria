using BibliotecaEFCore.Datos;
using BibliotecaEFCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaEFCore.Controllers;

[ApiController]
[Route("api/categorias")]
public class CategoriasApiController(ApplicationDbContext contexto) : ControllerBase
{
    private readonly ApplicationDbContext _contexto = contexto;

    [HttpGet]
    public async Task<IActionResult> GetCategorias()
    {
        var categorias = await _contexto.Categorias
            .Include(c => c.Libros)
            .ToListAsync();

        return Ok(categorias);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCategoria(int id)
    {
        var categoria = await _contexto.Categorias
            .Include(c => c.Libros)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (categoria == null)
        {
            return NotFound(new { mensaje = $"Categoría con ID {id} no encontrada." });
        }

        return Ok(categoria);
    }

    [HttpPost]
    public async Task<IActionResult> CrearCategoria(Categoria categoria)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _contexto.Categorias.Add(categoria);
        await _contexto.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetCategoria),
            new { id = categoria.Id },
            categoria
        );
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> EditarCategoria(int id, Categoria categoria)
    {
        if (id != categoria.Id)
        {
            return BadRequest(new { mensaje = "El ID de la ruta no coincide con el ID de la categoría." });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var categoriaDb = await _contexto.Categorias.FindAsync(id);
        if (categoriaDb == null)
        {
            return NotFound(new { mensaje = $"Categoría con ID {id} no encontrada." });
        }

        categoriaDb.Nombre = categoria.Nombre;

        await _contexto.SaveChangesAsync();

        return Ok(categoriaDb);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> EliminarCategoria(int id)
    {
        var categoria = await _contexto.Categorias.FindAsync(id);

        if (categoria == null)
        {
            return NotFound(new { mensaje = $"Categoría con ID {id} no encontrada." });
        }

        _contexto.Categorias.Remove(categoria);
        await _contexto.SaveChangesAsync();

        return NoContent();
    }
}
