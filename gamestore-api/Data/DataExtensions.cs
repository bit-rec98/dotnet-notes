using Microsoft.EntityFrameworkCore;

namespace gamestore_api.Data;

// This class contains extension methods for the WebApplication class
// to handle database migrations.
// It is used to ensure that the database is created and up-to-date
// when the application starts.
public static class DataExtensions
{
    // This method applies any pending migrations to the database.
    // It creates a scope to resolve the GameStoreContext from the service provider
    // and then calls the Migrate method on the database.
    // This is useful for ensuring that the database schema is up-to-date
    // with the current model when the application starts.
    public static void MigrateDb(this WebApplication app)
    {
        using var scope = app.Services.CreateScope(); // Create a scope to resolve services
        var dbContext = scope.ServiceProvider.GetRequiredService<GameStoreContext>(); // Get the GameStoreContext
        dbContext.Database.Migrate(); // Apply any pending migrations
    }
}