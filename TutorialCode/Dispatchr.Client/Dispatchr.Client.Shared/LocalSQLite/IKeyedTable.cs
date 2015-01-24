namespace LocalSQLite
{
    public interface IKeyedTable<T>
    {
        [PrimaryKey, AutoIncrement]
        [Column("Id")]
        T Id { get; set; }
    }
}