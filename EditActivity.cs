﻿
using System;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using Com.Bumptech.Glide;
using System.Collections.Generic;
using Android.Content;
using Android.Util;

namespace TabletArtco
{
    //[Activity(Label = "EditActivity", MainLauncher = true)]
    [Activity(Theme = "@style/AppTheme")]
    public class EditActivity : Activity, Delegate, DataSource
    {
        private List<Bitmap> originList = new List<Bitmap>();
        private List<Bitmap> operateList = new List<Bitmap>();
        private int mIndex = -1;
        private CustomView mCustomView;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            Window.SetFlags(Android.Views.WindowManagerFlags.Fullscreen, Android.Views.WindowManagerFlags.Fullscreen);
            SetContentView(Resource.Layout.activity_edit);
            RequestedOrientation = Android.Content.PM.ScreenOrientation.Landscape;
            //initData();
            InitView();
        }

        private void initData() {
            Intent intent = this.Intent;
            Bundle bundle = intent.GetBundleExtra("bundle");
            int position = bundle.GetInt("position");
            if (position<Project.mSprites.Count)
            {
                ActivatedSprite sprite = Project.mSprites[position];
                for (int i = 0; i < sprite.originBitmapList.Count; i++)
                {
                    Bitmap bitmap = Bitmap.CreateBitmap(sprite.originBitmapList[i]);
                    originList.Add(bitmap);
                    operateList.Add(Bitmap.CreateBitmap(bitmap));
                }
                if (sprite.originBitmapList.Count>0)
                {
                    mIndex = 0;
                }
            }
               
        }

        private void InitView()
        {
            InitTopView();
            InitMaterialList();
            InitToolView();
            InitOperateView();
        }

        private void InitTopView()
        {
            int h = (int)(ScreenUtil.ScreenHeight(this) * 57 / 800.0);
            int logoW = (int)(70 / 43.0 * h);
            int titleW = (int)(242 / 35.0 * h);
            ImageView logoIv = FindViewById<ImageView>(Resource.Id.edit_logo_view);
            ImageView titleIv = FindViewById<ImageView>(Resource.Id.edit_title_view);
            ViewUtil.SetViewWidth(logoIv, logoW);
            ViewUtil.SetViewWidth(titleIv, titleW);
        }

        private void InitMaterialList()
        {
            int sw = ScreenUtil.ScreenWidth(this);
            double width = sw * 190 / 1280.0;
            LinearLayout editListWrapper = FindViewById<LinearLayout>(Resource.Id.edit_list_wrapper);
            editListWrapper.SetPadding((int)(28.0 / 1280.0 * sw), 0, (int)(4 / 1280.0 * sw), (int)(8 / 1280.0 * sw));
            int listW = (int)((190 - 60) / 1280.0 * sw);
            int itemW = listW - 30;
            ListView listView = FindViewById<ListView>(Resource.Id.edit_material_list);
            listView.SetPadding(30, 28, 45, 50);
            listView.Adapter = new SpriteAdapter(this, this);

        }

