Proyecto realizado como prueba técnica para posición de Desarrollador .NET.

Arquitectura
Backend — Catiphy.Api (ASP.NET Core Web API, .NET 8)

Endpoints:

      GET /api/fact → Cat Fact aleatorio
      GET /api/gif?query=... → GIF relacionado
      GET /api/history?skip=0&take=20 → Historial paginado
      GET /api/history/export → Exportación CSV
      Persistencia: SQL Server usando ADO.NET puro
      Integraciones:
      Cliente HTTP → Cat Facts API
      Cliente HTTP → Giphy API
      Repositorio: SearchHistoryRepository
      Frontend — CatiphyWeb (Blazor Server, .NET 8)

Pestañas:

Buscar: muestra 1–4 GIFs con su fact, botón Refrescar GIF y selector de cantidad.
Historial: tabla paginada con fecha, fact, tres palabras y URL del GIF. Incluye botón Exportar CSV.

INSTRUCCIONES PARA EJECUCIÓN:

EN SQLSERVER:

    CREATE DATABASE CatiphyDB;
    GO
    USE CatiphyDB;
    GO

    CREATE TABLE dbo.SearchHistory (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Fecha DATETIMEOFFSET(0) NOT NULL DEFAULT SYSUTCDATETIME(),
        FactText NVARCHAR(1024) NOT NULL,
        ThreeWords NVARCHAR(128) NOT NULL,
        GifUrl NVARCHAR(1024) NOT NULL
    );
    GO

      // Índices sugeridos
    CREATE INDEX IX_SearchHistory_Fecha ON dbo.SearchHistory(Fecha DESC);
    CREATE INDEX IX_SearchHistory_ThreeWords ON dbo.SearchHistory(ThreeWords);
    

Configuración:
API (Catiphy.Api)

Editar appsettings.json:

    "ConnectionStrings": {
      "Default": "Server=localhost;Database=CatiphyDB;User Id=tU_USUARIO;Password=tU_PASSWORD;TrustServerCertificate=true;"
    }


Web (CatiphyWeb)

En Program.cs, apuntar el HttpClient al puerto de la API:

    builder.Services.AddHttpClient<CatiphyWeb.Services.CatiphyApi>(c =>
    {
        c.BaseAddress = new Uri("https://localhost:44360/"); // cambia según el puerto de tu API
    });

Cómo ejecutar

Levantar la API

    cd Catiphy.Api
    dotnet restore
    dotnet run


Abrir Swagger → https://localhost:44360/swagger

Levantar el frontend

    cd CatiphyWeb
    dotnet restore
    dotnet run


Abrir la web → https://localhost:44334/  //puerto asignado
