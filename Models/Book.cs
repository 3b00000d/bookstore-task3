namespace BookStore.Models;

public abstract class Book : IEntity
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Author { get; set; } = "";
    public string Category { get; set; } = "";
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public int SoldCount { get; set; }

    public abstract string GetFormat();
}
