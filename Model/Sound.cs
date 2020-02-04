using System;
using System.Collections.Generic;

namespace TabletArtco
{
    public class Sound
    {

        public static List<List<Sound>> _sounds = new List<List<Sound>>();
        
        public string name;
        public string localPath;

        public Sound(string name, string localPath)
        {
            this.name = name;
            this.localPath = localPath;
        }
    }
}
