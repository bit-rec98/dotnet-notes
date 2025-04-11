using System.ComponentModel.DataAnnotations;

namespace gamestore_api.DTO;

// Se recomienda crear un DTO particular para cada operación que sea necesaria
public record UpdateGameDto(
    [Required][StringLength(50)] string Name,
    int GenreId,
    [Range(1, 100)] decimal Price,
    DateOnly ReleaseDate
);