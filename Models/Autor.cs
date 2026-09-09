using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace BibliotecaEFCore.Models;

public class Autor
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre del autor es obligatorio.")]
    [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
    [Display(Name = "Nombre del Autor")]
    public string Nombre { get; set; } = string.Empty;

    // Propiedad de navegación (1:N)
    [ValidateNever]
    public List<Libro> Libros { get; set; } = [];
}
