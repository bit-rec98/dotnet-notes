using gamestore_api.Data;
using gamestore_api.DTO;
using gamestore_api.Entities;
using gamestore_api.Mapping;
using Microsoft.EntityFrameworkCore;

namespace gamestore_api.Controllers;

public static class GamesController
{
    const string GetGameEndpointName = "GetGame"; // Se define un nombre para la ruta de obtener un juego por su ID

    // Creación de una lista de juegos - Simulación de una base de datos
    private static readonly List<GameSummaryDto> games = [
        new GameSummaryDto(
        1,
        "The Legend of Zelda: Breath of the Wild",
        "Action-adventure",
        59.99m,
        new DateOnly(2017, 3, 3)
    ),
    new GameSummaryDto(
        2,
        "Super Mario Odyssey",
        "Platformer",
        59.99m,
        new DateOnly(2017, 10, 27)
    ),
    new GameSummaryDto(
        3,
        "Mario Kart 8 Deluxe",
        "Racing",
        59.99m,
        new DateOnly(2017, 4, 28)
    ),
    new GameSummaryDto(
        4,
        "Super Smash Bros. Ultimate",
        "Fighting",
        59.99m,
        new DateOnly(2018, 12, 7)
    ),
    new GameSummaryDto(
        5,
        "Animal Crossing: New Horizons",
        "Social simulation",
        59.99m,
        new DateOnly(2020, 3, 20)
    )
    ];

    // Método extensión para mapear los endpoints de los juegos - no se usa en este ejemplo
    public static WebApplication MapGamesEndpointsWithWebApplication(this WebApplication app)
    {
        var group = app.MapGroup("games"); // Se crea un grupo de rutas para los endpoints de los juegos, lo que permite definir una ruta base para todos los endpoints

        // Petición GET para obtener todos los juegos
        app.MapGet("games", () => games); // Minimal API -> No se realiza lógica compleja, por lo que no es necesario incluir la keyword return ni usar llaves. 

        // Petición GET para obtener un juego por su ID
        app.MapGet("games/{id}", (int id) =>
        {
            GameSummaryDto? game = games.Find(game => game.Id == id); // Se busca el juego por su ID

            return game is null ? Results.NotFound() : Results.Ok(game); // Se retorna el juego si se encuentra, de lo contrario se retorna un código 404 Not Found
        })
        .WithName(GetGameEndpointName); // Se le asigna un nombre a la ruta para poder referenciarla en otros lugares

        // Petición POST para crear un juego
        // Al inyectar el contexto de la base de datos en el endpoint, ASP.NET Core se va a encargar de crear una nueva instancia del contexto de la base de datos para cada solicitud, lo que garantiza que cada solicitud tenga su propia instancia del contexto y evita problemas de concurrencia y rendimiento.
        app.MapPost("games", (CreateGameDto newGame, GameStoreContext dbContext) =>
            {
                // Creación de un nuevo juego con los datos recibidos en el payload del body
                Game game = new()
                {
                    Name = newGame.Name,
                    Genre = dbContext.Genres.Find(newGame.GenreId), // Se busca el género del juego en la base de datos a través de su ID
                    GenreId = newGame.GenreId,
                    Price = newGame.Price,
                    ReleaseDate = newGame.ReleaseDate
                };

                // Creación manual de los DTOs - Ya no es necesario al implementar la creación de entidades con el contexto de la base de datos
                // ----
                // GameDto game = new(
                //     games.Count + 1,
                //     newGame.Name,
                //     newGame.Genre,
                //     newGame.Price,
                //     newGame.ReleaseDate
                // ); 
                // ----
                // Se crea un nuevo juego con los datos recibidos en el payload del body

                // Se añade el nuevo juego a la lista de juegos
                // games.Add(game);

                // Se añade la nueva entidad al contexto de la base de datos
                dbContext.Games.Add(game); // Se puede omitir el DbSet escribiendo dbContext.Add(game) pero es recomendable usar el DbSet para mantener la claridad del código y seguir las mejores prácticas de programación.

                // Se guardan los cambios en la base de datos (se transforman los cambios en el contexto para transformarlos a queries SQL y enviarlos a la base de datos)
                dbContext.SaveChanges();

                return Results.CreatedAtRoute(GetGameEndpointName, new { id = game.Id }, game); // Se retorna un código 201 Created y la ruta de la nueva entidad creada junto con el payload de la entidad creada
            }
        );

        // Petición PUT para actualizar un juego particular que se selecciona por su ID
        app.MapPut("games/{id}", (int id, UpdateGameDto updatedGame) =>
            {
                // Se busca el índice del juego a actualizar
                // var index = games.FindIndex(game => game.Id == id); 

                // -1 como resultado de la búsqueda por ID indica que no se encontró el juego
                /* if (index == -1)
                {
                    - Se retorna un código 404 Not Found si no se encuentra el juego
                    return Results.NotFound(); 
                } */

                // Se actualiza el juego con los datos recibidos en el payload del body
                /* games[index] = new GameSummaryDto(
                    id,
                    updatedGame.Name,
                    updatedGame.Genre,
                    updatedGame.Price,
                    updatedGame.ReleaseDate
                );  */

                // Se retorna un código 204 No Content
                // return Results.NoContent(); 
            }
        );

        // Petición DELETE para eliminar un juego particular que se selecciona por su ID
        app.MapDelete("games/{id}", (int id) =>
            {
                games.RemoveAll(game => game.Id == id); // Se elimina el juego de la lista de juegos

                return Results.NoContent(); // Se retorna un código 204 No Content
            }
        );

        return app; // Se retorna la instancia de WebApplication para poder encadenar la configuración de los endpoints

        //* app.MapGet(rutaEjecutora, handler); -> Dependiendo el verbo definido y la ruta definida, se puede ejecutar lo declarado en el handler (función lambda)
        // app.MapGet("/", () => "Hello World!"); 
        // Se crea una instancia de WebApplication -> Es el host de la app que permite representar/introducir la implementación de un servidor HTTP para empezar a escuchar y/o hacer peticiones HTTP 

        //? Se pueden configurar diversas opciones en el objeto app
        /* 
            Ejemplo con crud: 

            *GET all
            app.MapGet("/products", () => getProducts())

            *GET by id
            app.MapGet("/products/{id}", (int id) => games.Find(game => game.Id == id))

            *POST
            app.MapPost("/products", (EntityType newEntity) => {
                EntityDto entity = new (
                    entities.Count + 1,
                    newEntity.Name,
                    newEntity.Genre,
                    newEntity.Price,
                    newEntity.ReleaseDate
                );

                entities.Add(entity);

                return Results.CreatedAtRoute(GetEntityEndpointName, new { id = entity.Id }, entity); -> Posible respuesta a retornar
            })

            *PUT
            app.MapGet("/product/:id", () => editProduct())

            *DELETE
            app.MapGet("/product/:id", () => deleteProduct())
        */
    }

