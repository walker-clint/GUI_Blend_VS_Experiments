using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Dispatchr.Client.Models;
using Dispatchr.Client.ServiceProxies;
using LocalSQLite;
using Dispatchr.Client.Services.SolarizrSqlLiteService;

namespace LocalSQLite
{
    public abstract class RepositoryBase<TTable, K> : IRepository<TTable, K>
        where TTable : IKeyedTable<K>, new()
    {
        protected ISolarizrSqlLiteService _solarizrSqlLiteService;

        public RepositoryBase(ISolarizrSqlLiteService solarizrSqlLiteService)
        {
            _solarizrSqlLiteService = solarizrSqlLiteService;
        }

        public Task<TTable> LoadByIdAsync(K id)
        {
            var query = _solarizrSqlLiteService.Conn.Table<TTable>().Where(item => item.Id.Equals(id));
            return query.FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<TTable>> LoadAllAsync()
        {
            var query = _solarizrSqlLiteService.Conn.Table<TTable>();
            var array = (await query.ToListAsync()).ToArray();
            return array;
        }

        public Task InsertAsync(TTable item)
        {
            return _solarizrSqlLiteService.Conn.InsertAsync(item);
        }

        public Task UpdateAsync(TTable item)
        {
            return _solarizrSqlLiteService.Conn.UpdateAsync(item);
        }

        public Task DeleteAsync(TTable item)
        {
            return _solarizrSqlLiteService.Conn.DeleteAsync(item);
        }

        public AsyncTableQuery<TTable> Query() 
        {
            return _solarizrSqlLiteService.Conn.Table<TTable>();
        }
    }
}
