using BibliotecaEFCore.Datos;
using BibliotecaEFCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaEFCore.Controllers;

public class AutoresController(ApplicationDbContext contexto) : Controller
{
    private readonly ApplicationDbContext _contexto = contexto;

    // GET: Autores
    public async Task<IActionResult> Index()
    {
        var autores = await _contexto.Autores
            .Include(a => a.Libros)
            .OrderBy(a => a.Nombre)
            .ToListAsync();
        return View(autores);
    }

    // GET: Autores/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Autores/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Autor autor)
    {
        if (ModelState.IsValid)
        {
            _contexto.Add(autor);
            await _contexto.SaveChangesAsync();
            TempData["Mensaje"] = "Autor creado exitosamente.";
            return RedirectToAction(nameof(Index));
        }
        return View(autor);
    }

    // GET: Autores/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var autor = await _contexto.Autores.FindAsync(id);
        if (autor == null)
        {
            return NotFound();
        }
        return View(autor);
    }

    // POST: Autores/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Autor autor)
    {
        if (id != autor.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _contexto.Update(autor);
                await _contexto.SaveChangesAsync();
                TempData["Mensaje"] = "Autor actualizado exitosamente.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AutorExists(autor.Id))
                {
                    return NotFound();
                }
                throw;
            }
            return RedirectToAction(nameof(Index));
        }
        return View(autor);
    }

    // GET: Autores/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var autor = await _contexto.Autores
            .Include(a => a.Libros)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (autor == null)
        {
            return NotFound();
        }

        return View(autor);
    }

    // POST: Autores/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var autor = await _contexto.Autores
            .Include(a => a.Libros)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (autor != null)
        {
            if (autor.Libros.Count > 0)
            {
                TempData["Error"] = "No se puede eliminar el autor porque tiene libros asociados.";
                return RedirectToAction(nameof(Index));
            }

            _contexto.Autores.Remove(autor);
            await _contexto.SaveChangesAsync();
            TempData["Mensaje"] = "Autor eliminado exitosamente.";
        }

        return RedirectToAction(nameof(Index));
    }

    private bool AutorExists(int id)
    {
        return _contexto.Autores.Any(e => e.Id == id);
    }
}
