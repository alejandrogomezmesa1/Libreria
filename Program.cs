using BibliotecaEFCore.Data;
using BibliotecaEFCore.Datos;
using Microsoft.EntityFrameworkCore;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

// Configurar puerto para Render si la variable de entorno PORT está presente
var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrEmpty(port))
{
    builder.WebHost.UseUrls($"http://*:{port}");
}

// Configurar conexión a la base de datos (Soporta desarrollo local y PostgreSQL de Render con DATABASE_URL)
var rawConnectionString = Environment.GetEnvironmentVariable("DATABASE_URL")
    ?? builder.Configuration.GetConnectionString("DefaultConnection");

string connectionString;
if (!string.IsNullOrEmpty(rawConnectionString) &&
    (rawConnectionString.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase) ||
     rawConnectionString.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase)))
{
    var uri = new Uri(rawConnectionString);
    var userInfo = uri.UserInfo.Split(':');
    var npgsqlBuilder = new Npgsql.NpgsqlConnectionStringBuilder
    {
        Host = uri.Host,
        Port = uri.Port > 0 ? uri.Port : 5432,
        Username = userInfo.Length > 0 ? userInfo[0] : "",
        Password = userInfo.Length > 1 ? userInfo[1] : "",
        Database = uri.LocalPath.TrimStart('/'),
        SslMode = Npgsql.SslMode.Require
    };
    connectionString = npgsqlBuilder.ToString();
}
else
{
    connectionString = rawConnectionString ?? "";
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// Agregar soporte para controladores y vistas con manejo de ciclos de referencias JSON
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

// Configurar Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.OpenApiInfo
    {
        Title = "Biblioteca EF Core API",
        Version = "v1",
        Description = "API RESTful para la gestión del sistema de biblioteca con Entity Framework Core"
    });
});

var app = builder.Build();

// Habilitar middleware de Swagger (disponible en todos los entornos para facilitar pruebas)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Biblioteca API v1");
    c.RoutePrefix = "swagger";
});

// Manejo de errores en producción
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// Ruta predeterminada
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Crear base de datos y cargar datos iniciales si no existen
using (var scope = app.Services.CreateScope())
{
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        context.Database.EnsureCreated();
        SeedData.Poblar(context);
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocurrió un error al inicializar o poblar la base de datos.");
    }
}

app.Run();
