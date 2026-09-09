using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace BibliotecaEFCore.Models;

public class Prestamo
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Debe seleccionar un usuario.")]
    [Display(Name = "Usuario")]
    public int UsuarioId { get; set; }

    // Propiedad de navegación (N:1 con Usuario)
    [ValidateNever]
    public Usuario? Usuario { get; set; }

    [Required(ErrorMessage = "Debe seleccionar un libro.")]
    [Display(Name = "Libro")]
    public int LibroId { get; set; }

    // Propiedad de navegación (N:1 con Libro)
    [ValidateNever]
    public Libro? Libro { get; set; }

    [Required(ErrorMessage = "La fecha de préstamo es obligatoria.")]
    [DataType(DataType.Date)]
    [Display(Name = "Fecha de Préstamo")]
    public DateTime FechaPrestamo { get; set; } = DateTime.Today;

    [DataType(DataType.Date)]
    [Display(Name = "Fecha de Devolución")]
    public DateTime? FechaDevolucion { get; set; }
}
