using System;
using Android.Views;
using Android.Content;
using Android.App;
using Android.Graphics;
using Android.Util;

using Java.Lang.Ref;
using Java.Lang.Reflect;
using Android.Preferences;

namespace TabletArtco {
    
    public class ScreenUtil {
        //Get the screen width
        public static int ScreenWidth(Context cxt) {
            return cxt.Resources.DisplayMetrics.WidthPixels;
        }

        //Get the screen height
        public static int ScreenHeight(Context cxt) {
            return cxt.Resources.DisplayMetrics.HeightPixels;
        }

        public static int StatusBarHeight(Activity ac) {
            Rect frame = new Rect();
            ac.Window.DecorView.GetWindowVisibleDisplayFrame(frame);
            return frame.Top;
        }
    }

    public class ViewUtil {
        public static int getViewWidth(View view) {
            int w = View.MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified);
            int h = View.MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified);
            view.Measure(w, h);
            return view.MeasuredWidth;
        }

        public static int getViewHeight(View view) {
            int w = View.MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified);
            int h = View.MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified);
            view.Measure(w, h);            
            return view.MeasuredHeight;           
        }

        public static void setViewHeight(View view, int height) {
            setViewSize(view, 0, height);
        }

        public static void setViewWidth(View view, int width) {
            setViewSize(view, width, 0);
        }

        public static void setViewSize(View view, int width, int height) {
            ViewGroup.LayoutParams layout = view.LayoutParameters;
            Log.Info("Util", "param:" + layout.Width);
            Log.Info("Util", "param:" + layout.Height);
            if (width != 0) {
                layout.Width = width;
            }
            if (height != 0) {
                layout.Height = height;
            }
            view.LayoutParameters = layout;
        }
    }

    
}
