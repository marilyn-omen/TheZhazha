using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TheZhazha.Events
{
    public delegate void ChangedEventHandler<T>(object sender, ChangedEventArgs<T> e) where T : class;

    public class ChangedEventArgs<T> : EventArgs where T : class
    {
        public T Argument { get; private set; }

        public ChangedEventArgs(T arg)
        {
            Argument = arg;
        }
    }
}
