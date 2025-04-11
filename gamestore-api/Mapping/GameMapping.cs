using gamestore_api.DTO;
using gamestore_api.Entities;

namespace gamestore_api.Mapping;

// Este archivo contiene la clase GameMapping que se encarga de mapear los objetos de transferencia de datos (DTO) a las entidades del modelo de dominio y viceversa.
// - El mapeo consiste en convertir un objeto de un tipo a otro, en este caso de un DTO a una entidad y viceversa. Esto es útil para separar la lógica de negocio de la lógica de presentación y para evitar que las entidades del modelo de dominio se expongan directamente a la capa de presentación o a la API.
// - El uso de DTOs permite tener un control más preciso sobre los datos que se envían y reciben a través de la API, así como también permite validar y transformar los datos antes de ser procesados por la lógica de negocio.
// - Además, el uso de DTOs permite tener una mayor flexibilidad y escalabilidad en la aplicación, ya que se pueden agregar o modificar los DTOs sin afectar directamente a las entidades del modelo de dominio. Esto es especialmente útil en aplicaciones grandes y complejas donde se requiere una mayor separación de responsabilidades y una mejor organización del código.
// - En este caso, la clase GameMapping contiene un método de extensión que permite convertir un objeto de tipo CreateGameDto a un objeto de tipo Game, lo que facilita el proceso de creación de nuevos juegos en la base de datos a partir de los datos enviados por el cliente a través de la API.
public static class GameMapping
{
    // Método de extensión que permite convertir un objeto de tipo CreateGameDto a un objeto de tipo Game
    // - Este método se utiliza para transformar un objeto de tipo CreateGameDto en un objeto de tipo Game, que es la entidad del modelo de dominio que representa un juego en la base de datos.
    public static Game ToEntity(this CreateGameDto game)
    {
        return new Game()
        {
            Name = game.Name,
            GenreId = game.GenreId,
            // Genre = new Genre { Id = game.GenreId }, -> No se necesita asignar el género aquí, ya que se asigna en la base de datos al crear el juego mediante el ID proporcionado.
            Price = game.Price,
            ReleaseDate = game.ReleaseDate
        };
    }

    // Método de extensión que permite convertir un objeto de tipo Game a un objeto de tipo GameDto
    // - Este método se utiliza para transformar un objeto de tipo Game en un objeto de tipo GameDto, que es una representación más simple y adecuada para ser enviada a través de la API.
    public static GameSummaryDto ToGameSummaryDto(this Game game)
    {
        return new GameSummaryDto(
            game.Id,
            game.Name,
            game.Genre!.Name,
            game.Price,
            game.ReleaseDate
        );
    }

    // Método de extensión que permite convertir un objeto de tipo Game a un objeto de tipo GameDtoDetails
    // - Este método se utiliza para transformar un objeto de tipo Game en un objeto de tipo GameDtoDetails, que es una representación más detallada y adecuada para ser enviada a través de la API.
    public static GameDetailsDto ToGameDetailsDto(this Game game)
    {
        return new GameDetailsDto(
            game.Id,
            game.Name,
            game.GenreId,
            game.Price,
            game.ReleaseDate
        );
    }

    // Método de extensión que permite convertir un objeto de tipo UpdateGameDto a un objeto de tipo Game
    // - Este método se utiliza para transformar un objeto de tipo UpdateGameDto en un objeto de tipo Game, que es la entidad del modelo de dominio que representa un juego en la base de datos.
    public static Game ToEntity(this UpdateGameDto game, int id)
    {
        return new Game()
        {
            Id = id,
            Name = game.Name,
            GenreId = game.GenreId,
            // Genre = new Genre { Id = game.GenreId },
            Price = game.Price,
            ReleaseDate = game.ReleaseDate
        };
    }
}
