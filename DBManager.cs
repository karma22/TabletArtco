using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Text;

namespace TabletArtco
{
    static class DBManager
    {
        public static string _address { get; } = "103.120.226.173";
        public static string _rootDir { get; } = "artco";


        public static bool CheckLogin(string id, string passwd)
        {
            string result;
            string url = "http://103.120.226.173/LoginCheck.php";

            using (WebClient client = GetWebClient())
            {
                NameValueCollection postData = new NameValueCollection(){
                                    { "id", id },  //order: {"parameter name", "parameter value"}
                                    { "passwd", passwd }
                                };

                result = Encoding.UTF8.GetString(client.UploadValues(url, postData));
            }

            return result.Equals("true");
        }

        public static void LoadSprites()
        {
            if (Sprite._sprites.Count == 0)
            {
                Sprite._sprites.Add(new List<Sprite>());
            }

            //I will add a security check function
            string result;
            string url = "http://103.120.226.173/SelectSpriteTable.php";
            using (WebClient client = GetWebClient())
            {
                byte[] bytes = client.DownloadData(url);
                result = Encoding.UTF8.GetString(bytes);
            }

            const int rowCnt = 5;
            string[] datas = result.Split('\n');
            for (int i = 0; i <= datas.Length - rowCnt; i += rowCnt)
            {
                // This is korean name
                // string name1 = datas[i];

                Sprite sprite = new Sprite()
                {
                    name = datas[i + 1],
                    category = int.Parse(datas[i + 2]),
                    remotePath = "http://" + _address + "/" + _rootDir + "/" + datas[i + 3],
                    isUser = datas[i + 4].Equals("1") ? true : false
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

            //I will add a security check function
            string result;
            string url = "http://103.120.226.173/SelectBackgroundTable.php";
            using (WebClient client = GetWebClient())
            {
                byte[] bytes = client.DownloadData(url);
                result = Encoding.UTF8.GetString(bytes);
            }

            const int rowCnt = 9;
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
                    remoteVideoPath = "http://" + _address + "/" + _rootDir + "/" + datas[i + 5] + ".mp4",
                    remotePreviewImgPath = "http://" + _address + "/" + _rootDir + "/" + datas[i + 5] + ".jpg",
                    isPng = datas[i + 6].Equals("1") ? true : false,
                    level = int.Parse(datas[i + 7]),
                    remoteSoundPath = "http://" + _address + "/" + _rootDir + "/" + datas[i + 8],
                };

                background.remoteSoundPath = (datas[i + 8] != string.Empty) ? "http://" + _address + "/" + _rootDir + "/" + datas[i + 8] : null;
                while (background.category >= Background._backgrounds.Count)
                {
                    Background._backgrounds.Add(new List<Background>());
                }

                Background._backgrounds[background.category].Add(background);
            }
        }
        public static void LoadBlocks()
        {
            string result;
            string url = "http://103.120.226.173/SelectBlockTable.php";
            using (WebClient client = GetWebClient())
            {
                byte[] bytes = client.DownloadData(url);
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
                    remotePath = "http://" + _address + "/" + _rootDir + "/" + datas[i + 5],

                };
                if (block.category >= Block.blocks.Count)
                    Block.blocks.Add(new List<Block>());

                Block.blocks[block.category].Add(block);
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