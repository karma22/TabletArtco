
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace TabletArtco
{
    //[Activity(Theme = "@style/AppTheme", MainLauncher = true)]
    [Activity(Label = "TestActivity")]
    public class TestActivity : Activity, Delegate, DataSource
    {
        //protected override void OnCreate(Bundle savedInstanceState)
        //{
        //    base.OnCreate(savedInstanceState);
        //    Xamarin.Essentials.Platform.Init(this, savedInstanceState);
        //    Window.SetFlags(Android.Views.WindowManagerFlags.Fullscreen, Android.Views.WindowManagerFlags.Fullscreen);
        //    SetContentView(Resource.Layout.activity_test);
        //    RequestedOrientation = Android.Content.PM.ScreenOrientation.Landscape;

        //}
        private List<Bitmap> originList = new List<Bitmap>();
        private List<Bitmap> operateList = new List<Bitmap>();
        private int mIndex = -1;
        private CutView mCustomView;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            Window.SetFlags(Android.Views.WindowManagerFlags.Fullscreen, Android.Views.WindowManagerFlags.Fullscreen);
            SetContentView(Resource.Layout.activity_test);
            RequestedOrientation = Android.Content.PM.ScreenOrientation.Landscape;
            initData();
            InitView();
        }

        // image List
        private void initData()
        {
            Intent intent = this.Intent;
            Bundle bundle = intent.GetBundleExtra("bundle");
            if (bundle != null)
            {
                int position = bundle.GetInt("position");
                if (position < Project.mSprites.Count)
                {
                    ActivatedSprite sprite = Project.mSprites[position];
                    for (int i = 0; i < sprite.originBitmapList.Count; i++)
                    {
                        Bitmap bitmap = Bitmap.CreateBitmap(sprite.originBitmapList[i]);
                        originList.Add(bitmap);
                        operateList.Add(Bitmap.CreateBitmap(bitmap));
                    }
                    if (sprite.originBitmapList.Count > 0)
                    {
                        mIndex = 0;
                    }
                }
            }

        }

        // init view
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
            RelativeLayout areaView = FindViewById<RelativeLayout>(Resource.Id.edit_area_view);
            FrameLayout.LayoutParams areaParams = (FrameLayout.LayoutParams)areaView.LayoutParameters;
            areaParams.Height = wrapperH - colorMargin;
            areaParams.Width = areaW - (colorMargin * 2);
            areaParams.TopMargin = topMargin + colorMargin;
            areaParams.LeftMargin = topMargin + colorMargin;
            areaView.LayoutParameters = areaParams;
            areaView.SetBackgroundResource(Resource.Drawable.xml_edit_bg);

            mCustomView = new CutView(this);
            mCustomView.SetBackgroundColor(Color.ParseColor("#80ff0000"));
            mCustomView.Click += (t, e) =>
            {

            };
            //mCustomView.mAction = (list) =>
            //{

            //};
            //mCustomView.AddBitmap(operateList[0]);
            //mCustomView.Rotation = 30;
            Matrix matrix = new Matrix();
            //matrix.SetRotate(10);
            mCustomView.SetScaleType(ImageView.ScaleType.Matrix);
            mCustomView.ImageMatrix = matrix;


            //mCustomView.ImageMatrix = 
            RelativeLayout.LayoutParams conParams = new RelativeLayout.LayoutParams(areaParams.Width, areaParams.Height);
            conParams.AddRule(LayoutRules.CenterInParent);
            areaView.AddView(mCustomView, conParams);


            //ImageView imageIv = new ImageView(this);
            //RelativeLayout.LayoutParams conParams1 = new RelativeLayout.LayoutParams(300, 300);
            //conParams.AddRule(LayoutRules.CenterInParent);
            //areaView.AddView(mCustomView, conParams);

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
                imgBt.Tag = i;
                contentView.AddView(imgBt);
                imgBt.Click += (t, e) =>
                {
                    int tag = (int)((ImageView)t).Tag;
                    if (tag < 8)
                    {
                        OperateType[] types = { OperateType.FlipY, OperateType.FlipX, OperateType.RotateR, OperateType.RotateL, OperateType.Enlarge, OperateType.Narrow, OperateType.Eraser, OperateType.Draw };
                        //mCustomView.Operate(types[tag]);
                    }
                };
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

    class CutView : ImageView
    {
        private Context mContext;

        private int mWidth;
        private int mHeight;

        private Bitmap curBitmap;
        private Canvas curCanvas;
        private Color curColor = Color.Black;
        private int curStrokeW = 10;
        private Paint mPaint;
        private Paint mErarsePaint;
        public Action<List<Bitmap>> mAction { get; set; }


        public static List<EditTarget> targetList = new List<EditTarget>();
        public static List<Bitmap> currentList = new List<Bitmap>();
        private int mIndex;
        private OperateType operateType;
        private Path mPath = new Path();

        public CutView(Context context) : base(context)
        {
            Initialize(context);
        }

        public CutView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            Initialize(context);
        }

        public CutView(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle)
        {
            Initialize(context);
        }

        private void Initialize(Context context)
        {
            mContext = context;
            mPaint = new Paint();
            mPaint.AntiAlias = true;
            mPaint.SetStyle(Paint.Style.Stroke);

            mErarsePaint = new Paint();
            mErarsePaint.AntiAlias = true;
            mErarsePaint.Alpha = 0;
            mErarsePaint.StrokeWidth = curStrokeW;
            mErarsePaint.SetXfermode(new PorterDuffXfermode(PorterDuff.Mode.DstIn));
            mErarsePaint.SetStyle(Paint.Style.Fill);

            SetImageBitmap(drawBitmap());
        }

        private int measure(int measureSpec)
        {
            MeasureSpecMode specMode = MeasureSpec.GetMode(measureSpec);
            int specSize = MeasureSpec.GetSize(measureSpec);
            //设置一个默认值，就是这个View的默认宽度为500，这个看我们自定义View的要求
            int result = 500;
            if (specMode == MeasureSpecMode.AtMost)
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
            //if (curBitmap != null)
            //{
            //    mWidth = curBitmap.Width;
            //    mHeight = curBitmap.Height;
            //}

            mWidth = 600;
            mHeight = 600;
            SetMeasuredDimension(mWidth, mHeight);
        }

        //public override void Draw(Canvas canvas)
        //{
        //    base.Draw(canvas);
            //canvas.DrawBitmap(bitmap1, null, new Rect(0, 0, 300, 300), mPaint);


            //canvas.Restore();


            //canvas.DrawRect(new Rect(0, 0, 300, 300), mErarsePaint);

            //if (curBitmap != null)  
            //{
            //    RectF r = new RectF(0, 0, curBitmap.Width, curBitmap.Height);
            //    if (curCanvas != null && !mPath.IsEmpty)
            //    {
            //        mPaint.StrokeWidth = curStrokeW;
            //        mErarsePaint.StrokeWidth = curStrokeW;
            //        curCanvas.DrawPath(mPath, operateType == OperateType.Draw ? mPaint : mErarsePaint);
            //    }
            //    canvas.DrawBitmap(curBitmap, null, r, mPaint);

            //    mPaint.Alpha = 1;
            //    mPaint.SetARGB(255, 0, 0, 0);
            //    mPaint.StrokeWidth = 6;
            //    mPaint.SetXfermode(null);
            //    canvas.DrawRect(new Rect(0, 0, curBitmap.Width, curBitmap.Height), mPaint);
            //}


        //}

        private Bitmap drawBitmap() {
            Bitmap bitmap = Bitmap.CreateBitmap(300, 300, Bitmap.Config.Argb8888);
            Canvas c = new Canvas(bitmap);
            mPaint.SetXfermode(null);
            mPaint.SetARGB(255, 255, 0, 0);
            mPaint.SetStyle(Paint.Style.Fill);
            c.DrawRect(new Rect(0, 0, 300, 300), mPaint);

            PorterDuff.Mode[] mode = {
                                        PorterDuff.Mode.Dst, PorterDuff.Mode.DstAtop , PorterDuff.Mode.DstIn , PorterDuff.Mode.DstOut, PorterDuff.Mode.DstOver,
                                        PorterDuff.Mode.Src , PorterDuff.Mode.SrcAtop , PorterDuff.Mode.SrcIn,PorterDuff.Mode.SrcOut, PorterDuff.Mode.SrcOver ,
                                        PorterDuff.Mode.Clear, PorterDuff.Mode.Add , PorterDuff.Mode.Xor , PorterDuff.Mode.Darken, PorterDuff.Mode.Lighten,
                                        PorterDuff.Mode.Screen, PorterDuff.Mode.Multiply
                                    };
            int originX = 10;
            int originY = 10;
            int margin = 10;
            int w = (300 - originX * 2 - margin * 4) / 5;
            for (int i = 0; i < mode.Length; i++)
            {
                Rect rect = new Rect(originX + (w + margin) * (i % 5), originY + (w + margin) * (i / 5), originX + (w + margin) * (i % 5) + w, originY + (w + margin) * (i / 5) + w);
                mPaint.SetARGB(255, 255, 255, 0);
                mPaint.SetStyle(Paint.Style.Fill);
                mPaint.SetXfermode(new PorterDuffXfermode(mode[i]));
                c.DrawRect(rect, mPaint);
            }

            //Bitmap bitmap1 = Bitmap.CreateBitmap(300, 300, Bitmap.Config.Argb8888);
            //Canvas c1 = new Canvas(bitmap1);
            //Path p = new Path();
            //p.AddCircle(100, 100, 50, Path.Direction.Cw);
            //Path p1 = new Path();
            //p1.AddRect(new RectF(0, 0, 300, 300), Path.Direction.Cw);



            ////c1.ClipOutPath(p);
            //mPaint.SetXfermode(null);
            //c1.DrawBitmap(bitmap, null, new Rect(0, 0, 300, 300), mPaint);
            //Matrix m = new Matrix();
            //RectF rectF = new RectF();
            //p.ComputeBounds(rectF, false);
            //LogUtil.CustomLog("ComputeBounds=" + rectF.ToString());
            //mPaint.SetXfermode(null);
            return bitmap;
        }

        

        private float mX;
        private float mY;
        float TOUCH_TOLERANCE = 4;
        private void touch_start(float x, float y)
        {
            mPath.Reset();
            mPath.MoveTo(x, y);
            mX = x;
            mY = y;
        }

        private void touch_move(float x, float y)
        {
            float dx = Math.Abs(x - mX);
            float dy = Math.Abs(y - mY);
            if (dx >= TOUCH_TOLERANCE || dy >= TOUCH_TOLERANCE)
            {
                mPath.LineTo(x, y);
                Invalidate();
            }
        }

        private void touch_up(float x, float y)
        {
            mPath.LineTo(x, y); EditTarget target = targetList[mIndex];
            target.Draw(operateType, mPath, "#000000", curStrokeW);
            mPath.Reset();
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            if (operateType == OperateType.Draw || operateType == OperateType.Eraser)
            {
                switch (e.Action)
                {
                    case MotionEventActions.Down:
                        touch_start(e.GetX(), e.GetY());
                        break;
                    case MotionEventActions.Move:
                        touch_move(e.GetX(), e.GetY());
                        break;
                    case MotionEventActions.Up:
                        touch_up(e.GetX(), e.GetY());
                        break;
                }
            }

            return true;
        }
    }

    public class BitmapUtil {

        public class BitmapTarget {
            Bitmap bitmap;

        }

        enum OperateType { FlipY, FlipX, RotateR, RotateL, Enlarge, Narrow, Eraser, Draw, Clip, Move };



        //struct HPosition
        //{
        //    public int row;
        //    public int column;
        //    public HPosition(int row, int column)
        //    {
        //        this.row = row;
        //        this.column = column;
        //    }
        //}

        private List<Bitmap> list = new List<Bitmap>();
        private float originscale = 2;
        private float scale = 1;




    }
}



    //enum OperateType { FlipY, FlipX, RotateR, RotateL, Enlarge, Narrow, Eraser, Draw, Clip, Move };

    //struct HPosition
    //{
    //    public int row;
    //    public int column;
    //    public HPosition(int row, int column)
    //    {
    //        this.row = row;
    //        this.column = column;
    //    }
    //}

    //// Edit step
    //class EditStep
    //{
    //    public OperateType operateType;

    //    public float scale;
    //    public float movePoint;

    //    public float rotate;

    //    public Path path;
    //    public string color;
    //    public int strokewidth;

    //    public EditStep(OperateType type)
    //    {
    //        operateType = type;
    //    }

    //    public EditStep(OperateType type, Path p, string c, int w)
    //    {
    //        operateType = type;
    //        color = c;
    //        path = p;
    //        strokewidth = w;
    //    }
    //}

    //class EditLayer
    //{
    //    private List<Bitmap> bitmaplist;
    //    private List<List<EditStep>> stepList;
    //    private int bitmapIndex = 0;
    //    private int stepIndex = 0;
    //    private Paint eraserPaint;
    //    private Paint drawPaint;

    //    public EditLayer(Bitmap bitmap)
    //    {
    //        eraserPaint = new Paint();
    //        eraserPaint.SetStyle(Paint.Style.Stroke);
    //        eraserPaint.AntiAlias = true;
    //        eraserPaint.Alpha = 0;
    //        eraserPaint.SetXfermode(new PorterDuffXfermode(PorterDuff.Mode.DstIn));

    //        drawPaint = new Paint();
    //        drawPaint.AntiAlias = true;
    //        drawPaint.SetStyle(Paint.Style.Stroke);

    //        bitmaplist = new List<Bitmap>();
    //        bitmaplist.Add(bitmap);
    //        stepList = new List<List<EditStep>>();
    //        stepList.Add(new List<EditStep>());
    //    }

    //    public Bitmap currentBitmap()
    //    {
    //        Bitmap bitmap = Bitmap.CreateBitmap(bitmaplist[0]);
    //        //Canvas canvas = new Canvas(bitmap);
    //        for (int i = 0; i < bitmapIndex + 1; i++)
    //        {
    //            List<EditStep> list = stepList[i];
    //            int count = i == bitmapIndex - 1 ? stepIndex : list.Count;
    //            Matrix matrix = new Matrix();
    //            for (int j = 0; j < count; j++)
    //            {
    //                EditStep step = list[j];
    //                switch (step.operateType)
    //                {
    //                    case OperateType.FlipY:
    //                        {
    //                            matrix.PreScale(-1, 1);
    //                            break;
    //                        }
    //                    case OperateType.FlipX:
    //                        {
    //                            matrix.PreScale(1, -1);
    //                            break;
    //                        }
    //                    case OperateType.RotateR:
    //                        {
    //                            matrix.PreRotate(90);
    //                            break;
    //                        }
    //                    case OperateType.RotateL:
    //                        {
    //                            matrix.PreRotate(-90);
    //                            break;
    //                        }
    //                    case OperateType.Enlarge:
    //                        {
    //                            matrix.PreScale(1.1f, 1.1f);
    //                            break;
    //                        }
    //                    case OperateType.Narrow:
    //                        {
    //                            matrix.PreScale(0.9f, 0.9f);
    //                            break;
    //                        }
    //                    case OperateType.Eraser:
    //                        {
    //                            Bitmap newB = Bitmap.CreateBitmap(bitmap, 0, 0, bitmap.Width, bitmap.Height, matrix, false);
    //                            bitmap.Recycle();
    //                            bitmap = newB;
    //                            matrix.Reset();

    //                            eraserPaint.StrokeWidth = step.strokewidth;
    //                            eraserPaint.Color = Color.ParseColor(step.color);
    //                            Canvas canvas = new Canvas(bitmap);
    //                            canvas.DrawPath(step.path, eraserPaint);

    //                            Bitmap temp = Bitmap.CreateBitmap(bitmap.Width, bitmap.Height, Bitmap.Config.Argb8888);
    //                            Canvas c = new Canvas(temp);
    //                            c.DrawBitmap(bitmap, null, new Rect(0, 0, bitmap.Width, bitmap.Height), drawPaint);
    //                            //c.DrawPath(step.path, eraserPaint);
    //                            bitmap.Recycle();
    //                            bitmap = temp;

    //                            break;
    //                        }
    //                    case OperateType.Draw:
    //                        {
    //                            Bitmap newB = Bitmap.CreateBitmap(bitmap, 0, 0, bitmap.Width, bitmap.Height, matrix, false);
    //                            bitmap.Recycle();
    //                            bitmap = newB;

    //                            matrix.Reset();
    //                            drawPaint.StrokeWidth = step.strokewidth;
    //                            drawPaint.Color = Color.ParseColor(step.color);
    //                            Canvas canvas = new Canvas(bitmap);
    //                            canvas.DrawPath(step.path, drawPaint);
    //                            break;
    //                        }

    //                    default:
    //                        break;
    //                }
    //                //Bitmap newBitmp = Bitmap.CreateBitmap(bitmap.Width, bitmap.Height, Bitmap.Config.Argb8888);


    //                Bitmap newBM = Bitmap.CreateBitmap(bitmap, 0, 0, bitmap.Width, bitmap.Height, matrix, false);
    //                bitmap.Recycle();
    //                bitmap = newBM;
    //                matrix.Reset();
    //            }

    //            //canvas = new Canvas(bitmap);
    //            //enum OperateType { FlipX, FlipY, RotateR, RotateL, Enlarge, Narrow, Eraser, Draw, Clip, Move };
    //        }
    //        return bitmap;
    //    }

    //    public void Operate(OperateType type)
    //    {
    //        List<EditStep> list = stepList[bitmapIndex];
    //        list.Add(new EditStep(type));
    //        stepIndex += 1;
    //    }

    //    public void Draw(OperateType type, Path path, string color, int width)
    //    {
    //        List<EditStep> list = stepList[bitmapIndex];
    //        list.Add(new EditStep(type, path, color, width));
    //        stepIndex += 1;
    //    }
    //}

    //class EditTarget
    //{
    //    private List<EditLayer> layerlist;
    //    private int layerIndex = 0;

    //    public EditTarget(Bitmap bitmap)
    //    {
    //        EditLayer editLayer = new EditLayer(bitmap);
    //        layerlist = new List<EditLayer>();
    //        layerlist.Add(editLayer);
    //    }

    //    // 获取当前图片
    //    public Bitmap currentBitmap()
    //    {
    //        EditLayer editLayer = layerlist[layerIndex];
    //        return editLayer.currentBitmap();
    //    }

    //    // 添加操作行为，只限FlipX, FlipY, RotateR, RotateL, Enlarge, Narrow
    //    public void Operate(OperateType type)
    //    {
    //        EditLayer editLayer = layerlist[layerIndex];
    //        editLayer.Operate(type);
    //    }

    //    public void Draw(OperateType type, Path path, string color, int width)
    //    {
    //        EditLayer editLayer = layerlist[layerIndex];
    //        editLayer.Draw(type, new Path(path), color, width);
    //    }

    //    // 裁剪
    //    public void Clip(Rect rect)
    //    {

    //    }

    //    // 移动
    //    public void Move(int x, int y)
    //    {

    //    }

    //    // 前进一步
    //    public void nextStep()
    //    {

    //    }

    //    // 后退一步
    //    public void previousStep()
    //    {

    //    }

    //    // 加入图片
    //    public void insertBitmap(Bitmap bitmap)
    //    {

    //    }

    //}

    //// draw image view
    //class CustomView : ImageView
    //{
    //    private Context mContext;

    //    private int mWidth;
    //    private int mHeight;

    //    private Bitmap curBitmap;
    //    private Canvas curCanvas;
    //    private Color curColor = Color.Black;
    //    private int curStrokeW = 10;
    //    private Paint mPaint;
    //    private Paint mErarsePaint;
    //    public Action<List<Bitmap>> mAction { get; set; }


    //    public static List<EditTarget> targetList = new List<EditTarget>();
    //    public static List<Bitmap> currentList = new List<Bitmap>();
    //    private int mIndex;
    //    private OperateType operateType;
    //    private Path mPath = new Path();

    //    public CustomView(Context context) : base(context)
    //    {
    //        Initialize(context);
    //    }

    //    public CustomView(Context context, IAttributeSet attrs) : base(context, attrs)
    //    {
    //        Initialize(context);
    //    }

    //    public CustomView(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle)
    //    {
    //        Initialize(context);
    //    }

    //    private void Initialize(Context context)
    //    {
    //        mContext = context;
    //        mPaint = new Paint();
    //        mPaint.AntiAlias = true;
    //        mPaint.SetStyle(Paint.Style.Stroke);

    //        mErarsePaint = new Paint();
    //        mErarsePaint.AntiAlias = true;
    //        mErarsePaint.Alpha = 0;
    //        mErarsePaint.StrokeWidth = curStrokeW;
    //        mErarsePaint.SetXfermode(new PorterDuffXfermode(PorterDuff.Mode.DstIn));
    //        mErarsePaint.SetStyle(Paint.Style.Stroke);
    //    }

    //    // 添加操作图片
    //    public void AddBitmap(Bitmap bitmap)
    //    {
    //        EditTarget target = new EditTarget(bitmap);
    //        targetList.Add(target);
    //        updateView();
    //    }
    //    // 删除照片
    //    public void DeleteBitmap(int position)
    //    {
    //        if (position == mIndex)
    //        {
    //            mIndex = position - 1;
    //        }
    //        updateView();
    //    }

    //    public void SetPosition(int position)
    //    {
    //        mIndex = position;
    //        updateView();
    //    }

    //    // 当前图片插入图片
    //    public void InsertBitmap(Bitmap bitmap)
    //    {
    //        if (targetList.Count == 0)
    //        {
    //            return;
    //        }
    //        targetList[mIndex].insertBitmap(bitmap);
    //        //Invalidate();
    //        updateView();
    //    }

    //    public void Operate(OperateType type)
    //    {
    //        switch (type)
    //        {
    //            case OperateType.FlipX:
    //            case OperateType.FlipY:
    //            case OperateType.RotateR:
    //            case OperateType.RotateL:
    //            case OperateType.Enlarge:
    //            case OperateType.Narrow:
    //                if (mIndex < targetList.Count)
    //                {
    //                    targetList[mIndex].Operate(type);
    //                }
    //                break;
    //            default:
    //                break;
    //        }
    //        operateType = type;
    //        updateView();
    //    }

    //    private int measure(int measureSpec)
    //    {
    //        MeasureSpecMode specMode = MeasureSpec.GetMode(measureSpec);
    //        int specSize = MeasureSpec.GetSize(measureSpec);
    //        //设置一个默认值，就是这个View的默认宽度为500，这个看我们自定义View的要求
    //        int result = 500;
    //        if (specMode == MeasureSpecMode.AtMost)
    //        {//相当于我们设置为wrap_content
    //            result = specSize;
    //        }
    //        else if (specMode == MeasureSpecMode.Exactly)
    //        {//相当于我们设置为match_parent或者为一个具体的值
    //            result = specSize;
    //        }
    //        LogUtil.CustomLog("measure=" + result);
    //        return result;
    //    }

    //    protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
    //    {
    //        base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
    //        mWidth = measure(widthMeasureSpec);
    //        mHeight = measure(heightMeasureSpec);
    //        if (curBitmap != null)
    //        {
    //            mWidth = curBitmap.Width;
    //            mHeight = curBitmap.Height;
    //        }
    //        SetMeasuredDimension(mWidth, mHeight);
    //    }

    //    public void updateView()
    //    {
    //        if (mIndex < targetList.Count)
    //        {
    //            if (curBitmap != null)
    //            {
    //                curBitmap.Recycle();
    //            }
    //            EditTarget target = targetList[mIndex];
    //            curBitmap = target.currentBitmap();
    //            RequestLayout();
    //            //SetImageBitmap(curBitmap);
    //            curCanvas = new Canvas(curBitmap);
    //            Invalidate();
    //        }
    //    }

    //    public override void Draw(Canvas canvas)
    //    {
    //        if (curBitmap != null)
    //        {
    //            RectF r = new RectF(0, 0, curBitmap.Width, curBitmap.Height);
    //            if (curCanvas != null && !mPath.IsEmpty)
    //            {
    //                mPaint.StrokeWidth = curStrokeW;
    //                mErarsePaint.StrokeWidth = curStrokeW;
    //                curCanvas.DrawPath(mPath, operateType == OperateType.Draw ? mPaint : mErarsePaint);
    //            }
    //            canvas.DrawBitmap(curBitmap, null, r, mPaint);

    //            mPaint.Alpha = 1;
    //            mPaint.SetARGB(255, 0, 0, 0);
    //            mPaint.StrokeWidth = 6;
    //            mPaint.SetXfermode(null);
    //            canvas.DrawRect(new Rect(0, 0, curBitmap.Width, curBitmap.Height), mPaint);

    //        }
    //    }

    //    public override bool OnTouchEvent(MotionEvent e)
    //    {
    //        if (operateType == OperateType.Draw || operateType == OperateType.Eraser)
    //        {
    //            switch (e.Action)
    //            {
    //                case MotionEventActions.Down:
    //                    touch_start(e.GetX(), e.GetY());
    //                    break;
    //                case MotionEventActions.Move:
    //                    touch_move(e.GetX(), e.GetY());
    //                    break;
    //                case MotionEventActions.Up:
    //                    touch_up(e.GetX(), e.GetY());
    //                    break;
    //            }
    //        }

    //        return true;
    //    }

    //    private float mX;
    //    private float mY;
    //    float TOUCH_TOLERANCE = 4;

    //    private void touch_start(float x, float y)
    //    {
    //        mPath.Reset();
    //        mPath.MoveTo(x, y);
    //        mX = x;
    //        mY = y;
    //    }

    //    private void touch_move(float x, float y)
    //    {
    //        float dx = Math.Abs(x - mX);
    //        float dy = Math.Abs(y - mY);
    //        if (dx >= TOUCH_TOLERANCE || dy >= TOUCH_TOLERANCE)
    //        {
    //            mPath.LineTo(x, y);
    //            Invalidate();
    //        }
    //    }

    //    private void touch_up(float x, float y)
    //    {
    //        mPath.LineTo(x, y); EditTarget target = targetList[mIndex];
    //        target.Draw(operateType, mPath, "#000000", curStrokeW);
    //        mPath.Reset();
    //        updateView();

    //    }
