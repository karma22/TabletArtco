using System;
using Android.Content;
using Android.Graphics;
using Android.Util;
using Android.Views;
using Java.Lang;
using static Android.Graphics.Paint;
using static Android.Graphics.Shader;

/**
* 取色器
* <p>
* 所有注释单位为dp的全局变量，初始都是dp值，在使用之前会乘上屏幕像素(mDensity)称为px值
*
* @author wangjinping
* @since 2020/03/05
*/
namespace TabletArtco
{
    class ColorPickerView : View
    {
        enum PANEL { SAT_VAL, HUE };

        /**
         * 显示H、SV的矩形的边框粗细（单位：dp）
         */
        private static float BORDER_WIDTH = 1;
        /**
         * H矩形的宽度（单位：dp）
         */
        private float mHuePanelWidth = 30f;
        /**
         * H、SV矩形间的间距（单位：dp）
         */
        private float mPanelSpacing = 10f;
        /**
         * 当mode为MeasureSpec.UNSPECIFIED时的首选高度（单位：dp）
         */
        private float mPreferredHeight = 200;
        /**
         * 当mode为MeasureSpec.UNSPECIFIED时的首选宽度（单位：dp）
         */
        private float mPreferredWidth = 200 + 30f + 10f;//mPreferredHeight + mHuePanelWidth + mPanelSpacing;
                                                        /**
                                                         * SV指示器的半径（单位：dp）
                                                         */
        private float mSVTrackerRadius = 5f;
        /**
         * SV指示器的半径（单位：dp）
         */
        private float mHTrackerHeight = 4f;
        /**
         * H、SV矩形与父布局的边距（单位：dp）
         */
        private float mRectOffset = 2f;
        /**
         * 屏幕密度
         */
        private float mDensity = 1f;
        /**
         * 绘制SV的画笔
         */
        private Paint mSatValPaint;
        /**
         * 绘制SV指示器的画笔
         */
        private Paint mSatValTrackerPaint;
        /**
         * 绘制H的画笔
         */
        private Paint mHuePaint;
        /**
         * 绘制H指示器的画笔
         */
        private Paint mHueTrackerPaint;
        /**
         * 绘制H、SV矩形的边线的画笔
         */
        private Paint mBorderPaint;

        //H、V着色器
        private Shader mHueShader;
        private Shader mValShader;

        //HSV的默认值
        private float mHue = 360f;
        private float mSat = 0f;
        private float mVal = 0f;

        /**
         * 用于显示被选择H的位置的指示器的颜色
         */
        private string mSliderTrackerColor = "#1c1c1c";
        /**
         * H、SV矩形的边框颜色
         */
        private string mBorderColor = "#6E6E6E";
        /**
         * 记录上一次被点击的颜色板
         */
        //@PANEL
        private PANEL mLastTouchedPanel = PANEL.SAT_VAL;
        /**
         * 边距
         */
        private float mDrawingOffset;
        /**
         * H指示器
         */
        private RectF mDrawingRect;
        /**
         * 用于选择SV的矩形
         */
        private RectF mSatValRect;
        /**
         * 用于选择H的矩形
         */
        private RectF mHueRect;
        /**
         * SV指示器
         */
        private Point mStartTouchPoint = null;


        public Action<Color> onColorChanged { get; set; }

        //private OnColorChangedListener mListener;

        //interface OnColorChangedListener
        //{
        //    void onColorChanged(int color);
        //}



        public ColorPickerView(Context context) : base(context)
        {
            init();
        }

        public ColorPickerView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            init();
        }

        public ColorPickerView(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle)
        {
            init();
        }

        private void init()
        {

            mDensity = Context.Resources.DisplayMetrics.Density;//获取屏幕密度
            mSVTrackerRadius *= mDensity;//灰度饱和度指示器的半径
            mHTrackerHeight *= mDensity;//色相指示器高度
            mRectOffset *= mDensity;//H、SV矩形与父布局的边距
            mHuePanelWidth *= mDensity;//H矩形的宽度
            mPanelSpacing *= mDensity;//H、SV矩形间的间距
            mPreferredHeight *= mDensity;//当mode为MeasureSpec.UNSPECIFIED时的首选高度
            mPreferredWidth *= mDensity;//当mode为MeasureSpec.UNSPECIFIED时的首选宽度

            mDrawingOffset = calculateRequiredOffset();//计算所需位移

            initPaintTools();//初始化画笔、画布
            //SetFocusable(ViewFocusability.Focusable);//设置可获取焦点

            //setFocusableInTouchMode(true);//设置在被触摸时会获取焦点

        }

