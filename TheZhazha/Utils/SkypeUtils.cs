using System.Linq;
using SKYPE4COMLib;
using TheZhazha.Data;

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

        public static bool IsUserAdmin(string userHandle, string chat)
        {
            if (string.IsNullOrEmpty(userHandle) || string.IsNullOrEmpty(chat))
                return false;

            return userHandle.Equals(Zhazha.MasterHandle) || Storage.IsUserAdmin(userHandle, chat);
        }

        public static IUser GetChatUserByHandler(IChat chat, string handler)
        {
            return chat.Members.Cast<IUser>().FirstOrDefault(user => user.Handle == handler);
        }

        public static IChatMember GetChatMemberByHandler(IChat chat, string handler)
        {
            return chat.MemberObjects.Cast<IChatMember>().FirstOrDefault(user => user.Handle == handler);
        }
    }
}