//}



//    public class CutoutView extends ImageView
//    {

//    private Paint mPaint;
//    private Path path;
//    private float startX, startY, stopX, stopY;
//    private Bitmap maskBitmap;
//    private List<PointF> pointFList;
//    private Matrix mMatrix = new Matrix();
//    private float[] points;

//    public CutoutView(Context context, AttributeSet attrs, int defStyleAttr)
//    {
//        super(context, attrs, defStyleAttr);
//        initPaint();
//        initPath();
//        pointFList = new ArrayList<>();
//        this.setLayerType(LAYER_TYPE_SOFTWARE, mPaint);
//    }

//    public CutoutView(Context context, AttributeSet attrs)
//    {
//        this(context, attrs, 0);
//    }

//    public CutoutView(Context context)
//    {
//        this(context, null);
//    }

//    @Override
//    protected void onDraw(Canvas canvas)
//    {
//        super.onDraw(canvas);
//        canvas.setDrawFilter(new PaintFlagsDrawFilter(0, Paint.ANTI_ALIAS_FLAG | Paint.FILTER_BITMAP_FLAG));
//        canvas.drawPath(path, mPaint);
//        //        mPaint.setXfermode(new PorterDuffXfermode(PorterDuff.Mode.DST_OUT));
//        //        canvas.drawBitmap(maskBitmap, mMatrix, mPaint);
//    }