    /* 
     ===================================================================
     ===================================================================
     ===================================================================
     ===================================================================
     */

    // Método extensión para mapear los endpoints de los juegos
    public static RouteGroupBuilder MapGamesEndpointsWithRouteGroupBuilder(this WebApplication app)
    {
        var group = app.MapGroup("games"); // Se crea un grupo de rutas para los endpoints de los juegos, lo que permite definir una ruta base para todos los endpoints

        // Petición GET para obtener todos los juegos
        // Minimal API -> No se realiza lógica compleja, por lo que no es necesario incluir la keyword return ni usar llaves. 
        // group.MapGet("/", () => games);

        // Se obtiene la lista de juegos desde la base de datos
        // La función lamba lleva previamente la definición de asincronía, por lo que se debe usar la palabra clave async para indicar que la función es asíncrona y se puede esperar con la palabra clave await 
        group.MapGet("/", async (GameStoreContext dbContext) =>

            // Para interactuar con el contexto de la base de datos, se debe incluir la palabra clave await antes de la llamada al método ToListAsync() ya que esta operación es asíncrona y puede tardar un tiempo en completarse
            // Se usa el método ToListAsync() para ejecutar la consulta de forma asíncrona y obtener los resultados como una lista
            await dbContext.Games
                // Se incluye el género del juego en la consulta para evitar problemas de carga diferida (lazy loading)
                // Si no se incluye la propiedad Genre en la consulta, el género del juego no se cargará automáticamente y se generará una consulta adicional a la base de datos para cargar el género del juego.
                // Esto puede causar problemas de rendimiento y aumentar la complejidad del código, por lo que es recomendable incluir la propiedad Genre en la consulta desde el principio.
                // Es posible que si no se incluye la propiedad Genre en la consulta, el resultado de la consulta sea un objeto Game sin el género cargado (null), lo que puede causar problemas al intentar acceder a la propiedad Genre del objeto Game.
                .Include(game => game.Genre)

                // Se mapean los juegos a DTOs para evitar devolver la entidad interna al cliente
                // se puede acceder a la definición de .ToGameSummaryDto() con FN + F12
                .Select(game => game.ToGameSummaryDto())

                // Se indica a Entity Framework que no se va a modificar la entidad, por lo que no es necesario trackearla. Esto mejora el rendimiento de la consulta y reduce el uso de memoria.
                .AsNoTracking()

                // Se convierten los resultados a una lista de forma asíncrona
                // Se usa el método ToListAsync() para ejecutar la consulta de forma asíncrona y obtener los resultados como una lista
                // Este método retorna una Task<List<GameSummaryDto>> que se puede esperar con la palabra clave await
                // Un objecto Task<List<GameSummaryDto>> es una representación de una operación asíncrona que se completará en el futuro y que devolverá una lista de objetos GameSummaryDto
                // Para cada operación que sea asíncrona, existe una convención que implica el uso del sufijo Async al final del nombre del método, por lo que se recomienda usar el sufijo Async para los métodos asíncronos
                .ToListAsync()
        );

        // Petición GET para obtener un juego por su ID
        group.MapGet("/{id}", async (int id, GameStoreContext dbContext) =>
        {
            // Se busca el juego por su ID
            // GameDto? game = games.Find(game => game.Id == id);

            // Uso del contexto de la base de datos para buscar el juego por su ID - SÍNCRONO
            // Game? game = dbContext.Games.Find(id);

            // Uso del contexto de la base de datos para buscar el juego por su ID - ASÍNCRONO
            Game? game = await dbContext.Games.FindAsync(id);

            // Se retorna el juego si se encuentra, de lo contrario se retorna un código 404 Not Found
            return game is null ?
                Results.NotFound() : Results.Ok(game.ToGameDetailsDto());
        })
        .WithName(GetGameEndpointName); // Se le asigna un nombre a la ruta para poder referenciarla en otros lugares

        // Petición POST para crear un juego
        group.MapPost("/", async (CreateGameDto newGame, GameStoreContext dbContext) =>
            {
                // Creación de un nuevo juego con los datos recibidos en el payload del body
                // Se puede usar el método de extensión ToEntity() para mapear el DTO a la entidad
                Game game = newGame.ToEntity();

                // Se busca el género del juego en la base de datos a través de su ID
                // game.Genre = dbContext.Genres.Find(newGame.GenreId); - No será necesaria esta línea debido a que EF se va a encargar de asignar el género al juego al momento de crear la entidad

                // Creación de un nuevo juego con los datos recibidos en el payload del body
                /* Game game = new()
                {
                    Name = newGame.Name,
                    Genre = dbContext.Genres.Find(newGame.GenreId),
                    GenreId = newGame.GenreId,
                    Price = newGame.Price,
                    ReleaseDate = newGame.ReleaseDate
                }; */

                // Creación manual de los DTOs - Ya no es necesario al implementar la creación de entidades con el contexto de la base de datos
                // ----
                // GameDto game = new(
                //     games.Count + 1,
                //     newGame.Name,
                //     newGame.Genre,
                //     newGame.Price,
                //     newGame.ReleaseDate
                // ); 
                // ----
                // Se crea un nuevo juego con los datos recibidos en el payload del body

                // Se añade el nuevo juego a la lista de juegos
                // games.Add(game);

                // Se añade la nueva entidad al contexto de la base de datos
                dbContext.Games.Add(game); // Se puede omitir el DbSet escribiendo dbContext.Add(game) pero es recomendable usar el DbSet para mantener la claridad del código y seguir las mejores prácticas de programación.

                // Se guardan los cambios en la base de datos (se transforman los cambios en el contexto para transformarlos a queries SQL y enviarlos a la base de datos) de manera síncrona
                // dbContext.SaveChanges();

                // Se guarda el nuevo juego en la base de datos de forma asíncrona
                await dbContext.SaveChangesAsync();

                // Creación de un nuevo DTO con los datos del juego creado, se debe crear para evitar devolver la entidad interna al cliente
                /* GameDto gameDto = new(
                    game.Id,
                    game.Name,
                    game.Genre!.Name,
                    game.Price,
                    game.ReleaseDate
                );  */

                // Se retorna un código 201 Created y la ruta de la nueva entidad creada junto con el payload de la entidad creada
                return Results.CreatedAtRoute(
                    GetGameEndpointName,
                    new { id = game.Id },
                    game.ToGameDetailsDto()
                );
            }
        )
        .WithParameterValidation(); // Se habilita la validación de los parámetros de la petición mediante Data Annotations en el DTO, esto se logra mediante el paquete MinimalApis.Extensions que se instala en el proyecto a través de NuGet

        // Petición PUT para actualizar un juego particular que se selecciona por su ID
        group.MapPut("/{id}", async (int id, UpdateGameDto updatedGame, GameStoreContext dbContext) =>
            {
                // Se busca el índice del juego a actualizar
                // var index = games.FindIndex(game => game.Id == id); 

                // Se busca el juego en la base de datos a través de su ID en el contexto de la base de datos de manera síncrona
                // var existingGame = dbContext.Games.Find(id);

                // Se busca el juego en la base de datos a través de su ID en el contexto de la base de datos de manera asíncrona
                var existingGame = await dbContext.Games.FindAsync(id);

                if (existingGame == null) // null como resultado de la búsqueda por ID indica que no se encontró el juego
                {
                    // Se retorna un código 404 Not Found si no se encuentra el juego
                    return Results.NotFound();
                }

                // Se actualiza el juego con los datos recibidos en el payload del body
                /* games[index] = new GameSummaryDto(
                    id,
                    updatedGame.Name,
                    updatedGame.Genre,
                    updatedGame.Price,
                    updatedGame.ReleaseDate
                ); */

                // Se actualizan los datos del juego en la base de datos mediante el contexto de la base de datos
                // - Se usa el método Entry() para obtener la entrada de la entidad en el contexto de la base de datos, se obtiene como resultado la entidad existente en la base de datos
                // - Se usa el método CurrentValues para obtener los valores actuales de la entidad en el contexto de la base de datos
                // - Se usa el método de extensión ToEntity() para mapear el DTO a la entidad
                // - Se usa el método SetValues() para actualizar los valores de la entidad existente con los valores del DTO
                dbContext.Entry(existingGame)
                    .CurrentValues
                    .SetValues(updatedGame.ToEntity(id));

                // Se aplican los cambios en la base de datos (se transforman los cambios en el contexto para transformarlos a queries SQL y enviarlos a la base de datos) de manera síncrona
                // dbContext.SaveChanges();

                // Se aplican los cambios en la base de datos de forma asíncrona
                await dbContext.SaveChangesAsync();

                return Results.NoContent(); // Se retorna un código 204 No Content
            }
        )
        .WithParameterValidation(); // Se habilita la validación de los parámetros de la petición mediante Data Annotations en el DTO, esto se logra mediante el paquete MinimalApis.Extensions que se instala en el proyecto a través de NuGet

        // Petición DELETE para eliminar un juego particular que se selecciona por su ID
        group.MapDelete("/{id}", async (int id, GameStoreContext dbContext) =>
            {
                // Se elimina el juego de la lista de juegos
                // games.RemoveAll(game => game.Id == id); 

                // Se busca el juego en la base de datos a través de su ID en el contexto de la base de datos con el método .Where(), este método devuelve una colección de entidades que cumplen con la condición especificada, en este caso el ID del juego
                // Se usa el método ExecuteDelete() para eliminar la entidad de la base de datos, este método se encarga de generar la consulta SQL DELETE y ejecutarla en la base de datos. 
                // Esta operación se la conoce como "Batch delete" dado que se puede eliminar una colección de entidades en una sola operación, en lugar de eliminar cada entidad individualmente.
                // Este método es síncrono
                // dbContext.Games
                //     .Where(game => game.Id == id)
                //     .ExecuteDelete();

                // Este método es asíncrono
                await dbContext.Games
                        .Where(game => game.Id == id)
                        .ExecuteDeleteAsync();

                // Se retorna un código 204 No Content
                return Results.NoContent();
            }
        );

        return group; // Se retorna la instancia de WebApplication para poder encadenar la configuración de los endpoints

        //* app.MapGet(rutaEjecutora, handler); -> Dependiendo el verbo definido y la ruta definida, se puede ejecutar lo declarado en el handler (función lambda)
        // app.MapGet("/", () => "Hello World!"); 
        // Se crea una instancia de WebApplication -> Es el host de la app que permite representar/introducir la implementación de un servidor HTTP para empezar a escuchar y/o hacer peticiones HTTP 

        //? Se pueden configurar diversas opciones en el objeto app
        /* 
            Ejemplo con crud: 

            *GET all
            app.MapGet("/products", () => getProducts())

            *GET by id
            app.MapGet("/products/{id}", (int id) => games.Find(game => game.Id == id))

            *POST
            app.MapPost("/products", (EntityType newEntity) => {
                EntityDto entity = new (
                    entities.Count + 1,
                    newEntity.Name,
                    newEntity.Genre,
                    newEntity.Price,
                    newEntity.ReleaseDate
                );

                entities.Add(entity);

                return Results.CreatedAtRoute(GetEntityEndpointName, new { id = entity.Id }, entity); -> Posible respuesta a retornar
            })

            *PUT
            app.MapGet("/product/:id", () => editProduct())

            *DELETE
            app.MapGet("/product/:id", () => deleteProduct())
        */

    }
}