        private void InitToolView()
        {
            int w = (int)(ScreenUtil.ScreenWidth(this) * 1055 / 1280.0);
            int h = (int)(ScreenUtil.ScreenHeight(this) * 6 / 8.0);
            //1090/1280.0
            //600 143
            //145 /543
            FrameLayout wrapperView = FindViewById<FrameLayout>(Resource.Id.area_wrapper_view);
            LinearLayout.LayoutParams layoutParams = (LinearLayout.LayoutParams)wrapperView.LayoutParameters;
            layoutParams.RightMargin = (int)(12 / 1280.0 * ScreenUtil.ScreenWidth(this));
            wrapperView.LayoutParameters = layoutParams;

            int topMargin = (int)(13 / 600.0 * h);
            int wrapperH = (int)(543 / 600.0 * h);
            int toolW = (int)(145 / 543.0 * wrapperH);
            int areaW = w - toolW - topMargin * 4;
            //140 //28 26
            int colorW = (int)(28 / 140.0 * toolW);
            int colorH = (int)(13 / 14.0 * colorW);
            int colorMargin = (int)((toolW - (colorW * 4)) / 5.0);

            //center edit area
            FrameLayout areaView = FindViewById<FrameLayout>(Resource.Id.edit_area_view);
            FrameLayout.LayoutParams areaParams = (FrameLayout.LayoutParams)areaView.LayoutParameters;
            areaParams.Height = wrapperH - colorMargin;
            areaParams.Width = areaW - (colorMargin * 2);
            areaParams.TopMargin = topMargin + colorMargin;
            areaParams.LeftMargin = topMargin + colorMargin;
            areaView.LayoutParameters = areaParams;
            areaView.SetBackgroundResource(Resource.Drawable.xml_edit_bg);

            mCustomView = new CustomView(this);
            mCustomView.mAction = (list) =>
            {

            };
            mCustomView.AddBitmap(operateList[0]);
            FrameLayout.LayoutParams conParams = new FrameLayout.LayoutParams(areaParams.Width, areaParams.Height);
            areaView.AddView(mCustomView, conParams);

            //right tool area
            FrameLayout toolView = FindViewById<FrameLayout>(Resource.Id.tool_wrapper_view);
            FrameLayout.LayoutParams toolParams = (FrameLayout.LayoutParams)toolView.LayoutParameters;
            toolParams.Height = wrapperH;
            toolParams.Width = toolW;
            toolParams.TopMargin = topMargin;
            toolParams.LeftMargin = (int)(w - toolW - (topMargin * 2));
            toolView.LayoutParameters = toolParams;

            String[] colors = {
                "#000000","", "", "",
                "#880016", "#b97a57", "#300088", "#ab57b9",
                "#ed1b24", "#feaec9", "#7c1ded", "#bbaefe",
                "#ff7f26", "#ffc90d", "#ec26ff", "#ff0ebd",
                "#fef200", "#efe3af", "#ff008e", "#efb1da",
                "#23b14d", "#b5e51d", "#b19321", "#e61d51",
                "#00a3e8", "#9ad9ea", "#2ee800", "#b1ea99",
                "#3f47cc", "#7092bf", "#3fcc7d", "#70bd75",
                "#a349a3", "#c7bfe8", "#4978a4", "#bfe7dc"
            };
            for (int i = 0; i < 36; i++)
            {
                if (0 < i && i < 4)
                {
                    continue;
                }
                FrameLayout.LayoutParams colorParams = new FrameLayout.LayoutParams(colorW, colorH);
                colorParams.TopMargin = colorMargin + (i / 4 * (colorH + colorMargin));
                colorParams.LeftMargin = colorMargin + (i % 4 * (colorW + colorMargin));
                ImageView imgIv = new ImageView(this);
                imgIv.LayoutParameters = colorParams;
                imgIv.SetBackgroundColor(Color.ParseColor(colors[i]));
                toolView.AddView(imgIv);
            }

            //The center of two button
            int cW = (int)((toolW - topMargin * 3) / 2.0);
            int cMargin = (int)(295 / 543.0 * wrapperH);
            int[] cIds = { Resource.Drawable.Button_pallet, Resource.Drawable.Button_spoid };
            for (int i = 0; i < cIds.Length; i++)
            {
                FrameLayout.LayoutParams btParams = new FrameLayout.LayoutParams(cW, cW);
                btParams.TopMargin = cMargin;
                btParams.LeftMargin = topMargin + i * (topMargin + cW);
                ImageView imgIv = new ImageView(this);
                imgIv.LayoutParameters = btParams;
                imgIv.SetImageResource(cIds[i]);
                toolView.AddView(imgIv);
            }

            //366
            int sTopMargin = (int)(366 / 543.0 * wrapperH);
            int sW = (int)(((colorW * 4) + (colorMargin * 2)) / 2.0);
            int sH = (int)(28 / 63.0 * sW);
            for (int i = 0; i < cIds.Length; i++)
            {
                FrameLayout.LayoutParams btParams = new FrameLayout.LayoutParams(sW, sH);
                btParams.TopMargin = sTopMargin;
                btParams.LeftMargin = colorMargin + i * (colorMargin + sW);
                if (i == 0)
                {
                    TextView textTv = new TextView(this);
                    textTv.LayoutParameters = btParams;
                    textTv.SetBackgroundResource(Resource.Drawable.edit_size_bg);
                    textTv.Text = "10";
                    textTv.Gravity = GravityFlags.CenterVertical;
                    textTv.SetPadding(4, 0, 4, 0);
                    textTv.TextSize = (float)(sH / 4.0);
                    toolView.AddView(textTv);
                }
                else
                {
                    ImageView imgIv = new ImageView(this);
                    imgIv.LayoutParameters = btParams;
                    imgIv.SetBackgroundColor(Color.ParseColor("#000000"));
                    toolView.AddView(imgIv);
                }
            }

            //The bottom of three button
            int[] resIds = { Resource.Drawable.Button_PictureCopy, Resource.Drawable.Button_PictureOpen, Resource.Drawable.Button_Restore };
            int btH = (int)(46 / 144.0 * toolW);
            int btTopMargin = (int)(2 / 144.0 * toolW);
            for (int i = 0; i < resIds.Length; i++)
            {
                FrameLayout.LayoutParams btParams = new FrameLayout.LayoutParams(toolW, btH);
                btParams.TopMargin = wrapperH - ((2 - i) * (btH + btTopMargin)) - btH;
                ImageView imgIv = new ImageView(this);
                imgIv.LayoutParameters = btParams;
                imgIv.SetImageResource(resIds[i]);
                toolView.AddView(imgIv);
            }
        }

