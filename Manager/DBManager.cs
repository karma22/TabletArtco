 using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using Java.Lang;

namespace TabletArtco
{
    static class DBManager
    {
        //public static string _address { get; } = "103.120.226.173";
        //public static string _address { get; } = "www.playartco.com:8081";
        //public static string _rootDir { get; } = "artco";
        //public static string _ip { get; } = "http://103.120.226.173:8081";
        //public static string _host { get; } = "http://www.playartco.com:8081";

        //public static string _host { get; } = "http://file.playartco.com";
        //public static string _host { get; } = "http://112.219.93.149";

        public static string _host { get; } = "http://182.151.21.32";
        

        public static string url_login = _host + "/LoginCheck.php";
        public static string url_sprite = _host + "/SelectSpriteTable.php";
        public static string url_background = _host + "/SelectBackgroundTable.php";
        public static string url_block = _host + "/SelectBlockTable.php";
        public static string url_sound = _host + "/SelectSoundTable.php";
        public static string url_music = _host + "/SelectMusicTable.php";

        public static string imgPath = _host + "/artco/";

        public static bool CheckLogin(string id, string passwd)
        {
            try
            {
                string result;
                using (WebClient client = GetWebClient())
                {
                    NameValueCollection postData = new NameValueCollection() {
                                    { "id", id },  //order: {"parameter name", "parameter value"}
                                    { "passwd", passwd }
                                };
                    result = Encoding.UTF8.GetString(client.UploadValues(url_login, postData));
                }
                return result.Equals("true");
            }
            catch (WebException ex)
            {
                using (HttpWebResponse hr = (HttpWebResponse)ex.Response)
                {
                   
                }
                return false;
            }
        }

        public static void LoadSprites()
        {
            if (Sprite._sprites.Count == 0)
            {
                Sprite._sprites.Add(new List<Sprite>());
            }
            else
            {
                return;
            }

            // user Sprite (category = 0)
            string[] fileNames = FTPManager.ftpManager.GetFtpFolderItems();
            LogUtil.CustomLog("===========LoadSprites==========" + fileNames.ToString());
            using (WebClient client = GetWebClient())
            {
                for (int i = 0; i < fileNames.Length; i++)
                {
                    string name = fileNames[i].Substring(0, fileNames[i].Length - 4);
                    string remotePath = imgPath + "sprites/" + GlideUtil.username + "/" + fileNames[i];
                    Sprite sprite = new Sprite()
                    {
                        name = name,
                        category = 0,
                        remotePath = remotePath,
                    };
                    Sprite._sprites[0].Add(sprite);
                }
            }

            //I will add a security check function
            string result;
            
            using (WebClient client = GetWebClient())
            {
                byte[] bytes = client.DownloadData(url_sprite);
                result = Encoding.UTF8.GetString(bytes);
            }
            LogUtil.CustomLog("=========" + result);
            const int rowCnt = 4;
            string[] datas = result.Split('\n');
            for (int i = 0; i <= datas.Length - rowCnt; i += rowCnt)
            {
                // This is korean name
                // string name1 = datas[i];

                Sprite sprite = new Sprite()
                {
                    name = datas[i + 1],
                    category = int.Parse(datas[i + 2]),
                    remotePath = imgPath + datas[i + 3],
                    //isUser = datas[i + 4].Equals("1") ? true : false
                };

                while (sprite.category >= Sprite._sprites.Count)
                {
                    Sprite._sprites.Add(new List<Sprite>());
                }

                Sprite._sprites[sprite.category].Add(sprite);
            }
        }

