using System.Text.RegularExpressions;
using SKYPE4COMLib;
using Shock.Logger;
using TheZhazha.Data;

namespace TheZhazha.Models
{
    public class Babka
    {
        private const string _pattern = "(бабка твоя)|(бабки твоей)|(бабки своей)|(бабке твоей)|(бабке своей)|(бабку твою)|(бабку свою)|(бабкой твоей)|(бабкой своей)|(твоя бабка)|(твоей бабки)|(своей бабки)|(твоей бабке)|(своей бабке)|(твою бабку)|(свою бабку)|(твоей бабкой)|(своей бабкой)|(в штанах)|(в шт@нах)|(в шtанах)|(6а6ка)|(babke)";
        
        private static readonly Regex _rex = new Regex(_pattern, RegexOptions.IgnoreCase);

        public static bool Match(string message)
        {
            return _rex.Match(message).Success;
        }

        public static bool ProcessMessage(ChatMessage message, out string resMessage)
        {
            var warnings = Storage.GetUserWarningsCount(message.Sender.Handle, message.ChatName);
            LoggerFacade.Log(string.Format("Got {0} warnings for {1}", warnings, message.Sender.Handle));

            if (warnings < 5)
            {
                warnings++;
                LoggerFacade.Log(string.Format("Add {0} warning for {1}", warnings, message.Sender.Handle));
                Storage.AddWarning(message.Sender.Handle, "babka", message.ChatName);
            }

            switch (warnings)
            {
                case 1:
                    resMessage = "Бабка у тебя в штанах, понял?! Первое предупреждение, {name}!";
                    return false;
                case 2:
                    resMessage = "Второе предупреждение! Бабка следит за тобой, {name}";
                    return false;
                case 3:
                    resMessage = "Бабка в штанах, штаны в яйце, яйцо в утке, утка в зайце, заец в Канаде. Третье  предупреждение, {name}.";
                    return false;
                case 4:
                    resMessage = "Китайское предупреждение, или я за себя не отвечаю! И за бабку твою тоже, {name}!";
                    return false;
                default:
                    resMessage = "Кажется, кто-то допизделся.";
                    return true;
            }
        }
    }
}
