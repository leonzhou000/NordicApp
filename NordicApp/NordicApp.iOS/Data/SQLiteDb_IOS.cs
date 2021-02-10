using System;
using System.IO;
using SQLite;
using Xamarin.Forms;
using NordicApp.iOS;

using Foundation;
using UIKit;
using NordicApp.iOS.Data;
using NordicApp.Data;

namespace NordicApp.iOS.Data
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