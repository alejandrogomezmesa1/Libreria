# 📚 Guía Oficial de Estudio y Sustentación
## Proyecto Integrador: Sistema de Gestión de Biblioteca (ASP.NET Core MVC + Entity Framework Core)
**Material basado en el programa de formación SENA**

---

## 📋 Tabla de Contenidos
1. [Dominio del Proyecto y Propósito](#1-dominio-del-proyecto-y-propósito)
2. [Conceptos Fundamentales de ASP.NET Core & Desarrollo Web](#2-conceptos-fundamentales-de-aspnet-core--desarrollo-web)
3. [Arquitectura MVC (Model - View - Controller)](#3-arquitectura-mvc-model---view---controller)
4. [Entity Framework Core & Persistencia de Datos](#4-entity-framework-core--persistencia-de-datos)
5. [Modelado de Entidades y Relaciones (1:1, 1:N, N:N)](#5-modelado-de-entidades-y-relaciones-11-1n-nn)
6. [Validaciones, Data Annotations y Seguridad](#6-validaciones-data-annotations-y-seguridad)
7. [⭐ La Pregunta Clave: Flujo Completo de Creación de un Libro](#7-⭐-la-pregunta-clave-flujo-completo-de-creación-de-un-libro)
8. [Banco de 20 Preguntas y Respuestas para Sustentar (SENA)](#8-banco-de-20-preguntas-y-respuestas-para-sustentar-sena)
9. [Recorrido por los Archivos del Código Fuente](#9-recorrido-por-los-archivos-del-código-fuente)

---

## 1. Dominio del Proyecto y Propósito

El sistema es una aplicación web interactiva desarrollada para gestionar el catálogo y los préstamos de una biblioteca.

### Entidades del Dominio:
* **Autor:** Persona que escribe uno o varios libros.
* **Libro:** Obra literaria que pertenece a un autor y puede clasificarse en una o varias categorías.
* **Categoría:** Clasificación o género del libro (Novela, Realismo Mágico, Ciencia Ficción, Historia, Fantasía).
* **Usuario:** Persona registrada en la biblioteca para solicitar préstamos.
* **TarjetaBiblioteca:** Carné o credencial única asignada a cada usuario.
* **Préstamo:** Registro del préstamo de un libro a un usuario con fecha de salida y devolución.

---

## 2. Conceptos Fundamentales de ASP.NET Core & Desarrollo Web

### ¿Qué es ASP.NET Core?
Es un framework multiplataforma (Windows, macOS, Linux), de alto rendimiento y de código abierto desarrollado por Microsoft para construir aplicaciones web modernas, APIs RESTful y servicios en la nube sobre el entorno de ejecución **.NET** (versión .NET 10 en este proyecto).

### Ciclo de Vida Petición / Respuesta (HTTP Request / Response)
1. **Request (Solicitud):** El usuario ingresa una URL o envía un formulario desde el navegador (ejemplo: `GET /Libros` o `POST /Autores/Create`).
2. **Middleware Pipeline:** La solicitud pasa por componentes en cadena que procesan archivos estáticos, enrutamiento, seguridad y excepciones.
3. **Routing (Enrutamiento):** Examina la URL y determina cuál controlador y cuál acción deben procesar la solicitud.
4. **Execution (Ejecución):** El controlador consulta la base de datos mediante Entity Framework Core, prepara el Modelo y selecciona la Vista.
5. **Response (Respuesta):** Se procesa la vista Razor enviando código HTML5 procesado al navegador del usuario.

### Inyección de Dependencias (Dependency Injection - DI)
Es un patrón de diseño software implementado de forma nativa en ASP.NET Core. Permite suministrar objetos (servicios) que una clase necesita en lugar de instanciarlos manualmente con `new`.
* **En el proyecto:** `Program.cs` registra `ApplicationDbContext` en el contenedor de servicios. Cuando un controlador (`LibrosController`) requiere el contexto, la plataforma se lo suministra en el constructor:
  ```csharp
  public class LibrosController(ApplicationDbContext contexto) : Controller
  {
      private readonly ApplicationDbContext _contexto = contexto;
  }
  ```

---

## 3. Arquitectura MVC (Model - View - Controller)

MVC es un patrón de arquitectura que separa la aplicación en 3 componentes principales:

```
                  ┌──────────────────────┐
                  │      CONTROLLER      │
                  │ (Lógica de Control)  │
                  └──────────┬───────────┘
                             │
               ┌─────────────┴─────────────┐
               ▼                           ▼
    ┌────────────────────┐       ┌────────────────────┐
    │       MODEL        │       │        VIEW        │
    │  (Datos / EF Core) │       │   (Interfaz Razor) │
    └────────────────────┘       └────────────────────┘
```

### 1. Model (Modelo) - *Ubicación: `Models/`*
Representa la estructura de los datos, reglas de negocio y entidades de la base de datos.
* **Ejemplos:** `Autor.cs`, `Libro.cs`, `Usuario.cs`, `Prestamo.cs`.

### 2. View (Vista) - *Ubicación: `Views/`*
Es la interfaz gráfica que ve el usuario final. Utiliza el motor de plantillas **Razor (`.cshtml`)** para mezclar HTML5 con código C#.
* **Tag Helpers principales:**
  - `asp-controller`: Indica a cuál controlador apunta un enlace o formulario.
  - `asp-action`: Indica la acción/método del controlador.
  - `asp-for`: Vincula un control HTML (`<input>`, `<select>`) con una propiedad del Modelo C#.
  - `asp-items`: Pobla elementos desplegables `<select>` desde una colección (ej: `ViewBag.AutorId`).

### 3. Controller (Controlador) - *Ubicación: `Controllers/`*
Recibe las peticiones HTTP del navegador, ejecuta la lógica de negocio, interactúa con la base de datos a través de EF Core y decide qué vista retornar.
* **Respuestas comunes (`IActionResult`):**
  - `View(modelo)`: Retorna la vista correspondiente pasando datos.
  - `RedirectToAction(nameof(Index))`: Redirecciona a otra acción.
  - `NotFound()`: Retorna código de estado HTTP 404.

### Mecanismos de comunicación Controlador ➡️ Vista:
* **Model:** Obra principal fuertemente tipada pasada a la vista (`@model Libro`).
* **ViewBag:** Objeto dinámico para pasar datos secundarios o auxiliares (ej: lista de autores para un desplegable).
* **TempData:** Almacena datos temporales que persisten entre redirecciones HTTP (ej: mensajes de éxito o error).

---

## 4. Entity Framework Core & Persistencia de Datos

### ¿Qué es EF Core?
Es un **ORM (Object-Relational Mapper)** para .NET. Permite interactuar con bases de datos relacionales mediante clases y objetos C#, sin necesidad de escribir sentencias SQL manuales como `INSERT INTO` o `SELECT * FROM`.

### Componentes Clave:
* **ApplicationDbContext:** Clase que hereda de `DbContext`. Representa la sesión con la base de datos y expone las colecciones de entidades.
* **DbSet<T>:** Representa una tabla de la base de datos.
  ```csharp
  public DbSet<Autor> Autores { get; set; }
  public DbSet<Libro> Libros { get; set; }
  ```
* **Consultas LINQ (Language Integrated Query):** Permite escribir consultas fuertemente tipadas sobre los `DbSet`.
  - `.ToListAsync()`: Ejecuta la consulta de forma asíncrona.
  - `.FirstOrDefaultAsync(predicate)`: Retorna el primer registro que cumpla una condición.
  - `.Include(l => l.Autor)`: Realiza un `JOIN` para cargar entidades relacionadas (Eager Loading / Carga Expansiva).

---

## 5. Modelado de Entidades y Relaciones (1:1, 1:N, N:N)

Entity Framework Core mapea las relaciones del modelo relacional mediante **propiedades de navegación** y **claves foráneas**.

### 1. Relación 1:1 (Uno a Uno) — `Usuario` y `TarjetaBiblioteca`
Un usuario tiene 1 tarjeta de biblioteca y cada tarjeta pertenece a 1 único usuario.

* **En el DbContext (`OnModelCreating`):**
  ```csharp
  modelBuilder.Entity<Usuario>()
      .HasOne(u => u.Tarjeta)
      .WithOne(t => t.Usuario)
      .HasForeignKey<TarjetaBiblioteca>(t => t.UsuarioId)
      .OnDelete(DeleteBehavior.Cascade);
  ```

### 2. Relación 1:N (Uno a Muchos) — `Autor` y `Libro`
Un autor escribe muchos libros, pero un libro pertenece a un solo autor.

* **En `Libro.cs`:**
  ```csharp
  public int AutorId { get; set; } // Clave foránea
  public Autor? Autor { get; set; } // Propiedad de navegación
  ```

* **En `Autor.cs`:**
  ```csharp
  public List<Libro> Libros { get; set; } = [];
  ```

### 3. Relación N:N (Muchos a Muchos) — `Libro` y `Categoria`
Un libro puede tener múltiples categorías y una categoría contiene múltiples libros.

* **En `ApplicationDbContext.cs`:**
  ```csharp
  modelBuilder.Entity<Libro>()
      .HasMany(l => l.Categorias)
      .WithMany(c => c.Libros)
      .UsingEntity(j => j.ToTable("CategoriaLibro"));
  ```
  *EF Core crea automáticamente la tabla intermedia `CategoriaLibro` con las claves foráneas `LibrosId` y `CategoriasId`.*

---

## 6. Validaciones, Data Annotations y Seguridad

### Data Annotations (Atributos de Validación)
Se colocan sobre las propiedades del modelo para definir reglas de negocio y restricciones:

```csharp
[Required(ErrorMessage = "El nombre del autor es obligatorio.")]
[StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
[Display(Name = "Nombre del Autor")]
public string Nombre { get; set; } = string.Empty;
```

### `ModelState.IsValid`
Comprueba en el servidor si los datos enviados en un formulario cumplen con todas las reglas definidas en el Modelo.
```csharp
if (ModelState.IsValid)
{
    _contexto.Add(autor);
    await _contexto.SaveChangesAsync();
    return RedirectToAction(nameof(Index));
}
```

### Prevención de Ataques CSRF (`[ValidateAntiForgeryToken]`)
Se coloca en los métodos HTTP POST de los controladores para verificar que las peticiones provienen de un formulario legítimo generado por la propia aplicación.

---

## 7. ⭐ La Pregunta Clave: Flujo Completo de Creación de un Libro

> **Nota para el aprendiz:** Esta es la pregunta más importante de la evaluación/sustentación.

### Paso a paso técnico desde que el usuario interactúa hasta que los datos llegan a la base de datos:

1. **Petición Inicial (GET):** El usuario ingresa a `/Libros/Create`.
2. **Ejecución `Create()` GET en `LibrosController`:**
   - El controlador consulta los autores y categorías disponibles.
   - Carga `ViewBag.AutorId` y `ViewBag.Categorias`.
   - Retorna la vista `Create.cshtml`.
3. **Renderizado de la Vista:** El navegador muestra el formulario HTML con los selectores y casillas de verificación.
4. **Diligenciamiento y Envío (POST):** El usuario escribe el título, selecciona el autor y las categorías, y presiona "Guardar".
5. **Model Binding:** ASP.NET Core toma los datos enviados en el formulario HTTP POST y los empaqueta automáticamente en un objeto `Libro libro` y un arreglo `int[] categoriaSeleccionadas`.
6. **Validación del Servidor:** `LibrosController.Create(POST)` ejecuta `ModelState.IsValid`.
7. **Asignación de Relaciones N:N:** Se recorren las IDs de `categoriaSeleccionadas`, se buscan en `_contexto.Categorias` y se agregan a `libro.Categorias`.
8. **Persistencia con EF Core:**
   - `_contexto.Add(libro)` registra la nueva entidad en la memoria del DbContext.
   - `await _contexto.SaveChangesAsync()` traduce la entidad a sentencias SQL relacionales y las ejecuta en PostgreSQL.
9. **Redirección:** `RedirectToAction(nameof(Index))` envía un encabezado HTTP 302 al navegador para cargar la lista actualizada de libros en `/Libros`.

---

## 8. Banco de 20 Preguntas y Respuestas para Sustentar (SENA)

### 1. ¿Cuál es el archivo que inicia la aplicación?
**Respuesta:** `Program.cs`. Es el punto de entrada ejecutable de la aplicación. Allí se configuran los servicios, el contenedor de dependencias, la conexión a la base de datos (`DbContext`), los middlewares y el enrutamiento.

### 2. ¿Qué patrón arquitectónico utiliza este proyecto?
**Respuesta:** El patrón **MVC (Model, View, Controller)**.
* **Model:** Representa las entidades y la lógica de datos.
* **View:** Presenta la interfaz gráfica al usuario (HTML/Razor).
* **Controller:** Procesa las solicitudes del usuario, interactúa con el Modelo y selecciona la Vista.

### 3. ¿Qué función cumple la carpeta `Models`?
**Respuesta:** Contiene las clases que definen las entidades del dominio de la aplicación (como `Autor.cs`, `Libro.cs`, `Usuario.cs`, `Prestamo.cs`), especificando sus propiedades, relaciones y reglas de validación.

### 4. ¿Qué es una vista Razor?
**Respuesta:** Es una plantilla HTML con extensión `.cshtml` que permite incrustar código C# mediante la sintaxis `@` para generar interfaz gráfica dinámica en el servidor.

### 5. ¿Dónde se encuentran las vistas de los libros?
**Respuesta:** En la carpeta `Views/Libros/`. Típicamente contiene `Index.cshtml`, `Create.cshtml`, `Edit.cshtml` y `Delete.cshtml`.

### 6. ¿Qué relación existe entre `LibrosController` y `Views/Libros`?
**Respuesta:** `LibrosController` contiene los métodos de acción que procesan las peticiones y por convención retornan las vistas ubicadas en `Views/Libros/` con el mismo nombre de la acción.

### 7. ¿Qué función cumple `ApplicationDbContext`?
**Respuesta:** Es la clase principal que extiende de `DbContext` en EF Core. Actúa como el puente o sesión entre el código C# y la base de datos relacional, permitiendo consultar y guardar datos mediante propiedades `DbSet`.

### 8. ¿Qué es Entity Framework Core?
**Respuesta:** Es un **ORM (Object-Relational Mapper)** ligero y multiplataforma de Microsoft que mapea tablas relacionales de bases de datos a clases de C#.

### 9. ¿Qué significa `DbSet<Libro>` dentro del `ApplicationDbContext`?
**Respuesta:** Representa la colección de todos los registros de la tabla `Libros` en la base de datos, permitiendo realizar operaciones CRUD como `_contexto.Libros.ToListAsync()` o `_contexto.Libros.Add(libro)`.

### 10. ¿Qué función cumple `LibrosController`?
**Respuesta:** Gestionar el ciclo de vida de las peticiones relacionadas con los libros (listar, crear, editar y eliminar).

### 11. ¿Qué significa CRUD?
**Respuesta:** Las cuatro operaciones fundamentales sobre los datos:
* **C**reate (Crear)
* **R**ead (Leer / Listar)
* **U**pdate (Actualizar / Editar)
* **D**elete (Eliminar)

### 12. ¿Cuál es la diferencia entre `Create()` GET y `Create()` POST?
**Respuesta:**
* **GET:** Solicita y muestra el formulario de creación vacío al usuario.
* **POST:** Recibe los datos enviados por el usuario desde el formulario, los valida y los guarda en la base de datos.

### 13. ¿Qué función cumple `ModelState.IsValid`?
**Respuesta:** Verifica si los datos recibidos en el servidor cumplen las reglas de validación (Data Annotations) definidas en las propiedades del Modelo.

### 14. ¿Qué función cumple `asp-for`?
**Respuesta:** Es un Tag Helper que enlaza un campo del formulario HTML con una propiedad específica del Modelo C#, generando automáticamente los atributos `id`, `name` y los valores correspondientes.

### 15. ¿Qué función cumple `asp-items`?
**Respuesta:** Es un Tag Helper utilizado en elementos `<select>` para poblar automáticamente la lista desplegable de opciones a partir de una colección de datos (como un `SelectList`).

### 16. ¿Qué es `ViewBag` y para qué se utiliza en este proyecto?
**Respuesta:** Es un objeto dinámico que permite transferir datos temporales desde el Controlador hacia la Vista. En este proyecto se usa para pasar listas de Autores y Categorías a los formularios desplegables.

### 17. ¿Qué significa la relación `public int AutorId { get; set; }` y `public Autor Autor { get; set; }` en `Libro`?
**Respuesta:** `AutorId` es la **clave foránea (Foreign Key)** física en la base de datos, mientras que `Autor` es la **propiedad de navegación** que permite acceder al objeto completo del autor asociado desde el libro.

### 18. ¿Por qué `Libro` tiene una propiedad `public List<Categoria> Categorias { get; set; }`?
**Respuesta:** Porque representa una relación de **muchos a muchos (N:N)**. Un libro puede tener varias categorías asignadas al mismo tiempo.

### 19. ¿Qué función cumple `.Include()`?
**Respuesta:** Especifica la carga expansiva (Eager Loading) de entidades relacionadas en consultas LINQ. Por ejemplo, `_contexto.Libros.Include(l => l.Autor)` incluye los datos del Autor junto con el Libro.

### 20. Explique el flujo al crear un libro.
**(Ver Sección 7 de esta guía para el desarrollo detallado).**

---

## 9. Recorrido por los Archivos del Código Fuente

### 📄 `Program.cs`
Punto de entrada de la aplicación. Configura la base de datos PostgreSQL mediante Npgsql, agrega los controladores MVC con vistas, llama a `EnsureCreated()` para crear las tablas y ejecuta `SeedData.Poblar(context)`.

### 📄 `appsettings.json`
Archivo de configuración JSON donde se define la cadena de conexión (`DefaultConnection`) a PostgreSQL.

### 📄 `Datos/ApplicationDbContext.cs`
Configura los `DbSet` y mapea las relaciones de entidades (1:1, 1:N y N:N con la tabla pivote `CategoriaLibro`).

### 📄 `Data/SeedData.cs`
Clase estática que inserta datos iniciales de prueba (autores, categorías, libros, usuarios con tarjetas y préstamos) si la base de datos está vacía.

---

### 💡 CONSEJO FINAL PARA LA SUSTENTACIÓN:
Muestra seguridad al responder, recuerda la diferencia entre **GET** (mostrar pantalla) y **POST** (guardar datos), y utiliza los términos correctos: **Patrón MVC**, **EF Core (ORM)**, **Tag Helpers**, **Relaciones de Entidades** y **Validaciones**. ¡Mucho éxito! 🚀
