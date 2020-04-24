using System;
using Android;
using Android.App;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Support.V7.App;

namespace TabletArtco
{
    class PermissionUtil
    {
        /**
        * 获取打开摄像机的权限，录音，文件读写
        *
        * @param activity
        */
        public static void checkPermission(Activity activity)
        {
            if ((int)Build.VERSION.SdkInt >= 23)
            {
                int checkPermission =
                        (int)(ContextCompat.CheckSelfPermission(activity, Manifest.Permission.RecordAudio))
                      + (int)(ContextCompat.CheckSelfPermission(activity, Manifest.Permission.ReadPhoneState))
                      + (int)(ContextCompat.CheckSelfPermission(activity, Manifest.Permission.WriteExternalStorage))
                      + (int)(ContextCompat.CheckSelfPermission(activity, Manifest.Permission.ReadExternalStorage));
                if (checkPermission != 0)
                {
                    //动态申请
                    ActivityCompat.RequestPermissions(activity, new String[]{
                        Manifest.Permission.RecordAudio,
                        Manifest.Permission.ReadPhoneState,
                        Manifest.Permission.WriteExternalStorage,
                        Manifest.Permission.ReadExternalStorage}, 123);
                    return;
                }
                else
                {
                    return;
                }
            }
            return;
        }

        //internal static void checkPermission(FullScreenActivity fullScreenActivity)
        //{
        //    throw new NotImplementedException();
        //}
    }
}