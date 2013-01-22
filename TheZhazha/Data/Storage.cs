using System;
using System.Data.SQLite;
using System.Reflection;
using System.Text;
using System.IO;
using System.Resources;
using TheZhazha.Models;
using System.Collections.Generic;
using Shock.Logger;

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

        #region Admin

        public static bool IsUserAdmin(string userHandle, string chat)
        {
            const string sql = "select user from admins where user=@user and chat=@chat";
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

        public static void AddAdmin(string userHandle, string chat)
        {
            const string sql = "insert into admins (user, chat) values (@user, @chat)";
            var cmd = new SQLiteCommand(sql);
            cmd.Parameters.AddWithValue("@user", userHandle);
            cmd.Parameters.AddWithValue("@chat", chat);
            cmd.Connection = _connection;
            _connection.Open();
            var trans = _connection.BeginTransaction();
            cmd.ExecuteNonQuery();
            trans.Commit();
            cmd.Dispose();
            _connection.Close();
        }

        public static void RemoveAdmin(string userHandle, string chat)
        {
            const string sql = "delete from admins where user=@user and chat=@chat";
            var cmd = new SQLiteCommand(sql);
            cmd.Parameters.AddWithValue("@user", userHandle);
            cmd.Parameters.AddWithValue("@chat", chat);
            cmd.Connection = _connection;
            _connection.Open();
            var trans = _connection.BeginTransaction();
            cmd.ExecuteNonQuery();
            trans.Commit();
            cmd.Dispose();
            _connection.Close();
        }

        public static string GetAdmins(string chat)
        {
            const string sql = "select user from admins where chat=@chat";
            var result = new StringBuilder();
            var cmd = new SQLiteCommand(sql);
            cmd.Parameters.AddWithValue("@chat", chat);
            cmd.Connection = _connection;
            _connection.Open();
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                result.Append(reader.GetString(0));
                result.Append(Environment.NewLine);
            }
            cmd.Dispose();
            _connection.Close();
            if (result.Length == 0)
            {
                result.Append("Нету админов, понел.");
            }
            else
            {
                result.Insert(0, string.Format("Администраторы:{0}", Environment.NewLine));
            }
            return result.ToString();
        }

        #endregion

        #region Statistic

        public static List<StatsEntry> GetAllStats()
        {
            var result = new List<StatsEntry>();
            const string sql = "select * from stats";
            var cmd = new SQLiteCommand(sql);
            cmd.Connection = _connection;
            _connection.Open();
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                result.Add(new StatsEntry(
                    reader.GetString(reader.GetOrdinal("chat")),
                    reader.GetString(reader.GetOrdinal("user")),
                    reader.GetInt64(reader.GetOrdinal("id")))
                    {
                        Messages = reader.GetInt64(reader.GetOrdinal("messages")),
                        Words = reader.GetInt64(reader.GetOrdinal("words")),
                        Symbols = reader.GetInt64(reader.GetOrdinal("symbols")),
                        Commands = reader.GetInt64(reader.GetOrdinal("commands")),
                        Started = DateTime.Parse(reader.GetString(reader.GetOrdinal("started"))),
                    });
            }
            cmd.Dispose();
            _connection.Close();
            return result;
        }

        public static void UpdateStats(StatsEntry entry)
        {
            const string insertSql = "insert into stats (chat, user, started, updated, messages, words, symbols, commands) values (@chat, @user, @started, @updated, @messages, @words, @symbols, @commands)";
            const string updateSql = "update stats set updated=@updated, messages=@messages, words=@words, symbols=@symbols, commands=@commands where id=@id";
            const string getIdSql = "select last_insert_rowid() from stats";
            var now = DateTime.Now.ToString(_dateFormat);
            var cmd = new SQLiteCommand();
            cmd.Connection = _connection;
            _connection.Open();
            var trans = _connection.BeginTransaction();

            // insert new stats entry
            if (entry.Id == 0)
            {
                cmd.CommandText = insertSql;
                cmd.Parameters.AddWithValue("@chat", entry.Chat);
                cmd.Parameters.AddWithValue("@user", entry.User);
                cmd.Parameters.AddWithValue("@started", now);
                cmd.Parameters.AddWithValue("@updated", now);
                cmd.Parameters.AddWithValue("@messages", entry.Messages);
                cmd.Parameters.AddWithValue("@words", entry.Words);
                cmd.Parameters.AddWithValue("@symbols", entry.Symbols);
                cmd.Parameters.AddWithValue("@commands", entry.Commands);

                try
                {
                    cmd.ExecuteScalar();
                    trans.Commit();
                    cmd.CommandText = getIdSql;
                    entry.SetId((long)cmd.ExecuteScalar());
                }
                catch (Exception)
                {
                    entry.SetId(0);
                }
                finally
                {
                    _connection.Close();
                    cmd.Dispose();
                }
            }
            // update existing stats entry
            else
            {
                cmd.CommandText = updateSql;
                cmd.Parameters.AddWithValue("@updated", now);
                cmd.Parameters.AddWithValue("@messages", entry.Messages);
                cmd.Parameters.AddWithValue("@words", entry.Words);
                cmd.Parameters.AddWithValue("@symbols", entry.Symbols);
                cmd.Parameters.AddWithValue("@commands", entry.Commands);
                cmd.Parameters.AddWithValue("@id", entry.Id);

                try
                {
                    cmd.ExecuteNonQuery();
                    trans.Commit();
                }
                catch (Exception)
                { }
                finally
                {
                    _connection.Close();
                    cmd.Dispose();
                }
            }
        }

        #endregion

        #region Settings

        public static List<SettingsEntry> GetAllSettings()
        {
            var result = new List<SettingsEntry>();
            const string sql = "select * from settings";
            var cmd = new SQLiteCommand(sql);
            cmd.Connection = _connection;
            _connection.Open();
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                result.Add(new SettingsEntry(
                    reader.GetString(reader.GetOrdinal("chat")),
                    reader.GetInt16(reader.GetOrdinal("id")),
                    reader.GetBoolean(reader.GetOrdinal("reply")),
                    reader.GetBoolean(reader.GetOrdinal("vbros")),
                    reader.GetBoolean(reader.GetOrdinal("babka")),
                    reader.GetBoolean(reader.GetOrdinal("savelog")))
                );
            }
            cmd.Dispose();
            _connection.Close();
            return result;
        }

        public static void UpdateSettings(SettingsEntry entry)
        {
            const string insertSql = "insert into settings (chat, reply, vbros, babka, savelog) values (@chat, @reply, @vbros, @babka, @savelog)";
            const string updateSql = "update settings set reply=@reply, vbros=@vbros, babka=@babka, savelog=@savelog where id=@id";
            const string getIdSql = "select last_insert_rowid() from settings";
            var cmd = new SQLiteCommand();
            cmd.Connection = _connection;
            _connection.Open();
            var trans = _connection.BeginTransaction();

            // insert new settings entry
            if (entry.Id == 0)
            {
                cmd.CommandText = insertSql;
                cmd.Parameters.AddWithValue("@chat", entry.Chat);
                cmd.Parameters.AddWithValue("@reply", entry.IsReplyEnabled);
                cmd.Parameters.AddWithValue("@vbros", entry.IsVbrosEnabled);
                cmd.Parameters.AddWithValue("@babka", entry.IsBabkaEnabled);
                cmd.Parameters.AddWithValue("@savelog", entry.IsSaveLogEnabled);

                try
                {
                    var res = cmd.ExecuteScalar();
                    trans.Commit();
                    cmd.CommandText = getIdSql;
                    entry.SetId((long)cmd.ExecuteScalar());
                }
                catch (Exception)
                {
                    entry.SetId(0);
                }
                finally
                {
                    _connection.Close();
                    cmd.Dispose();
                }
            }
            // update existing settings entry
            else
            {
                cmd.CommandText = updateSql;
                cmd.Parameters.AddWithValue("@reply", entry.IsReplyEnabled);
                cmd.Parameters.AddWithValue("@vbros", entry.IsVbrosEnabled);
                cmd.Parameters.AddWithValue("@babka", entry.IsBabkaEnabled);
                cmd.Parameters.AddWithValue("@savelog", entry.IsSaveLogEnabled);
                cmd.Parameters.AddWithValue("@id", entry.Id);

                try
                {
                    cmd.ExecuteNonQuery();
                    trans.Commit();
                }
                catch (Exception)
                { }
                finally
                {
                    _connection.Close();
                    cmd.Dispose();
                }
            }
        }

        #endregion

        #region Private methods

        private static void PrepareConnection()
        {
            try
            {
                _connection = new SQLiteConnection(GetConnectionString());
            }
            catch(Exception ex)
            {
                LoggerFacade.Log(ex.Message);
            }
            try
            {
                ValidateTables();
            }
            catch(Exception ex)
            {
                LoggerFacade.Log(ex.Message);
            }
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
