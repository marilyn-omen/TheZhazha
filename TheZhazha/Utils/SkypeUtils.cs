using SKYPE4COMLib;

namespace TheZhazha.Utils
{
    public static class SkypeUtils
    {
        public static string GetUserName(User user)
        {
            var name = user.FullName;
            if (string.IsNullOrEmpty(name))
            {
                name = user.Handle;
            }
            return name;
        }
    }
}
