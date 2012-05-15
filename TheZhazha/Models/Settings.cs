using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheZhazha.Data;

namespace TheZhazha.Models
{
    public class Settings
    {
        #region Fields

        private readonly Dictionary<string, SettingsEntry> _entries;
        
        #endregion

        #region Lazy singleton impl

        private static readonly Lazy<Settings> _instance = new Lazy<Settings>(() => new Settings());

        public static Settings Instance
        {
            get { return _instance.Value; }
        }

        private Settings()
        {
            _entries = new Dictionary<string, SettingsEntry>();
            LoadData();
        }

        #endregion

        #region Public methods

        public SettingsEntry Get(string chat)
        {
            if (!_entries.ContainsKey(chat))
            {
                var entry = new SettingsEntry(chat);
                entry.Save();
                _entries.Add(chat, entry);

            }
            return _entries[chat];
        }

        #endregion

        #region Private methods

        private void LoadData()
        {
            var allEntries = Storage.GetAllSettings();
            foreach (var entry in allEntries)
            {
                if (_entries.ContainsKey(entry.Chat))
                    _entries[entry.Chat] = entry;
                else
                    _entries.Add(entry.Chat, entry);
            }
        }

        #endregion
    }

    public class SettingsEntry
    {
        #region Fields

        private bool _isReplyEnabled = false;
        private bool _isVbrosEnabled = false;
        private bool _isBabkaEnabled = false;
        
        #endregion

        #region Properties
        
        public long Id { get; private set; }
        public string Chat { get; private set; }

        public bool IsReplyEnabled
        {
            get { return _isReplyEnabled; }
            set
            {
                if (_isReplyEnabled == value)
                    return;
                _isReplyEnabled = value;
                Save();
            }
        }

        public bool IsVbrosEnabled
        {
            get { return _isVbrosEnabled; }
            set
            {
                if (_isVbrosEnabled == value)
                    return;
                _isVbrosEnabled = value;
                Save();
            }
        }

        public bool IsBabkaEnabled
        {
            get { return _isBabkaEnabled; }
            set
            {
                if (_isBabkaEnabled == value)
                    return;
                _isBabkaEnabled = value;
                Save();
            }
        }

        #endregion

        public SettingsEntry(string chat)
        {
            Chat = chat;
        }

        public SettingsEntry(string chat, long id)
            : this(chat)
        {
            Id = id;
        }

        public SettingsEntry(string chat, long id, bool reply, bool vbros, bool babka)
            : this(chat, id)
        {
            _isReplyEnabled = reply;
            _isVbrosEnabled = vbros;
            _isBabkaEnabled = babka;
        }

        public void SetId(long value)
        {
            Id = value;
        }

        public void Save()
        {
            Storage.UpdateSettings(this);
        }
    }
}