        /**
         * mSVTrackerRadius、
         * mRectOffset、
         * BORDER_WIDTH * mDensity
         * 三者的最大值
         * 的1.5倍
         *
         * @return 边距
         */
        private float calculateRequiredOffset()
        {
            float offset = Java.Lang.Math.Max(mSVTrackerRadius, mRectOffset);
            offset = Java.Lang.Math.Max(offset, BORDER_WIDTH * mDensity);
            return offset * 1.5f;
        }

        private void initPaintTools()
        {
            mSatValPaint = new Paint();
            mSatValTrackerPaint = new Paint();
            mHuePaint = new Paint();
            mHueTrackerPaint = new Paint();
            mBorderPaint = new Paint();

            mSatValTrackerPaint.SetStyle(Style.Stroke);
            mSatValTrackerPaint.StrokeWidth = 2f * mDensity;
            mSatValTrackerPaint.AntiAlias = true;

            mHueTrackerPaint.Color = Color.ParseColor(mSliderTrackerColor);
            mHueTrackerPaint.SetStyle(Style.Stroke);
            mHueTrackerPaint.StrokeWidth = 2f * mDensity;
            mHueTrackerPaint.AntiAlias = true;
        }


        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {

            MeasureSpecMode widthMode = MeasureSpec.GetMode(widthMeasureSpec);
            MeasureSpecMode heightMode = MeasureSpec.GetMode(heightMeasureSpec);
            int widthAllowed = MeasureSpec.GetSize(widthMeasureSpec);
            int heightAllowed = MeasureSpec.GetSize(heightMeasureSpec);

            widthAllowed = isUnspecified(widthMode) ? (int)mPreferredWidth : widthAllowed;
            heightAllowed = isUnspecified(heightMode) ? (int)mPreferredHeight : heightAllowed;

            int width = widthAllowed;
            int height = (int)(widthAllowed - mPanelSpacing - mHuePanelWidth);
            //当根据宽度计算出来的高度大于可允许的最大高度时 或 当前是横屏
            if (height > heightAllowed || "landscape".Equals(Tag))
            {
                height = heightAllowed;
                width = (int)(height + mPanelSpacing + mHuePanelWidth);
            }
            SetMeasuredDimension(width, height);
        }

        private static bool isUnspecified(MeasureSpecMode mode)
        {
            return !(mode == MeasureSpecMode.Exactly || mode == MeasureSpecMode.AtMost);
        }


        public override void Draw(Canvas canvas)
        {
            if (mDrawingRect.Width() <= 0 || mDrawingRect.Height() <= 0) return;
            drawSatValPanel(canvas);//绘制SV选择区域
            drawHuePanel(canvas);//绘制右侧H选择区域
        }

        /**
         * 绘制S、V选择区域（矩形）
         *
         * @param canvas 画布
         */
        private void drawSatValPanel(Canvas canvas)
        {
            //描边（先画一个大矩形, 再在内部画一个小矩形，就可以显示出描边的效果）
            mBorderPaint.Color = Color.ParseColor(mBorderColor);
            canvas.DrawRect(
                    mDrawingRect.Left,
                    mDrawingRect.Top,
                    mSatValRect.Right + BORDER_WIDTH,
                    mSatValRect.Bottom + BORDER_WIDTH,
                    mBorderPaint);

            //组合着色器 = 明度线性着色器 + 饱和度线性着色器
            ComposeShader mShader = generateSVShader();
            mSatValPaint.SetShader(mShader);
            canvas.DrawRect(mSatValRect, mSatValPaint);

            //初始化选择器的位置
            Point p = satValToPoint(mSat, mVal);
            //绘制显示SV值的选择器
            mSatValTrackerPaint.Color = Color.ParseColor("#000000");
            canvas.DrawCircle(p.X, p.Y, mSVTrackerRadius - 1f * mDensity, mSatValTrackerPaint);
            //绘制外圆
            mSatValTrackerPaint.Color = Color.ParseColor("#dddddd");
            canvas.DrawCircle(p.X, p.Y, mSVTrackerRadius, mSatValTrackerPaint);
        }

        /**
         * 创建SV着色器(明度线性着色器 + 饱和度线性着色器)
         *
         * @return 着色器
         */
        private ComposeShader generateSVShader()
        {
            //明度线性着色器
            if (mValShader == null)
            {
                mValShader = new LinearGradient(mSatValRect.Left, mSatValRect.Top, mSatValRect.Left, mSatValRect.Bottom, Color.ParseColor("#ffffff"), Color.ParseColor("#000000"), TileMode.Clamp);
            }
            //HSV转化为RGB
            Color rgb = Color.HSVToColor(new float[] { mHue, 1f, 1f });
            //饱和线性着色器
            Shader satShader = new LinearGradient(mSatValRect.Left, mSatValRect.Top, mSatValRect.Right, mSatValRect.Top, Color.ParseColor("#ffffff"), rgb, TileMode.Clamp);
            //组合着色器 = 明度线性着色器 + 饱和度线性着色器
            return new ComposeShader(mValShader, satShader, PorterDuff.Mode.Multiply);
        }

