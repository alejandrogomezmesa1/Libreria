using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace BibliotecaEFCore.Models;

public class Libro
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El título del libro es obligatorio.")]
    [StringLength(150, ErrorMessage = "El título no puede superar los 150 caracteres.")]
    [Display(Name = "Título")]
    public string Titulo { get; set; } = string.Empty;

    [Required(ErrorMessage = "Debe seleccionar un autor.")]
    [Display(Name = "Autor")]
    public int AutorId { get; set; }

    // Propiedad de navegación (1:N con Autor)
    [ValidateNever]
    public Autor? Autor { get; set; }

    // Propiedad de navegación (N:N con Categoría)
    [ValidateNever]
    public List<Categoria> Categorias { get; set; } = [];

    // Propiedad de navegación (1:N con Préstamo)
    [ValidateNever]
    public List<Prestamo> Prestamos { get; set; } = [];
}
