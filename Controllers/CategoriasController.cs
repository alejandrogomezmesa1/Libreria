using BibliotecaEFCore.Datos;
using BibliotecaEFCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaEFCore.Controllers;

public class CategoriasController(ApplicationDbContext contexto) : Controller
{
    private readonly ApplicationDbContext _contexto = contexto;

    // GET: Categorias
    public async Task<IActionResult> Index()
    {
        var categorias = await _contexto.Categorias
            .Include(c => c.Libros)
            .OrderBy(c => c.Nombre)
            .ToListAsync();
        return View(categorias);
    }

    // GET: Categorias/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Categorias/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Categoria categoria)
    {
        if (ModelState.IsValid)
        {
            _contexto.Add(categoria);
            await _contexto.SaveChangesAsync();
            TempData["Mensaje"] = "Categoría creada exitosamente.";
            return RedirectToAction(nameof(Index));
        }
        return View(categoria);
    }

    // GET: Categorias/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var categoria = await _contexto.Categorias.FindAsync(id);
        if (categoria == null)
        {
            return NotFound();
        }
        return View(categoria);
    }

    // POST: Categorias/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Categoria categoria)
    {
        if (id != categoria.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _contexto.Update(categoria);
                await _contexto.SaveChangesAsync();
                TempData["Mensaje"] = "Categoría actualizada exitosamente.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoriaExists(categoria.Id))
                {
                    return NotFound();
                }
                throw;
            }
            return RedirectToAction(nameof(Index));
        }
        return View(categoria);
    }

    // GET: Categorias/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var categoria = await _contexto.Categorias
            .Include(c => c.Libros)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (categoria == null)
        {
            return NotFound();
        }

        return View(categoria);
    }

    // POST: Categorias/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var categoria = await _contexto.Categorias
            .Include(c => c.Libros)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (categoria != null)
        {
            if (categoria.Libros.Count > 0)
            {
                TempData["Error"] = "No se puede eliminar la categoría porque tiene libros asociados.";
                return RedirectToAction(nameof(Index));
            }

            _contexto.Categorias.Remove(categoria);
            await _contexto.SaveChangesAsync();
            TempData["Mensaje"] = "Categoría eliminada exitosamente.";
        }

        return RedirectToAction(nameof(Index));
    }

    private bool CategoriaExists(int id)
    {
        return _contexto.Categorias.Any(e => e.Id == id);
    }
}
