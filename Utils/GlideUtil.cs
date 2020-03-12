using System;
using System.Text;
using Com.Bumptech.Glide.Load.Model;
using Java.IO;
using Java.Math;

namespace TabletArtco
{
    public class GlideUtil
    {
        public static string username { get; set; }
        public static string pwd { get; set; }

        public static string GetAuthorization() {
            return "Basic " + EncodeBase64("utf-8", username + ":" + pwd);
        } 

        public static GlideUrl GetGlideUrl(string path) {
            LazyHeaders.Builder builder = new LazyHeaders.Builder();
            builder.AddHeader("Authorization", GetAuthorization());
            builder.Build();
            return new GlideUrl(path, builder.Build());
        }

        ///编码
        public static string EncodeBase64(string code_type, string code)
        {
            string encode = "";
            byte[] bytes = Encoding.GetEncoding(code_type).GetBytes(code);
            try
            {
                encode = Convert.ToBase64String(bytes);
            }
            catch
            {
                encode = code;
            }
            return encode;
        }

        ///解码
        public static string DecodeBase64(string code_type, string code)
        {
            string decode = "";
            byte[] bytes = Convert.FromBase64String(code);
            try
            {
                decode = Encoding.GetEncoding(code_type).GetString(bytes);
            }
            catch
            {
                decode = code;
            }
            return decode;
        }

        public static string GetCacheSize(File file)
        {
            try
            {
                return GetFormatSize(GetFolderSize(file));
            }
            catch (Exception e)
            {
                //e.PrintStackTrace();
                return "获取失败";
            }
        }

        // 获取指定文件夹内所有文件大小的和
        private static long GetFolderSize(File file) 
        {
            long size = 0;
            try {
                File[] fileList = file.ListFiles();
                for (int i = 0; i < fileList.Length; i++)
                {
                    File f = fileList[i];
                    if (f.IsDirectory)
                    {
                        size = size + GetFolderSize(f);
                    }
                    else
                    {
                        size = size + f.Length();
                    }
                }
            } catch (Exception e) {
                //e.printStackTrace();
            }
            return size;
         }

        // 格式化单位
        private static string GetFormatSize(double size)
        {
            double kiloByte = size / 1024;
            if (kiloByte < 1)
            {
                return size + "B";
            }
            double megaByte = kiloByte / 1024;
            if (megaByte < 1)
            {
                BigDecimal result1 = new BigDecimal(kiloByte);
                return result1.SetScale(2, BigDecimal.RoundHalfUp).ToPlainString() + "KB";
            }
            double gigaByte = megaByte / 1024;
            if (gigaByte < 1)
            {
                BigDecimal result2 = new BigDecimal(megaByte);
                return result2.SetScale(2, BigDecimal.RoundHalfUp).ToPlainString() + "MB";
            }
            double teraBytes = gigaByte / 1024;
            if (teraBytes < 1)
            {
                BigDecimal result3 = new BigDecimal(gigaByte);
                return result3.SetScale(2, BigDecimal.RoundHalfUp).ToPlainString() + "GB";
            }
            BigDecimal result4 = new BigDecimal(teraBytes);
            return result4.SetScale(2, BigDecimal.RoundHalfUp).ToPlainString() + "TB";
        }

    }
}
