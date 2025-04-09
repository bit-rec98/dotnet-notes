using System.ComponentModel.DataAnnotations;

namespace gamestore_api.DTO;
public record class CreateGameDto(
    [Required][StringLength(50)] string Name,
    [Required][StringLength(20)] string Genre,
    [Range(1, 100)] decimal Price,
    DateOnly ReleaseDate
);

// A los DTOs se les puede implementar Data Annotations para validar los datos que se envían al servidor. En este caso, se está utilizando el atributo [Required] para indicar que los campos son obligatorios, [StringLength] para indicar la longitud máxima de los campos y [Range] para indicar el rango de valores permitidos.