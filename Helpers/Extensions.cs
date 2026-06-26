using BookStore.Models;

namespace BookStore.Helpers;

public static class Extensions
{
    public static bool IsValidEmail(this string email)
    {
        return !string.IsNullOrWhiteSpace(email)
               && email.Contains('@')
               && email.Contains('.');
    }

    public static string ToCurrency(this decimal price)
    {
        return price.ToString("C");
    }

    public static List<Book> FilterByCategory(this List<Book> books, string category)
    {
        return books
            .Where(b => b.Category.Equals(category, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    public static List<Book> FilterByAuthor(this List<Book> books, string author)
    {
        return books
            .Where(b => b.Author.Contains(author, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    public static List<Book> FilterByPriceRange(this List<Book> books, decimal min, decimal max)
    {
        return books.Where(b => b.Price >= min && b.Price <= max).ToList();
    }
}
