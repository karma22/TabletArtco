using System.Collections.Generic;

namespace TabletArtco
{
    class Background
    {
        public static List<List<Background>> backgrounds { get; set; } = new List<List<Background>>();
        public static Background currentBack { get; set; } = null;

        public string _name { get; set; }
        public int _category { get; set; }
        public string _path { get; set; }
        public string _coverPath { get; set; }
        public string _soundPath { get; set; }
        public int _level { get; set; }
        public int _mode { get; set; }
        public int _idx { get; set; }
        public bool _isPng { get; set; }
    }
}