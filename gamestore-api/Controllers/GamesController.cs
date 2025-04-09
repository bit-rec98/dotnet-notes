using gamestore_api.Data;
using gamestore_api.DTO;
using gamestore_api.Entities;

namespace gamestore_api.Controllers;

public static class GamesController
{
    const string GetGameEndpointName = "GetGame"; // Se define un nombre para la ruta de obtener un juego por su ID

    // Creación de una lista de juegos
    private static readonly List<GameDto> games = [
        new GameDto(
        1,
        "The Legend of Zelda: Breath of the Wild",
        "Action-adventure",
        59.99m,
        new DateOnly(2017, 3, 3)
    ),
    new GameDto(
        2,
        "Super Mario Odyssey",
        "Platformer",
        59.99m,
        new DateOnly(2017, 10, 27)
    ),
    new GameDto(
        3,
        "Mario Kart 8 Deluxe",
        "Racing",
        59.99m,
        new DateOnly(2017, 4, 28)
    ),
    new GameDto(
        4,
        "Super Smash Bros. Ultimate",
        "Fighting",
        59.99m,
        new DateOnly(2018, 12, 7)
    ),
    new GameDto(
        5,
        "Animal Crossing: New Horizons",
        "Social simulation",
        59.99m,
        new DateOnly(2020, 3, 20)
    )
    ];

    // Método extensión para mapear los endpoints de los juegos
    public static WebApplication MapGamesEndpointsWithWebApplication(this WebApplication app)
    {
        var group = app.MapGroup("games"); // Se crea un grupo de rutas para los endpoints de los juegos, lo que permite definir una ruta base para todos los endpoints

        // Petición GET para obtener todos los juegos
        app.MapGet("games", () => games); // Minimal API -> No se realiza lógica compleja, por lo que no es necesario incluir la keyword return ni usar llaves. 

        // Petición GET para obtener un juego por su ID
        app.MapGet("games/{id}", (int id) =>
        {
            GameDto? game = games.Find(game => game.Id == id); // Se busca el juego por su ID

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
                var index = games.FindIndex(game => game.Id == id); // Se busca el índice del juego a actualizar

                if (index == -1) // -1 como resultado de la búsqueda por ID indica que no se encontró el juego
                {
                    return Results.NotFound(); // Se retorna un código 404 Not Found si no se encuentra el juego
                }

                games[index] = new GameDto(
                    id,
                    updatedGame.Name,
                    updatedGame.Genre,
                    updatedGame.Price,
                    updatedGame.ReleaseDate
                ); // Se actualiza el juego con los datos recibidos en el payload del body

                return Results.NoContent(); // Se retorna un código 204 No Content
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

    // Método extensión para mapear los endpoints de los juegos
    public static RouteGroupBuilder MapGamesEndpointsWithRouteGroupBuilder(this WebApplication app)
    {
        var group = app.MapGroup("games"); // Se crea un grupo de rutas para los endpoints de los juegos, lo que permite definir una ruta base para todos los endpoints

        // Petición GET para obtener todos los juegos
        group.MapGet("/", () => games); // Minimal API -> No se realiza lógica compleja, por lo que no es necesario incluir la keyword return ni usar llaves. 

        // Petición GET para obtener un juego por su ID
        group.MapGet("/{id}", (int id) =>
        {
            GameDto? game = games.Find(game => game.Id == id); // Se busca el juego por su ID

            return game is null ? Results.NotFound() : Results.Ok(game); // Se retorna el juego si se encuentra, de lo contrario se retorna un código 404 Not Found
        })
        .WithName(GetGameEndpointName); // Se le asigna un nombre a la ruta para poder referenciarla en otros lugares

        // Petición POST para crear un juego
        group.MapPost("/", (CreateGameDto newGame, GameStoreContext dbContext) =>
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

                // Creación de un nuevo DTO con los datos del juego creado, se debe crear para evitar devolver la entidad interna al cliente
                GameDto gameDto = new(
                    game.Id,
                    game.Name,
                    game.Genre!.Name,
                    game.Price,
                    game.ReleaseDate
                ); 

                return Results.CreatedAtRoute(GetGameEndpointName, new { id = game.Id }, gameDto); // Se retorna un código 201 Created y la ruta de la nueva entidad creada junto con el payload de la entidad creada
            }
        )
        .WithParameterValidation(); // Se habilita la validación de los parámetros de la petición mediante Data Annotations en el DTO, esto se logra mediante el paquete MinimalApis.Extensions que se instala en el proyecto a través de NuGet

        // Petición PUT para actualizar un juego particular que se selecciona por su ID
        group.MapPut("/{id}", (int id, UpdateGameDto updatedGame) =>
            {
                var index = games.FindIndex(game => game.Id == id); // Se busca el índice del juego a actualizar

                if (index == -1) // -1 como resultado de la búsqueda por ID indica que no se encontró el juego
                {
                    return Results.NotFound(); // Se retorna un código 404 Not Found si no se encuentra el juego
                }

                games[index] = new GameDto(
                    id,
                    updatedGame.Name,
                    updatedGame.Genre,
                    updatedGame.Price,
                    updatedGame.ReleaseDate
                ); // Se actualiza el juego con los datos recibidos en el payload del body

                return Results.NoContent(); // Se retorna un código 204 No Content
            }
        )
        .WithParameterValidation(); // Se habilita la validación de los parámetros de la petición mediante Data Annotations en el DTO, esto se logra mediante el paquete MinimalApis.Extensions que se instala en el proyecto a través de NuGet

        // Petición DELETE para eliminar un juego particular que se selecciona por su ID
        group.MapDelete("/{id}", (int id) =>
            {
                games.RemoveAll(game => game.Id == id); // Se elimina el juego de la lista de juegos

                return Results.NoContent(); // Se retorna un código 204 No Content
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