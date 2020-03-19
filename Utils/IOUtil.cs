using System;
using Java.IO;
using Android.Net;
using Android.Provider;
using Android.Content;
using Java.Lang;
using Android.Support.V4.Content;

namespace TabletArtco
{
    class IOUtil
    {
        public static bool CopyFile(string from, string to)
        {
            bool result;
            if (System.IO.File.Exists(from))
            {
                FileInputStream fis = new FileInputStream(from);
                to += "/" + System.IO.Path.GetFileName(from);
                FileOutputStream newfos = new FileOutputStream(to);
                try
                {
                    int readcount = 0;
                    byte[] buffer = new byte[1024];
                    while ((readcount = fis.Read(buffer, 0, 1024)) != -1)
                    {
                        newfos.Write(buffer, 0, readcount);
                    }
                }
                catch (Java.Lang.Exception e)
                {
                    result = false;
                }
                finally
                {
                    newfos.Close();
                    fis.Close();
                }

                result = true;
            }
            else
            {
                result = false;
            }

            return result;
        }

        public static string GetRealPathFromURI(Context context, Android.Net.Uri uri)
        {
            // DocumentProvider
            if (DocumentsContract.IsDocumentUri(context, uri))
            {

                // ExternalStorageProvider
                if (IsExternalStorageDocument(uri))
                {
                    string docId = DocumentsContract.GetDocumentId(uri);
                    string[] split = docId.Split(":");
                    string type = split[0];
                    if ("primary".Equals(type,StringComparison.InvariantCultureIgnoreCase))
                    {
                        return Android.OS.Environment.ExternalStorageDirectory.Path + "/"
                                + split[1];
                    }
                    else
                    {
                        string SDcardpath = GetRemovableSDCardPath(context).Split("/Android")[0];
                        return SDcardpath + "/" + split[1];
                    }
                }

                // DownloadsProvider
                else if (IsDownloadsDocument(uri))
                {
                    string id = DocumentsContract.GetDocumentId(uri);
                    Android.Net.Uri contentUri = ContentUris.WithAppendedId(
                            Android.Net.Uri.Parse("content://downloads/public_downloads"),
                            long.Parse(id));

                    return GetDataColumn(context, contentUri, null, null);
                }

                // MediaProvider
                else if (IsMediaDocument(uri))
                {
                    string docId = DocumentsContract.GetDocumentId(uri);
                    string[] split = docId.Split(":");
                    string type = split[0];

                    Android.Net.Uri contentUri = null;
                    if ("image".Equals(type))
                    {
                        contentUri = MediaStore.Images.Media.ExternalContentUri;
                    }
                    else if ("video".Equals(type))
                    {
                        contentUri = MediaStore.Video.Media.ExternalContentUri;
                    }
                    else if ("audio".Equals(type))
                    {
                        contentUri = MediaStore.Audio.Media.ExternalContentUri;
                    }

                    string selection = "_id=?";
                    string[] selectionArgs = new string[] { split[1] };

                    return GetDataColumn(context, contentUri, selection, selectionArgs);
                }
            }
            else if ("content".Equals(uri.Scheme, StringComparison.InvariantCultureIgnoreCase))
            {
                // Return the remote address
                if (IsGooglePhotosUri(uri))
                    return uri.LastPathSegment;
                return GetDataColumn(context, uri, null, null);
            }
            else if ("file".Equals(uri.Scheme, StringComparison.InvariantCultureIgnoreCase))
            {
                return uri.Path;
            }
            return null;
        }

        private static string GetRemovableSDCardPath(Context context)
        {
            File[] storages = ContextCompat.GetExternalFilesDirs(context, null);
            if (storages.Length > 1 && storages[0] != null && storages[1] != null)
                return storages[1].ToString();
            else
                return "";
        }


        private static string GetDataColumn(Context context, Android.Net.Uri uri, string selection, string[] selectionArgs)
        {
            
            Android.Database.ICursor cursor = null;
            string column = "_data";
            string[] projection = { column };

            try
            {
                cursor = context.ContentResolver.Query(uri, projection, selection, selectionArgs, null);
                if (cursor != null && cursor.MoveToFirst())
                {
                    int index = cursor.GetColumnIndexOrThrow(column);
                    return cursor.GetString(index);
                }
            }
            finally
            {
                if (cursor != null)
                    cursor.Close();
            }
            return null;
        }


        private static bool IsExternalStorageDocument(Android.Net.Uri uri)
        {
            return "com.android.externalstorage.documents".Equals(uri.Authority);
        }


        private static bool IsDownloadsDocument(Android.Net.Uri uri)
        {
            return "com.android.providers.downloads.documents".Equals(uri.Authority);
        }


        private static bool IsMediaDocument(Android.Net.Uri uri)
        {
            return "com.android.providers.media.documents".Equals(uri.Authority);
        }


        private static bool IsGooglePhotosUri(Android.Net.Uri uri)
        {
            return "com.google.android.apps.photos.content".Equals(uri.Authority);
        }

    }
}