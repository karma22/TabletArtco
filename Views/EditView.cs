using System;

using Android.Content;
using Android.Util;
using Android.Views;
using Android.Graphics;
using System.Collections.Generic;

namespace TabletArtco
{
    class ImageUtil
    {
        public static Bitmap FlipX(Bitmap src)
        {
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
            Bitmap newBM = Bitmap.CreateScaledBitmap(src, (int)(src.Width * scale), (int)(src.Height * scale), true);
            src.Recycle();
            return newBM;
        }

        public static Bitmap Eraser(Bitmap src, Path path, float w)
        {
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

        public static List<Bitmap> Clip(Bitmap src, Path path)
        {
            Bitmap bitmap = Bitmap.CreateBitmap(src);
            Paint p = new Paint();
            p.SetStyle(Paint.Style.Fill);
            p.AntiAlias = true;
            p.Alpha = 0;
            p.SetXfermode(new PorterDuffXfermode(PorterDuff.Mode.Clear));
            Canvas canvas = new Canvas(bitmap);
            canvas.DrawPath(path, p);

            //Bitmap bitmap1 = Bitmap.CreateBitmap(src.Width, src.Height, Bitmap.Config.Argb8888);
            //Canvas c = new Canvas(bitmap1);
            //c.ClipPath(path);
            //c.DrawBitmap(bitmap, null, new Rect(0, 0, src.Width, src.Height), new Paint());



            
            Bitmap bitmap1 = Bitmap.CreateBitmap(src.Width, src.Height, Bitmap.Config.Argb8888);
            Canvas c = new Canvas(bitmap1);
            c.ClipPath(path);
            c.DrawBitmap(src, 0, 0, new Paint());
            //c.DrawBitmap(src, new Rect(0, 0, src.Width, src.Height), new RectF(0, 0, src.Width, src.Height), p);

            //RectF r1 = new RectF();
            //path.ComputeBounds(r1, false);
            //Rect r2 = new Rect(0, 0, src.Width, src.Height);
            //Rect rect = new Rect(Math.Max(r1.Left, r2.Left), Math.Max(r1.Top, r2.Top), Math.Min())

            RectF rect = new RectF();
            path.ComputeBounds(rect, false);
            Bitmap b = Bitmap.CreateBitmap(bitmap1, (int)rect.Left, (int)rect.Top, (int)(rect.Right - rect.Left), (int)(rect.Bottom - rect.Top), null, true);

            //int width = (int)(src.Width > rect.Left + rect.Width() ? rect.Width() : Math.Max(0, src.Width - rect.Left));
            //int height = (int)(src.Height > rect.Top + rect.Height() ? rect.Height() : Math.Max(0, src.Height - rect.Top));
            //Bitmap b = Bitmap.CreateBitmap(bitmap1, (int)Math.Min(rect.Left, src.Width), (int)Math.Min(rect.Top, src.Height), width, height);
            bitmap1.Recycle();
            src.Recycle();
            return new List<Bitmap>() { bitmap, b };
        }
    }

    // Operate type
    enum OperateType { FlipY, FlipX, RotateR, RotateL, Enlarge, Narrow, Eraser, Draw, RectCut, FreeCut, Add, Previous, Next, Move, Rotate };

    // Edit step
    class EditStep
    {
        public OperateType operateType;

        // 放大参数
        public float scale;

        // 移动参数
        public float moveX;
        public float moveY;

        // 旋转参数
        public float rotate;

        // 擦除和画线参数
        public Path path;
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

        public EditStep(OperateType type, float x, float y)
        {
            operateType = type;
            moveX = x;
            moveY = y;
        }

        public EditStep(OperateType type, float degree)
        {
            operateType = type;
            rotate = degree;
        }
    }

    class EditBitmap
    {
        private Bitmap src;
        private List<EditStep> stepList;
        private int stepIndex = -1;
        public float scale = 1;

        private int bitmapType = 0;  // 0底层图片， 1插入的图片， 2裁剪的图片
        public Path borderPath;
        public RectF borderRect = new RectF();
        public float curMoveX;
        public float curMoveY;

