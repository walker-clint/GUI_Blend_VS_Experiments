using System.Threading.Tasks;
using LocalSQLite;

namespace Dispatchr.Client.Services.SolarizrSqlLiteService
{
    public interface ISolarizrSqlLiteService
    {
        SQLiteAsyncConnection Conn { get; }
        Task<object> ClearLocalDb();
    }
}