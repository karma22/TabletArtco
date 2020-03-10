using System;
using Android.Media;
using Android.Content;
using Android.Runtime;
using Android.Widget;
using Android.Views;
using Android.Graphics;
using Com.Bumptech.Glide;
using Org.Opencv.Videoio;
using System.Threading;
using System.Diagnostics;
using Org.Opencv.Core;

namespace TabletArtco
{
    public class MediaManager2 : Java.Lang.Object, ISurfaceHolderCallback
    {
        private SurfaceView mSurfaceView;
        private ImageView preImgIv;
        private Context mContext;
        private string mPath = null;//"http://103.120.226.173/artco/backgrounds/3%20Playgrounds.mp4"; //http://username:password@host:8080/directory/file?query#ref:
        private bool isPlay = false;
        private ISurfaceHolder mSurfaceHolder;

        public static VideoCapture _capture;
        public static Thread _backgroundRunThread;
        public static object _lockObject = new object();



        public MediaManager2(SurfaceView surfaceView, ImageView imgIv, Context cxt)
        {
            mSurfaceView = surfaceView;
            preImgIv = imgIv;
            mContext = cxt;
            InitPlayer();
        }

        private void InitPlayer()
        {
            mSurfaceView.SetZOrderOnTop(false);
            mSurfaceView.Holder.AddCallback(this);
            //Android.Net.Uri url = Android.Net.Uri.Parse("android.resource://" + mContext.PackageName + "/raw/" + Resource.Raw.default_video);
            //Play();
        }

        public void SetPath(string path, string img)
        {
            preImgIv.Visibility = ViewStates.Gone;
            mPath = path;
        }

        public void Play()
        {
            isPlay = true;
            preImgIv.Visibility = ViewStates.Gone;

            _capture?.Dispose();
            _capture = new VideoCapture();

            _backgroundRunThread = new Thread(BackgroundRunThread);
            _backgroundRunThread.IsBackground = true;
            _backgroundRunThread.Start();
        }

        public void Stop()
        {
            lock (_lockObject)
            {
                isPlay = false;
            }
        }

        public void BackgroundRunThread()
        {
            //int fps = (int)_capture.;
            //int expectedProcessTimePerFrame = 1000 / fps;
            //Stopwatch st = new Stopwatch();
            //st.Start();

            Canvas canvas = null;

            canvas = mSurfaceHolder.LockCanvas();

            Bitmap bitmap = BitmapFactory.DecodeResource(mContext.Resources, Resource.Drawable.Actblock_Flash);
            canvas.DrawBitmap(bitmap, 0, 0, null);
            mSurfaceHolder.UnlockCanvasAndPost(canvas);


            //while (true)
            //{
            //    lock (_lockObject)
            //    {
            //        if (!isPlay)
            //            break;
            //    }

            //    long started = st.ElapsedMilliseconds;
            //    using (Mat image = new Mat())
            //    {
            //        _capture.Read(image);
            //        if (image.Empty())
            //        {
            //            break;
            //        }
            //        image.ConvertTo
            //        //MainForm._safeDrawBackground(image.ToBitmap());


            //        canvas = mSurfaceHolder.LockCanvas();
            //        try
            //        {
            //            synchronized(mSurfaceHolder) {
            //                canvas.DrawBitmap(image, 0, 0, null);
            //            }
            //        }
            //        finally
            //        {
            //            if (canvas == null)
            //                return;
            //            mSurfaceHolder.unlockCanvasAndPost(canvas);
            //        }

            //    }

            //    int elapsed = (int)(st.ElapsedMilliseconds - started);
            //    int delay = expectedProcessTimePerFrame - elapsed;
            //    Thread.Sleep((delay > 0) ? delay : 30);
            //}

            //Release();
            ////MainForm._backThreadStopComplete?.Invoke();
        }

        public void Release()
        {
            _capture.Dispose();
        }

        public void ClickHomeButton()
        {
            preImgIv.Visibility = ViewStates.Visible;
            mPath = null;
        }

        void ISurfaceHolderCallback.SurfaceCreated(ISurfaceHolder holder)
        {
            mSurfaceHolder = holder;

        }

        void ISurfaceHolderCallback.SurfaceChanged(ISurfaceHolder holder, [GeneratedEnum] Format format, int width, int height)
        {
            LogUtil.CustomLog("SurfaceChanged", "holder: width" + width + ", height:" + height);
        }

        void ISurfaceHolderCallback.SurfaceDestroyed(ISurfaceHolder holder)
        {
            LogUtil.CustomLog("SurfaceDestroyed", "SurfaceDestroyed");
        }

    }
}
