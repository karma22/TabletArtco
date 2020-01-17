
using System;

using Android.Content;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Views.Animations;
using Android.Provider;

namespace TabletArtco
{

    public interface DragDelegate
    {
        View CreateDragView(View selectView);

        void ExchangePosition(int originalPosition, int nowPosition, bool isMove);
    }

    /**
     * Created by Administrator on 2016/2/19.
     */
    public class DragGridView : GridView, Animation.IAnimationListener
    {

        private static String TAG = "DragGridView";
        private IWindowManager mWindowManager;

        private static int MODE_DRAG = 1;
        private static int MODE_NORMAL = 2;

        private int mode = MODE_NORMAL;
        private View view;
        private View dragView;
        // 要移动的item原先位置
        private int position;

        private int tempPosition;

        private int dropPosition;

        private WindowManagerLayoutParams layoutParams;
        // view的x差值
        private float mX;
        // view的y差值
        private float mY;
        // 手指按下时的x坐标(相对于整个屏幕)
        private float mWindowX;
        // 手指按下时的y坐标(相对于整个屏幕)
        private float mWindowY;
        private float originX;
        private float originY;
        private Context mContext;
        private DragDelegate mDragDelegate;

        public DragGridView(Context context) : base(context)
        {
            Initialize(context);
        }

        public DragGridView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            Initialize(context);
        }

        public DragGridView(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle)
        {
            Initialize(context);
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            int expandSpec = MeasureSpec.MakeMeasureSpec(int.MaxValue >> 2, MeasureSpecMode.AtMost);
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
        }

        void Initialize(Context context)
        {
            mContext = context;
        }

        public void setWindowManager(IWindowManager windowmanager) {
            mWindowManager = windowmanager;
        }

        public void SetDragDelegate(DragDelegate dragDelegate) {
            mDragDelegate = dragDelegate;
        }
        
        public override bool OnInterceptTouchEvent(MotionEvent e)
        {
            switch (e.Action)
            {
                case MotionEventActions.Down:
                    mWindowX = e.RawX;
                    mWindowY = e.RawY;
                    //startDrag();
                    break;
                case MotionEventActions.Move:
                    break;
                case MotionEventActions.Up:
                    break;
            }
            return base.OnInterceptHoverEvent(e);
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            switch (e.Action)
            {
                case MotionEventActions.Down:
                    break;
                case MotionEventActions.Move:
                    if (mode == MODE_DRAG)
                    {
                        updateWindow(e);
                    }
                    break;
                case MotionEventActions.Up:
                    if (mode == MODE_DRAG)
                    {
                        closeWindow(e.GetX(), e.GetY());
                    }
                    break;
            }
            return base.OnTouchEvent(e);
        }

        #region
        

        void Animation.IAnimationListener.OnAnimationEnd(Animation animation)
        {
            if (mDragDelegate != null)
            {
                mDragDelegate.ExchangePosition(position, tempPosition, true);
            }
            position = tempPosition;
        }

        void Animation.IAnimationListener.OnAnimationRepeat(Animation animation)
        {

        }

        void Animation.IAnimationListener.OnAnimationStart(Animation animation)
        {

        }
        #endregion
        private void startDrag()
        {

            if (mode == MODE_DRAG)
            {
                return;
            }

            int[] location = new int[2];
            this.GetLocationOnScreen(location);
            int x = location[0];//获取当前位置的横坐标  
            int y = location[1];//获取当前位置的纵坐标
           
            mX = mWindowX - x - this.Left;
            mY = mWindowY - y - this.Top;
            this.position = PointToPosition((int)mX, (int)mY);
            this.tempPosition = this.position;
            this.view = GetChildAt(position);
            if (this.view == null) {
                return;
            }
            // 如果是Android 6.0 要动态申请权限
            if ((int)Build.VERSION.SdkInt >= 23)
            {
                if (Settings.CanDrawOverlays(mContext))
                {
                    initWindow();
                }
                else
                {
                    // 跳转到悬浮窗权限管理界面
                    Intent intent = new Intent(Settings.ActionManageOverlayPermission);
                    mContext.StartActivity(intent);
                }
            }
            else
            {
                // 如果小于Android 6.0 则直接执行
                initWindow();
            }
        }

