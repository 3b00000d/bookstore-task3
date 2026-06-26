namespace BookStore.Models;

public class Purchase : IEntity
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public List<int> BookIds { get; set; } = new();
    public DateTime Date { get; set; }
    public decimal Total { get; set; }
}
