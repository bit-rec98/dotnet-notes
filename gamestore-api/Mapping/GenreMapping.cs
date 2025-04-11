using gamestore_api.DTO;
using gamestore_api.Entities;

namespace gamestore_api.Mapping;

// El modificador static es necesario para que la clase sea un contenedor de métodos de extensión y no una clase normal.
// Un contenedor de métodos de extensión es una clase que contiene métodos estáticos que se pueden llamar como si fueran métodos de instancia de una clase diferente.
// Un método de extensión es un método estático que permite agregar funcionalidad a una clase existente sin modificar su código original.
// En C# , los métodos de extensión se definen en una clase estática y el primer parámetro del método debe tener el modificador this seguido del tipo de la clase que se va a extender.
// Esto se hace para agregar funcionalidad a una clase existente sin modificar su código original y permite extender la funcionalidad de una clase sin necesidad de heredar de ella o modificar su código original.
// En este caso, la clase GenreMapping es un contenedor de métodos de extensión que contiene métodos para mapear entre diferentes tipos de objetos relacionados con géneros de videojuegos.
public static class GenreMapping
{
    public static GenreDto ToDto(this Genre genre)
    {
        return new GenreDto(genre.Id, genre.Name);
    }
}