//    //    public void setMaskBitmap(Bitmap bitmap) {
//    //        Bitmap b = Bitmap.createBitmap(bitmap.getWidth() + 10, bitmap.getHeight() + 10, Bitmap.Config.ARGB_8888);
//    //        Canvas maskCanvas = new Canvas(b);
//    //        maskCanvas.setDrawFilter(new PaintFlagsDrawFilter(0, Paint.ANTI_ALIAS_FLAG | Paint.FILTER_BITMAP_FLAG));
//    //        points = new float[]{0, 0, bitmap.getWidth() + 10, 0, bitmap.getHeight() + 10, bitmap.getWidth() + 10, bitmap.getHeight() + 10};
//    //        mMatrix.mapPoints(points);
//    //        maskCanvas.drawBitmap(bitmap, 5, 5, null);
//    //        this.maskBitmap = b;
//    //        invalidate();
//    //    }

//    private void initPaint()
//    {
//        mPaint = new Paint();
//        mPaint.setAntiAlias(true);
//        mPaint.setDither(true);
//        mPaint.setStyle(Paint.Style.STROKE);
//        mPaint.setStrokeWidth(3.0f);
//        mPaint.setColor(Color.YELLOW);
//    }

//    private void initPath()
//    {
//        path = new Path();
//    }

