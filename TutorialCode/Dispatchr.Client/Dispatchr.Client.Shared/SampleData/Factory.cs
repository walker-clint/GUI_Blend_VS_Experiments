using Dispatchr.Client.Common;
using System;
using System.Collections.Generic;
using System.Text;
using Dispatchr.Client.Models;

namespace Dispatchr.Client.SampleData
{
    public static class Factory
    {
        public static Models.User GenUser()
        {
            return new Models.User
            {
                Email = "john@solarizr.onmicrosoft.com",
                Enabled = true,
                Name = "John Smith"
            };
        }

        public static IEnumerable<Models.Appointment> GenAppointments()
        {
            int id = 1;
            int streetNumber = 500;
            const int increment = 10;
            const string addressSuffix = " 12th Street Golden CO 80401";
            var date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 8, 0, 0) - TimeSpan.FromDays(1);
            var random = new Random();

            for (int days = 0; days < 10; days++)
            {
                int numberOfAppointments = random.Next(3, 15);
                for (int number = 0; number < numberOfAppointments; number++)
                {
                    var appointment = CreateAppointment(date, days, number, increment, addressSuffix, ref id, ref streetNumber);
                    yield return appointment;
                }
                streetNumber = 500;
            }
        }

        private static Appointment CreateAppointment(DateTime date, int days, int number, int increment, string addressSuffix,
            ref int id, ref int streetNumber)
        {
            var appointment = new Models.Appointment
            {
                Id = id++,
                Date = date + TimeSpan.FromDays(days) + TimeSpan.FromMinutes(30 * number),
                Map = "ms-appx:///SampleData/Images/DesignHeroMap.png",
                Location = (streetNumber += increment) + addressSuffix,
                Latitude = 39.7392,
                Longitude = -104.9847,
                Details = streetNumber + " Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nullam nec fringilla eros. "
                + "Etiam ipsum odio, rutrum vel neque non, facilisis commodo lectus. Nam sodales imperdiet arcu, "
                + "ac tincidunt urna accumsan non. In eget interdum lectus, luctus accumsan quam. "
                + "Sed id ante pretium, interdum magna sit amet, porta ante.",
                StatusId = 1
            };
            return appointment;
        }

        public static Models.Appointment GenAppointment()
        {
            int id = 1;
            int streetNumber = 500;
            const int increment = 10;
            const string addressSuffix = " 12th Street Golden CO 80401";
            var date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 8, 0, 0) - TimeSpan.FromDays(1);

            return CreateAppointment(date, 2, 5, increment, addressSuffix, ref id, ref streetNumber);
        }

    }
}
