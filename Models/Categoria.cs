using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace BibliotecaEFCore.Models;

public class Categoria
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre de la categoría es obligatorio.")]
    [StringLength(50, ErrorMessage = "La categoría no puede exceder los 50 caracteres.")]
    [Display(Name = "Nombre de la Categoría")]
    public string Nombre { get; set; } = string.Empty;

    // Relación muchos a muchos (N:N)
    [ValidateNever]
    public List<Libro> Libros { get; set; } = [];
}
