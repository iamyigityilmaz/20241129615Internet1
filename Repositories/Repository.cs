using HaberPortali2.Data;                        // AppDbContext için
using Microsoft.EntityFrameworkCore;             // DbSet ve EF Core metotları için
using System;
using System.Collections.Generic;                 // IEnumerable<T> için
using System.Linq;
using System.Linq.Expressions;                   // Expression<Func<T, bool>> için
using System.Threading.Tasks;                    // Task ve async işlemler için

namespace HaberPortali2.Repositories
{
    // Repository<T>: IRepository<T> arayüzünü somutlaştıran generic sınıf.
    // T => herhangi bir entity tipi (User, News, Category vs.)
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly AppDbContext _context;   // Veritabanı bağlantısı
        private readonly DbSet<T> _dbSet;         // T tipine karşılık gelen tabloyu temsil eder

        // Constructor: DI (Dependency Injection) ile context alıyoruz.
        public Repository(AppDbContext context)
        {
            _context = context;                   // Dışarıdan gelen context'i saklıyoruz
            _dbSet = _context.Set<T>();           // EF Core üzerinden T'ye ait DbSet'i alıyoruz
        }

        // Tablodaki tüm kayıtları asenkron olarak döner.
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        // Id'ye göre tek kayıt getirir. Bulamazsa null döner.
        public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        // Verilen şarta (predicate) göre tek bir kayıt bulur.
        public async Task<T?> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.FirstOrDefaultAsync(predicate);
        }

        // Yeni bir kayıt ekler.
        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);        // EF Core'a "bunu ekle" diyoruz
            await _context.SaveChangesAsync();    // Değişiklikleri veritabanına yazar
        }

        // Var olan bir kaydı günceller.
        public async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);                // EF'e bu entity değişti diyoruz
            await _context.SaveChangesAsync();    // Değişiklikleri kaydediyoruz
        }

        // Var olan bir kaydı siler.
        public async Task DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);                // Entity'i silinmek üzere işaretliyoruz
            await _context.SaveChangesAsync();    // Değişiklikleri kaydediyoruz
        }
    }
}
