using BibliotecaEFCore.Datos;
using BibliotecaEFCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaEFCore.Controllers;

public class LibrosController(ApplicationDbContext contexto) : Controller
{
    private readonly ApplicationDbContext _contexto = contexto;

    // GET: Libros
    public async Task<IActionResult> Index()
    {
        var libros = await _contexto.Libros
            .Include(l => l.Autor)
            .Include(l => l.Categorias)
            .OrderBy(l => l.Titulo)
            .ToListAsync();
        return View(libros);
    }

    // GET: Libros/Create
    public IActionResult Create()
    {
        ViewBag.AutorId = new SelectList(_contexto.Autores.OrderBy(a => a.Nombre), "Id", "Nombre");
        ViewBag.Categorias = _contexto.Categorias.OrderBy(c => c.Nombre).ToList();
        return View();
    }

    // POST: Libros/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Libro libro, int[]? categoriaSeleccionadas)
    {
        if (ModelState.IsValid)
        {
            libro.Categorias ??= [];

            if (categoriaSeleccionadas != null)
            {
                foreach (var id in categoriaSeleccionadas)
                {
                    var categoria = await _contexto.Categorias.FindAsync(id);
                    if (categoria != null)
                    {
                        libro.Categorias.Add(categoria);
                    }
                }
            }

            _contexto.Add(libro);
            await _contexto.SaveChangesAsync();
            TempData["Mensaje"] = "Libro registrado exitosamente.";
            return RedirectToAction(nameof(Index));
        }

        ViewBag.AutorId = new SelectList(_contexto.Autores.OrderBy(a => a.Nombre), "Id", "Nombre", libro.AutorId);
        ViewBag.Categorias = _contexto.Categorias.OrderBy(c => c.Nombre).ToList();
        return View(libro);
    }

    // GET: Libros/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var libro = await _contexto.Libros
            .Include(l => l.Categorias)
            .FirstOrDefaultAsync(l => l.Id == id);

        if (libro == null)
        {
            return NotFound();
        }

        ViewBag.AutorId = new SelectList(_contexto.Autores.OrderBy(a => a.Nombre), "Id", "Nombre", libro.AutorId);
        ViewBag.Categorias = _contexto.Categorias.OrderBy(c => c.Nombre).ToList();
        return View(libro);
    }

    // POST: Libros/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Libro libro, int[]? categoriaSeleccionadas)
    {
        if (id != libro.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            var libroDb = await _contexto.Libros
                .Include(l => l.Categorias)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (libroDb == null)
            {
                return NotFound();
            }

            libroDb.Titulo = libro.Titulo;
            libroDb.AutorId = libro.AutorId;

            libroDb.Categorias.Clear();
            if (categoriaSeleccionadas != null)
            {
                foreach (var catId in categoriaSeleccionadas)
                {
                    var categoria = await _contexto.Categorias.FindAsync(catId);
                    if (categoria != null)
                    {
                        libroDb.Categorias.Add(categoria);
                    }
                }
            }

            await _contexto.SaveChangesAsync();
            TempData["Mensaje"] = "Libro actualizado exitosamente.";
            return RedirectToAction(nameof(Index));
        }

        ViewBag.AutorId = new SelectList(_contexto.Autores.OrderBy(a => a.Nombre), "Id", "Nombre", libro.AutorId);
        ViewBag.Categorias = _contexto.Categorias.OrderBy(c => c.Nombre).ToList();
        return View(libro);
    }

    // GET: Libros/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var libro = await _contexto.Libros
            .Include(l => l.Autor)
            .Include(l => l.Categorias)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (libro == null)
        {
            return NotFound();
        }

        return View(libro);
    }

    // POST: Libros/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var libro = await _contexto.Libros.FindAsync(id);
        if (libro != null)
        {
            _contexto.Libros.Remove(libro);
            await _contexto.SaveChangesAsync();
            TempData["Mensaje"] = "Libro eliminado exitosamente.";
        }

        return RedirectToAction(nameof(Index));
    }
}
