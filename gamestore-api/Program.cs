//* Este archivo es el primer archivo que se ejecuta en la aplicación
using gamestore_api.Controllers;
using gamestore_api.Data;

var builder = WebApplication.CreateBuilder(args);

// Cadena de conexión a la base de datos SQLite, no se suele agregar acá, sino en el archivo de configuración de la aplicación (appsettings.json) o en variables de entorno
// var connectionString = "Data Source=GameStore.db"; 
var connectionString = builder.Configuration.GetConnectionString("GameStore");

// Se añade el contexto de la base de datos al contenedor de servicios, utilizando SQLite como proveedor de base de datos y la cadena de conexión definida anteriormente.
// De esta manera se pueden agregar instancias de diversos servicios a la aplicación, como el contexto de la base de datos, controladores, servicios, etc.
builder.Services.AddSqlite<GameStoreContext>(connectionString);

//? El builder se puede configurar de diversas maneras según sea necesario a partir de la creación del builder
// builder.Configuration();

/* 
    ? En el builder se puede configurar:
    - Kestrel
    - Variables de entorno
    - Logs de consola
    - Servicios de configuración
 */

//? Al crear app, se comienza a crear el pipeline de configuración
var app = builder.Build(); // Creación de la instancia de WebApplication y ejecución de la configuración del builder definida previamente

// Se configuran los endpoints para procesar diferentes operaciones sobre todos los juegos
app.MapGamesEndpointsWithRouteGroupBuilder();

// Se configuran los endpoints para procesar diferentes operaciones sobre un juego específico
app.MigrateDb(); // Aplicación de migraciones pendientes a la base de datos al inicio de la aplicación

app.Run(); // Ejecución del host 
