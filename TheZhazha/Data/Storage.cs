using System;
using System.Data.SQLite;
using System.Reflection;
using System.Text;
using System.IO;
using System.Resources;

namespace TheZhazha.Data
{
    public static class Storage
    {
        #region Constants

        private const string _dbFileName = "TheZhazha.db";
        private const string _dateFormat = "yyyy-MM-dd HH:mm:ss";

        #endregion

        #region Fields

        private static SQLiteConnection _connection;

        #endregion

        #region Constructors

        static Storage()
        {
            PrepareConnection();
        }

        #endregion

        #region Properties



        #endregion

        #region Quots

        public static void AddQuot(Quot quot, string chat)
        {
            const string sql = "insert into quots (chat, quot, user, date) values (@chat, @quot, @user, @date)";
            var cmd = new SQLiteCommand(sql);
            cmd.Parameters.AddWithValue("@chat", chat);
            cmd.Parameters.AddWithValue("@quot", quot.Text);
            cmd.Parameters.AddWithValue("@user", quot.User);
            cmd.Parameters.AddWithValue("@date", quot.Date.ToString(_dateFormat));
            cmd.Connection = _connection;
            _connection.Open();
            var trans = _connection.BeginTransaction();
            cmd.ExecuteNonQuery();
            trans.Commit();
            cmd.Dispose();
            _connection.Close();
        }

        public static bool CanAddQuot(Quot quot, string chat)
        {
            const string sql = "select date from quots where chat=@chat and user=@user order by date desc";
            var cmd = new SQLiteCommand(sql);
            cmd.Parameters.AddWithValue("@chat", chat);
            cmd.Parameters.AddWithValue("@user", quot.User);
            cmd.Connection = _connection;
            _connection.Open();
            var res = cmd.ExecuteScalar();
            cmd.Dispose();
            _connection.Close();
            if (res == null)
                return true;
            var date = DateTime.Parse(res.ToString());
            return DateTime.Now.Subtract(date).TotalMinutes > Quot.QuotDelayPerUser;
        }

        public static string GetTodayQuots(string chat)
        {
            const string sql = "select quot from quots where chat=@chat and date>@date order by date asc";
            var result = new StringBuilder();
            var now = DateTime.Now;
            var noon = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
            var cmd = new SQLiteCommand(sql);
            cmd.Parameters.AddWithValue("@chat", chat);
            cmd.Parameters.AddWithValue("@date", noon.ToString(_dateFormat));
            cmd.Connection = _connection;
            _connection.Open();
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                result.Append(reader.GetString(0));
                result.Append(" ");
            }
            cmd.Dispose();
            _connection.Close();
            if(result.Length == 0)
            {
                result.Append("Сегодняшний цитатник еще пуст, непорядок!");
            }
            else
            {
                result.Insert(0, "Цитаты дня: ");
            }
            return result.ToString();
        }

        #endregion

        #region Bans

        public static bool IsUserBanned(string userHandle, string chat)
        {
            const string sql = "select reason from banned where user=@user and chat=@chat";
            var cmd = new SQLiteCommand(sql);
            cmd.Parameters.AddWithValue("@user", userHandle);
            cmd.Parameters.AddWithValue("@chat", chat);
            cmd.Connection = _connection;
            _connection.Open();
            var res = cmd.ExecuteScalar();
            cmd.Dispose();
            _connection.Close();
            return res != null;
        }

        #endregion

        #region Warnings

        public static void AddWarning(string user, string reason, string chat)
        {
            const string sql = "insert into warnings (user, chat, date, reason) values (@user, @chat, @date, @reason)";
            var cmd = new SQLiteCommand(sql);
            cmd.Parameters.AddWithValue("@user", user);
            cmd.Parameters.AddWithValue("@chat", chat);
            cmd.Parameters.AddWithValue("@date", DateTime.Now.ToString(_dateFormat));
            cmd.Parameters.AddWithValue("@reason", reason);
            cmd.Connection = _connection;
            _connection.Open();
            var trans = _connection.BeginTransaction();
            cmd.ExecuteNonQuery();
            trans.Commit();
            cmd.Dispose();
            _connection.Close();
        }

        public static int GetUserWarningsCount(string user, string chat)
        {
            const string sql = "select count(user) from warnings where user=@user and chat=@chat and date>@date";
            var now = DateTime.Now;
            var noon = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
            var cmd = new SQLiteCommand(sql);
            cmd.Parameters.AddWithValue("@user", user);
            cmd.Parameters.AddWithValue("@chat", chat);
            cmd.Parameters.AddWithValue("@date", noon.ToString(_dateFormat));
            cmd.Connection = _connection;
            _connection.Open();
            var res = cmd.ExecuteScalar();
            cmd.Dispose();
            _connection.Close();
            return res == null ? 0 : Convert.ToInt32(res);
        }

        #endregion

        #region Private methods

        private static void PrepareConnection()
        {
            _connection = new SQLiteConnection(GetConnectionString());
            ValidateTables();
        }

        private static void ValidateTables()
        {
            var cmd = new SQLiteCommand(Properties.Resources.CreateTables);
            cmd.Connection = _connection;
            _connection.Open();
            var trans = _connection.BeginTransaction();
            cmd.ExecuteNonQuery();
            trans.Commit();
            cmd.Dispose();
            _connection.Close();
        }

        private static string GetConnectionString()
        {
            var path = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return string.Format(@"data source={0}\{1}", path, _dbFileName);
        }

        #endregion
    }
}
