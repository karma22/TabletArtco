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

            using (WebClient client = new WebClient())
            {
                NameValueCollection postData = new NameValueCollection()
                {
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

            // I will add a security check function
            string result;
            string url = "http://103.120.226.173/SelectSpriteTable.php";
            using (WebClient client = new WebClient())
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
                    remotePath = "ftp://" + _address + "/" + _rootDir + "/" + datas[i + 3],
                    isUser = datas[i + 4].Equals("1") ? true : false
                };

                while (sprite.category >= Sprite._sprites.Count)
                {
                    Sprite._sprites.Add(new List<Sprite>());
                }

                Sprite._sprites[sprite.category].Add(sprite);
            }
        }

        public static void LoadBlocks()
        {

        }
    }
}