using System;
using Android.Media;
using Android.Content;
using Android.Runtime;
using Android.Widget;
using Android.Views;
using Android.Graphics;
using Com.Bumptech.Glide;
  
namespace TabletArtco
{
    public class MediaManager : Java.Lang.Object, ISurfaceHolderCallback, MediaPlayer.IOnPreparedListener, MediaPlayer.IOnCompletionListener, MediaPlayer.IOnErrorListener, MediaPlayer.IOnInfoListener, MediaPlayer.IOnSeekCompleteListener
    {
        private SurfaceView mSurfaceView;
        private ImageView preImgIv;
        private MediaPlayer mediaPlayer;
        private Context mContext;
        private string mPath = null;//"http://103.120.226.173/artco/backgrounds/3%20Playgrounds.mp4"; //http://username:password@host:8080/directory/file?query#ref:
        private bool isOnPrepared = false;
        private bool isPlay = false;
        private bool isDefault = true;
        private String remotePreviewImgPath = null;
        private ISurfaceHolder mSurfaceHolder;
       

        public MediaManager(SurfaceView surfaceView, ImageView imgIv, Context cxt)
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
            mediaPlayer = new MediaPlayer();
            mediaPlayer.SetOnCompletionListener(this);
            mediaPlayer.SetOnErrorListener(this);
            mediaPlayer.SetOnPreparedListener(this);
            mediaPlayer.SetOnSeekCompleteListener(this);
            Android.Net.Uri url = Android.Net.Uri.Parse("android.resource://" + mContext.PackageName + "/raw/" + Resource.Raw.default_video);
            mediaPlayer.SetDataSource(mContext, url);
            Play();
        }

        public void SetPath(string path, string img)
        {
            if (isDefault)
            {
                isDefault = false;
                isPlay = false;
            }
            LogUtil.CustomLog("path:" + path + ", prepath:" + img);
            //string start = "http://www.playartco.com:8081/artco/backgrounds/";
            //mPath = start + Android.Net.Uri.Encode(path.Substring(start.Length));
            preImgIv.Visibility = ViewStates.Gone;
            mPath = path;
            remotePreviewImgPath = img;
            if (mediaPlayer != null)
            {
                try
                {
                    mediaPlayer.Stop();
                    mediaPlayer.Release();
                    mediaPlayer = null;
                    mediaPlayer = new MediaPlayer();
                    mediaPlayer.SetOnCompletionListener(this);
                    mediaPlayer.SetOnErrorListener(this);
                    mediaPlayer.SetOnPreparedListener(this);
                    mediaPlayer.SetOnSeekCompleteListener(this);
                    mediaPlayer.SetDataSource(mPath);
                    if (mSurfaceView.Holder != null && mSurfaceView.Holder.IsCreating)
                    {
                        mediaPlayer.SetDisplay(mSurfaceView.Holder);
                        mediaPlayer.PrepareAsync();
                    }
                    //if (img != null)
                    //{
                    //    Glide.With(mContext).Load(GlideUtil.GetGlideUrl(remotePreviewImgPath)).Into(preImgIv);
                    //}
                    //preImgIv.Visibility = ViewStates.Visible;
                    isOnPrepared = false;
                    if (isPlay)
                    {
                        Play();
                    }
                }
                catch (Exception e)
                {
                    LogUtil.CustomLog("Set path for mediaplayer error " + e.ToString());
                }
            }
        }

        public void Play()
        {
            if (mediaPlayer != null && !mediaPlayer.IsPlaying && isOnPrepared && (isDefault || mPath != null))
            {
                mediaPlayer.SeekTo(0);
                mediaPlayer.Start();
                isPlay = true;
            }
            else
            { 
                isPlay = true;
            }
        }

        public void Stop()
        {
            isPlay = false;
            if (mediaPlayer != null && mediaPlayer.IsPlaying)
            {
                mediaPlayer.Pause();
            }
        }

        public void ClickHomeButton()
        {
            preImgIv.Visibility = ViewStates.Visible;
            mPath = null;
        }

        void ISurfaceHolderCallback.SurfaceCreated(ISurfaceHolder holder)
        {
            mSurfaceHolder = holder;
            LogUtil.CustomLog("SurfaceCreated");
            //preImgIv.Visibility = ViewStates.Visible;
            if (mediaPlayer!= null)
            {
                mediaPlayer.SetDisplay(holder);
                try
                {
                    mediaPlayer.PrepareAsync();
                }
                catch (Exception e)
                {
                    LogUtil.CustomLog("SurfaceCreated Exception", e.ToString());
                }
            }
        }

        void ISurfaceHolderCallback.SurfaceChanged(ISurfaceHolder holder, [GeneratedEnum] Format format, int width, int height)
        {
            LogUtil.CustomLog("SurfaceChanged", "holder: width" + width + ", height:" + height);
        }

        void ISurfaceHolderCallback.SurfaceDestroyed(ISurfaceHolder holder)
        {
            LogUtil.CustomLog("SurfaceDestroyed", "SurfaceDestroyed");
        }

        void MediaPlayer.IOnPreparedListener.OnPrepared(MediaPlayer mp)
        {
            LogUtil.CustomLog("OnPrepared", "OnPrepared");
            //preImgIv.Visibility = ViewStates.Visible;
            mp.SetOnInfoListener(this);
            isOnPrepared = true;

            mp.SeekTo(0);
            if (isPlay)
            {
                mp.Start();
            }
        }

        bool MediaPlayer.IOnErrorListener.OnError(MediaPlayer mp, [GeneratedEnum] MediaError what, int extra)
        {
            LogUtil.CustomLog("OnError", ""+what);
            return false;
        }

        bool MediaPlayer.IOnInfoListener.OnInfo(MediaPlayer mp, MediaInfo what, int extra)
        {
            LogUtil.CustomLog("MediaPlayer.OnInfo", "" + what);
            if (what == MediaInfo.VideoRenderingStart) {
                preImgIv.Visibility = ViewStates.Gone;
            }
            return true;
        }

        void MediaPlayer.IOnCompletionListener.OnCompletion(MediaPlayer mp)
        {
            LogUtil.CustomLog("MediaPlayer.OnCompletion", "OnCompletion");
            try
            {
                if (!isDefault && isPlay)
                {
                    mp.SeekTo(0);
                    mp.Start();
                }
                else
                {
                    isPlay = false;
                    isDefault = false;
                    preImgIv.Visibility = ViewStates.Visible;
                }
            }
            catch (Exception e)
            {
                LogUtil.CustomLog(e.ToString());
            }
        }

        public void OnSeekComplete(MediaPlayer mp)
        {
            LogUtil.CustomLog("MediaPlayer.OnSeekComplete", "OnSeekComplete");
            preImgIv.Visibility = ViewStates.Gone;
        }
    }
}