        public EditBitmap(Bitmap bitmap)
        {
            src = bitmap;
            stepList = new List<EditStep>();
        }

        public EditBitmap(Bitmap bitmap, Path path, int type)
        {
            src = bitmap;
            stepList = new List<EditStep>();
            borderPath = path;
            path.ComputeBounds(borderRect, false);
            bitmapType = type;
        }

        // 添加操作行为，只限FlipX, FlipY, RotateR, RotateL, Enlarge, Narrow
        public void Operate(OperateType type)
        {
            // 添加新的步骤时，将当前步骤后面的记录清除掉
            if (stepIndex != stepList.Count - 1)
            {
                stepList.RemoveRange(stepIndex + 1, stepList.Count - stepIndex - 1);
            }
            stepIndex += 1;
            stepList.Add(new EditStep(type));
        }

        // 擦除和画线Eraser, Draw
        public void Draw(OperateType type, Path path, string color, int width)
        {
            // 添加新的步骤时，将当前步骤后面的记录清除掉
            if (stepIndex != stepList.Count - 1)
            {
                stepList.RemoveRange(stepIndex + 1, stepList.Count - stepIndex - 1);
            }
            stepIndex += 1;
            stepList.Add(new EditStep(type, new Path(path), color, width));
        }

        // 添加的或裁剪的图片移动
        public void Move(float x, float y)
        {
            if (stepIndex != stepList.Count - 1)
            {
                stepList.RemoveRange(stepIndex + 1, stepList.Count - stepIndex - 1);
            }
            stepIndex += 1;
            stepList.Add(new EditStep(OperateType.Move, x, y));
        }

        // 选择添加或裁剪的图旋转
        public void Rotate(float degree)
        {
            if (stepIndex != stepList.Count - 1)
            {
                stepList.RemoveRange(stepIndex + 1, stepList.Count - stepIndex - 1);
            }
            stepIndex += 1;
            stepList.Add(new EditStep(OperateType.Rotate, degree));
        }


        //enum OperateType { FlipY, FlipX, RotateR, RotateL, Enlarge, Narrow, Eraser, Draw, RectCut, FreeCut, Add, Back, Forward, Move, Rotate };
        public Bitmap currentBitmap(bool isBorder = false, bool isScale = true)
        {
            Bitmap bitmap = Bitmap.CreateBitmap(src);
            if (stepList.Count == 0)
            {
                if (isScale)
                {
                    bitmap = ImageUtil.ScaleFliter(bitmap, 3);
                }
                return bitmap;
            }
            scale = 1;
            curMoveX = 0;
            curMoveY = 0;
            for (int i = 0; i < stepList.Count; i++)
            {
                if (i <= stepIndex)
                {
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
                        case OperateType.Rotate:
                            {
                                bitmap = ImageUtil.Rotate(bitmap, step.rotate);
                                break;
                            }
                        default:
                            break;
                    }
                }
            }
            if (isScale)
            {
                bitmap = ImageUtil.ScaleFliter(bitmap, 3);
            }
            return bitmap;
        }

        // 上一步操作
        public bool PreviousStep()
        {
            // 当前已不能后退一步
            if (stepIndex == -2)
            {
                return false;
            }
            stepIndex -= 1;
            return true;
        }

        // 下一步操作
        public bool NextStep()
        {
            if (stepIndex < stepList.Count - 1)
            {
                stepIndex += 1;
                return true;
            }
            return false;
        }

        // 资源回收
        public void Recycle()
        {
            src.Recycle();
        }
    }

    // 一个图层最多包含两个EditBitmap对象
    class EditLayer
    {
        public List<EditBitmap> list;
        private int index = 0;
        // 返回第一个EditBitmap的缩放度
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
            RemoveBitmap();
            list[index].Operate(type);
        }

        // 擦除或画线
        public void Draw(OperateType type, Path path, string color, int width)
        {
            RemoveBitmap();
            list[index].Draw(type, new Path(path), color, width);
        }

        // 加入图层
        public void Add(Bitmap bitmap, Path path, OperateType type)
        {
            EditBitmap editBitmap = new EditBitmap(bitmap, path, type==OperateType.Add ? 2 : 1);
            list.Add(editBitmap);
            index = 1;
        }

