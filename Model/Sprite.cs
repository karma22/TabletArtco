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
        public static List<List<Sprite>> _sprites = new List<List<Sprite>>();

        public string name;
        public int category;
        public string remotePath;
        public bool isUser;
        public Bitmap bitmap;

        public override string ToString()
        {
            return name + "\n" + category + "\n" + remotePath + "\n" + isUser.ToString().ToLower();
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

        //public static Sprite ToSprite(string path)
        //{

        //    return null;
        //}
    }    
}