using System;
using TheZhazha.Models;

namespace TheZhazha
{
    public static class Zhazha
    {
        private static SkypeManager _skypeMgr;

        public static Random Rnd { get; private set; }

        static Zhazha()
        {
            Rnd = new Random();
        }

        public static void Start()
        {
            
            if (_skypeMgr == null)
                _skypeMgr = new SkypeManager();
            _skypeMgr.StartListening();
        }

        public static void Stop()
        {
            _skypeMgr.StopListening();
        }
    }
}