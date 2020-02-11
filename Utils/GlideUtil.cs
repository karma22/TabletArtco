using System;
using System.Text;
using Com.Bumptech.Glide.Load.Model;
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

    }
}
