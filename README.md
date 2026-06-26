# BookStore Console Application

Simple .NET 8 console app to manage books, customers, and purchases.



## Features

- Add, remove, search, and list books
- Register customers
- Record purchases with multiple books
- Input validation with clear error messages
- Statistics: total revenue, best-selling book, top customer
- Filter books by category, author, or price range
- Book formats: Paperback, Ebook, Audiobook (easy to add new formats)
- Generic in-memory repository
- Custom rules (example: 10% discount)
- Out-of-stock notification
- Save and load data from JSON file (async)


## Example Usage

1. Add a few books (option 1)
2. Register a customer (option 5)
3. Record a purchase with 2 or more books (option 6)
4. Check statistics (option 7)

Data is saved automatically to `bookstore_data.json` when you exit.
