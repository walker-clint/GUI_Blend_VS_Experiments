// generated 9/25/2014 6:48:47 PM
namespace Dispatchr.Client.Models
{
    using Common;
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Poco = Dispatchr.Models.Poco;
    using LocalSQLite;

    [Table("User")]
    public partial class User : Models.ModelBase<User, int>
    {        
        [Ignore] // child/navigation property
        public ICollection<Appointment> Appointments { get { return GetProperty<ICollection<Appointment>>(); } set { SetProperty(value); } }            
		[PrimaryKey, Column("Id")]	
        public override int Id { get { return GetProperty<int>(); } set { SetProperty(value); } }            
		[Column("Name")]
        public string Name { get { return GetProperty<string>(); } set { SetProperty(value); } }             
		[Column("Email")]
        public string Email { get { return GetProperty<string>(); } set { SetProperty(value); } }             
		[Column("Enabled")]
        public Nullable<bool> Enabled { get { return GetProperty<Nullable<bool>>(); } set { SetProperty(value); } } 

        public User() 
        { 
            PartialConstructor();
        }

        partial void PartialConstructor();

        public Poco.User ToPoco()
        {
            var poco = new Poco.User();
            if (this.Appointments != null)
                poco.Appointments = this.Appointments.Select(x => x.ToPoco()).ToList();
            poco.Id = this.Id;
            poco.Name = this.Name;
            poco.Email = this.Email;
            poco.Enabled = this.Enabled;
            return poco;
        }

        public static User FromPoco(Poco.User poco)
        {
            var entity = new User();
            if (poco.Appointments != null)
                entity.Appointments = poco.Appointments.Select(x => Appointment.FromPoco(x)).ToList();
            entity.Id = poco.Id;
            entity.Name = poco.Name;
            entity.Email = poco.Email;
            entity.Enabled = poco.Enabled;
            return entity;
        }
    }
}

