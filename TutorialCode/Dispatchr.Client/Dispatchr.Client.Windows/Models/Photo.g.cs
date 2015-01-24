// generated 9/25/2014 6:48:47 PM
namespace Dispatchr.Client.Models
{
    using Common;
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Poco = Dispatchr.Models.Poco;
    using LocalSQLite;

    [Table("Photo")]
    public partial class Photo : Models.ModelBase<Photo, System.Guid>
    {            
		[PrimaryKey, Column("Id")]
        public override System.Guid Id { get { return GetProperty<System.Guid>(); } set { SetProperty(value); } }            
		[Column("AppointmentId")]
        public Nullable<int> AppointmentId { get { return GetProperty<Nullable<int>>(); } set { SetProperty(value); } }             
		[Column("Path")]
        public string Path { get { return GetProperty<string>(); } set { SetProperty(value); } } 

        public Photo() 
        { 
            PartialConstructor();
        }

        partial void PartialConstructor();

        public Poco.Photo ToPoco()
        {
            var poco = new Poco.Photo();
            poco.Id = this.Id;
            poco.AppointmentId = this.AppointmentId;
            poco.Path = this.Path;
            return poco;
        }

        public static Photo FromPoco(Poco.Photo poco)
        {
            var entity = new Photo();
            entity.Id = poco.Id;
            entity.AppointmentId = poco.AppointmentId;
            entity.Path = poco.Path;
            return entity;
        }
    }
}

