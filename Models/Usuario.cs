using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace BibliotecaEFCore.Models;

public class Usuario
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre del usuario es obligatorio.")]
    [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
    [Display(Name = "Nombre de Usuario")]
    public string Nombre { get; set; } = string.Empty;

    // Relación 1:1 con TarjetaBiblioteca
    [ValidateNever]
    public TarjetaBiblioteca? Tarjeta { get; set; }

    // Relación 1:N con Préstamo
    [ValidateNever]
    public List<Prestamo> Prestamos { get; set; } = [];
}