        // 移动
        public void Move(int x, int y)
        {
            if (index == 1)
            {
                list[index].Move(x, y);
            }
        }

        // 选择添加或裁剪的图旋转
        public void Rotate(float degree)
        {
            if (index == 1)
            {
                list[index].Rotate(degree);
            }
        }

        // 后退一步
        public bool PreviousStep()
        {
            bool backOk = list[index].PreviousStep();
            if (backOk)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // 前进一步
        public bool NextStep()
        {
            bool nextOk = list[index].NextStep();
            if (nextOk)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // 资源回收
        public void Recycle()
        {
            for (int i = 0; i < list.Count; i++)
            {
                EditBitmap item = list[i];
                item.Recycle();
            }
        }

        // 返回当前图层的图片
        public Bitmap LayerImage()
        {
            List<EditBitmap> imgList = EditBitmapList();
            if (imgList.Count == 1)
            {
                return imgList[0].currentBitmap(false, false);
            }
            EditBitmap editBitmap = imgList[1];
            Bitmap bitmap = imgList[0].currentBitmap(false, false);
            Bitmap bitmap1 = editBitmap.currentBitmap(false, false);
            RectF borderRect = editBitmap.borderRect;
            borderRect.Offset(editBitmap.curMoveX, editBitmap.curMoveY);
            Rect r = new Rect(0,0,(int)(bitmap1.Width*editBitmap.scale), (int)(bitmap1.Height*editBitmap.scale));
            r.Offset((int)(borderRect.CenterX()-r.CenterX()), (int)(borderRect.CenterY()-r.CenterY()));
            Canvas canvas = new Canvas(bitmap);
            canvas.DrawBitmap(bitmap1, null, r, new Paint());
            bitmap1.Recycle();
            return bitmap;
        }

        // 移除后面的操作步骤
        private void RemoveBitmap()
        {
            if (index == 0 && list.Count > 1)
            {
                EditBitmap item = list[1];
                item.Recycle();
                list.Remove(item);
            }
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
                for (int i = 0; i < layerIndex + 1; i++)
                {
                    temp *= layerlist[i].scale;
                }
                return temp;
            }
        }

        public EditTarget(Bitmap bitmap)
        {
            EditLayer editLayer = new EditLayer(bitmap);
            layerlist = new List<EditLayer>();
            layerlist.Add(editLayer);
        }

        // 获取当前EditLayer
        public EditLayer EditLayer()
        {
            EditLayer editLayer = layerlist[layerIndex];
            return editLayer;
        }

        // 获取当前图片
        public List<EditBitmap> EditBitmapList()
        {
            EditLayer editLayer = layerlist[layerIndex];
            return editLayer.EditBitmapList();
        }

        // 添加操作行为，只限FlipX, FlipY, RotateR, RotateL, Enlarge, Narrow
        public void Operate(OperateType type)
        {
            RemoveLayers();
            EditLayer editLayer = layerlist[layerIndex];
            editLayer.Operate(type);
        }

        // 擦除或画线
        public void Draw(OperateType type, Path path, string color, int width)
        {
            RemoveLayers();
            EditLayer editLayer = layerlist[layerIndex];
            if (editLayer.list.Count == 2)
            {
                Bitmap b = editLayer.LayerImage();
                EditLayer layer = new EditLayer(b);
                layerlist.Add(layer);
                layerIndex++;
                layer.Draw(type, new Path(path), color, width);
            }
            else {
                editLayer.Draw(type, new Path(path), color, width);
            }   
        }

        // 裁剪
        public void Clip(Path path)
        {
            RemoveLayers();
            EditLayer editLayer = layerlist[layerIndex];
            Bitmap bitmap = editLayer.LayerImage();
            List<Bitmap> bitmaps = ImageUtil.Clip(bitmap, path);
            EditLayer layer = new EditLayer(bitmaps[0]);
            layer.Add(bitmaps[1], path, OperateType.FreeCut);
            layerlist.Add(layer);
            layerIndex ++;
        }

        // 加入图片
        public void Add(Bitmap bitmap)
        {
            RemoveLayers();
            bitmap = ImageUtil.Scale(bitmap, 1/scale);
            EditLayer editLayer = layerlist[layerIndex];
            Bitmap b = editLayer.LayerImage();
            EditLayer layer = new EditLayer(b);
            RectF rect = new RectF(0, 0, bitmap.Width, bitmap.Height);
            rect.Offset(b.Width/2-rect.CenterX(), b.Height/2-rect.CenterY());
            Path path = new Path();
            path.AddRect(rect, Path.Direction.Cw);
            layer.Add(bitmap, path, OperateType.Add);
            layerlist.Add(layer);
            layerIndex++;
        }

        // 移动
        public void Move(int x, int y)
        {
            RemoveLayers();
            EditLayer editLayer = layerlist[layerIndex];
            editLayer.Move(x, y);
        }

        // 后退一步
        public void PreviousStep()
        {
            bool backOk = layerlist[layerIndex].PreviousStep();
            if (!backOk)
            {
                if (layerIndex > 0)
                {
                    layerIndex -= 1;
                }
            }
        }

        // 前进一步
        public void NextStep()
        {
            bool nextOk = layerlist[layerIndex].NextStep();
            if (!nextOk)
            {
                if (layerIndex < layerlist.Count - 1)
                {
                    layerIndex += 1;
                }
            }
        }

        // 资源回收
        public void Recycle()
        {
            for (int i = layerlist.Count - 1; i >= 0; i--)
            {
                EditLayer item = layerlist[i];
                item.Recycle();
                layerlist.Remove(item);
            }
        }

        // 添加新的EditLayer前，移除当前之后的EditLayer
        private void RemoveLayers()
        {
            if (layerIndex != layerlist.Count - 1)
            {
                for (int i = layerlist.Count - 1; i > layerIndex; i--)
                {
                    EditLayer item = layerlist[i];
                    item.Recycle();
                    layerlist.Remove(item);
                }
            }
        }
    }

