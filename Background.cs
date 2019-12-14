using System.Collections.Generic;

namespace TabletArtco
{
    class Background
    {
        public static List<List<Background>> _backgrounds { get; set; } = new List<List<Background>>();
        public static Background currentBack { get; set; } = null;

        public string name { get; set; }
        public int category { get; set; }
        public string remoteVideoPath { get; set; }
        public string remotePreviewImgPath { get; set; }
        public string remoteSoundPath { get; set; }
        public int level { get; set; }
        public int mode { get; set; }
        public int idx { get; set; }
        public bool isPng { get; set; }
    }
}