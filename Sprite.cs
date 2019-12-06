using System.Collections.Generic;
using System.Drawing;

namespace TabletArtco
{
    class Sprite
    {
        public static List<List<Sprite>> sprites {get; set;} = new List<List<Sprite>>();
        public string name { get; set; }
        public int category { get; set; }
        public string remotePath { get; set; }
        public bool isUser { get; set; }
    }

    class ActivatedSprite : Sprite
    {
        public List<List<Block>> codes { get; set; } = new List<List<Block>>();

        // SpriteMiniView

        public List<Bitmap> spriteBits { get; set; } = new List<Bitmap>();
        public List<Bitmap> bigSpriteBits { get; set; } = new List<Bitmap>();
        public List<Bitmap> normalSpriteBits { get; set; } = new List<Bitmap>();
        public object lockObj { get; set; } = new object();

        public string speakText { get; set; } = null;
        public int curSpriteNum { get; set; }
        public int curEditorPageNum { get; set; }
        public int maxEditorPageNum { get; set; }
        public int programCnt { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public bool isVisible { get; set; } = true;
        public int arrow { get; set; } = -1;
        public float angle { get; set; }
    }
}