using gamestore_api.Entities;
using Microsoft.EntityFrameworkCore;

namespace gamestore_api.Data;

// This class represents the DbContext for the game store application.
// It is responsible for managing the database connection and mapping the entities to the database tables.
// This class should be configured with the appropriate database provider (e.g., SQL Server, SQLite, etc.) in the Startup.cs file.
// This class receives the DbContextOptions<GameStoreContext> in its constructor, which contains the configuration for the database connection.
// This class should receive the "DbContextOptions" from the dependency injection container with this class type defined for defining the operations scope.

public class GameStoreContext(DbContextOptions<GameStoreContext> options) : DbContext(options)
{
    // It's necessary to define the DbSet properties for each entity that you want to map to a database table.
    // Each DbSet property represents a table in the database, and the type of the property represents the entity type.
    // LINQ queries can be performed on these DbSet properties to handle data from the database.    
    public DbSet<Game> Games => Set<Game>(); // This property represents the "Games" table in the database. The Set<Game>() method is used to create a DbSet for the Game entity.

    public DbSet<Genre> Genres => Set<Genre>();

    // This method is called when the model for the context is being created.
    // It can be used to configure the model and relationships between entities using the Fluent API.
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // This data seeding method is used to seed the database with initial data.
        // Not recommended for complex data seeding which required complex operations, but useful for simple data.

        modelBuilder.Entity<Genre>().HasData(
            new { Id = 1, Name = "Roleplaying" },
            new { Id = 2, Name = "Action" },
            new { Id = 3, Name = "Adventure" },
            new { Id = 4, Name = "Strategy" },
            new { Id = 5, Name = "Simulation" }
        );
    }

}
