using System.Collections.Specialized;
using System.Net;
using System.Text;

namespace TabletArtco
{
    static class DBManager
    {
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

        }

        public static void LoadBlocks()
        {

        }
    }
}