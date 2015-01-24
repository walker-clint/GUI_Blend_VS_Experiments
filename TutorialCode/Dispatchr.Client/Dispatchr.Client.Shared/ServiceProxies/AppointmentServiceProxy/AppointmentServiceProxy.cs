using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Dispatchr.Client.SampleData;
using poco = Dispatchr.Models.Poco;

namespace Dispatchr.Client.ServiceProxies
{
    class AppointmentServiceProxy : IAppointmentServiceProxy
    {
        Services.ISettings _settings;
        Services.IWebApiService _webApiService;
        LocalSQLite.Repositories.IAppointmentSQLiteRepository _appointmentSQLiteRepository;
        public AppointmentServiceProxy(Services.IWebApiService webApiService,
            Services.ISettings settings,
            LocalSQLite.Repositories.IAppointmentSQLiteRepository appointmentSQLiteRepository)
        {
            _settings = settings;
            _webApiService = webApiService;
            _appointmentSQLiteRepository = appointmentSQLiteRepository;
        }

        public async System.Threading.Tasks.Task<IEnumerable<Models.Appointment>> LoadAsync(Models.User user)
        {
            if (_settings.UseSampleOnly)
                return SampleData.Factory.GenAppointments();
            else
            {
                IEnumerable<Models.Appointment> poco;
                try
                {
                    // load from service
                    var uri = new Uri(System.IO.Path.Combine(_settings.WebApiBase, "Appointments"));
                    poco = (await _webApiService.GetAsync<IEnumerable<poco.Appointment>>(uri))
                        .Select(x => Models.Appointment.FromPoco(x));

                    // insert anything that may be new into local db
                    await _appointmentSQLiteRepository.MergeAppointments(poco);
                }
                catch (Exception)
                {
                    // in this case, the service is failing
                    // don't stop here, we will default to local data
                }

                // assume the local database is the master
                return await _appointmentSQLiteRepository.LoadAllAsync();
            }
        }

        public async Task UpdateAsync(Models.Appointment appointment)
        {
            if (_settings.UseSampleOnly)
                // sample data isn't ever saved
                return;
            else
            {
                // submit means remote server
                // note: pictures have already been (or are in the process of being) uploaded by this time
                var uri = new Uri(System.IO.Path.Combine(_settings.WebApiBase, "Appointments/", appointment.Id.ToString()));
                await _webApiService.PutAsync(uri, appointment.ToPoco());
            }
        }
    }
}
