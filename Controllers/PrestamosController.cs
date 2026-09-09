using BibliotecaEFCore.Datos;
using BibliotecaEFCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaEFCore.Controllers;

public class PrestamosController(ApplicationDbContext contexto) : Controller
{
    private readonly ApplicationDbContext _contexto = contexto;

    // GET: Prestamos
    public async Task<IActionResult> Index()
    {
        var prestamos = await _contexto.Prestamos
            .Include(p => p.Usuario)
            .Include(p => p.Libro)
            .OrderByDescending(p => p.FechaPrestamo)
            .ToListAsync();
        return View(prestamos);
    }

    // GET: Prestamos/Create
    public IActionResult Create()
    {
        ViewBag.Usuarios = new SelectList(_contexto.Usuarios.OrderBy(u => u.Nombre), "Id", "Nombre");
        ViewBag.Libros = new SelectList(_contexto.Libros.OrderBy(l => l.Titulo), "Id", "Titulo");
        return View();
    }

    // POST: Prestamos/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Prestamo prestamo)
    {
        if (ModelState.IsValid)
        {
            prestamo.FechaPrestamo = prestamo.FechaPrestamo == default
                ? DateTime.Today
                : prestamo.FechaPrestamo;

            _contexto.Add(prestamo);
            await _contexto.SaveChangesAsync();
            TempData["Mensaje"] = "Préstamo registrado exitosamente.";
            return RedirectToAction(nameof(Index));
        }

        ViewBag.Usuarios = new SelectList(_contexto.Usuarios.OrderBy(u => u.Nombre), "Id", "Nombre", prestamo.UsuarioId);
        ViewBag.Libros = new SelectList(_contexto.Libros.OrderBy(l => l.Titulo), "Id", "Titulo", prestamo.LibroId);
        return View(prestamo);
    }

    // GET: Prestamos/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var prestamo = await _contexto.Prestamos.FindAsync(id);
        if (prestamo == null)
        {
            return NotFound();
        }

        ViewBag.Usuarios = new SelectList(_contexto.Usuarios.OrderBy(u => u.Nombre), "Id", "Nombre", prestamo.UsuarioId);
        ViewBag.Libros = new SelectList(_contexto.Libros.OrderBy(l => l.Titulo), "Id", "Titulo", prestamo.LibroId);
        return View(prestamo);
    }

    // POST: Prestamos/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Prestamo prestamo)
    {
        if (id != prestamo.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _contexto.Update(prestamo);
                await _contexto.SaveChangesAsync();
                TempData["Mensaje"] = "Préstamo actualizado exitosamente.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PrestamoExists(prestamo.Id))
                {
                    return NotFound();
                }
                throw;
            }
            return RedirectToAction(nameof(Index));
        }

        ViewBag.Usuarios = new SelectList(_contexto.Usuarios.OrderBy(u => u.Nombre), "Id", "Nombre", prestamo.UsuarioId);
        ViewBag.Libros = new SelectList(_contexto.Libros.OrderBy(l => l.Titulo), "Id", "Titulo", prestamo.LibroId);
        return View(prestamo);
    }

    // POST: Prestamos/Devolver/5 (Acción rápida para registrar devolución hoy)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Devolver(int id)
    {
        var prestamo = await _contexto.Prestamos.FindAsync(id);
        if (prestamo != null)
        {
            prestamo.FechaDevolucion = DateTime.Today;
            await _contexto.SaveChangesAsync();
            TempData["Mensaje"] = "Devolución registrada exitosamente.";
        }
        return RedirectToAction(nameof(Index));
    }

    // GET: Prestamos/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var prestamo = await _contexto.Prestamos
            .Include(p => p.Usuario)
            .Include(p => p.Libro)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (prestamo == null)
        {
            return NotFound();
        }

        return View(prestamo);
    }

    // POST: Prestamos/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var prestamo = await _contexto.Prestamos.FindAsync(id);
        if (prestamo != null)
        {
            _contexto.Prestamos.Remove(prestamo);
            await _contexto.SaveChangesAsync();
            TempData["Mensaje"] = "Préstamo eliminado exitosamente.";
        }

        return RedirectToAction(nameof(Index));
    }

    private bool PrestamoExists(int id)
    {
        return _contexto.Prestamos.Any(e => e.Id == id);
    }
}
