using System;
using System.Collections.Generic;           // IEnumerable<T> için
using System.Linq.Expressions;             // Expression<Func<T, bool>> için
using System.Threading.Tasks;              // Task ve async işlemler için

namespace HaberPortali2.Repositories
{
    // IRepository<T>: T tipindeki entity'ler (User, News, Category vs.) için
    // temel CRUD (Create, Read, Update, Delete) işlemlerini tanımlar.
    // where T : class  => T sadece referans tip (sınıf) olabilir, int string vs. olamaz.
    public interface IRepository<T> where T : class
    {
        // Veritabanındaki tüm kayıtları listeler.
        Task<IEnumerable<T>> GetAllAsync();

        // Id'ye göre tek bir kayıt getirir. Bulamazsa null dönebilir.
        Task<T?> GetByIdAsync(int id);

        // Şarta göre tek bir kayıt bulur (örneğin: x => x.Email == "a@a.com").
        Task<T?> FindAsync(Expression<Func<T, bool>> predicate);

        // Yeni bir kayıt ekler.
        Task AddAsync(T entity);

        // Var olan bir kaydı günceller.
        Task UpdateAsync(T entity);

        // Var olan bir kaydı siler.
        Task DeleteAsync(T entity);
    }
}
