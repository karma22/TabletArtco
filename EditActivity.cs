
using System;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using System.Collections.Generic;
using Android.Content;
using Android.Util;

namespace TabletArtco
{
    //[Activity(Label = "EditActivity", MainLauncher = true)]

    // Edit Image Activity
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
            initData();
            InitView();
        }

        // image List
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

            mCustomView = new CustomView(this);
            mCustomView.mAction = (list) =>
            {

            };
            mCustomView.SetSrcBitmap(operateList[0]);
            RelativeLayout.LayoutParams conParams = new RelativeLayout.LayoutParams(areaParams.Width, areaParams.Height);
            conParams.AddRule(LayoutRules.CenterInParent);
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
                Resource.Drawable.EraseBtn, Resource.Drawable.BrushBtn,
                Resource.Drawable.RectCutBtn, Resource.Drawable.FreeCutBtn, Resource.Drawable.EditAddSprite,Resource.Drawable.EditAddSprite,Resource.Drawable.EditAddSprite
            };
            int sw = (int)(ScreenUtil.ScreenWidth(this) * 931 / 1280.0);
            FrameLayout contentView = FindViewById<FrameLayout>(Resource.Id.edit_bt_content_view);
            int itemW = (int)(sw / 13.0);
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
                    int tag = (int) ((ImageView)t).Tag;
                    if (tag < 11)
                    {
                        OperateType[] types = { OperateType.FlipX, OperateType.FlipY, OperateType.RotateR, OperateType.RotateL,
                                                OperateType.Enlarge, OperateType.Narrow, OperateType.Eraser, OperateType.Draw,
                                                OperateType.RectCut, OperateType.FreeCut, OperateType.Add
                                              };
                        mCustomView.Operate(types[tag]);
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


    // Operate type
    enum OperateType { FlipY, FlipX, RotateR, RotateL, Enlarge, Narrow, Eraser, Draw, RectCut, FreeCut, Add, Back, Forward, Move, Rotate};

    // Edit step
    class EditStep {
        public OperateType operateType;

        // 放大参数
        public float scale;

        // 移动参数
        public float moveX;
        public float moveY;

        // 旋转参数
        public float rotate;

        // 擦除和画线参数
        public Path path; // 也包含裁剪
        public string color;
        public int strokewidth;
        
        public EditStep(OperateType type)
        {
            operateType = type;
        }

        public EditStep(OperateType type, Path p, string c, int w)
        {
            operateType = type;
            color = c;
            path = p;
            strokewidth = w;
        }
    }

    class ImageUtil {

        public static Bitmap FlipX(Bitmap src) {
            Matrix matrix = new Matrix();
            matrix.SetScale(-1, 1);
            Bitmap newBM = Bitmap.CreateBitmap(src, 0, 0, src.Width, src.Height, matrix, false);
            src.Recycle();
            return newBM;
        }

        public static Bitmap FlipY(Bitmap src)
        {
            Matrix matrix = new Matrix();
            matrix.SetScale(1, -1);
            Bitmap newBM = Bitmap.CreateBitmap(src, 0, 0, src.Width, src.Height, matrix, false);
            src.Recycle();
            return newBM;
        }
       
        public static Bitmap Rotate(Bitmap src, float degree)
        {
            Matrix matrix = new Matrix();
            matrix.SetRotate(degree);
            Bitmap newBM = Bitmap.CreateBitmap(src, 0, 0, src.Width, src.Height, matrix, false);
            src.Recycle();
            return newBM;
        }

        public static Bitmap Scale(Bitmap src, float scale)
        {
            Matrix matrix = new Matrix();
            matrix.SetScale(scale, scale);
            Bitmap newBM = Bitmap.CreateBitmap(src, 0, 0, src.Width, src.Height, matrix, false);
            src.Recycle();
            return newBM;
        }

        public static Bitmap ScaleFliter(Bitmap src, float scale)
        {
            Bitmap newBM = Bitmap.CreateScaledBitmap(src, (int)(src.Width*scale), (int)(src.Height*scale), true);
            src.Recycle();
            return newBM;
        }

        public static Bitmap Eraser(Bitmap src, Path path, float w) {
            Paint eraserPaint = new Paint();
            eraserPaint.SetStyle(Paint.Style.Stroke);
            eraserPaint.AntiAlias = true;
            eraserPaint.Alpha = 0;
            eraserPaint.SetXfermode(new PorterDuffXfermode(PorterDuff.Mode.Clear));
            eraserPaint.StrokeWidth = w;
            Canvas canvas = new Canvas(src);
            canvas.DrawPath(path, eraserPaint);
            return src;
        }

        public static Bitmap Draw(Bitmap src, Path path, float w, Color color)
        {
            Paint paint = new Paint();
            paint.SetStyle(Paint.Style.Stroke);
            paint.AntiAlias = true;
            paint.StrokeWidth = w;
            paint.Color = color;
            Canvas canvas = new Canvas(src);
            canvas.DrawPath(path, paint);
            return src;
        }

        public static List<Bitmap> Clip(Bitmap src, Path path) {
            Bitmap bitmap = Bitmap.CreateBitmap(src);
            Paint eraserPaint = new Paint();
            eraserPaint.SetStyle(Paint.Style.Fill);
            eraserPaint.AntiAlias = true;
            eraserPaint.Alpha = 0;
            eraserPaint.SetXfermode(new PorterDuffXfermode(PorterDuff.Mode.Clear));
            Canvas canvas = new Canvas(bitmap);
            canvas.DrawPath(path, eraserPaint);

            Bitmap bitmap1 = Bitmap.CreateBitmap(src.Width, src.Height, Bitmap.Config.Argb8888);
            Canvas c = new Canvas(bitmap1);
            c.ClipPath(path);
            c.DrawBitmap(bitmap, null, new Rect(0, 0, src.Width, src.Height), new Paint());

            RectF rect = new RectF();
            path.ComputeBounds(rect, false);
            int width = (int) (src.Width > rect.Left + rect.Width() ? rect.Width() : Math.Max(0, src.Width - rect.Left));
            int height = (int)(src.Height > rect.Top + rect.Height() ? rect.Height() : Math.Max(0, src.Height - rect.Top));
            Bitmap b = Bitmap.CreateBitmap(bitmap1, (int)Math.Min(rect.Left, src.Width), (int)Math.Min(rect.Top, src.Height), width, height);
            bitmap1.Recycle();
            src.Recycle();
            return new List<Bitmap>() { bitmap, b };
        }
    }

    class EditBitmap
    {
        private Bitmap src;
        private List<EditStep> stepList;
        private Bitmap curBitmap;
        private Path clipPath;
        public float curMoveX;
        public float curMoveY;
        public float scale = 1;
        private int stepIndex = 0;

        public EditBitmap(Bitmap bitmap) {
            src = bitmap;
            stepList = new List<EditStep>();
        }

        public EditBitmap(Bitmap bitmap, Path path)
        {
            src = bitmap;
            stepList = new List<EditStep>();
            clipPath = new Path(path);
            stepIndex = -1;
        }

        // 添加操作行为，只限FlipX, FlipY, RotateR, RotateL, Enlarge, Narrow
        public void Operate(OperateType type)
        {
            stepIndex += 1;
            stepList.Add(new EditStep(type));
        }

        // 擦除和画线
        public void Draw(OperateType type, Path path, string color, int width)
        {
            stepIndex += 1;
            stepList.Add(new EditStep(type ,new Path(path), color, width));
        }

        public Bitmap currentBitmap(bool isBorder = false)
        {
            Bitmap bitmap = Bitmap.CreateBitmap(src);
            if (stepList.Count == 0)
            {
                bitmap = ImageUtil.ScaleFliter(bitmap, 3);
                //bitmap = ImageUtil.Scale(bitmap, 3);
                return bitmap;
            }
            scale = 1;
            curMoveX = 0;
            curMoveY = 0;
            for (int i = 0; i < stepList.Count ; i++)
            {
                if (i < stepIndex) {
                    EditStep step = stepList[i];
                    switch (step.operateType)
                    {
                        case OperateType.FlipX:
                            {
                                bitmap = ImageUtil.FlipX(bitmap);
                                break;
                            }
                        case OperateType.FlipY:
                            {
                                bitmap = ImageUtil.FlipY(bitmap);
                                break;
                            }
                        case OperateType.RotateR:
                            {
                                bitmap = ImageUtil.Rotate(bitmap, 90);
                                break;
                            }
                        case OperateType.RotateL:
                            {
                                bitmap = ImageUtil.Rotate(bitmap, -90);
                                break;
                            }
                        case OperateType.Enlarge:
                            {
                                scale = scale * 1.1f;
                                break;
                            }
                        case OperateType.Narrow:
                            {
                                scale = scale / 1.1f;
                                break;
                            }
                        case OperateType.Eraser:
                            {
                                bitmap = ImageUtil.Eraser(bitmap, step.path, step.strokewidth);
                                break;
                            }
                        case OperateType.Draw:
                            {
                                bitmap = ImageUtil.Draw(bitmap, step.path, step.strokewidth, Color.ParseColor(step.color));
                                break;
                            }
                        case OperateType.Move:
                            {
                                curMoveX += step.moveX;
                                curMoveY += step.moveY;
                                break;
                            }
                        case OperateType.RectCut:
                        case OperateType.FreeCut:
                            {

                                break;
                            }
                        case OperateType.Add:
                            {
                                break;
                            }
                        default:
                            break;
                    }
                }   
            }
            if (stepIndex == stepList.Count)
            {

            }
            bitmap = ImageUtil.ScaleFliter(bitmap, 3);
            //bitmap = ImageUtil.Scale(bitmap, 3);
            return bitmap;
        }

        public void Recycle() {
            src.Recycle();
        }
    }

    class EditLayer
    {
        private List<EditBitmap> list;
        private int index = 0;
        public float scale
        {
            get
            {
                return list[0].scale;
            }
        }

        public EditLayer(Bitmap bitmap)
        {
            list = new List<EditBitmap>();
            EditBitmap editBitmap = new EditBitmap(bitmap);
            list.Add(editBitmap);
        }

        public List<EditBitmap> EditBitmapList()
        {
            if (index == 0)
            {
                return new List<EditBitmap>() { list[0] };
            }
            return list;
        }

        // 添加操作行为，只限FlipX, FlipY, RotateR, RotateL, Enlarge, Narrow
        public void Operate(OperateType type)
        {
            list[index].Operate(type);
        }

        // 擦除或画线
        public void Draw(OperateType type, Path path, string color, int width)
        {
            list[index].Draw(type, new Path(path), color, width);
        }

        // 裁剪
        public void Clip(Path path)
        {

        }

        // 加入图片
        public void Add(Bitmap bitmap)
        {

        }

        // 移动
        public void Move(int x, int y)
        {

        }

        public void Recycle() {
            
        }
    }

    class EditTarget
    {
        private List<EditLayer> layerlist;
        private int layerIndex = 0;
        public float scale
        {
            get
            {
                float temp = 1;
                for (int i = 0; i < layerIndex+1; i++)
                {
                   temp *= layerlist[i].scale;
                }
                return temp;
            }
        }

        public EditTarget(Bitmap bitmap) {
            EditLayer editLayer = new EditLayer(bitmap);
            layerlist = new List<EditLayer>();
            layerlist.Add(editLayer);
        }

        // 获取当前图片
        public List<EditBitmap> EditBitmapList() {
            EditLayer editLayer = layerlist[layerIndex];
            return editLayer.EditBitmapList();
        }

        // 添加操作行为，只限FlipX, FlipY, RotateR, RotateL, Enlarge, Narrow
        public void Operate(OperateType type) {
            EditLayer editLayer = layerlist[layerIndex];
            editLayer.Operate(type);
        }

        // 擦除或画线
        public void Draw(OperateType type, Path path, string color, int width)
        {
            EditLayer editLayer = layerlist[layerIndex];
            editLayer.Draw(type, new Path(path), color, width);
        }

        // 裁剪
        public void Clip(Path path) {

        }

        // 加入图片
        public void Add(Bitmap bitmap)
        {

        }

        // 移动
        public void Move(int x, int y) {
            
        }

        // 前进一步
        public void nextStep()
        {

        }

        // 后退一步
        public void previousStep()
        {

        }

        public void Recycle() {

        }
    }

    // draw image view
    class CustomView : ImageView {
        private Context mCxt;

        private int mWidth;
        private int mHeight;

        private Bitmap bBitmap;
        private Bitmap tBitmap;
        private Canvas curCanvas;
        private List<EditBitmap> list;

        private string curColor = "#000000";
        private int curStrokeW = 10;
        private Paint mPaint;
        public Action<List<Bitmap>> mAction { get; set; }


        public EditTarget editTarget = null;
        public Bitmap srcBm;
        private OperateType operateType;
        private Path mPath = new Path();
        private Path drawPath = new Path();
        private Path cutPath = new Path();


        // 触摸相关事件
        private float mX;
        private float mY;
        float TOUCH_TOLERANCE = 2;

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
            mCxt = context;
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
            mWidth = measure(widthMeasureSpec);
            mHeight = measure(heightMeasureSpec);
            if (bBitmap != null)
            {
                mWidth = (int) (bBitmap.Width * editTarget.scale);
                mHeight = (int) (bBitmap.Height * editTarget.scale);
            }
            SetMeasuredDimension(mWidth, mHeight);
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
            return result;
        }

        public override void Draw(Canvas canvas)
        {
            if (bBitmap != null)
            {
                RectF r = new RectF(0, 0, mWidth, mHeight);
                canvas.DrawBitmap(bBitmap, null, r, mPaint);
                if (curCanvas != null && !mPath.IsEmpty)
                {
                    if (operateType == OperateType.Draw)
                    {
                        SetPaint(curColor, curStrokeW, null);
                    }
                    else
                    {
                        SetPaint(curColor, curStrokeW, new PorterDuffXfermode(PorterDuff.Mode.Clear));
                    }
                    curCanvas.DrawPath(mPath, mPaint);
                }
                // 边框
                SetPaint();
                canvas.DrawRect(new Rect(0, 0, mWidth, mHeight), mPaint);
            }
        }

        public void SetPaint(string color = "#000000", int w = 6, Xfermode xfermode = null) {
            if (mPaint == null)
            {
                mPaint = new Paint();
                //mPaint.AntiAlias = true;
                mPaint.SetStyle(Paint.Style.Stroke);
                
            }
            mPaint.Color = Color.ParseColor(color);
            mPaint.StrokeWidth = w;
            mPaint.SetXfermode(xfermode);
        }

        public void SetSrcBitmap(Bitmap bitmap) {
            srcBm?.Recycle();
            srcBm = Bitmap.CreateBitmap(bitmap);
            editTarget?.Recycle();
            if (editTarget == null)
            {
                editTarget = new EditTarget(srcBm);
            }
            updateView();
        }

        // 添加操作图片
        public void AddBitmap(Bitmap bitmap)
        {
            updateView();
        }
     
        // 当前图片插入图片
        public void InsertBitmap(Bitmap bitmap)
        {
            if (editTarget == null)
            {
                return;
            }
            editTarget.Add(bitmap);
            updateView();
        }

        public void Operate(OperateType type)
        {    
            switch (type)
            {
                case OperateType.FlipX:
                case OperateType.FlipY:
                case OperateType.RotateR:
                case OperateType.RotateL:
                case OperateType.Enlarge:
                case OperateType.Narrow:
                    editTarget?.Operate(type);
                    break;
                default:
                    break;
            }
            operateType = type;
            updateView();
        }

        public void updateView() {
            bBitmap?.Recycle();
            tBitmap?.Recycle();
            EditTarget target = editTarget;
            list = target?.EditBitmapList();
            bBitmap = list[0].currentBitmap();
            tBitmap = list.Count > 1 ? list[1].currentBitmap() : null;
            RequestLayout();
            curCanvas = new Canvas(bBitmap);
            Invalidate();
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            if (operateType == OperateType.Draw || operateType == OperateType.Eraser || operateType == OperateType.RectCut || operateType == OperateType.FreeCut)
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

        // touch begin
        private void touch_start(float x, float y)
        {
            mPath.Reset();
            drawPath.Reset();
            float scale = editTarget.scale;
            float scaleTwo = 3 * scale;
            mPath.MoveTo(x/scale, y/scale);
            drawPath.MoveTo(x/scaleTwo, y/ scaleTwo);
            mX = x;
            mY = y;
        }

        // touch move
        private void touch_move(float x, float y)
        {

            float dx = Math.Abs(x - mX);
            float dy = Math.Abs(y - mY);
            if (dx >= TOUCH_TOLERANCE || dy >= TOUCH_TOLERANCE)
            {
                float scale = editTarget.scale;
                float scaleTwo = 3 * scale;
                mPath.LineTo(x / scale, y / scale);
                drawPath.LineTo(x / scaleTwo, y / scaleTwo);
                Invalidate();
            }
        }

        // touch end
        private void touch_up(float x, float y)
        {
            float scale = editTarget.scale;
            float scaleTwo = 3 * scale;
            mPath.LineTo(x / scale, y / scale);
            drawPath.LineTo(x / scaleTwo, y / scaleTwo);
            editTarget?.Draw(operateType, drawPath, curColor, curStrokeW/3);
            mPath.Reset();
            drawPath.Reset();
            updateView();
        }
    }
}


