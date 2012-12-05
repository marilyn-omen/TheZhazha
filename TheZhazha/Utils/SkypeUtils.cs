using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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

        public static string GetFsSafeName(string chatName)
        {
            var invalidSymbols = System.IO.Path.GetInvalidFileNameChars();
            return invalidSymbols.Aggregate(chatName, (current, symbol) => current.Replace(symbol, '_'));
        }

        public static string RemoveSmilies(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            for (var i = 0; i < SmilesTable.Count; i++)
            {
                text = text.Replace(SmilesTable[i], string.Format("gsom{0}mosg", i));
            }
            return text;
        }

        public static string ReturnSmilies(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            for (var i = 0; i < SmilesTable.Count; i++)
            {
                text = text.Replace(string.Format("gsom{0}mosg", i), SmilesTable[i]);
            }
            return text;
        }

        public static List<string> SmilesTable = new List<string>
        {
            ":)",
            ":(",
            ":D",
            "8)",
            ":o",
            ";(",
            "(:|",
            ":|",
            ":*",
            ":P",
            ":$",
            ":^)",
            "|-)",
            "|(",
            "(inlove)",
            ">:)",
            "(talk)",
            "(yawn)",
            ":&",
            "(doh)",
            "xD",
            "(wasntme)",
            "(party)",
            ":S",
            "(mm)",
            "8|",
            ":x",
            "(hi)",
            "(call)",
            "(devil)",
            "(angel)",
            "(envy)",
            "(wait)",
            "(makeup)",
            "(giggle)",
            "(clap)",
            ":?",
            "(rofl)",
            "(whew)",
            "(happy)",
            "(smirk)",
            "(nod)",
            "(shake)",
            "(punch)",
            "(emo)",
            "(finger)",
            "(bandit)",
            "(drunk)",
            "(snoke)",
            "(rock)",
            "(headbang)",
            "(fubar)",
            "(swear)",
            "(tmi)",
            "(bug)",
            "(heidy)",
            "(mooning)",
            "(bear)",
            "(y)",
            "(n)",
            "(handshake)",
            "(h)",
            "(u)",
            "(e)",
            "(f)",
            "(rain)",
            "(sun)",
            "(time)",
            "(music)",
            "(movie)",
            "(ph)",
            "(coffee)",
            "(pizza)",
            "(cash)",
            "(muscle)",
            "(cake)",
            "(beer)",
            "(d)",
            "(*)",
            "(skype)",
            "(dance)",
            "(ninja)",
            "(bow)",
            "(toivo)",
            "(poolparty)",
            "(hug)",
            "(smoking)",
            "(worry)",
            "(facepalm)",
            "(star)",
            "(drink)",
            "(tumbleweed)",
            ":)",
            ":=)",
            ":-)",
            ":(",
            ":=(",
            ":-(",
            ":D",
            ":=D",
            ":-D",
            ":d",
            ":=d",
            ":-d",
            "8)",
            "8=)",
            "8-)",
            "B)",
            "B=)",
            "B-)",
            "(cool)",
            ":o",
            ":=o",
            ":-o",
            ":O",
            ":=O",
            ":-O",
            ";(",
            ";-(",
            ";=(",
            "(sweat)",
            "(:|",
            ":|",
            ":=|",
            ":-|",
            "(kiss)",
            ":*",
            ":=*",
            ":-*",
            ":P",
            ":=P",
            ":-P",
            ":p",
            ":=p",
            ":-p",
            "(blush)",
            ":$",
            ":-$",
            ":=$",
            ":\">",
            ":^)",
            "|-)",
            "I-)",
            "I=)",
            "(snooze)",
            "(dull)",
            "|(",
            "|-(",
            "|=(",
            "(inlove)",
            "(love)",
            "]:)",
            ">:)",
            "(grin)",
            "(talk)",
            "(yawn)",
            "|-()",
            "(puke)",
            ":&",
            ":-&",
            ":=&",
            "(doh)",
            "(angry)",
            ":@",
            ":-@",
            ":=@",
            "x(",
            "x-(",
            "x=(",
            "X(",
            "X-(",
            "X=(",
            "(wasntme)",
            "(party)",
            "(worried)",
            "(worry)",
            ":S",
            ":-S",
            ":=S",
            ":s",
            ":-s",
            ":=s",
            "(mm)",
            "8-|",
            "B-|",
            "8|",
            "B|",
            "8=|",
            "B=|",
            "(nerd)",
            ":x",
            ":-x",
            ":X",
            ":-X",
            ":#",
            ":-#",
            ":=x",
            ":=X",
            ":=#",
            "(hi)",
            "(call)",
            "(devil)",
            "(angel)",
            "(envy)",
            "(wait)",
            "(bear)",
            "(hug)",
            "(makeup)",
            "(kate)",
            "(giggle)",
            "(chuckle)",
            "(clap)",
            "(think)",
            ":?",
            ":-?",
            ":=?",
            "(bow)",
            "(rofl)",
            "(whew)",
            "(happy)",
            "(smirk)",
            "(nod)",
            "(shake)",
            "(punch)",
            "(emo)",
            "(y)",
            "(Y)",
            "(ok)",
            "(n)",
            "(N)",
            "(handshake)",
            "(skype)",
            "(ss)",
            "(h)",
            "(H)",
            "(l)",
            "(L)",
            "(u)",
            "(U)",
            "(mail)",
            "(e)",
            "(m)",
            "(flower)",
            "(f)",
            "(F)",
            "(rain)",
            "(london)",
            "(sun)",
            "(o)",
            "(O)",
            "(time)",
            "(music)",
            "(~)",
            "(film)",
            "(movie)",
            "(phone)",
            "(mp)",
            "(ph)",
            "(coffee)",
            "(pizza)",
            "(pi)",
            "(cash)",
            "(mo)",
            "($)",
            "(muscle)",
            "(flex)",
            "(^)",
            "(cake)",
            "(beer)",
            "(drink)",
            "(d)",
            "(D)",
            "(dance)",
            "\\o/",
            "\\:D/",
            "\\:d/",
            "(ninja)",
            "(star)",
            "(*)",
            "(mooning)",
            "(finger)",
            "(bandit)",
            "(drunk)",
            "(smoking)",
            "(smoke)",
            "(ci)",
            "(toivo)",
            "(rock)",
            "(headbang)",
            "(banghead)",
            "(bug)",
            "(fubar)",
            "(poolparty)",
            "(swear)",
            "(tmi)",
            "(heidy)",
            "(MySpace)",
            "(malthe)",
            "(tauri)",
            "(priidu)"
        };
    }
}