    // draw image view
    class EditView : View
    {
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
        private Path mPath = new Path();   //当前绘制path
        private Path imgPath = new Path(); //绘制到bitmap上的path        

        // 触摸相关事件
        private float mX;
        private float mY;
        float TOUCH_TOLERANCE = 2;

        public EditView(Context context) : base(context)
        {
            Initialize(context);
        }

        public EditView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            Initialize(context);
        }

        public EditView(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle)
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
                mWidth = (int)(bBitmap.Width * editTarget.scale);
                mHeight = (int)(bBitmap.Height * editTarget.scale);
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
                if (tBitmap != null)
                {
                    EditBitmap editBitmap = list[1];
                    PointF p = new PointF(editBitmap.borderRect.CenterX(), editBitmap.borderRect.CenterY());
                    p.X = (p.X + editBitmap.curMoveX) * 3 * editTarget.scale;
                    p.Y = (p.Y + editBitmap.curMoveY) * 3 * editTarget.scale;
                    RectF rect = new RectF(0, 0, tBitmap.Width * editTarget.scale * editBitmap.scale, tBitmap.Height * editTarget.scale * editBitmap.scale);
                    rect.Offset(p.X - rect.CenterX(), p.Y - rect.CenterY());
                    canvas.DrawBitmap(tBitmap, null, rect, mPaint);
                }
                if (curCanvas != null && !mPath.IsEmpty)
                {
                    if (operateType == OperateType.Draw || operateType == OperateType.Eraser)
                    {
                        Xfermode mode = operateType == OperateType.Eraser ? new PorterDuffXfermode(PorterDuff.Mode.Clear) : null;
                        SetPaint(curColor, curStrokeW, mode);
                        curCanvas.DrawPath(mPath, mPaint);
                    }
                }
                // 边框
                SetPaint();
                canvas.DrawRect(new Rect(0, 0, mWidth, mHeight), mPaint);
            }
        }

        public void SetPaint(string color = "#000000", int w = 6, Xfermode xfermode = null)
        {
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

        public void SetSrcBitmap(Bitmap bitmap)
        {
            srcBm?.Recycle();
            srcBm = Bitmap.CreateBitmap(bitmap);
            editTarget?.Recycle();
            if (editTarget == null)
            {
                editTarget = new EditTarget(srcBm);
            }
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
                case OperateType.Previous:
                    editTarget?.PreviousStep();
                    break;
                case OperateType.Next:
                    editTarget?.NextStep();
                    break;
                default:
                    break;
            }
            operateType = type;
            updateView();
        }

        public void updateView()
        {
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
            mX = x;
            mY = y;
            float scale = editTarget.scale;
            float scaleTwo = 3 * scale;
            mPath.Reset();
            imgPath.Reset();
            if (operateType == OperateType.Draw || operateType == OperateType.Eraser || operateType == OperateType.FreeCut)
            {
                mPath.MoveTo(x / scale, y / scale);
                imgPath.MoveTo(x / scaleTwo, y / scaleTwo);
            }
        }

        // touch move
        private void touch_move(float x, float y)
        {
            if (x < 0 || x > mWidth || y < 0 || y > mHeight)
            {
                return;
            }

            float dx = Math.Abs(x - mX);
            float dy = Math.Abs(y - mY);
            if (dx >= TOUCH_TOLERANCE || dy >= TOUCH_TOLERANCE)
            {
                float scale = editTarget.scale;
                float scaleTwo = 3 * scale;
                if (operateType == OperateType.Draw || operateType == OperateType.Eraser || operateType == OperateType.FreeCut)
                {

                    mPath.LineTo(x / scale, y / scale);
                    imgPath.LineTo(x / scaleTwo, y / scaleTwo);
                    Invalidate();
                }
                else if (operateType == OperateType.RectCut)
                {
                    mPath.Reset();
                    imgPath.Reset();
                    RectF r = new RectF();
                    r.Left = Math.Min(mX, x);
                    r.Top = Math.Min(mY, y);
                    r.Right = Math.Max(mX, x);
                    r.Bottom = Math.Max(mY, y);
                    mPath.AddRect(new RectF(r.Left / scale, r.Top / scale, r.Right / scale, r.Bottom / scale), Path.Direction.Cw);
                    imgPath.AddRect(new RectF(r.Left / scaleTwo, r.Top / scaleTwo, r.Right / scaleTwo, r.Bottom / scaleTwo), Path.Direction.Cw);
                }
            }
        }

        // touch end
        private void touch_up(float x, float y)
        {
            if (x < 0 || x > mWidth || y < 0 || y > mHeight)
            {
                return;
            }
            float scale = editTarget.scale;
            float scaleTwo = 3 * scale;
            if (operateType == OperateType.Draw || operateType == OperateType.Eraser || operateType == OperateType.FreeCut)
            {

                mPath.LineTo(x / scale, y / scale);
                imgPath.LineTo(x / scaleTwo, y / scaleTwo);
                if (operateType == OperateType.FreeCut)
                {
                    int width = bBitmap != null ? bBitmap.Width / 3 : 0;
                    int height = bBitmap != null ? bBitmap.Height / 3 : 0;
                    imgPath.Close();
                    editTarget?.Clip(imgPath);
                }
                else
                {
                    editTarget?.Draw(operateType, imgPath, curColor, curStrokeW / 3);
                }

                mPath.Reset();
                imgPath.Reset();
                updateView();
            }
            else if (operateType == OperateType.RectCut)
            {
                int width = bBitmap != null ? bBitmap.Width / 3 : 0;
                int height = bBitmap != null ? bBitmap.Height / 3 : 0;
                mPath.Reset();
                imgPath.Reset();
                RectF r = new RectF();
                r.Left = Math.Min(mX, x);
                r.Top = Math.Min(mY, y);
                r.Right = Math.Max(mX, x);
                r.Bottom = Math.Max(mY, y);
                mPath.AddRect(new RectF(r.Left / scale, r.Top / scale, r.Right / scale, r.Bottom / scale), Path.Direction.Cw);
                imgPath.AddRect(new RectF(r.Left / scaleTwo, r.Top / scaleTwo, r.Right / scaleTwo, r.Bottom / scaleTwo), Path.Direction.Cw);
                editTarget?.Clip(imgPath);
                mPath.Reset();
                imgPath.Reset();
                updateView();
            }
        }
    }
}
