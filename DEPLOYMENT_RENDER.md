# Guía de Despliegue en Render (Servicio Web + Base de Datos PostgreSQL)

Esta guía detalla los pasos para desplegar toda la aplicación (**ASP.NET Core 10** + **PostgreSQL**) en [Render](https://render.com/).

---

## 🏗️ Servicios Incluidos

El despliegue consta de dos servicios integrados:
1. **Base de Datos Gestionada (PostgreSQL):** Instancia de PostgreSQL en Render con credenciales seguras.
2. **Servicio Web (ASP.NET Core 10 en Docker):**
   - Aloja la aplicación MVC y los controladores de la API REST.
   - Incluye **Swagger UI** accesible públicamente.
   - Crea automáticamente las tablas y realiza la carga inicial de datos (*seeding*) al arrancar.

---

## 🚀 Opción 1: Despliegue Automático con Render Blueprint (Recomendado)

El repositorio incluye el archivo [`render.yaml`](./render.yaml), que define la infraestructura como código.

### Pasos:
1. **Sube tus cambios a GitHub/GitLab:**
   ```bash
   git add .
   git commit -m "Configuración para despliegue en Render"
   git push origin main
   ```
2. Inicia sesión en tu cuenta de [Render Dashboard](https://dashboard.render.com/).
3. Haz clic en el botón **New +** (arriba a la derecha) y selecciona **Blueprint**.
4. Conecta tu repositorio de GitHub donde se encuentra este proyecto.
5. Render detectará automáticamente el archivo `render.yaml` y te mostrará los dos recursos a crear:
   - `biblioteca-db` (PostgreSQL)
   - `biblioteca-web` (Web Service con Docker)
6. Haz clic en **Apply**.
7. ¡Listo! Render aprovisionará la base de datos y compilará la imagen de Docker.

---

## 🛠️ Opción 2: Despliegue Manual desde el Dashboard de Render

Si prefieres crear los servicios paso a paso:

### Paso 1: Crear la Base de Datos PostgreSQL
1. En Render Dashboard, haz clic en **New +** > **PostgreSQL**.
2. Configura los siguientes campos:
   - **Name:** `biblioteca-db`
   - **Database:** `biblioteca`
   - **User:** `biblioteca_user`
   - **Region:** Selecciona la más cercana (ej. *Oregon* o *Frankfurt*).
   - **Plan:** *Free* (o el de tu preferencia).
3. Haz clic en **Create Database**.
4. Una vez creada, copia el valor de **Internal Database URL** (empieza con `postgres://...` o `postgresql://...`).

### Paso 2: Crear el Web Service
1. En Render Dashboard, haz clic en **New +** > **Web Service**.
2. Conecta tu repositorio de GitHub.
3. Configura:
   - **Name:** `biblioteca-web`
   - **Region:** La misma región donde creaste la base de datos.
   - **Runtime:** `Docker`
   - **Plan:** *Free*.
4. En la sección **Advanced** > **Environment Variables**, añade:
   - `DATABASE_URL`: Pega la URL interna de la base de datos obtenida en el Paso 1.
   - `ASPNETCORE_ENVIRONMENT`: `Production`
5. Haz clic en **Create Web Service**.

---

## 🔍 Verificación Post-Despliegue

Una vez completado el despliegue (estado **Live**):
- **Sitio Web Principal (Vistas MVC):**  
  `https://<nombre-de-tu-servicio>.onrender.com`
- **Documentación Interactiva Swagger:**  
  `https://<nombre-de-tu-servicio>.onrender.com/swagger`
- **Endpoints de la API:**  
  - `https://<nombre-de-tu-servicio>.onrender.com/api/libros`
  - `https://<nombre-de-tu-servicio>.onrender.com/api/autores`
  - `https://<nombre-de-tu-servicio>.onrender.com/api/categorias`
  - `https://<nombre-de-tu-servicio>.onrender.com/api/usuarios`
  - `https://<nombre-de-tu-servicio>.onrender.com/api/tarjetas`
  - `https://<nombre-de-tu-servicio>.onrender.com/api/prestamos`

---

## ⚙️ Ajustes Clave Implementados para Producción en la Nube

1. **Parser de Cadena de Conexión Npgsql:** `Program.cs` detecta automáticamente si `DATABASE_URL` proviene de Render en formato URI (`postgresql://user:pass@host:port/db`) y la transforma a parámetros nativos de Npgsql con SSL obligatorio (`SslMode.Require`).
2. **Compatibilidad de Puerto Dinámico:** Soporta la variable `$PORT` inyectada por Render y expone el puerto estándar `8080`.
3. **Inicialización y Semillado Resiliente:** Se encapsuló la migración automática (`EnsureCreated`) y el cargado de datos (`SeedData.Poblar`) con captura de excepciones y logging para evitar caídas durante el arranque en frío del contenedor.