        /**
         * 绘制右侧H选择区域
         *
         * @param canvas 画布
         */
        private void drawHuePanel(Canvas canvas)
        {
            RectF rect = mHueRect;

            mBorderPaint.Color = Color.ParseColor(mBorderColor);
            canvas.DrawRect(rect.Left - BORDER_WIDTH,
                    rect.Top - BORDER_WIDTH,
                    rect.Right + BORDER_WIDTH,
                    rect.Bottom + BORDER_WIDTH,
                    mBorderPaint);
            //初始化H线性着色器
            if (mHueShader == null)
            {
                int[] hue = new int[361];
                int count = 0;
                for (int i = hue.Length - 1; i >= 0; i--, count++)
                {
                    hue[count] = Color.HSVToColor(new float[] { i, 1f, 1f });
                }
                mHueShader = new LinearGradient(
                        rect.Left,
                        rect.Top,
                        rect.Left,
                        rect.Bottom,
                        hue,
                        null,
                        TileMode.Clamp);
                mHuePaint.SetShader(mHueShader);
            }

            canvas.DrawRect(rect, mHuePaint);

            float halfHTrackerHeight = mHTrackerHeight / 2;
            //初始化H选择器选择条位置
            Point p = hueToPoint(mHue);

            RectF r = new RectF();
            r.Left = rect.Left - mRectOffset;
            r.Right = rect.Right + mRectOffset;
            r.Top = p.Y - halfHTrackerHeight;
            r.Bottom = p.Y + halfHTrackerHeight;

            //绘制选择条
            canvas.DrawRoundRect(r, 2, 2, mHueTrackerPaint);
        }

        private Point hueToPoint(float hue)
        {
            RectF rect = mHueRect;
            float height = rect.Height();

            Point p = new Point();
            p.Y = (int)(height - (hue * height / 360f) + rect.Top);
            p.X = (int)rect.Left;
            return p;
        }

        private Point satValToPoint(float sat, float val)
        {
            float height = mSatValRect.Height();
            float width = mSatValRect.Width();

            Point p = new Point();
            p.X = (int)(sat * width + mSatValRect.Left);
            p.Y = (int)((1f - val) * height + mSatValRect.Top);
            return p;
        }

        private float[] pointToSatVal(float x, float y)
        {
            RectF rect = mSatValRect;
            float[] result = new float[2];

            float width = rect.Width();
            float height = rect.Height();

            if (x < rect.Left)
            {
                x = 0f;
            }
            else if (x > rect.Right)
            {
                x = width;
            }
            else
            {
                x = x - rect.Left;
            }

            if (y < rect.Top)
            {
                y = 0f;
            }
            else if (y > rect.Bottom)
            {
                y = height;
            }
            else
            {
                y = y - rect.Top;
            }
            result[0] = 1.0f / width * x;
            result[1] = 1.0f - (1.0f / height * y);
            return result;
        }

        private float pointToHue(float y)
        {
            RectF rect = mHueRect;
            float height = rect.Height();
            if (y < rect.Top)
            {
                y = 0f;
            }
            else if (y > rect.Bottom)
            {
                y = height;
            }
            else
            {
                y = y - rect.Top;
            }
            return 360f - (y * 360f / height);
        }


        public override bool OnTrackballEvent(MotionEvent e)
        {


            float x = e.GetX();
            float y = e.GetY();
            bool isUpdated = false;
            if (e.Action == MotionEventActions.Move)
            {
                switch (mLastTouchedPanel)
                {
                    case PANEL.SAT_VAL:
                        float sat, val;
                        sat = mSat + x / 50f;
                        val = mVal - y / 50f;
                        if (sat < 0f)
                        {
                            sat = 0f;
                        }
                        else if (sat > 1f)
                        {
                            sat = 1f;
                        }
                        if (val < 0f)
                        {
                            val = 0f;
                        }
                        else if (val > 1f)
                        {
                            val = 1f;
                        }
                        mSat = sat;
                        mVal = val;
                        isUpdated = true;
                        break;
                    case PANEL.HUE:
                        float hue = mHue - y * 10f;
                        if (hue < 0f)
                        {
                            hue = 0f;
                        }
                        else if (hue > 360f)
                        {
                            hue = 360f;
                        }
                        mHue = hue;
                        isUpdated = true;
                        break;
                }
            }
            if (isUpdated)
            {
                //if (mListener != null)
                //{
                //    mListener.onColorChanged(Color.HSVToColor(new float[] { mHue, mSat, mVal }));
                //}
                onColorChanged?.Invoke(Color.HSVToColor(new float[] { mHue, mSat, mVal }));
                Invalidate();
                return true;
            }
            return base.OnTrackballEvent(e);
        }



