//using System;
//using System.IO;
//using System.Net;

//namespace TabletArtco
//{
//    class FTPManager2
//    {
//        public static FTPManager2 ftpManager { get; set; }

//        static FTPManager2()
//        {
//            ftpManager = new FTPManager2("103.120.226.173", "sangsang", "sangsang", "artco");
//        }

//        private readonly string id;
//        private readonly string passwd;
//        private readonly string host;
//        private readonly string root;

//        public FTPManager2(string host, string id, string passwd, string root)
//        {
//            this.host = host;
//            this.id = id;
//            this.passwd = passwd;
//            this.root = root;
//        }

//        public bool UploadResource(Stream fileStream, string fileName)
//        {
//            try
//            {
//                string userName = GlideUtil.username;
//                string path = "ftp://" + host + "/" + root + "/sprites/" + userName + "/" + fileName;

//                FtpWebRequest request = GetFtpRequest(path);
//                request.Method = WebRequestMethods.Ftp.UploadFile;

//                int length = 4096;
//                byte[] buffer = new byte[length];

//                using (var uploadStream = request.GetRequestStream())
//                {
//                    int bytesRead = fileStream.Read(buffer, 0, length);

//                    while (bytesRead != 0)
//                    {
//                        uploadStream.Write(buffer, 0, bytesRead);
//                        bytesRead = fileStream.Read(buffer, 0, length);
//                    }
//                }

//                request.Abort();

//                return true;
//            }
//            catch (Exception)
//            {
//                return false;
//            }
//        }

//        public string[] GetFtpFolderItems(string ftpURL)
//        {
//            try
//            {
//                FtpWebRequest request = GetFtpRequest(ftpURL);
//                request.Method = WebRequestMethods.Ftp.ListDirectory;

//                FtpWebResponse response = (FtpWebResponse)request.GetResponse();

//                Stream responseStream = response.GetResponseStream();
//                StreamReader reader = new StreamReader(responseStream);

//                return reader.ReadToEnd().Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
//            }
//            catch (Exception e)
//            {
//                throw e;
//            }
//        }

//        private FtpWebRequest GetFtpRequest(string path)
//        {
//            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(path);
//            request.Credentials = new NetworkCredential(id, passwd);
//            return request;
//        }
//    }
//}