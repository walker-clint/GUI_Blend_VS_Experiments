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
    public interface IAppointmentSQLiteRepository
    {
        Task MergeAppointments(IEnumerable<Appointment> serverAppointments);
        Task<Appointment> LoadByIdAsync(Int32 id);
        Task<IEnumerable<Appointment>> LoadAllAsync();
        Task InsertAsync(Appointment item);
        Task UpdateAsync(Appointment item);
        Task DeleteAsync(Appointment item);
        AsyncTableQuery<Appointment> Query();
    }

    public class AppointmentSQLiteRepository : RepositoryBase<Appointment, int>, IAppointmentSQLiteRepository
    {
        public AppointmentSQLiteRepository(ISolarizrSqlLiteService solarizrSqlLiteService)
            : base(solarizrSqlLiteService)
        {
        }

        public async Task MergeAppointments(IEnumerable<Appointment> serverAppointments)
        {
            var localAppointments = await this.LoadAllAsync();
            foreach (var serverAppointment in serverAppointments)
            {
                var isLocal = localAppointments.Any(la => la.Id == serverAppointment.Id);
                if (!isLocal)
                {
                    try
                    {
                        await this.InsertAsync(serverAppointment);
                    }
                    catch (SQLiteException se)
                    {
                        var a = se.Message;
                    }
                }
            }
        }
    }
}
