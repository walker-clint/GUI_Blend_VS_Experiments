// generated 9/25/2014 6:48:47 PM
namespace Dispatchr.Client.Models
{
    using Common;
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Poco = Dispatchr.Models.Poco;
    using LocalSQLite;

    [Table("Status")]
    public partial class Status : Models.ModelBase<Status, int>
    {            
		[PrimaryKey, Column("Id")]	
        public override int Id { get { return GetProperty<int>(); } set { SetProperty(value); } }            
		[Column("Name")]
        public string Name { get { return GetProperty<string>(); } set { SetProperty(value); } } 

        public Status() 
        { 
            PartialConstructor();
        }

        partial void PartialConstructor();

        public Poco.Status ToPoco()
        {
            var poco = new Poco.Status();
            poco.Id = this.Id;
            poco.Name = this.Name;
            return poco;
        }

        public static Status FromPoco(Poco.Status poco)
        {
            var entity = new Status();
            entity.Id = poco.Id;
            entity.Name = poco.Name;
            return entity;
        }
    }
}