        private void InitOperateView()
        {
            int[] resIds = {
                Resource.Drawable.FlipXBtn, Resource.Drawable.FlipYBtn, Resource.Drawable.RRotate,
                Resource.Drawable.LRotate, Resource.Drawable.SizeIncreseBtn, Resource.Drawable.SizeDecreseBtn,
                Resource.Drawable.EraseBtn, Resource.Drawable.BrushBtn, Resource.Drawable.MagicBackBtn,
                Resource.Drawable.RectCutBtn, Resource.Drawable.FreeCutBtn, Resource.Drawable.EditAddSprite
            };
            int sw = (int)(ScreenUtil.ScreenWidth(this) * 931 / 1280.0);
            FrameLayout contentView = FindViewById<FrameLayout>(Resource.Id.edit_bt_content_view);
            int itemW = (int)(sw / 12.0);
            int imgW = (int)(10.0 / 11 * itemW);
            for (int i = 0; i < resIds.Length; i++)
            {
                FrameLayout.LayoutParams layoutParams = new FrameLayout.LayoutParams(imgW, imgW);
                layoutParams.LeftMargin = i * itemW + 8;
                layoutParams.TopMargin = 8;
                ImageView imgBt = new ImageView(this);
                imgBt.SetBackgroundResource(resIds[i]);
                imgBt.LayoutParameters = layoutParams;
                contentView.AddView(imgBt);
            }
        }


        /*
         * 
         * Delegate and DataSource 
         * 
        */
        public int GetItemsCount(Java.Lang.Object adapter)
        {
            return operateList.Count;
        }

        public View GetItemView(Java.Lang.Object adapter, ViewGroup parent)
        {
            View convertView = LayoutInflater.From(this).Inflate(Resource.Layout.item_sprite, parent, false);
            //int itemW = (int)(ScreenUtil.ScreenWidth(this) * 146 / 1280.0 - ScreenUtil.dip2px(this, 24));

            int sw = ScreenUtil.ScreenWidth(this);
            double width = sw * 190 / 1280.0;
            int listW = (int)((190 - 60) / 1280.0 * sw);
            int itemW = listW - 30;

            ViewUtil.SetViewHeight(convertView, itemW);
            ViewHolder holder = new ViewHolder();
            holder.bgIv = convertView.FindViewById<ImageView>(Resource.Id.selected_material_bgIv);
            holder.imgIv = convertView.FindViewById<ImageView>(Resource.Id.selected_material_imgIv);
            convertView.Tag = holder;
            convertView.Click += (t, e) =>
            {
                ViewHolder viewHolder = (ViewHolder)(((View)t).Tag);
                //ClickItem(position);
            };
            return convertView;
        }

        public void UpdateItemView(Java.Lang.Object adapter, View contentView, int position)
        {
            //List<List<Sprite>> sprites = Sprite._sprites;
            //if (mIndex >= sprites.Count)
            //{
            //    return;
            //}
            //List<Sprite> list = sprites[mIndex];
            Bitmap bitmap = operateList[position];
            ViewHolder viewHolder = (ViewHolder)contentView.Tag;
            viewHolder.imgIv.SetImageBitmap(bitmap);
            //Glide.With(this).Load(sprite.remotePath).Into(viewHolder.imgIv);
            //viewHolder.txtTv.Text = sprite.name;
            //viewHolder.txtTv.Tag = position;
        }

        public void ClickItem(int position)
        {
            //Android.Util.Log.Info("position", "position===" + position);
            //List<List<Sprite>> sprites = Sprite._sprites;
            //if (mIndex >= sprites.Count)
            //{
            //    return;
            //}
            //List<Sprite> list = sprites[mIndex];
            //Sprite sprite = list[position];
            //Intent intent = new Intent();
            //Bundle bundle = new Bundle();
            //bundle.PutString("model", sprite.ToString());
            //intent.PutExtra("bundle", bundle);
            //SetResult(Result.Ok, intent);
            //Finish();
        }

