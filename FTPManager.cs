using System.IO;
using System.Net;

namespace TabletArtco
{
    static class FTPManager
    {
        public static string _ftpID { get; } = "sangsang";
        public static string _ftpPWD { get; } = "sangsang";

        /*
         * You can use it in the following way.
         * for(int category=0; category<Sprite._sprites.Count; category++)
         * {
         *      for(int idx=0; idx<Sprite._sprites[category].Count; idx++)
         *      {
         *          Stream stream = FTPManager.GetStreamFromFTP(Sprite._sprites[category][idx].remotePath);
         *      }
         * }         
         */
        public static Stream GetStreamFromFTP(string path)
        {
            FtpWebRequest request = GetFtpRequest(path);
            request.Method = WebRequestMethods.Ftp.DownloadFile;

            FtpWebResponse res = (FtpWebResponse)request.GetResponse();
            return res.GetResponseStream();
        }

        private static FtpWebRequest GetFtpRequest(string path)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(path);
            request.Credentials = new NetworkCredential(_ftpID, _ftpPWD);
            return request;
        }
    }
}