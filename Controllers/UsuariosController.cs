using BibliotecaEFCore.Datos;
using BibliotecaEFCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaEFCore.Controllers;

public class UsuariosController(ApplicationDbContext contexto) : Controller
{
    private readonly ApplicationDbContext _contexto = contexto;

    // GET: Usuarios
    public async Task<IActionResult> Index()
    {
        var usuarios = await _contexto.Usuarios
            .Include(u => u.Tarjeta)
            .Include(u => u.Prestamos)
            .OrderBy(u => u.Nombre)
            .ToListAsync();
        return View(usuarios);
    }

    // GET: Usuarios/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Usuarios/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Usuario usuario, string? codigoTarjeta)
    {
        if (ModelState.IsValid)
        {
            if (!string.IsNullOrWhiteSpace(codigoTarjeta))
            {
                usuario.Tarjeta = new TarjetaBiblioteca
                {
                    Codigo = codigoTarjeta.Trim()
                };
            }

            _contexto.Add(usuario);
            await _contexto.SaveChangesAsync();
            TempData["Mensaje"] = "Usuario registrado exitosamente.";
            return RedirectToAction(nameof(Index));
        }
        return View(usuario);
    }

    // GET: Usuarios/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var usuario = await _contexto.Usuarios
            .Include(u => u.Tarjeta)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (usuario == null)
        {
            return NotFound();
        }

        ViewBag.CodigoTarjeta = usuario.Tarjeta?.Codigo;
        return View(usuario);
    }

    // POST: Usuarios/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Usuario usuario, string? codigoTarjeta)
    {
        if (id != usuario.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            var usuarioDb = await _contexto.Usuarios
                .Include(u => u.Tarjeta)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (usuarioDb == null)
            {
                return NotFound();
            }

            usuarioDb.Nombre = usuario.Nombre;

            if (!string.IsNullOrWhiteSpace(codigoTarjeta))
            {
                if (usuarioDb.Tarjeta != null)
                {
                    usuarioDb.Tarjeta.Codigo = codigoTarjeta.Trim();
                }
                else
                {
                    usuarioDb.Tarjeta = new TarjetaBiblioteca
                    {
                        Codigo = codigoTarjeta.Trim(),
                        UsuarioId = usuarioDb.Id
                    };
                }
            }
            else if (usuarioDb.Tarjeta != null)
            {
                // Si se vacía el campo, se elimina la tarjeta
                _contexto.TarjetasBiblioteca.Remove(usuarioDb.Tarjeta);
            }

            await _contexto.SaveChangesAsync();
            TempData["Mensaje"] = "Usuario actualizado exitosamente.";
            return RedirectToAction(nameof(Index));
        }

        ViewBag.CodigoTarjeta = codigoTarjeta;
        return View(usuario);
    }

    // GET: Usuarios/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var usuario = await _contexto.Usuarios
                .Include(u => u.Tarjeta)
                .Include(u => u.Prestamos)
                .FirstOrDefaultAsync(m => m.Id == id);

        if (usuario == null)
        {
            return NotFound();
        }

        return View(usuario);
    }

    // POST: Usuarios/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var usuario = await _contexto.Usuarios
            .Include(u => u.Tarjeta)
            .Include(u => u.Prestamos)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (usuario != null)
        {
            if (usuario.Prestamos.Any(p => p.FechaDevolucion == null))
            {
                TempData["Error"] = "No se puede eliminar el usuario porque tiene préstamos pendientes de devolución.";
                return RedirectToAction(nameof(Index));
            }

            _contexto.Usuarios.Remove(usuario);
            await _contexto.SaveChangesAsync();
            TempData["Mensaje"] = "Usuario eliminado exitosamente.";
        }

        return RedirectToAction(nameof(Index));
    }
}
