using BibliotecaEFCore.Models;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaEFCore.Datos;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Autor> Autores { get; set; }
    public DbSet<Libro> Libros { get; set; }
    public DbSet<Categoria> Categorias { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<TarjetaBiblioteca> TarjetasBiblioteca { get; set; }
    public DbSet<Prestamo> Prestamos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Relación 1:1 entre Usuario y TarjetaBiblioteca
        modelBuilder.Entity<Usuario>()
            .HasOne(u => u.Tarjeta)
            .WithOne(t => t.Usuario)
            .HasForeignKey<TarjetaBiblioteca>(t => t.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relación 1:N entre Autor y Libro
        modelBuilder.Entity<Autor>()
            .HasMany(a => a.Libros)
            .WithOne(l => l.Autor)
            .HasForeignKey(l => l.AutorId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación N:N entre Libro y Categoria (tabla CategoriaLibro)
        modelBuilder.Entity<Libro>()
            .HasMany(l => l.Categorias)
            .WithMany(c => c.Libros)
            .UsingEntity(j => j.ToTable("CategoriaLibro"));

        // Relación 1:N entre Usuario y Prestamo
        modelBuilder.Entity<Prestamo>()
            .HasOne(p => p.Usuario)
            .WithMany(u => u.Prestamos)
            .HasForeignKey(p => p.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relación 1:N entre Libro y Prestamo
        modelBuilder.Entity<Prestamo>()
            .HasOne(p => p.Libro)
            .WithMany(l => l.Prestamos)
            .HasForeignKey(p => p.LibroId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
