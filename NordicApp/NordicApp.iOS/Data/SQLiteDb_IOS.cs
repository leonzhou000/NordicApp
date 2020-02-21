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
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var path = Path.Combine(documentsPath, "RaceDb.db3");

            return new SQLiteAsyncConnection(path);
        }
    }
}