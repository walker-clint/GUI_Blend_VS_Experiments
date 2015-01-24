using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dispatchr.Client.Models;
using Dispatchr.Client.Services.SolarizrSqlLiteService;
using LocalSQLite;

namespace LocalSQLite.Repositories
{
    public interface IPhotoSQLiteRepository
    {
        Task<Photo> LoadByIdAsync(Guid id);
        Task<IEnumerable<Photo>> LoadAllAsync();
        Task InsertAsync(Photo item);
        Task UpdateAsync(Photo item);
        Task DeleteAsync(Photo item);
        AsyncTableQuery<Photo> Query();
    }

    public class PhotoSQLiteRepository : RepositoryBase<Photo, Guid>, IPhotoSQLiteRepository
    {
        public PhotoSQLiteRepository(ISolarizrSqlLiteService solarizrSqlLiteService)
            : base(solarizrSqlLiteService)
        {
        }
    }
}
