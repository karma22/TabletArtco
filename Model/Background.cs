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

        public override string ToString()
        {
            return name + "\n" + idx + "\n" + category + "\n" + mode + "\n" + remoteVideoPath + "\n" + remotePreviewImgPath + "\n" + isPng + "\n" + level + "\n" + (remoteSoundPath == null ? "" : remoteSoundPath);
        }

        public static Background ToBackground(string backgroundstr) {
            if (backgroundstr != null)
            {
                string[] datas = backgroundstr.Split('\n');
                if (datas.Length == 9)
                {
                    Background background = new Background()
                    {
                        name = datas[0],
                        idx = int.Parse(datas[1]),
                        category = int.Parse(datas[2]),
                        mode = int.Parse(datas[3]),
                        remoteVideoPath = datas[4],
                        remotePreviewImgPath = datas[5],
                        isPng = (datas[6].Equals("true") || datas[6].Equals("TRUE")) ? true : false,
                        level = int.Parse(datas[7]),
                        remoteSoundPath = datas[8]
                    };
                    return background;
                }
            }
            return null;
        }
    }
}