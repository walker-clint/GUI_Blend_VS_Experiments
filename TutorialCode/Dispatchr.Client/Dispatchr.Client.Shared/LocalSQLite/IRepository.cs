using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace LocalSQLite
{
    interface IRepository<T, K>
     where T : IKeyedTable<K>, new()
    {
        Task<T> LoadByIdAsync(K id);
        Task<IEnumerable<T>> LoadAllAsync();
        Task InsertAsync(T item);
        Task UpdateAsync(T item);
        Task DeleteAsync(T item);
        AsyncTableQuery<T> Query();

    }
}
