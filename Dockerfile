# Etapa 1: Compilación y publicación
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copiar archivo de proyecto y restaurar dependencias (aprovechando cache de capas)
COPY ["BibliotecaEFCore.csproj", "./"]
RUN dotnet restore "BibliotecaEFCore.csproj"

# Copiar todo el código y generar el artefacto de publicación
COPY . .
RUN dotnet publish "BibliotecaEFCore.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Etapa 2: Imagen final de ejecución
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# Configuración de variables de entorno para ejecución en contenedores
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "BibliotecaEFCore.dll"]
