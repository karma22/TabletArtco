using Android.Support.Design.Transformation;
using System;
using System.IO;
using System.Net;

namespace TabletArtco
{
    class FTPManager
    {
        public static FTPManager ftpManager { get; set; }

        static FTPManager()
        {
            //ftpManager = new FTPManager("103.120.226.173", "sangsang", "sangsang", "artco");
            //ftpManager = new FTPManager("112.219.93.149", "sangsang", "sangsang", "artco");
            ftpManager = new FTPManager("182.151.21.32", "sangsang", "sangsang1024", "artco");
        }

        private readonly string id;
        private readonly string passwd;
        private readonly string host;
        private readonly string root;

        public FTPManager(string host, string id, string passwd, string root)
        {
            this.host = host;
            this.id = id;
            this.passwd = passwd;
            this.root = root;
        }

        public bool UploadResource(Stream fileStream, string fileName)
        {
            try
            {
                string userName = GlideUtil.username;
                string path = "ftp://" + host + "/" + root + "/sprites/" + userName + "/" + fileName;

                FtpWebRequest request = GetFtpRequest(path);
                request.Method = WebRequestMethods.Ftp.UploadFile;

                int length = 4096;
                byte[] buffer = new byte[length];

                using (var uploadStream = request.GetRequestStream())
                {
                    int bytesRead = fileStream.Read(buffer, 0, length);

                    while (bytesRead != 0)
                    {
                        uploadStream.Write(buffer, 0, bytesRead);
                        bytesRead = fileStream.Read(buffer, 0, length);
                    }
                }

                request.Abort();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        public bool DeleteFileFromFTP(string path)
        {
            string remotePath = "ftp" + path.Substring(4);
            try
            {
                FtpWebRequest request = GetFtpRequest(remotePath);
                request.Method = WebRequestMethods.Ftp.DeleteFile;
                request.GetResponse();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public string[] GetFtpFolderItems()
        {
            string userName = GlideUtil.username;
            string ftpURL = "ftp://" + host + "/" + root + "/sprites/" + userName + "/";
            try
            {
                FtpWebRequest request = GetFtpRequest(ftpURL);
                request.Method = WebRequestMethods.Ftp.ListDirectory;

                FtpWebResponse response = (FtpWebResponse)request.GetResponse();

                Stream responseStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(responseStream);

                return reader.ReadToEnd().Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private FtpWebRequest GetFtpRequest(string path)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(path);
            request.Credentials = new NetworkCredential(id, passwd);
            return request;
        }
    }
}