        public override bool OnTouchEvent(MotionEvent e)
        {
            bool isUpdated = false;
            switch (e.Action)
            {
                case MotionEventActions.Down:
                    {
                        mStartTouchPoint = new Point((int)e.GetX(), (int)e.GetY());
                        isUpdated = moveTrackersIfNeeded(e);
                        break;
                    }
                case MotionEventActions.Move:
                    {
                        isUpdated = moveTrackersIfNeeded(e);
                        break;
                    }

                case MotionEventActions.Up:
                    {
                        mStartTouchPoint = null;
                        isUpdated = moveTrackersIfNeeded(e);
                        break;
                    }
            }
            if (isUpdated)
            {
                onColorChanged?.Invoke(Color.HSVToColor(new float[] { mHue, mSat, mVal }));
                Invalidate();
                return true;
            }
            return base.OnTouchEvent(e);
        }

        private bool moveTrackersIfNeeded(MotionEvent e)
        {

            if (mStartTouchPoint == null) return false;
            bool update = false;
            int startX = mStartTouchPoint.X;
            int startY = mStartTouchPoint.Y;
            if (mHueRect.Contains(startX, startY))
            {
                mLastTouchedPanel = PANEL.HUE;
                mHue = pointToHue(e.GetY());
                update = true;
            }
            else if (mSatValRect.Contains(startX, startY))
            {
                mLastTouchedPanel = PANEL.SAT_VAL;
                float[] result = pointToSatVal(e.GetX(), e.GetY());
                mSat = result[0];
                mVal = result[1];
                update = true;
            }
            return update;
        }




        protected override void OnSizeChanged(int w, int h, int oldw, int oldh)
        {
            base.OnSizeChanged(w, h, oldw, oldh);
            mDrawingRect = new RectF();

            mDrawingRect.Left = mDrawingOffset + PaddingLeft;
            mDrawingRect.Right = w - mDrawingOffset - PaddingRight;
            mDrawingRect.Top = mDrawingOffset + PaddingTop;
            mDrawingRect.Bottom = h - mDrawingOffset - PaddingBottom;
            //当DatePickerView的长宽改变时，重新计算SV、H矩形大小
            setUpSatValRect();
            setUpHueRect();
        }

        private void setUpSatValRect()
        {
            RectF dRect = mDrawingRect;
            float panelSide = dRect.Height() - BORDER_WIDTH * 2;
            float left = dRect.Left + BORDER_WIDTH;
            float top = dRect.Top + BORDER_WIDTH;
            float bottom = top + panelSide;
            float right = Left + panelSide;
            mSatValRect = new RectF(left, top, right, bottom);
        }

        private void setUpHueRect()
        {
            RectF dRect = mDrawingRect;
            float left = dRect.Right - mHuePanelWidth + BORDER_WIDTH;
            float top = dRect.Top + BORDER_WIDTH;
            float bottom = dRect.Bottom - BORDER_WIDTH;
            float right = dRect.Right - BORDER_WIDTH;
            mHueRect = new RectF(left, top, right, bottom);
        }


        /**
         * 设置边框颜色
         *
         * @param color 边框颜色
         */
        public void setBorderColor(string color)
        {
            mBorderColor = color;
            Invalidate();
        }

        /**
         * 获取边框颜色
         *
         * @return 边框颜色
         */
        public string getBorderColor()
        {
            return mBorderColor;
        }

        /**
         * 获取当前颜色
         *
         * @return 当前颜色
         */
        public Color GetCurColor()
        {
            return Color.HSVToColor(new float[] { mHue, mSat, mVal });
        }

        /**
         * 设置选择的颜色
         *
         * @param color 被选择的颜色
         */
        public void setColor(string color)
        {
            setColor(color, false);
        }

        /**
         * 设置被选择的颜色
         *
         * @param color    被选择的颜色
         * @param callback 是否触发OnColorChangedListener
         */
        public void setColor(string color, bool callback)
        {
            float[] hsv = new float[3];
            Color.ColorToHSV(Color.ParseColor(color), hsv);
            mHue = hsv[0];
            mSat = hsv[1];
            mVal = hsv[2];
            if (callback)
            {
                onColorChanged?.Invoke(Color.HSVToColor(new float[] { mHue, mSat, mVal }));
            }
            Invalidate();
        }

        /**
         * ColorPickerView的padding
         *
         * @return padding（单位：px）
         */
        public float getDrawingOffset()
        {
            return mDrawingOffset;
        }

        public void setSliderTrackerColor(string color)
        {
            mSliderTrackerColor = color;
            mHueTrackerPaint.Color = Color.ParseColor(mSliderTrackerColor);
            Invalidate();
        }

        public string getSliderTrackerColor()
        {
            return mSliderTrackerColor;
        }
    }
}
