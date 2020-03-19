using Android.Views;
using Android.Content;
using Android.App;
using Android.Graphics;
using Java.Lang;
using Android.Content.PM;

namespace TabletArtco
{

    public class ScreenUtil
    {
        //Get the screen width
        public static int ScreenWidth(Context cxt)
        {
            return cxt.Resources.DisplayMetrics.WidthPixels;
        }

        //Get the screen height
        public static int ScreenHeight(Context cxt)
        {
            return cxt.Resources.DisplayMetrics.HeightPixels;
        }

        public static int StatusBarHeight(Activity ac)
        {
            Rect frame = new Rect();
            ac.Window.DecorView.GetWindowVisibleDisplayFrame(frame);
            return frame.Top;
        }

        public static int dip2px(Context context, float dpValue)
        {
            float scale = context.Resources.DisplayMetrics.Density;
            return (int)(dpValue * scale + 0.5f);
        }

        public static int px2dip(Context context, float pxValue)
        {
            float scale = context.Resources.DisplayMetrics.Density;
            return (int)(pxValue / scale + 0.5f);
        }
    }

    public class ViewUtil
    {
        public static int GetViewWidth(View view)
        {
            int w = View.MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified);
            int h = View.MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified);
            view.Measure(w, h);
            return view.MeasuredWidth;
        }

        public static int GetViewHeight(View view)
        {
            int w = View.MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified);
            int h = View.MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified);
            view.Measure(w, h);
            return view.MeasuredHeight;
        }

        public static void SetViewHeight(View view, int height)
        {
            SetViewSize(view, 0, height);
        }

        public static void SetViewWidth(View view, int width)
        {
            SetViewSize(view, width, 0);
        }

        public static void SetViewSize(View view, int width, int height)
        {
            ViewGroup.LayoutParams layout = view.LayoutParameters;
            if (width != 0)
            {
                layout.Width = width;
            }
            if (height != 0)
            {
                layout.Height = height;
            }
            view.LayoutParameters = layout;
        }

        /**
        * view转bitmap
        */
        public static Bitmap viewConversionBitmap(View v)
        {
            int w = v.Width;
            int h = v.Height;
            Bitmap bmp = Bitmap.CreateBitmap(w, h, Bitmap.Config.Argb8888);
            Canvas c = new Canvas(bmp);
            v.Layout(0, 0, w, h);
            v.Draw(c);
            return bmp;
        }
    }

    public class LogUtil {

        private static bool isDebug = true;

        public static void CustomLog(string message) {
            if (isDebug)
            {
                Android.Util.Log.Info("LogUtil", "\n");
                Android.Util.Log.Info("LogUtil", message);
                Android.Util.Log.Info("LogUtil", "\n");
            }
        }

        public static void CustomLog(string tag, string message)
        {
            if (isDebug)
            {
                Android.Util.Log.Info("LogUtil", "\n");
                Android.Util.Log.Info("LogUtil", "tag: " + tag + ",   message:" + message);
                Android.Util.Log.Info("LogUtil", "\n");
            }
        }
    }

    public class ToastUtil
    {
        public static void ShowToast(Context cxt, string message)
        {
            Android.Widget.Toast.MakeText(cxt, message, Android.Widget.ToastLength.Long).Show();
        }
    }

    public class ColorUtil
    {
        public static string ColorToString(Color color)
        {
            int r = color.R;
            int g = color.G;
            int b = color.B;

            string cR = Java.Lang.Integer.ToHexString(r);
            cR = cR.Length == 1 ? "0" + cR : cR;

            string cG = Java.Lang.Integer.ToHexString(g);
            cG = cG.Length == 1 ? "0" + cG : cG;

            string cB = Java.Lang.Integer.ToHexString(b);
            cB = cB.Length == 1 ? "0" + cB : cB;

            return "#" + cR + cG + cB;
        }


        public static Color HSVToColor(float mHue, float mSat, float mVal)
        {
            return Color.HSVToColor(new float[] { mHue, mSat, mVal });
        }
    }

    public class RectUtil {

        public static bool intersect(Rect r1,  Rect r2)
        {
            int Left    = Math.Max(r1.Left, r2.Left);
            int Top     = Math.Max(r1.Top, r2.Top);
            int Right   = Math.Min(r1.Right, r2.Right);
            int Bottom  = Math.Min(r1.Bottom, r2.Bottom);

            LogUtil.CustomLog("r1:" +r1);
            LogUtil.CustomLog("r2:" + r2);

            LogUtil.CustomLog("Left:" + Left);
            LogUtil.CustomLog("Top:" + Top);
            LogUtil.CustomLog("Rigth:" + Right);
            LogUtil.CustomLog("Bottom:" + Bottom);
            if (Left<Right && Top<Bottom)
            {
                return true;
            }
            return false;
        }

    }

    public class VersionUtil
    {
         
        public static string GetAppVersionName(Context context)
        {
            string versionName = null;
            try
            {
                PackageManager pm = context.PackageManager;
                PackageInfo pi = pm.GetPackageInfo(context.PackageName, 0);
                versionName = pi.VersionName;
            }
            catch (Exception e)
            {
                
            }
            return versionName;
        }

    }

    public class UserDirectoryPath
    {
        // Android.OS.Environment.ExternalStorageDirectory.Path = "/storage/emulated/0"

        public static string objectPath { get; } = Android.OS.Environment.ExternalStorageDirectory.Path + "/Artco/object";
        public static string projectPath { get; } = Android.OS.Environment.ExternalStorageDirectory.Path + "/Artco/project";
        public static string userBackgroundPath { get; } = Android.OS.Environment.ExternalStorageDirectory.Path + "/Artco/background";
        public static string userSoundPath { get; } = Android.OS.Environment.ExternalStorageDirectory.Path + "/Artco/sound";
    }

}
