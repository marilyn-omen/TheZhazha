using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml;
using SKYPE4COMLib;
using TheZhazha.Utils;

namespace TheZhazha.Models
{
    public class QuoteGenerator : IComparable<QuoteGenerator>
    {
        #region Fields

        private readonly List<string> _quotes;
        private Regex _pattern;
        private string _name;
        private double _actionChance;
        private int _priority;
        private int _lastChoice = -1;

        #endregion

        #region Properties

        public bool AlwaysRespond { get; private set; }

        #endregion

        #region Constructors

        public QuoteGenerator(string file)
        {
            _quotes = new List<string>();
            var doc = new XmlDocument();
            doc.Load(file);

            FillFromElement(doc.DocumentElement);
        }

        #endregion

        #region Public methods

        public bool Matches(string message)
        {
            if (_quotes.Count == 0)
                return false;

            return Zhazha.Rnd.NextDouble() <= _actionChance && _pattern.Match(message).Success;
        }

        public string GetQuote(User user)
        {
            if (_quotes.Count == 0)
                return string.Empty;

            int i;
            do
            {
                i = Zhazha.Rnd.Next(_quotes.Count);
            } while (i == _lastChoice);
            _lastChoice = i;
            var quot = _quotes[i];
            return quot.Replace("{name}", SkypeUtils.GetUserName(user));
        }

        #endregion

        #region Private methods

        private void FillFromElement(XmlElement element)
        {
            _pattern = new Regex(element.GetAttribute("pattern"), RegexOptions.IgnoreCase);
            _name = element.GetAttribute("name");
            _actionChance = Double.Parse(element.GetAttribute("chance"), NumberFormatInfo.InvariantInfo);
            var priority = element.GetAttribute("priority");
            if(!string.IsNullOrEmpty(priority))
            {
                _priority = int.Parse(priority);
            }
            if("yes".Equals(element.GetAttribute("alwaysRespond")))
            {
                AlwaysRespond = true;
            }
            foreach (XmlNode node in element.ChildNodes)
            {
                if(node.Name.Equals("quote"))
                {
                    _quotes.Add(node.InnerText);
                }
            }
        }

        #endregion

        #region Implementation of IComparable<in QuoteGenerator>

        public int CompareTo(QuoteGenerator other)
        {
            return _priority == other._priority
                       ? _actionChance.CompareTo(other._actionChance)
                       : other._priority.CompareTo(_priority);
        }

        #endregion
    }
}
