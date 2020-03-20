using System;
using System.Collections.Generic;

namespace TabletArtco
{
    public class Music
    {
        public static List<List<Music>> _bgms = new List<List<Music>>();
        
        public string name;
        public string path;

        public Music(string name, string path)
        {
            this.name = name;
            this.path = path;
        }
    }
}
