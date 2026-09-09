using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace BibliotecaEFCore.Models;

public class TarjetaBiblioteca
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El código de la tarjeta es obligatorio.")]
    [StringLength(50, ErrorMessage = "El código no puede superar los 50 caracteres.")]
    [Display(Name = "Código de Tarjeta")]
    public string Codigo { get; set; } = string.Empty;

    [Display(Name = "Usuario")]
    public int UsuarioId { get; set; }

    // Propiedad de navegación (1:1 con Usuario)
    [ValidateNever]
    public Usuario? Usuario { get; set; }
}
