using System.Text;
using System.Reflection;
namespace TheZhazha.Models
{
    public static class Commands
    {
        //public static string GameNews = "game";
        public const string Help = "help";
        public const string Siske = "siske";
        public const string Generate = "vbros";
        public const string Disable = "disable";
        public const string Enable = "enable";
        public const string Admin = "admin";
        public const string Admins = "admins";
        public const string Mute = "mute";
        public const string Promote = "promote";
        public const string Demote = "demote";
        public const string Diablo3 = "d3";
        public const string Statistic = "stat";

        public static class Type
        {
            public const string Add = "add";
            public const string Remove = "remove";
            public const string On = "on";
            public const string Off = "off";
        }

        public static new string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Доступные команды:");
            var fieldInfos = typeof(Commands).GetFields();
            foreach (var fieldInfo in fieldInfos)
            {
                var propValue = fieldInfo.GetValue(null);
                if (propValue != null)
                {
                    sb.AppendLine(string.Format("  @{0}", propValue.ToString()));
                }
            }
            return sb.ToString();
        }
    }
}
