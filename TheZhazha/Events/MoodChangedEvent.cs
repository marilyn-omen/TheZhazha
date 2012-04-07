using System;

namespace TheZhazha.Events
{
    public delegate void MoodChangedEventHandler(object sender, MoodChangedEventArgs e);

    public class MoodChangedEventArgs : EventArgs
    {
        public string Mood { get; private set; }

        public MoodChangedEventArgs(string mood)
        {
            Mood = mood;
        }
    }
}
