using System;

namespace TheZhazha.Events
{
    public delegate void SendMessageEventHandler(object sender, MoodChangedEventArgs e);

    public class SendMessageEvent : EventArgs
    {
        public string Mesage { get; private set; }

        public SendMessageEvent(string message)
        {
            Mesage = message;
        }
    }
}
