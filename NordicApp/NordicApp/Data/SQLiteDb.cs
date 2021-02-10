using SQLite;

namespace NordicApp.Data
{
    public interface ISQLiteDb
    {
        SQLiteAsyncConnection GetConnection();
    }
}
