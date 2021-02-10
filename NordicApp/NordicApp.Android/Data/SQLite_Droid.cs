using System;
using System.IO;
using SQLite;
using Xamarin.Forms;
using NordicApp.Data;
using NordicApp.Droid.Data;

[assembly: Dependency(typeof(SQLiteDb))]

namespace NordicApp.Droid.Data
{
    public class SQLiteDb : ISQLiteDb
    {
        public SQLiteAsyncConnection GetConnection()
        {
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var path = Path.Combine(documentsPath, "SQLiteDB.db3");

            return new SQLiteAsyncConnection(path);
        }
    }
}