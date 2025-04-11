using gamestore_api.Data;
using gamestore_api.Mapping;
using Microsoft.EntityFrameworkCore;

namespace gamestore_api.Controllers;

// Clase para definir los endpoints de la API relacionados con los géneros de videojuegos
public static class GenresEndpoints
{
    public static RouteGroupBuilder MapGenresEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("genres");

        group.MapGet("/", async (GameStoreContext dbContext) =>
            await dbContext.Genres
                    .Select(genre => genre.ToDto())
                    .AsNoTracking()
                    .ToListAsync()
        );

        // Retorno del grupo de endpoints
        return group;
    }
}
