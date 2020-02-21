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
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var path = Path.Combine(documentsPath, "RaceDb.db3");

            return new SQLiteAsyncConnection(path);
        }
    }
}