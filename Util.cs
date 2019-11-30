using System;
using Android.Views;
using Android.Content;
using Android.App;
using Android.Graphics;
using Android.Util;

using Java.Lang.Ref;
using Java.Lang.Reflect;
using Android.Preferences;

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
    }


}
