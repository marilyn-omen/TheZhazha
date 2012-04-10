namespace TheZhazha.Models
{
    public static class Commands
    {
        public const string GameNews = "game";
        public const string Siske = "siske";
        public const string Generate = "vbros";
        public const string Disable = "disable";
        public const string Enable = "enable";
        public const string Admin = "admin";
        public const string Admins = "admins";
        public const string Mute = "mute";
        public const string Promote = "promote";
        public const string Demote = "demote";

        public static class Type
        {
            public const string Add = "add";
            public const string Remove = "remove";
            public const string On = "on";
            public const string Off = "off";
        }
    }
}