        public static void LoadBackgrounds()
        {
            if (Background._backgrounds.Count == 0)
            {
                Background._backgrounds.Add(new List<Background>());
            }
            else
            {
                return;
            }

            //I will add a security check function
            string result;
            using (WebClient client = GetWebClient())
            {
                byte[] bytes = client.DownloadData(url_background);
                result = Encoding.UTF8.GetString(bytes);
            }
            
            const int rowCnt = 7;
            string[] datas = result.Split('\n');
            for (int i = 0; i <= datas.Length - rowCnt; i += rowCnt)
            {
                // This is korean name
                // string name1 = datas[i];
                
                Background background = new Background()
                {
                    name = datas[i + 1],
                    idx = int.Parse(datas[i + 2]),
                    category = int.Parse(datas[i + 3]),
                    mode = int.Parse(datas[i + 4]),
                    remoteVideoPath = imgPath + System.Uri.EscapeUriString(datas[i + 5]) + ".mp4",
                    remotePreviewImgPath = imgPath + datas[i + 5] + ".jpg",
                    level = int.Parse(datas[i + 6]),
                };

                while (background.category >= Background._backgrounds.Count)
                {
                    Background._backgrounds.Add(new List<Background>());
                }

                Background._backgrounds[background.category].Add(background);
            }
        }

        public static void LoadBlocks()
        {
            if (Block.blocks.Count > 0)
                return;

            string result;
            using (WebClient client = GetWebClient())
            {
                byte[] bytes = client.DownloadData(url_block);
                result = Encoding.UTF8.GetString(bytes);
            }

            const int rowCnt = 6;

            string[] datas = result.Split('\n');
            for (int i = 0; i <= datas.Length - rowCnt; i += rowCnt)
            {
                Block block = new Block()
                {
                    name = datas[i + 1],
                    category = int.Parse(datas[i + 2]),
                    inputState = int.Parse(datas[i + 3]),
                    idx = int.Parse(datas[i + 4]),
                    remotePath = imgPath + datas[i + 5],

                };
                if (block.category >= Block.blocks.Count)
                    Block.blocks.Add(new List<Block>());

                Block.blocks[block.category].Add(block);
            }
        }

        public static void LoadSounds()
        {
            if (Sound._sounds.Count > 0)
                return;

            string result;
            using (WebClient client = GetWebClient())
            {
                byte[] bytes = client.DownloadData(url_sound);
                result = Encoding.UTF8.GetString(bytes);
            }

            const int rowCnt = 5;
            string[] datas = result.Split('\n');
            for (int i = 0; i <= datas.Length - rowCnt; i += rowCnt)
            {
                string name = datas[i + 1];
                //int idx = int.Parse(datas[i + 2]);
                int category = int.Parse(datas[i + 3]);
                string localPath = imgPath + datas[i + 4];

                for (; category >= Sound._sounds.Count;)
                    Sound._sounds.Add(new List<Sound>());

                Sound._sounds[category].Add(new Sound(name, localPath));
            }
            
        }

        public static void LoadMusic()
        {
            if (Music._bgms.Count > 0)
                return;

            string result;
            using (WebClient client = GetWebClient())
            {
                byte[] bytes = client.DownloadData(url_music);
                result = Encoding.UTF8.GetString(bytes);
            }

            const int rowCnt = 5;
            string[] datas = result.Split('\n');
            for (int i = 0; i <= datas.Length - rowCnt; i += rowCnt)
            {
                // This is korean name
                // string name1 = datas[i];
                string name = datas[i + 1];
                int category = int.Parse(datas[i + 2]);
                //int idx = int.Parse(datas[i + 3]);
                string path = imgPath + System.Uri.EscapeUriString(datas[i + 4]) + ".wav";

                for (; category >= Music._bgms.Count;)
                    Music._bgms.Add(new List<Music>());

                Music._bgms[category].Add(new Music(name, path));
            }
        }

        public static Stream LoadPractise() 
        {
            return GetStreamFromHTTP(_host +  "/artco/backgrounds/Practice_Explain/path.xml");
        }

        public static WebClient GetHttpClient()
        {
            return new WebClient();
        }

        public static Stream GetStreamFromHTTP(string path)
        {
            string remotePath = path;
            var client = GetHttpClient();
            try
            {
                return client.OpenRead(remotePath);
            }
            catch (Exception e) 
            {
                return null;
            }
            
        }

        public static WebClient GetWebClient()
        {
            WebClient client = new WebClient();
            client.Credentials = new NetworkCredential("sangsang", "sangsang1024");
            return client;
        }
    }
}