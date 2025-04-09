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
// Registrando el contexto de la base de datos en el contenedor de servicios mediante el método .AddScoped<GameStoreContext>(connectionString) aunque no se vea explícitamente en el código, ya que es un método de extensión de la clase WebApplicationBuilder. De esta manera el contexto de la base de datos se inyecta en los controladores y servicios que lo necesiten, permitiendo acceder a la base de datos de manera sencilla y eficiente.
// Al usar implícitamente el método .AddScoped<GameStoreContext>(connectionString) en el builder, se está registrando el contexto de la base de datos en el contenedor de servicios de la aplicación. Esto significa que cada vez que se inyecte el contexto de la base de datos en un controlador o servicio, se creará una nueva instancia del contexto para esa solicitud específica. Esto es útil para evitar problemas de concurrencia (por ejemplo, al tener una única instancia que maneja varias peticiones) y garantizar que cada solicitud tenga su propia instancia del contexto de la base de datos. 
// También asegurarse que las conexiones sean abiertas y cerradas correctamente, evitando problemas de rendimiento y recursos.
// Dado que dbContext no es thread safe (no es seguro para múltiples hilos), es importante que cada solicitud tenga su propia instancia del contexto de la base de datos.
// Es importante tener en cuenta la transaccionalidad ya que al tener un contexto de la base de datos por cada solicitud, si una solicitud falla, no afectará a las demás solicitudes que se estén procesando en ese momento. Esto es especialmente útil en aplicaciones web donde múltiples usuarios pueden estar interactuando con la aplicación al mismo tiempo y cada uno de ellos puede estar realizando diferentes operaciones en la base de datos. Por lo que así se logra asegurar que hay una única unidad de trabajo por cada solicitud y que los cambios realizados en la base de datos son consistentes y no se ven afectados por otras solicitudes que se estén procesando al mismo tiempo.
// El hecho de reusar la misma instancia del contexto de la base de datos para manejar múltiples peticiones puede llevar a problemas de uso de memoria dado que el contexto de la base de datos mantiene un seguimiento de los cambios realizados en las entidades y si se reutiliza la misma instancia para múltiples peticiones, puede llevar a un uso excesivo de memoria y a problemas de rendimiento. Además, si se reutiliza la misma instancia del contexto de la base de datos para manejar múltiples peticiones, puede llevar a problemas de concurrencia y a errores al intentar acceder a los mismos datos desde diferentes hilos. Por lo que es recomendable crear una nueva instancia del contexto de la base de datos para cada solicitud.
// Por eso es útil usar el método .AddScoped<GameStoreContext>(connectionString) en estos casos para lograr una mejor gestión de los recursos y evitar problemas de concurrencia y rendimiento. 

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
