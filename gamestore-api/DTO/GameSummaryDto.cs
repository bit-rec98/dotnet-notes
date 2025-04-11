namespace gamestore_api.DTO;

/* Se utilizan records en lugar de clases debido a que los records son inmutables, por lo que su modificación estructural para el envío de información del punto A al punto B no sería necesaria. */
/* Los records también evitan la repetición de código que implica la definición de clases que se dedican a contener información */
public record class GameSummaryDto(
    int Id, 
    string Name,
    string Genre,
    decimal Price,
    DateOnly ReleaseDate
);