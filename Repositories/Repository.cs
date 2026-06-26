using System;
using System.Collections.Generic;
using System.Linq;
using BookStore.Data;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Repositories
{
   
    public class Repository<T> where T : class
    {
        private readonly BookStoreDbContext _context;
        private readonly DbSet<T> _dbSet;

       
        public Repository(BookStoreDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        
        public void Add(T entity)
        {
            _dbSet.Add(entity);
            _context.SaveChanges();
        }

       
        public void Remove(T entity)
        {
            _dbSet.Remove(entity);
            _context.SaveChanges();
        }

       
        public IEnumerable<T> GetAll()
        {
            return _dbSet.ToList();
        }
        public void Update(T entity)
        {
            _dbSet.Update(entity);
            _context.SaveChanges();
        }

       
        public T GetFirstOrDefault(Func<T, bool> filter)
        {
            return _dbSet.FirstOrDefault(filter);
        }
    }
}