        /**
         * 初始化window
         */
        private void initWindow()
        {
            if (dragView == null)
            {
                if (mDragDelegate != null)
                {
                    dragView = mDragDelegate.CreateDragView(view);
                }
                else
                {
                    throw new NotImplementedException();
                }
                //dragView = View.Inflate(GetContext(), R.layout.drag_item, null);
                //TextView tv_text = (TextView)dragView.findViewById(R.id.tv_text);
                //tv_text.setText(((TextView)view.findViewById(R.id.tv_text)).getText());
            }
            if (layoutParams == null)
            {
                int[] location = new int[2];
                view.GetLocationOnScreen(location);
                int x = location[0];//获取当前位置的横坐标  
                int y = location[1];
                layoutParams = new WindowManagerLayoutParams();
                layoutParams.Type = WindowManagerTypes.Phone;
                layoutParams.Format = Android.Graphics.Format.Rgba8888;
                layoutParams.Gravity = GravityFlags.Top | GravityFlags.Left;
                layoutParams.Flags = WindowManagerFlags.NotTouchModal | WindowManagerFlags.NotFocusable;  //悬浮窗的行为，比如说不可聚焦，非模态对话框等等
                layoutParams.Width = view.Width;
                layoutParams.Height = view.Height;
                //layoutParams.X = view.Left + this.Left+x;  //悬浮窗X的位置
                //layoutParams.Y = view.Top + this.Top+y;  //悬浮窗Y的位置
                layoutParams.X = x;  //悬浮窗X的位置
                layoutParams.Y = y;  //悬浮窗Y的位置
                originX = x;
                originY = y;
                //view.Visibility = ViewStates.Invisible;
            }
            mWindowManager.AddView(dragView, layoutParams);
            mode = MODE_DRAG;
        }

        /**
         * 触摸移动时，window更新
         *
         * @param ev
         */
        private void updateWindow(MotionEvent ev)
        {
            if (mode == MODE_DRAG)
            {
                float x = ev.RawX - mWindowX;
                float y = ev.RawY - mWindowY;
                if (layoutParams != null)
                {
                    layoutParams.X = (int)(x + originX);
                    layoutParams.Y = (int)(y + originY);
                    mWindowManager.UpdateViewLayout(dragView, layoutParams);
                }
                float mx = ev.GetX();
                float my = ev.GetY();
                dropPosition = PointToPosition((int)mx, (int)my);
                if (dropPosition == tempPosition || dropPosition == GridView.InvalidPosition)
                {
                    return;
                }
                itemMove(dropPosition);
            }
        }

        /**
         * 关闭window
         *
         * @param x
         * @param y
         */
        private void closeWindow(float x, float y)
        {
            if (dragView != null)
            {
                mWindowManager.RemoveView(dragView);
                dragView = null;
                layoutParams = null;
            }
            itemDrop();
            mode = MODE_NORMAL;
        }

        /**
         * 判断item移动
         *
         * @param dropPosition
         */
        private void itemMove(int dropPosition)
        {
            TranslateAnimation translateAnimation;
            if (dropPosition < tempPosition)
            {
                for (int i = dropPosition; i < tempPosition; i++)
                {
                    View view = GetChildAt(i);
                    View nextView = GetChildAt(i + 1);
                    float xValue = (nextView.Left - view.Left) * 1f / view.Width;
                    float yValue = (nextView.Top - view.Top) * 1f / view.Height;
                    translateAnimation = new TranslateAnimation(Dimension.RelativeToSelf, 0f, Dimension.RelativeToSelf, xValue, Dimension.RelativeToSelf, 0f, Dimension.RelativeToSelf, yValue);
                    translateAnimation.Interpolator = new LinearInterpolator();
                    translateAnimation.FillAfter = true;
                    translateAnimation.Duration = 100;
                    if (i == tempPosition - 1)
                    {
                        translateAnimation.SetAnimationListener(this);
                    }
                    view.StartAnimation(translateAnimation);
                }
            }
            else
            {
                for (int i = tempPosition + 1; i <= dropPosition; i++)
                {
                    View view = GetChildAt(i);
                    View prevView = GetChildAt(i - 1);
                    float xValue = (prevView.Left - view.Left) * 1f / view.Width;
                    float yValue = (prevView.Top - view.Top) * 1f / view.Height;
                    translateAnimation = new TranslateAnimation(Dimension.RelativeToSelf, 0f, Dimension.RelativeToSelf, xValue, Dimension.RelativeToSelf, 0f, Dimension.RelativeToSelf, yValue);
                    translateAnimation.Interpolator = new LinearInterpolator();
                    translateAnimation.FillAfter = true;
                    translateAnimation.Duration = 100;
                    if (i == dropPosition)
                    {
                        translateAnimation.SetAnimationListener(this);
                    }
                    view.StartAnimation(translateAnimation);
                }
            }
            tempPosition = dropPosition;
        }

        /**
         * 手指抬起时，item下落
         */
        private void itemDrop()
        {
            if (tempPosition == position || tempPosition == GridView.InvalidPosition)
            {
                GetChildAt(position).Visibility = ViewStates.Visible;
            }
            else
            {
                if (mDragDelegate != null)
                {
                    mDragDelegate.ExchangePosition(position, tempPosition, false);
                }
                GetChildAt(tempPosition).Visibility = ViewStates.Visible;

            }
        }

    }
}