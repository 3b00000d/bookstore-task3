using BookStore.Data;
using BookStore.Repositories;
using BookStore.Helpers;
using BookStore.Models;
using BookStore.Services;
using System;
using System.Collections.Generic;
using System.Linq;


using var context = new BookStoreDbContext();


var bookRepo = new Repository<Book>(context);
var customerRepo = new Repository<Customer>(context);
var purchaseRepo = new Repository<Purchase>(context);


var service = new BookStoreService(bookRepo, customerRepo, purchaseRepo);

service.OnBookOutOfStock += book =>
{
    Console.WriteLine($"\n*** '{book.Title}' is now OUT OF STOCK! ***\n");
};

Console.WriteLine("Welcome to BookStore Console App!");
RunMenu(service);
Console.WriteLine("Goodbye!");

static void RunMenu(BookStoreService service)
{
    while (true)
    {
        Console.WriteLine("\n========== BookStore Menu ==========");
        Console.WriteLine("1. Add Book");
        Console.WriteLine("2. Remove Book");
        Console.WriteLine("3. Search Books");
        Console.WriteLine("4. List All Books");
        Console.WriteLine("5. Register Customer");
        Console.WriteLine("6. Record Purchase");
        Console.WriteLine("7. Statistics");
        Console.WriteLine("8. Filter Books");
        Console.WriteLine("9. Apply Discount (10%)");
        Console.WriteLine("0. Exit");
        Console.Write("Choose option: ");

        string? choice = Console.ReadLine();

        try
        {
            switch (choice)
            {
                case "1": AddBook(service); break;
                case "2": RemoveBook(service); break;
                case "3": SearchBooks(service); break;
                case "4": ListBooks(service); break;
                case "5": RegisterCustomer(service); break;
                case "6": RecordPurchase(service); break;
                case "7": ShowStatistics(service); break;
                case "8": FilterBooks(service); break;
                case "9": ApplyDiscount(service); break;
                case "0": return;
                default: Console.WriteLine("Invalid choice. Try again."); break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}

static void AddBook(BookStoreService service)
{
    Console.Write("Title: ");
    string title = ReadRequiredText();

    Console.Write("Author: ");
    string author = ReadRequiredText();

    Console.Write("Category: ");
    string category = ReadRequiredText();

    Console.Write("Price: ");
    decimal price = ReadPositiveDecimal();

    Console.Write("Stock: ");
    int stock = ReadNonNegativeInt();

    Console.Write("Format (Paperback / Ebook / Audiobook): ");
    string format = Console.ReadLine()?.Trim() ?? "Paperback";

    var book = service.AddBook(title, author, category, price, stock, format);
    Console.WriteLine($"Book added! ID: {book.Id}, Format: {book.GetFormat()}");
}

static void RemoveBook(BookStoreService service)
{
    Console.Write("Book ID to remove: ");
    int id = ReadPositiveInt();

    if (service.RemoveBook(id))
        Console.WriteLine("Book removed successfully.");
    else
        Console.WriteLine("Book not found.");
}

static void SearchBooks(BookStoreService service)
{
    Console.Write("Search keyword: ");
    string keyword = ReadRequiredText();

    var results = service.SearchBooks(keyword);

    if (results.Count == 0)
    {
        Console.WriteLine("No books found.");
        return;
    }

    PrintBooks(results);
}

static void ListBooks(BookStoreService service)
{
    var books = service.BookRepo.GetAll().ToList();

    if (books.Count == 0)
    {
        Console.WriteLine("No books yet.");
        return;
    }

    PrintBooks(books);
}

static void RegisterCustomer(BookStoreService service)
{
    Console.Write("Name: ");
    string name = ReadRequiredText();

    Console.Write("Email: ");
    string email = Console.ReadLine()?.Trim() ?? "";

    if (!email.IsValidEmail())
        throw new Exception("Invalid email. Example: user@mail.com");

    var customer = service.RegisterCustomer(name, email);
    Console.WriteLine($"Customer registered! ID: {customer.Id}");
}

static void RecordPurchase(BookStoreService service)
{
    var customers = service.CustomerRepo.GetAll().ToList();
    if (customers.Count == 0)
        throw new Exception("No customers registered yet.");

    var books = service.BookRepo.GetAll().ToList();
    if (books.Count == 0)
        throw new Exception("No books available.");

    Console.WriteLine("\nCustomers:");
    foreach (var c in customers)
        Console.WriteLine($"{c.Id}. {c.Name} - {c.Email}");

    Console.Write("Customer ID: ");
    int customerId = ReadPositiveInt();

    Console.WriteLine("\nBooks:");
    PrintBooks(books);

    var bookIds = new List<int>();

    Console.WriteLine("Enter book IDs one by one (type 0 when done):");
    while (true)
    {
        Console.Write("Book ID: ");
        int id = ReadInt();

        if (id == 0)
            break;

        if (id < 0)
            throw new Exception("Book ID must be positive.");

        bookIds.Add(id);
    }

    var purchase = service.RecordPurchase(customerId, bookIds);
    Console.WriteLine($"Purchase done! Total: {purchase.Total.ToCurrency()}");
}

static void ShowStatistics(BookStoreService service)
{
    decimal revenue = service.GetTotalRevenue();
    var bestBook = service.GetBestSellingBook();
    var topCustomer = service.GetTopCustomer();

    Console.WriteLine($"\nTotal Revenue: {revenue.ToCurrency()}");

    if (bestBook == null || bestBook.SoldCount == 0)
        Console.WriteLine("Best Selling Book: No sales yet.");
    else
        Console.WriteLine($"Best Selling Book: {bestBook.Title} ({bestBook.SoldCount} sold)");

    if (topCustomer == null || topCustomer.TotalSpent == 0)
        Console.WriteLine("Top Customer: No purchases yet.");
    else
        Console.WriteLine($"Top Customer: {topCustomer.Name} ({topCustomer.TotalSpent.ToCurrency()})");
}

static void FilterBooks(BookStoreService service)
{
    var books = service.BookRepo.GetAll().ToList();

    if (books.Count == 0)
    {
        Console.WriteLine("No books to filter.");
        return;
    }

    Console.WriteLine("1. By Category");
    Console.WriteLine("2. By Author");
    Console.WriteLine("3. By Price Range");
    Console.Write("Choose filter: ");

    string? choice = Console.ReadLine();
    List<Book> result;

    switch (choice)
    {
        case "1":
            Console.Write("Category: ");
            string category = ReadRequiredText();
            result = books.FilterByCategory(category);
            break;
        case "2":
            Console.Write("Author: ");
            string author = ReadRequiredText();
            result = books.FilterByAuthor(author);
            break;
        case "3":
            Console.Write("Min price: ");
            decimal min = ReadDecimal();
            Console.Write("Max price: ");
            decimal max = ReadDecimal();

            if (min > max)
                throw new Exception("Min price cannot be greater than max price.");

            result = books.FilterByPriceRange(min, max);
            break;
        default:
            throw new Exception("Invalid filter choice.");
    }

    if (result.Count == 0)
        Console.WriteLine("No books match your filter.");
    else
        PrintBooks(result);
}

static void ApplyDiscount(BookStoreService service)
{
    var books = service.BookRepo.GetAll().ToList();

    if (books.Count == 0)
    {
        Console.WriteLine("No books to discount.");
        return;
    }

    service.ApplyRuleToBooks(books, b => b.Price *= 0.9m);
    Console.WriteLine("10% discount applied to all books.");
}

static void PrintBooks(List<Book> books)
{
    foreach (var b in books)
    {
        Console.WriteLine(
            $"{b.Id}. {b.Title} | {b.Author} | {b.Category} | {b.Price.ToCurrency()} | Stock: {b.Stock} | {b.GetFormat()}");
    }
}

static string ReadRequiredText()
{
    string? text = Console.ReadLine()?.Trim();
    if (string.IsNullOrWhiteSpace(text))
        throw new Exception("This field cannot be empty.");
    return text;
}

static int ReadInt()
{
    string? input = Console.ReadLine();
    if (!int.TryParse(input, out int value))
        throw new Exception("Please enter a valid number.");
    return value;
}

static int ReadPositiveInt()
{
    int value = ReadInt();
    if (value <= 0)
        throw new Exception("Please enter a number greater than 0.");
    return value;
}

static int ReadNonNegativeInt()
{
    int value = ReadInt();
    if (value < 0)
        throw new Exception("Stock cannot be negative.");
    return value;
}

static decimal ReadDecimal()
{
    string? input = Console.ReadLine();
    if (!decimal.TryParse(input, out decimal value))
        throw new Exception("Please enter a valid price.");
    return value;
}

static decimal ReadPositiveDecimal()
{
    decimal value = ReadDecimal();
    if (value <= 0)
        throw new Exception("Price must be greater than 0.");
    return value;
}