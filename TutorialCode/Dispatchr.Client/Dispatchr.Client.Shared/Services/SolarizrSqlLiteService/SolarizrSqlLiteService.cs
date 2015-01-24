using System.Collections.Generic;
using System.Threading.Tasks;
using Dispatchr.Client.Models;
using LocalSQLite;

namespace Dispatchr.Client.Services.SolarizrSqlLiteService
{
    [Table("Information")]
    public class Information
    {
        [Column("Version")]
        public int Version { get; set; }
    }

    internal class SolarizrSqlLiteService : ISolarizrSqlLiteService
    {
        private readonly ISettings _settings;
        private readonly SQLiteAsyncConnection _conn;

        public SolarizrSqlLiteService(ISettings settings)
        {
            _settings = settings;
            // this will create the db if it doesn't exist
            _conn = new SQLiteAsyncConnection("solarizer.db");
            InitDb();
        }

        public SQLiteAsyncConnection Conn
        {
            get { return _conn; }
        }

        private async Task<object> InitDb()
        {
            // Create tables - does nothing if the tables already exist (except add missing columns)
            var createTasks = new Task[]
            {
                Conn.CreateTableAsync<Appointment>(),
                Conn.CreateTableAsync<Status>(),
                Conn.CreateTableAsync<Photo>(),
                Conn.CreateTableAsync<User>(),
            };

            Task.WaitAll(createTasks);

            // seed the statuses
            if (await Conn.Table<Status>().CountAsync() == 0)
            {
                await Conn.InsertAllAsync(new[]
                {
                    new Status {Id = 1, Name = "Pending"},
                    new Status {Id = 2, Name = "Approved"},
                    new Status {Id = 3, Name = "Denied"},
                });
            }

            return InsertTestDataAsync();
        }

        private async Task InsertTestDataAsync()
        {
            if (_settings.UseSampleOnly && await Conn.Table<Appointment>().CountAsync() == 0)
            {
                // seed the appointments
                var sampleAppointments = SampleData.Factory.GenAppointments();
                await Conn.InsertAllAsync(sampleAppointments);
            }
        }

        public async Task<object> ClearLocalDb()
        {
            await Conn.DropTableAsync<User>();
            await Conn.DropTableAsync<Photo>();
            await Conn.DropTableAsync<Status>();
            await Conn.DropTableAsync<Appointment>();
            return await InitDb();
        }
    }
}