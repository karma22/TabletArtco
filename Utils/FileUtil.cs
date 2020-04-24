
using Android.Content;
using Android.Media;
using Android.Net;
using Android.OS;
using Android.Provider;
using Android.Text;
using Java.IO;
using Java.Lang;

namespace TabletArtco
{
    class FileUtil
    {
        /**
        * 删除SD卡中的文件或目录
        *
        * @param path
        * @return
        */
        public static bool deleteSDFile(string path)
        {
            return deleteSDFile(path, false);
        }

        /**
         * 删除SD卡中的文件或目录
         *
         * @param path
         * @param deleteParent true为删除父目录
         * @return
         */
        public static bool deleteSDFile(string path, bool deleteParent)
        {
            if (TextUtils.IsEmpty(path))
            {
                return false;
            }

            File file = new File(path);
            if (!file.Exists())
            {
                //不存在
                return true;
            }
            return deleteFile(file, deleteParent);
        }

        /**
         * @param file
         * @param deleteParent true为删除父目录
         * @return
         */
        public static bool deleteFile(File file, bool deleteParent)
        {
            bool flag = false;
            if (file == null)
            {
                return flag;
            }
            if (file.IsDirectory)
            {
                //是文件夹
                File[] files = file.ListFiles();
                if (files.Length > 0)
                {
                    for (int i = 0; i < files.Length; i++)
                    {
                        flag = deleteFile(files[i], true);
                        if (!flag)
                        {
                            return flag;
                        }
                    }
                }
                if (deleteParent)
                {
                    flag = file.Delete();
                }
            }
            else
            {
                flag = file.Delete();
            }
            file = null;
            return flag;
        }

        /**
         * 添加到媒体数据库
         * @param context 上下文
         */
        public static Uri fileScanVideo(Context context, string videoPath, int videoWidth, int videoHeight, int videoTime)
        {
            File file = new File(videoPath);
            if (file.Exists())
            {
                Uri uri = null;
                long size = file.Length();
                string fileName = file.Name;
                long dateTaken = JavaSystem.CurrentTimeMillis();
                ContentValues values = new ContentValues(11);
                values.Put(MediaStore.Video.Media.InterfaceConsts.Data, videoPath); // 路径;
                values.Put(MediaStore.Video.Media.InterfaceConsts.Title, fileName); // 标题;
                values.Put(MediaStore.Video.Media.InterfaceConsts.Duration, videoTime * 1000); // 时长
                values.Put(MediaStore.Video.Media.InterfaceConsts.Width, videoWidth); // 视频宽
                values.Put(MediaStore.Video.Media.InterfaceConsts.Height, videoHeight); // 视频高
                values.Put(MediaStore.Video.Media.InterfaceConsts.Size, size); // 视频大小;
                values.Put(MediaStore.Video.Media.InterfaceConsts.DateTaken, dateTaken); // 插入时间;
                values.Put(MediaStore.Video.Media.InterfaceConsts.DisplayName, fileName);// 文件名;
                values.Put(MediaStore.Video.Media.InterfaceConsts.DateModified, dateTaken / 1000);// 修改时间;
                values.Put(MediaStore.Video.Media.InterfaceConsts.DateAdded, dateTaken / 1000); // 添加时间;
                values.Put(MediaStore.Video.Media.InterfaceConsts.MimeType, "video/mp4");

                ContentResolver resolver = context.ContentResolver;
                if (resolver != null)
                {
                    try
                    {
                        uri = resolver.Insert(MediaStore.Video.Media.InternalContentUri, values);
                    }
                    catch (Exception e)
                    {
                        e.PrintStackTrace();
                        uri = null;
                    }
                }

                if (uri == null)
                {
                    MediaScannerConnection.ScanFile(context, new string[] { videoPath }, new string[] { "video/*" }, null);
                }

                return uri;
            }

            return null;
        }

        /**
        * SD卡存在并可以使用
        */
        public static bool IsSDExists()
        {
            return Environment.ExternalStorageState.Equals(Environment.MediaMounted);
        }

        /**
        * 获取SD卡的剩余容量，单位是Byte
        *
        * @return
        */
        public static long getSDFreeMemory()
        {
            try
            {
                if (IsSDExists())
                {
                    File pathFile = Environment.ExternalStorageDirectory;
                    // Retrieve overall information about the space on a filesystem.
                    // This is a Wrapper for Unix statfs().
                    StatFs statfs = new StatFs(pathFile.Path);
                    // 获取SDCard上每一个block的SIZE
                    long nBlockSize = statfs.BlockSize;
                    // 获取可供程序使用的Block的数量
                    // long nAvailBlock = statfs.getAvailableBlocksLong();
                    long nAvailBlock = statfs.AvailableBlocks;
                    // 计算SDCard剩余大小Byte
                    long nSDFreeSize = nAvailBlock * nBlockSize;
                    return nSDFreeSize;
                }
            }
            catch (Exception ex)
            {
                ex.PrintStackTrace();
            }
            return 0;
        }
    }
}