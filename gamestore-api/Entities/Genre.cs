namespace gamestore_api.Entities;

public class Genre
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty; // adding required is also valid if a default value is not provided
}
