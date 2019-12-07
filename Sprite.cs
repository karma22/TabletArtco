using System.Collections.Generic;
using Android.Graphics;

namespace TabletArtco
{
    class Sprite
    {
        // List by category
        public static List<List<Sprite>> _sprites { get; set; } = new List<List<Sprite>>();

        public string name { get; set; }
        public int category { get; set; }
        public string remotePath { get; set; }
        public bool isUser { get; set; }
    }
}