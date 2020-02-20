using System;
using Android.Widget;

using Android.Content;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Views.Animations;
using Android.Provider;

namespace TabletArtco
{
  
    public class DragImgView: ImageView
    {
        private int nap = 200;
        private long currentTime;
        private float rawX;
        private float rawY;

        public Action<DragImgView> ClickAction { get; set; }
        public Action<DragImgView, float, float> MoveAction { get; set; }

        public DragImgView(Context context) : base(context)
        {
            Initialize(context);
        }

        public DragImgView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            Initialize(context);
        }

        public DragImgView(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle)
        {
            Initialize(context);
        }

        private void Initialize(Context cxt) {
            
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            int expandSpec = MeasureSpec.MakeMeasureSpec(int.MaxValue >> 2, MeasureSpecMode.AtMost);
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
        }

        public override bool DispatchTouchEvent(MotionEvent e)
        {
            return base.DispatchTouchEvent(e);
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            long time = Java.Lang.JavaSystem.CurrentTimeMillis();
            switch (e.Action)
            {
                case MotionEventActions.Down:
                    rawX = e.GetX();
                    rawY = e.GetY();
                    currentTime = Java.Lang.JavaSystem.CurrentTimeMillis();
                    LogUtil.CustomLog("Down---x:" + e.GetX() + ", y: " + e.GetY());
                    break;
                case MotionEventActions.Move:
                    if (time- nap > currentTime)
                    {
                        //MoveAction?.Invoke(this, e.GetX()- rawX, e.GetY()- rawY);
                        //rawX = e.GetX();
                        //rawY = e.GetY();
                        MoveAction?.Invoke(this, e.GetX()-rawX, e.GetY()-rawY);
                    }
                    LogUtil.CustomLog("Move---x:" + e.GetX() + ", y: " + e.GetY());
                    break;
                case MotionEventActions.Up:
                    if (time - nap <= currentTime)
                    {
                        ClickAction?.Invoke(this);
                    }
                    else
                    {
                        //MoveAction?.Invoke(this, e.GetX() - rawX, e.GetY() - rawY);
                        //rawX = e.GetX();
                        //rawY = e.GetY();
                        MoveAction?.Invoke(this, e.GetX() - rawX, e.GetY() - rawY);
                    }
                    LogUtil.CustomLog("Up---x:" + e.GetX() + ", y: " + e.GetY());
                    break;
            }
            return true;
        }

       
    }
}