//    @Override
//    public boolean onTouchEvent(MotionEvent event)
//    {
//        switch (event.getAction()) {
//            case MotionEvent.ACTION_DOWN:
//                touchDown(event);
//            break;
//            case MotionEvent.ACTION_MOVE:
//                touchMove(event);
//            break;
//            case MotionEvent.ACTION_UP:
//                stopX = event.getX();
//            stopY = event.getY();
//            mMatrix.setTranslate(stopX - maskBitmap.getWidth() / 2 - 5,
//                    stopY - maskBitmap.getHeight() / 2 - 5);
//            break;
//        }
//        invalidate();
//        return true;
//    }

//    private void touchDown(MotionEvent event)
//    {
//        path.reset();
//        pointFList.clear();
//        startX = event.getX();
//        startY = event.getY();
//        updatePoints(startX, startY);
//        path.moveTo(startX, startY);

//    }

//    private void updatePoints(float x, float y)
//    {

//        float pts[] = new float[2];
//        pts[0] = x;
//        pts[1] = y;
//        Matrix matrix = getImageMatrix();
//        matrix.mapPoints(pts);
//        PointF pointF = new PointF();
//        pointF.set(pts[0], pts[1]);
//        pointFList.add(pointF);
//    }

//    private void touchMove(MotionEvent event)
//    {
//        final float x = event.getX();
//        final float y = event.getY();