        //定义ViewHolder内部类，用于对控件实例进行缓存
        class ViewHolder : Java.Lang.Object
        {
            public ImageView bgIv;
            public ImageView imgIv;
        }
    }

    enum OperateTarget { Default, RightUp, RightDown, LeftDown, LeftUp, Right, Down, Left, Up };
    enum OperateAction { FlipX, FlipY, RotateR, RotateL, Enlarge, Narrow, Eraser, Draw, Move };

    struct HPosition {
        public int row;
        public int column;
        public HPosition(int row, int column) {
            this.row = row;
            this.column = column;
        }
    }

    class CustomView : View {

        private Context mContext;
        
        private int mWidth;
        private int mHeight;

        private Bitmap mBitmap;

        private Paint mEraserPaint;
        private Paint mPaint;
        public Action<List<Bitmap>> mAction { get; set; }

        public static List<List<List<Bitmap>>> layersList = new List<List<List<Bitmap>>>();
        public static List<List<List<Dictionary<OperateAction, Java.Lang.Object>>>> operateList = new List<List<List<Dictionary<OperateAction, Java.Lang.Object>>>>();
        public static List<HPosition> positionList = new List<HPosition>();

        public static List<List<Bitmap>> curlayersList = new List<List<Bitmap>>();
        public static List<List<Dictionary<OperateAction, Java.Lang.Object>>> curOperateList = new List<List<Dictionary<OperateAction, Java.Lang.Object>>>();
        public static HPosition curHPosition;
        private int mIndex;

        public CustomView(Context context) : base(context)
        {
            Initialize(context);
        }

        public CustomView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            Initialize(context);
        }

        public CustomView(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle)
        {
            Initialize(context);
        }

        private void Initialize(Context context)
        {
            mContext = context;
            mPaint = new Paint();
        }

        public void AddBitmap(Bitmap bitmap)
        {
            // 添加操作图片
            List<Bitmap> list = new List<Bitmap>();
            list.Add(Bitmap.CreateBitmap(bitmap));
            curlayersList.RemoveRange(0, curlayersList.Count);
            curlayersList.Add(list);
            layersList.Add(curlayersList);
            // 添加操作
            List<Dictionary<OperateAction, Java.Lang.Object>> tempList = new List<Dictionary<OperateAction, Java.Lang.Object>>();
            curOperateList.RemoveRange(0, curOperateList.Count);
            curOperateList.Add(tempList);
            operateList.Add(curOperateList);
            // 添加操作历史
            curHPosition = new HPosition(layersList.Count-1, 0);
            positionList.Add(curHPosition);
            updateBitmap();
        }

        public void DeleteBitmap(int position)
        {
            if (position == mIndex)
            {
                mIndex = position - 1;
                curlayersList = layersList[mIndex];
                curOperateList = operateList[mIndex];
                curHPosition = positionList[mIndex];
                layersList.RemoveAt(position);
                operateList.RemoveAt(position);
                positionList.RemoveAt(position);
            }
            updateBitmap();
        }

        // 当前图片插入图片
        public void InsertBitmap(Bitmap bitmap)
        {
            List<Bitmap> list = new List<Bitmap>();
            list.Add(Bitmap.CreateBitmap(mBitmap));
            list.Add(Bitmap.CreateBitmap(bitmap));
            curlayersList.Add(list);

            List<Dictionary<OperateAction, Java.Lang.Object>> tempList = new List<Dictionary<OperateAction, Java.Lang.Object>>();
            curOperateList.Add(tempList);
            List<Dictionary<OperateAction, Java.Lang.Object>> tempList1 = new List<Dictionary<OperateAction, Java.Lang.Object>>();
            curOperateList.Add(tempList1);
            curHPosition.row = curlayersList.Count - 1;
            curHPosition.column = 0;

            updateBitmap();
        }

        public void SetPosition(int position) {
            mIndex = position;
            curlayersList = layersList[mIndex];
            curOperateList = operateList[mIndex];
            curHPosition = positionList[mIndex];
            updateBitmap();
        }

        public void OperateImg(OperateAction action)
        {
            
        }

        private int measure(int measureSpec)
        {
            MeasureSpecMode specMode = MeasureSpec.GetMode(measureSpec);
            int specSize = MeasureSpec.GetSize(measureSpec);
            //设置一个默认值，就是这个View的默认宽度为500，这个看我们自定义View的要求
            int result = 500;
            if (specMode ==  MeasureSpecMode.AtMost)
            {//相当于我们设置为wrap_content
                result = specSize;
            }
            else if (specMode == MeasureSpecMode.Exactly)
            {//相当于我们设置为match_parent或者为一个具体的值
                result = specSize;
            }
            LogUtil.CustomLog("measure=" + result);
            return result;
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
            mWidth = measure(widthMeasureSpec);
            mHeight = measure(heightMeasureSpec);
            SetMeasuredDimension(mWidth, mHeight);
        }

        public override void Draw(Canvas canvas)
        {
            base.Draw(canvas);

            //mPaint.AntiAlias = true;
            //mPaint.SetStyle(Paint.Style.Fill);
            //mPaint.SetXfermode(null);
            //mPaint.SetARGB(200, 255, 0, 0);
            //Rect rect = new Rect(0, 0, mWidth, mHeight);
            //Bitmap bitmap = Bitmap.CreateBitmap(mWidth, mHeight, Bitmap.Config.Argb8888);
            //Canvas c = new Canvas(bitmap);
            //c.DrawRect(rect, mPaint);
            ////c.Save();
            
            ////c.ClipRect(new Rect(mWidth / 4, mHeight / 4, 3 * mWidth / 4, 3 * mHeight / 4));
            
            //mPaint.SetARGB(255, 255, 255, 0);
            //mPaint.SetStyle(Paint.Style.Fill);

            //Rect r = new Rect(mWidth / 4, mHeight / 4, 3 * mWidth / 4, 3 * mHeight / 4);
            //for (int i = r.Left; i < r.Right; i++)
            //{
            //    for (int j = r.Top; j < r.Bottom; j++)
            //    {
            //        bitmap.SetPixel(i, j, Color.Transparent);
            //    }
            //}
            //c.Save();
            //c.Rotate(10);
            //c.DrawRect(r, mPaint);


            //mPaint.SetARGB(255, 0,0,0);
            //mPaint.SetStyle(Paint.Style.Stroke);
            //mPaint.StrokeWidth = 4;
            //mPaint.SetPathEffect(new DashPathEffect(new float[] { 15, 15 }, 0));
            //c.DrawRect(r, mPaint);
            //c.Restore();
                
       
            //canvas.DrawBitmap(bitmap, rect, rect, mPaint);
            //Bitmap.createBitmap(w, h, Bitmap.Config.ARGB_8888);
        }

        private void updateBitmap() {
            if (mBitmap!=null)
            {
                mBitmap.Recycle();
            }

            List<Bitmap> list = curlayersList[curHPosition.row];
            List<Dictionary<OperateAction, Java.Lang.Object>> operateList = curOperateList[curHPosition.row];

            int count = 0;
            for (int i = 0; i < list.Count; i++)
            {
                Bitmap bitmap = Bitmap.CreateBitmap(list[i]);
                Dictionary<OperateAction, Java.Lang.Object> map = operateList[i];

                mBitmap = Bitmap.CreateBitmap(mWidth, mHeight, Bitmap.Config.Argb8888);
            }
            



            Invalidate();
        }

        //private void touch_start(float x, float y)
        //{
        //    mPath.reset();
        //    mPath.moveTo(x, y);
        //    mX = x;
        //    mY = y;
        //    //如果是“画笔”模式就用mPaint画笔进行绘制
        //    if (mMode == Pen)
        //    {
        //        mCanvas.drawPath(mPath, mPaint);
        //    }
        //    //如果是“橡皮擦”模式就用mEraserPaint画笔进行绘制
        //    if (mMode == Eraser)
        //    {
        //        mCanvas.drawPath(mPath, mEraserPaint);
        //    }

        //}

        //private void touch_move(float x, float y)
        //{
        //    float dx = Math.abs(x - mX);
        //    float dy = Math.abs(y - mY);
        //    if (dx >= TOUCH_TOLERANCE || dy >= TOUCH_TOLERANCE)
        //    {
        //        mPath.quadTo(mX, mY, (x + mX) / 2, (y + mY) / 2);
        //        mX = x;
        //        mY = y;
        //        if (mMode == Pen)
        //        {
        //            mCanvas.drawPath(mPath, mPaint);
        //        }
        //        if (mMode == Eraser)
        //        {
        //            mCanvas.drawPath(mPath, mEraserPaint);
        //        }
        //    }
        //}


        //private void touch_up()
        //{
        //    mPath.lineTo(mX, mY);
        //    if (mMode == Pen)
        //    {
        //        mCanvas.drawPath(mPath, mPaint);
        //    }
        //    if (mMode == Eraser)
        //    {
        //        mCanvas.drawPath(mPath, mEraserPaint);
        //    }
        //}


        public override bool OnTouchEvent(MotionEvent e)
        {
            switch (e.Action)
            {
                case MotionEventActions.Down:
                    break;
                case MotionEventActions.Move:
                    
                    break;
                case MotionEventActions.Up:
                   
                    break;
            }
            return base.OnTouchEvent(e);
        }


    }
}
