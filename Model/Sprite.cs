using System.Collections.Generic;
using Android.Graphics;
using Android.Util;
using Android.Content;
using System.Threading;
using Com.Bumptech.Glide;


namespace TabletArtco
{
    public class Sprite
    {
        // List by category
        public static List<List<Sprite>> _sprites { get; set; } = new List<List<Sprite>>();

        public string name { get; set; }
        public int category { get; set; }
        public string remotePath { get; set; }
        public bool isUser { get; set; }
        public Bitmap bitmap { get; set; }

        public override string ToString()
        {
            return name + "\n" + category + "\n" + remotePath + "\n" + isUser;
        }
        
        public static Sprite ToSprite(string spriteStr)
        {
            if (spriteStr != null)
            {
                string[] datas = spriteStr.Split('\n');
                if (datas.Length == 4)
                {
                    Sprite sprite = new Sprite()
                    {
                        name = datas[0],
                        category = int.Parse(datas[1]),
                        remotePath = datas[2],
                        isUser = (datas[3].Equals("true") || datas[3].Equals("TRUE")) ? true : false
                    };
                    return sprite;
                }
            }
            return null;
        }
    }

    


}