//        final float previousX = startX;
//        final float previousY = startY;

//        final float dx = Math.abs(x - previousX);
//        final float dy = Math.abs(y - previousY);

//        //两点之间的距离大于等于3时，生成贝塞尔绘制曲线
//        if (dx >= 3 || dy >= 3)
//        {
//            //设置贝塞尔曲线的操作点为起点和终点的一半
//            stopX = (x + previousX) / 2;
//            stopY = (y + previousY) / 2;

//            new Handler().post(new Runnable() {
//                @Override
//                public void run()
//            {
//                updatePoints(stopX, stopY);
//                mMatrix.setTranslate(stopX - maskBitmap.getWidth() / 2 - 5,
//                        stopY - maskBitmap.getHeight() / 2 - 5);
//            }
//        });

//        //二次贝塞尔，实现平滑曲线；previousX, previousY为操作点，cX, cY为终点
//        path.quadTo(previousX, previousY, stopX, stopY);

//        //第二次执行时，第一次结束调用的坐标值将作为第二次调用的初始坐标值
//        startX = x;
//        startY = y;
//    }
//}

//public Bitmap drawCutoutCanvas(Bitmap bitmap, Matrix matrix)
//{
//    Paint paint = new Paint();
//    paint.setAntiAlias(true);
//    paint.setDither(true);
//    paint.setStyle(Paint.Style.FILL);
//    paint.setStrokeWidth(3.0f);
//    paint.setColor(Color.YELLOW);
//    paint.setMaskFilter(new BlurMaskFilter(10, BlurMaskFilter.Blur.NORMAL));
//    bitmap = bitmap.copy(Bitmap.Config.ARGB_8888, true);
//    Bitmap newBitmap = null;
//    try
//    {
//        newBitmap = Bitmap.createBitmap(getWidth(), getHeight(), Bitmap.Config.ARGB_8888);
//    }
//    catch (OutOfMemoryError e)
//    {
//        newBitmap = Bitmap.createBitmap(getWidth(), getHeight(), Bitmap.Config.ARGB_4444);
//    }
//    if (pointFList.size() > 0)
//    {
//        Canvas mCanvas = new Canvas(newBitmap);
//        int sc = mCanvas.saveLayer(0, 0, getWidth(), getHeight(),
//                null, Canvas.ALL_SAVE_FLAG);
//        mCanvas.setDrawFilter(new PaintFlagsDrawFilter(0, Paint.ANTI_ALIAS_FLAG | Paint.FILTER_BITMAP_FLAG));
//        path.reset();
//        path.moveTo(pointFList.get(0).x, pointFList.get(0).y);
//        for (int i = 1; i < pointFList.size(); i++)
//        {
//            path.quadTo(pointFList.get(i - 1).x, pointFList.get(i - 1).y, pointFList.get(i).x, pointFList.get(i).y);
//            mCanvas.drawPath(path, paint);
//        }
//        //            mCanvas.drawBitmap(maskBitmap, mMatrix, paint);
//        //            paint.setXfermode(new PorterDuffXfermode(PorterDuff.Mode.SRC_IN));
//        mCanvas.drawBitmap(bitmap, 0, 0, paint);
//        mCanvas.drawBitmap(bitmap, matrix, paint);
//        mCanvas.restoreToCount(sc);
//    }
//    return newBitmap;
//}
//}






