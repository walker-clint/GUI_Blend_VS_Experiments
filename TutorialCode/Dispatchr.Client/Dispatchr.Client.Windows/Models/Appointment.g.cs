// generated 9/25/2014 6:48:47 PM
namespace Dispatchr.Client.Models
{
    using Common;
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Poco = Dispatchr.Models.Poco;
    using LocalSQLite;

    [Table("Appointment")]
    public partial class Appointment : Models.ModelBase<Appointment, int>
    {        
        [Ignore] // child/navigation property
        public ICollection<Photo> Photos { get { return GetProperty<ICollection<Photo>>(); } set { SetProperty(value); } }            
		[PrimaryKey, Column("Id")]	
        public override int Id { get { return GetProperty<int>(); } set { SetProperty(value); } }            
		[Column("Date")]
        public System.DateTime Date { get { return GetProperty<System.DateTime>(); } set { SetProperty(value); } }             
		[Column("Details")]
        public string Details { get { return GetProperty<string>(); } set { SetProperty(value); } }             
		[Column("Agent")]
        public Nullable<int> Agent { get { return GetProperty<Nullable<int>>(); } set { SetProperty(value); } }             
		[Column("Location")]
        public string Location { get { return GetProperty<string>(); } set { SetProperty(value); } }             
		[Column("Latitude")]
        public double Latitude { get { return GetProperty<double>(); } set { SetProperty(value); } }             
		[Column("Longitude")]
        public double Longitude { get { return GetProperty<double>(); } set { SetProperty(value); } }             
		[Column("StatusId")]
        public Nullable<int> StatusId { get { return GetProperty<Nullable<int>>(); } set { SetProperty(value); } }             
		[Column("Phone")]
        public string Phone { get { return GetProperty<string>(); } set { SetProperty(value); } }             
		[Column("Map")]
        public string Map { get { return GetProperty<string>(); } set { SetProperty(value); } } 

        public Appointment() 
        { 
            PartialConstructor();
        }

        partial void PartialConstructor();

        public Poco.Appointment ToPoco()
        {
            var poco = new Poco.Appointment();
            if (this.Photos != null)
                poco.Photos = this.Photos.Select(x => x.ToPoco()).ToList();
            poco.Id = this.Id;
            poco.Date = this.Date;
            poco.Details = this.Details;
            poco.Agent = this.Agent;
            poco.Location = this.Location;
            poco.Latitude = this.Latitude;
            poco.Longitude = this.Longitude;
            poco.StatusId = this.StatusId;
            poco.Phone = this.Phone;
            poco.Map = this.Map;
            return poco;
        }

        public static Appointment FromPoco(Poco.Appointment poco)
        {
            var entity = new Appointment();
            if (poco.Photos != null)
                entity.Photos = poco.Photos.Select(x => Photo.FromPoco(x)).ToList();
            entity.Id = poco.Id;
            entity.Date = poco.Date;
            entity.Details = poco.Details;
            entity.Agent = poco.Agent;
            entity.Location = poco.Location;
            entity.Latitude = poco.Latitude;
            entity.Longitude = poco.Longitude;
            entity.StatusId = poco.StatusId;
            entity.Phone = poco.Phone;
            entity.Map = poco.Map;
            return entity;
        }
    }
}

