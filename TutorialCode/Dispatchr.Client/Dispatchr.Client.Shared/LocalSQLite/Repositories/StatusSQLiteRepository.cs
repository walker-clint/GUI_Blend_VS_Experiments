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
    public interface IStatusSQLiteRepository
    {
        void MergeStatus(IEnumerable<Status> serverStatuses);
        Task<Status> LoadByIdAsync(Int32 id);
        Task<IEnumerable<Status>> LoadAllAsync();
        Task InsertAsync(Status item);
        Task UpdateAsync(Status item);
        Task DeleteAsync(Status item);
    }

    public class StatusSQLiteRepository : RepositoryBase<Status, int>, IStatusSQLiteRepository
    {
        public StatusSQLiteRepository(ISolarizrSqlLiteService solarizrSqlLiteService)
            : base(solarizrSqlLiteService)
        {
        }

        public async void MergeStatus(IEnumerable<Status> serverStatuses)
        {
            var localStatuses = await this.LoadAllAsync();
            foreach (var serverStatus in serverStatuses)
            {
                var isLocal = localStatuses.Any(la => la.Id == serverStatus.Id);
                if (!isLocal)
                {
                    await this.InsertAsync(serverStatus);
                }
            }
        }
    }
}
