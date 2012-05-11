using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheZhazha.Data;
using SKYPE4COMLib;

namespace TheZhazha.Models
{
    public class StatsManager
    {
        #region Lazy singleton impl

        private static readonly Lazy<StatsManager> _instance = new Lazy<StatsManager>(() => new StatsManager());

        public static StatsManager Instance
        {
            get { return _instance.Value; }
        }

        private StatsManager()
        {
            _entries = new Dictionary<string, Dictionary<string, StatsEntry>>();
            LoadData();
        }

        #endregion

        #region Fields

        //                 chat               user    stat
        private readonly Dictionary<string, Dictionary<string, StatsEntry>> _entries;

        #endregion

        #region Public methods

        public void ProcessMessage(ChatMessage message)
        {
            if (message.Chat.Members.Count <= 2
                || !(message.Type == TChatMessageType.cmeEmoted || message.Type == TChatMessageType.cmeSaid))
                return;

            if (!_entries.ContainsKey(message.ChatName))
                _entries.Add(message.ChatName, new Dictionary<string, StatsEntry>());
            
            var chatStats = _entries[message.ChatName];
            
            if (!chatStats.ContainsKey(message.Sender.Handle))
                chatStats.Add(message.Sender.Handle, new StatsEntry(message.ChatName, message.Sender.Handle));

            chatStats[message.Sender.Handle].Apply(message.Body);
            Storage.UpdateStats(chatStats[message.Sender.Handle]);
        }

        public string GetStatistic(string chat)
        {
            var sb = new StringBuilder();
            if (!_entries.ContainsKey(chat) || _entries[chat].Count == 0)
            {
                sb.Append("Для этого чата нет статистики.");
            }
            else
            {
                var list = _entries[chat].Values.ToList<StatsEntry>();
                list.Sort((x, y) => { return y.Symbols.CompareTo(x.Symbols); });
                sb.AppendLine("---");
                sb.AppendLine("    User | Symbols | Words | Messages | Commands");
                for (var i = 0; i < list.Count; i++)
                {
                    if(i < 9) sb.Append(" ");
                    sb.Append(i + 1);
                    sb.Append(". ");
                    sb.Append(list[i].User);
                    sb.Append(" | ");
                    sb.Append(list[i].Symbols);
                    sb.Append(" | ");
                    sb.Append(list[i].Words);
                    sb.Append(" | ");
                    sb.Append(list[i].Messages);
                    sb.Append(" | ");
                    sb.AppendLine(list[i].Commands.ToString());
                }
                sb.AppendLine("---");
            }
            return sb.ToString();
        }

        #endregion

        #region Private methods

        private void LoadData()
        {
            var allEntries = Storage.GetAllStats();
            foreach (var entry in allEntries)
            {
                if (!_entries.ContainsKey(entry.Chat))
                    _entries.Add(entry.Chat, new Dictionary<string, StatsEntry>());
                _entries[entry.Chat].Add(entry.User, entry);
            }
        }

        #endregion
    }
}
