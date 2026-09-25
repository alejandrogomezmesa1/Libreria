# Documentación de la API REST - BibliotecaEFCore

Esta API RESTful fue desarrollada con **ASP.NET Core 10** y **Entity Framework Core**, proporcionando operaciones CRUD completas para todas las entidades y tablas del sistema de biblioteca.

---

## 🚀 Swagger / OpenAPI

La documentación interactiva y pruebas en vivo de los endpoints están disponibles a través de Swagger UI:

- **Ruta Swagger:** `http://localhost:<puerto>/swagger` o `https://localhost:<puerto>/swagger`
- **Especificación OpenAPI (JSON):** `/swagger/v1/swagger.json`
- **Acceso rápido:** Puedes acceder directamente desde la barra de navegación del sistema haciendo clic en el botón **"Swagger API"**.

---

## 🛠️ Resumen de Endpoints por Entidad

| Entidad | Controlador | Ruta Base | Operaciones CRUD |
|---|---|---|---|
| **Autores** | `AutoresApiController` | `/api/autores` | GET (todos), GET (por id), POST, PUT, DELETE |
| **Libros** | `LibrosApiController` | `/api/libros` | GET (todos), GET (por id), POST, PUT, DELETE |
| **Categorías** | `CategoriasApiController` | `/api/categorias` | GET (todos), GET (por id), POST, PUT, DELETE |
| **Usuarios** | `UsuariosApiController` | `/api/usuarios` | GET (todos), GET (por id), POST, PUT, DELETE |
| **Tarjetas** | `TarjetasBibliotecaApiController` | `/api/tarjetas` | GET (todas), GET (por id), POST, PUT, DELETE |
| **Préstamos** | `PrestamosApiController` | `/api/prestamos` | GET (todos), GET (por id), POST, PUT, DELETE |

---

## 1. ✍️ Autores (`/api/autores`)

Gestiona los autores de los libros de la biblioteca.

### Endpoints
* **`GET /api/autores`**: Obtiene el listado de todos los autores junto con sus libros.
  - **Respuesta 200 OK:**
    ```json
    [
      {
        "id": 1,
        "nombre": "Gabriel García Márquez",
        "libros": [
          { "id": 1, "titulo": "Cien años de soledad" }
        ]
      }
    ]
    ```

* **`GET /api/autores/{id}`**: Obtiene un autor específico por su ID.
  - **Respuesta 200 OK:** Objeto `Autor`.
  - **Respuesta 404 Not Found:** `{ "mensaje": "Autor con ID {id} no encontrado." }`

* **`POST /api/autores`**: Crea un nuevo autor.
  - **Body (JSON):**
    ```json
    {
      "nombre": "Mario Vargas Llosa"
    }
    ```
  - **Respuesta 201 Created:** Retorna el autor creado y encabezado `Location`.

* **`PUT /api/autores/{id}`**: Modifica el nombre de un autor existente.
  - **Body (JSON):**
    ```json
    {
      "id": 1,
      "nombre": "Gabriel García Márquez (Actualizado)"
    }
    ```
  - **Respuesta 200 OK:** Objeto `Autor` actualizado.

* **`DELETE /api/autores/{id}`**: Elimina un autor.
  - **Validación de Integridad:** Si el autor posee libros vinculados, el sistema previene la eliminación (`DeleteBehavior.Restrict`) y retorna:
    - **Respuesta 409 Conflict:** `{ "mensaje": "No se puede eliminar el autor porque tiene libros asociados." }`
  - **Respuesta 204 No Content:** Eliminación exitosa.

---

## 2. 📖 Libros (`/api/libros`)

Gestiona el catálogo de libros, incluyendo sus relaciones con autores y categorías.

### Endpoints
* **`GET /api/libros`**: Listado de todos los libros con sus autores y categorías asociadas.
  - **Respuesta 200 OK:**
    ```json
    [
      {
        "id": 1,
        "titulo": "Cien años de soledad",
        "autorId": 1,
        "autor": {
          "id": 1,
          "nombre": "Gabriel García Márquez"
        },
        "categorias": [
          { "id": 1, "nombre": "Novela" },
          { "id": 2, "nombre": "Realismo Mágico" }
        ]
      }
    ]
    ```

* **`GET /api/libros/{id}`**: Detalle de un libro por su ID.
  - **Respuesta 200 OK:** Objeto `Libro`.
  - **Respuesta 404 Not Found:** `{ "mensaje": "Libro con ID {id} no encontrado." }`

* **`POST /api/libros`**: Registra un nuevo libro.
  - **Body (JSON):**
    ```json
    {
      "titulo": "El amor en los tiempos del cólera",
      "autorId": 1
    }
    ```
  - **Validación:** Verifica que el `autorId` exista en la base de datos (400 Bad Request si no existe).
  - **Respuesta 201 Created.**

* **`PUT /api/libros/{id}`**: Actualiza los datos de un libro.
  - **Body (JSON):**
    ```json
    {
      "id": 1,
      "titulo": "Cien años de soledad (Edición Especial)",
      "autorId": 1
    }
    ```
  - **Respuesta 200 OK.**

* **`DELETE /api/libros/{id}`**: Elimina un libro del catálogo.
  - **Respuesta 204 No Content.**

---

## 3. 🏷️ Categorías (`/api/categorias`)

Gestiona las categorías o géneros literarios (relación muchos a muchos con Libros).

