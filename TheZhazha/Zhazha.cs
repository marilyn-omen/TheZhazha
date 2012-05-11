using System;
using TheZhazha.Models;

namespace TheZhazha
{
    public static class Zhazha
    {
        public static string MasterHandle { get; set; }

        public static SkypeManager Manager
        {
            get { return _skypeMgr; }
        }

        private static SkypeManager _skypeMgr;

        public static Random Rnd { get; private set; }

        static Zhazha()
        {
            if (_skypeMgr == null)
                _skypeMgr = new SkypeManager();
            Rnd = new Random();
        }

        public static void Start()
        {
            _skypeMgr.StartListening();
        }

        public static void Stop()
        {
            _skypeMgr.StopListening();
        }
    }
}