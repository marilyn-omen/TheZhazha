using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TheZhazha.Models
{
    public class StatsEntry
    {
        #region Fields

        private readonly char[] _wordDelimiters = new[] { ' ', '\t', '\n', ',', '.', '!', '?', '(', ')', '"' };

        #endregion

        #region Properties

        public long Id { get; private set; }
        public string Chat { get; set; }
        public string User { get; set; }
        public DateTime? Started { get; set; }
        public long Messages { get; set; }
        public long Words { get; set; }
        public long Symbols { get; set; }
        public long Commands { get; set; }

        #endregion

        #region Constructors

        public StatsEntry(string chat, string user)
        {
            Chat = chat;
            User = user;
        }

        public StatsEntry(string chat, string user, long id)
            : this (chat, user)
        {
            Id = id;
        }

        #endregion

        #region Public methods

        public void SetId(long value)
        {
            Id = value;
        }

        public void Apply(string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                if (message.StartsWith("@"))
                    Commands++;
                else
                {
                    Messages++;
                    Symbols += message.Length;
                    Words += message.Split(_wordDelimiters, StringSplitOptions.RemoveEmptyEntries).Length;
                }
            }
        }

        #endregion
    }
}
