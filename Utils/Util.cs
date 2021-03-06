﻿using Android.Views;
using Android.Content;
using Android.App;
using Android.Graphics;
using Java.Lang;
using Android.Content.PM;
using Android.Views.InputMethods;
using Android.Widget;
using Android.Text;
using Java.Util.Regex;
using System.Collections.Generic;

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

        public static Android.Util.DisplayMetricsDensity DensityDpi(Context cxt)
        {
            return cxt.Resources.DisplayMetrics.DensityDpi;
        }

        public static int Density(Context cxt)
        {
            return (int)cxt.Resources.DisplayMetrics.Density;
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

    public class LogUtil
    {

        private static bool isDebug = true;

        public static void CustomLog(string message)
        {
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

    public class RectUtil
    {

        public static bool intersect(Rect r1, Rect r2)
        {
            int Left = Math.Max(r1.Left, r2.Left);
            int Top = Math.Max(r1.Top, r2.Top);
            int Right = Math.Min(r1.Right, r2.Right);
            int Bottom = Math.Min(r1.Bottom, r2.Bottom);

            LogUtil.CustomLog("r1:" + r1);
            LogUtil.CustomLog("r2:" + r2);

            LogUtil.CustomLog("Left:" + Left);
            LogUtil.CustomLog("Top:" + Top);
            LogUtil.CustomLog("Rigth:" + Right);
            LogUtil.CustomLog("Bottom:" + Bottom);
            if (Left < Right && Top < Bottom)
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
        public static string userMusicPath { get; } = Android.OS.Environment.ExternalStorageDirectory.Path + "/Artco/music";
        public static string userVideoPath { get; } = Android.OS.Environment.ExternalStorageDirectory.Path + "/Artco/video";
    }


    public class Keyboard
    {
        public static void hideKeyboard(View view)
        {
            InputMethodManager manager = (InputMethodManager)view.Context.GetSystemService(Context.InputMethodService);
            manager.HideSoftInputFromWindow(view.WindowToken, 0);
        }
    }


    public class TextViewUtil
    {
        public static void setMaxLength(TextView et, int length)
        {
            if (length > 0)
            {
                et.SetFilters(new Android.Text.IInputFilter[] { new InputFilterLengthFilter(length) });
            }
        }
    }

    public class JPWStringUtil
    {
        public static string stringFilter(string str)
        {
            // 只允许字母、数字和汉字其余的还可以随时添加比如下划线什么的，但是注意引文符号和中文符号区别
            string regEx = "[^a-zA-Z0-9\u4E00-\u9FA5]";//正则表达式
            Pattern p = Pattern.Compile(regEx);
            Matcher m = p.Matcher(str);
            return m.ReplaceAll("").Trim();
        }

    }

    public class LoopStack
    {
        private readonly List<List<(int, int)>> list = new List<List<(int, int)>>();

        public void Init(int count)
        {
            list.Clear();
            for (int i = 0; i < count; i++)
                list.Add(new List<(int, int)>());
        }

        public void Push(int lineNumber, int pc, int value)
        {
            list[lineNumber].Add((pc, value));
        }

        public int Pop(int lineNumber)
        {
            int listCount = list[lineNumber].Count;
            int lastIdx = listCount - 1;
            int loopCnt = list[lineNumber][lastIdx].Item2 - 1;
            int savedPC = list[lineNumber][lastIdx].Item1;

            if (loopCnt == 0)
            {
                list[lineNumber].RemoveAt(lastIdx);
                return 0;
            }

            list[lineNumber][lastIdx] = (savedPC, loopCnt);
            return savedPC;
        }
    }
}
