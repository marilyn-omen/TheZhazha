using SKYPE4COMLib;
using TheZhazha.Events;

namespace TheZhazha.Models
{
    public interface IMessageProcessor
    {
        void AddMessage(ChatMessage message);
        event MoodChangedEventHandler MoodChanged;
//        event SendMessageEventHandler SendMessage;
    }
}
