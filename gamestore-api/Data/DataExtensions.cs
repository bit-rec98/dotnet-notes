using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace gamestore_api.Data;

// Esta clase contiene métodos de extensión para la clase WebApplication
// para manejar las migraciones de la base de datos.
// Se utiliza para garantizar que la base de datos se cree y esté actualizada
// cuando la aplicación se inicie.
public static class DataExtensions
{
    // Este método aplica cualquier migración pendiente a la base de datos.
    // Crea un alcance para resolver el GameStoreContext desde el proveedor de servicios
    // y luego llama al método Migrate en la base de datos.
    // Esto es útil para garantizar que el esquema de la base de datos esté actualizado
    // con el modelo actual cuando la aplicación se inicie.
    // Se realiza de manera asíncrona y devuelve un objeto Task dado que es un método asíncrono.
    // - El objeto Task representa la operación asíncronsa.
    // - No devuelve ningún valor, por lo que el tipo de retorno es Task.
    // - Se utiliza el modificador 'this' para indicar que es un método de extensión para WebApplication.

    public static async Task MigrateDbAsync(this WebApplication app)
    {
        // Crea un alcance para resolver servicios
        using var scope = app.Services.CreateScope();
        // Obtiene el GameStoreContext
        var dbContext = scope.ServiceProvider.GetRequiredService<GameStoreContext>();
        // Aplica cualquier migración pendiente
        // dbContext.Database.Migrate();

        // Aplica migraciones pendientes y crea la base de datos si no existe de manera asíncrona
        await dbContext.Database.MigrateAsync();
    }
}