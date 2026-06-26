using BookStore.Helpers;
using BookStore.Models;
using BookStore.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BookStore.Services;

public class BookStoreService
{
    private readonly object _lock = new();

   
    public Repository<Book> BookRepo { get; }
    public Repository<Customer> CustomerRepo { get; }
    public Repository<Purchase> PurchaseRepo { get; }

    public event Action<Book>? OnBookOutOfStock;

    // Constructor Injection
    public BookStoreService(Repository<Book> bookRepo, Repository<Customer> customerRepo, Repository<Purchase> purchaseRepo)
    {
        BookRepo = bookRepo;
        CustomerRepo = customerRepo;
        PurchaseRepo = purchaseRepo;
    }

    public Book AddBook(string title, string author, string category, decimal price, int stock, string format)
    {
        Book book = format.ToLower() switch
        {
            "ebook" => new EbookBook(),
            "audiobook" => new Audiobook(),
            _ => new PaperbackBook()
        };

        book.Title = title;
        book.Author = author;
        book.Category = category;
        book.Price = price;
        book.Stock = stock;

        BookRepo.Add(book);
        return book;
    }

    public bool RemoveBook(int id)
    {
        
        var book = BookRepo.GetAll().FirstOrDefault(b => b.Id == id);
        if (book != null)
        {
            BookRepo.Remove(book);
            return true;
        }
        return false;
    }

    public List<Book> SearchBooks(string keyword)
    {
        return BookRepo.GetAll()
            .Where(b =>
                b.Title.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                b.Author.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                b.Category.Contains(keyword, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    public Customer RegisterCustomer(string name, string email)
    {
        var customer = new Customer
        {
            Name = name,
            Email = email
        };

        CustomerRepo.Add(customer);
        return customer;
    }

    public Purchase RecordPurchase(int customerId, List<int> bookIds)
    {
        lock (_lock)
        {
            var customer = CustomerRepo.GetAll().FirstOrDefault(c => c.Id == customerId)
                ?? throw new Exception("Customer not found.");

            if (bookIds.Count == 0)
                throw new Exception("Purchase must contain at least one book.");

            var books = new List<Book>();

            foreach (var bookId in bookIds)
            {
                var book = BookRepo.GetAll().FirstOrDefault(b => b.Id == bookId)
                    ?? throw new Exception($"Book with ID {bookId} not found.");

                if (book.Stock <= 0)
                    throw new Exception($"'{book.Title}' is out of stock.");

                books.Add(book);
            }

            decimal total = 0;

            foreach (var book in books)
            {
                book.Stock--;
                book.SoldCount++;
                total += book.Price;

                if (book.Stock == 0)
                    OnBookOutOfStock?.Invoke(book);
            }

            customer.TotalSpent += total;

            var purchase = new Purchase
            {
                CustomerId = customerId,
                BookIds = bookIds,
                Date = DateTime.Now,
                Total = total
            };

           
            return purchase;
        }
    }

    public void ApplyRuleToBooks(List<Book> books, Action<Book> rule)
    {
        foreach (var book in books)
        {
            rule(book);
            BookRepo.Update(book); 
        }
    }

    public decimal GetTotalRevenue()
    {
        return CustomerRepo.GetAll().Sum(c => c.TotalSpent);
    }

    public Book? GetBestSellingBook()
    {
        return BookRepo.GetAll()
            .OrderByDescending(b => b.SoldCount)
            .FirstOrDefault();
    }

    public Customer? GetTopCustomer()
    {
        return CustomerRepo.GetAll()
            .OrderByDescending(c => c.TotalSpent)
            .FirstOrDefault();
    }

    
}