### Endpoints
* **`GET /api/categorias`**: Lista todas las categorías y los libros que las contienen.
* **`GET /api/categorias/{id}`**: Detalle de la categoría solicitada.
* **`POST /api/categorias`**: Crea una nueva categoría.
  - **Body (JSON):**
    ```json
    {
      "nombre": "Ciencia Ficción"
    }
    ```
  - **Respuesta 201 Created.**
* **`PUT /api/categorias/{id}`**: Modifica el nombre de una categoría.
  - **Respuesta 200 OK.**
* **`DELETE /api/categorias/{id}`**: Elimina una categoría.
  - **Respuesta 204 No Content.**

---

## 4. 👤 Usuarios (`/api/usuarios`)

Gestiona los usuarios de la biblioteca, su tarjeta y su historial de préstamos.

### Endpoints
* **`GET /api/usuarios`**: Lista los usuarios registrados incluyendo su tarjeta de biblioteca y préstamos.
  - **Respuesta 200 OK:**
    ```json
    [
      {
        "id": 1,
        "nombre": "Carlos Mendoza",
        "tarjeta": {
          "id": 1,
          "codigo": "TB-001"
        },
        "prestamos": []
      }
    ]
    ```
* **`GET /api/usuarios/{id}`**: Detalle del usuario solicitado.
* **`POST /api/usuarios`**: Registra un nuevo usuario.
  - **Body (JSON):**
    ```json
    {
      "nombre": "Alejandro Gómez"
    }
    ```
  - **Respuesta 201 Created.**
* **`PUT /api/usuarios/{id}`**: Actualiza el nombre del usuario.
  - **Respuesta 200 OK.**
* **`DELETE /api/usuarios/{id}`**: Elimina el usuario (elimina en cascada su tarjeta y sus préstamos).
  - **Respuesta 204 No Content.**

---

## 5. 🪪 Tarjetas de Biblioteca (`/api/tarjetas`)

Gestiona la asignación de tarjetas de membresía (relación 1:1 estricta con Usuario).

### Endpoints
* **`GET /api/tarjetas`**: Lista todas las tarjetas emitidas con el usuario propietario.
* **`GET /api/tarjetas/{id}`**: Consulta una tarjeta por su ID.
* **`POST /api/tarjetas`**: Asigna una tarjeta a un usuario.
  - **Body (JSON):**
    ```json
    {
      "codigo": "TB-9988",
      "usuarioId": 2
    }
    ```
  - **Reglas de Negocio:**
    - El `usuarioId` debe existir (400 Bad Request si no existe).
    - Un usuario solo puede tener una única tarjeta. Si ya posee una, responde **409 Conflict**:
      `{ "mensaje": "El usuario especificado ya cuenta con una tarjeta de biblioteca." }`
  - **Respuesta 201 Created.**
* **`PUT /api/tarjetas/{id}`**: Actualiza el código de la tarjeta o reasigna el usuario comprobando unicidad.
  - **Respuesta 200 OK.**
* **`DELETE /api/tarjetas/{id}`**: Da de baja una tarjeta de biblioteca.
  - **Respuesta 204 No Content.**

---

## 6. 🔄 Préstamos (`/api/prestamos`)

Gestiona el registro de salida y devolución de libros.

### Endpoints
* **`GET /api/prestamos`**: Consulta todos los préstamos junto con el usuario y libro relacionados.
  - **Respuesta 200 OK:**
    ```json
    [
      {
        "id": 1,
        "usuarioId": 1,
        "usuario": { "id": 1, "nombre": "Carlos Mendoza" },
        "libroId": 2,
        "libro": { "id": 2, "titulo": "Rayuela" },
        "fechaPrestamo": "2026-09-20T00:00:00",
        "fechaDevolucion": null
      }
    ]
    ```
* **`GET /api/prestamos/{id}`**: Detalle de un préstamo en específico.
* **`POST /api/prestamos`**: Registra un nuevo préstamo.
  - **Body (JSON):**
    ```json
    {
      "usuarioId": 1,
      "libroId": 2,
      "fechaPrestamo": "2026-09-24T00:00:00",
      "fechaDevolucion": null
    }
    ```
  - **Validaciones:** Comprueba que tanto `usuarioId` como `libroId` existan.
  - **Respuesta 201 Created.**
* **`PUT /api/prestamos/{id}`**: Actualiza los datos de un préstamo (por ejemplo para marcar la fecha de devolución).
  - **Body (JSON):**
    ```json
    {
      "id": 1,
      "usuarioId": 1,
      "libroId": 2,
      "fechaPrestamo": "2026-09-24T00:00:00",
      "fechaDevolucion": "2026-09-30T00:00:00"
    }
    ```
  - **Respuesta 200 OK.**
* **`DELETE /api/prestamos/{id}`**: Elimina el registro del préstamo.
  - **Respuesta 204 No Content.**

---

## ⚙️ Características Técnicas Implementadas

1. **Prevención de Ciclos de Referencia en JSON:**
   Se configuró `ReferenceHandler.IgnoreCycles` en `Program.cs` para evitar errores `JsonException: A possible object cycle was detected` al serializar relaciones bidireccionales de EF Core (por ejemplo `Autor -> Libros -> Autor`).
2. **Validación Automática del Modelo (`ModelState`):**
   Todos los controladores validan las anotaciones de datos (`[Required]`, `[StringLength]`, etc.) y retornan código `400 Bad Request` ante entradas inválidas.
3. **Manejo de Errores e Integridad Relacional:**
   Se incorporaron validaciones explícitas de existencia de claves foráneas y restricciones relacionales (como la unicidad 1:1 en tarjetas y la protección contra borrado en autores con libros).
