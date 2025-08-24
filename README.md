Catiphy — Prueba técnica (.NET + Frontend)

App web que consume Cat Facts y Giphy.
Muestra un fact y su GIF relacionado (vía backend propio) y guarda un historial de búsquedas. Incluye exportación a CSV.



ARQUITECTURA
/Catiphy.Api           -> ASP.NET Core Web API (.NET 8)
  - Endpoints: /api/fact, /api/gif, /api/history, /api/history/export
  - ADO.NET puro para SQL Server
  - Clientes Http: CatFacts, Giphy
  - Repositorio: SearchHistoryRepository

/CatiphyWeb            -> Blazor Server (.NET 8)
  - Pestañas: Buscar (resultado actual) / Historial
  - Buscar: Tarteja por Gif: GIF izquierda + Fact derecha (centrado), botón Refrescar GIF, selector de cantidad (1–4)
  - Historial: tabla paginada + botón Exportar CSV


Requisitos

.NET 8 SDK

SQL Server (Developer/Express/LocalDB)

Git

Visual Studio 2022 / VS Code
//////////////////////////////////////////////////////////
Base de datos

Ejecuta en SQL Server (ajusta dbo si usas otro esquema):

CREATE DATABASE CatiphyDB;
GO
USE CatiphyDB;
GO

CREATE TABLE dbo.SearchHistory (
    Id          INT IDENTITY(1,1) PRIMARY KEY,
    Fecha       DATETIMEOFFSET(0) NOT NULL DEFAULT SYSUTCDATETIME(), -- fecha de búsqueda (UTC)
    FactText    NVARCHAR(1024)    NOT NULL,
    ThreeWords  NVARCHAR(128)     NOT NULL,
    GifUrl      NVARCHAR(1024)    NOT NULL
);
GO

-- Índices sugeridos
CREATE INDEX IX_SearchHistory_Fecha ON dbo.SearchHistory(Fecha DESC);
CREATE INDEX IX_SearchHistory_ThreeWords ON dbo.SearchHistory(ThreeWords);

///////////////////////////////////////////////////////////////////////

Configuración

Cambiar la cadena de conexion a la bd, cambiar user id y el pasword por el uses.


API (Catiphy.Api)

En appsettings.json 

{
  "ConnectionStrings": {
    "Default": "Server=localhost;Database=CatiphyDB;User Id=catiphy_user;Password=Bigotes12345;TrustServerCertificate=true;"
  },
}


confrimar que la key   "GIPHY_API_KEY": "voaNIOg1u7ONPbckzWK71C48YqCOkhVP"   este ubiucada en launchsettings.json en propierties debajo de:
    "IIS Express": {
      "commandName": "IISExpress",
      "launchBrowser": true,
      "launchUrl": "swagger",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development",

///////////////////////////////////////////////////////////////////////////////////////

Web (CatiphyWeb)

En Program.cs del proyecto Web, el HttpClient debe apuntar a la API:

builder.Services.AddHttpClient<CatiphyWeb.Services.CatiphyApi>(c =>
{
    c.BaseAddress = new Uri("https://localhost:44360/"); 
});

SI AL LEVANTAR LA API, LE SALE OTRO PUERTO, POR FAVOR ACTUALIZAAR EL PROGRAM.CS

Ajusta el puerto al que usa tu API (míralo en Swagger o launchSettings.json).

▶️ Cómo ejecutar
1) Levanta la API
cd Catiphy.Api
dotnet restore
dotnet run


Abre Swagger: https://localhost:44360/swagger

2) Levanta el Web (Blazor Server)
cd CatiphyWeb
dotnet restore
dotnet run


Abre: https://localhost:44334/

🧪 Endpoints principales

GET /api/fact
Trae un Cat Fact aleatorio.

GET /api/gif?query=...&fact=...&prev=...
Busca GIF en Giphy por query (las primeras 3 palabras del fact).

GET /api/history?skip=0&take=20
Paginado: retorna { items: [...], total: N }.

GET /api/history/export
Descarga HistorialCatiphy.csv (UTF-8 con BOM para Excel).

Frontend (flujo)

  -Pestaña Buscar

  -Al iniciar o clic en “Nuevo dato”:

  -GET /api/fact

  -Tomar tres primeras palabras → query
  
  -GET /api/gif?query=...&fact=... → muestra GIF + guarda historial
  
  -Refrescar GIF: vuelve a llamar /api/gif con mismo fact (se registra nuevo GIF en historial).
  
  -Cantidad de GIFs: selector 1–4 (por defecto 1 para cumplir el requerimiento); cuando >=2, grilla 2 columnas.
  
  -Pestaña Historial
  
  -Tabla paginada con Fecha (UTC) · Cat Fact · Tres palabras · URL.
  
  -Exportar CSV (botón arriba a la derecha).


  /////////////////////////////////////////

Posibles errores


404 al exportar CSV: el botón debe apuntar a la API, no al puerto del Web.
En CatiphyApi.GetExportUrl() se construye con HttpClient.BaseAddress (44360).