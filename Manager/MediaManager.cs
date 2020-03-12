using System;
using Android.Media;
using Android.Content;
using Android.Runtime;
using Android.Widget;
using Android.Views;
using Android.Graphics;
using Com.Bumptech.Glide;
using Android.Views.Animations;

namespace TabletArtco
{
    public class MediaManager : Java.Lang.Object, ISurfaceHolderCallback, MediaPlayer.IOnPreparedListener, MediaPlayer.IOnCompletionListener, MediaPlayer.IOnErrorListener, MediaPlayer.IOnInfoListener, MediaPlayer.IOnSeekCompleteListener, Animation.IAnimationListener
    {
        private SurfaceView mSurfaceView;
        private ImageView preImgIv;
        private MediaPlayer mediaPlayer;
        private Context mContext;
        private string mPath = null;
        private bool isOnPrepared = false;
        private bool isPlay = false;
        private bool isDefault = true;
       
        public MediaManager(SurfaceView surfaceView, ImageView imgIv, Context cxt)
        {
            mSurfaceView = surfaceView;
            preImgIv = imgIv;
            mContext = cxt;
            InitPlayer();
        }

        private void InitPlayer()
        {
            //mSurfaceView.SetZOrderOnTop(false);
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

        public void SetPath(string path, string img, string soundPath)
        {
            if (isDefault)
            {
                isDefault = false;
                isPlay = false;
            }
            LogUtil.CustomLog("path:" + path + ", prepath:" + img);
            AlphaAnimation(1, 0, 100);
            if (img != null && img.Length>0)
            {
                Glide.With(mContext).Load(GlideUtil.GetGlideUrl(img)).Into(preImgIv);
            }
            mPath = path;
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

        // play media
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

        // Stop media
        public void Stop()
        {
            isPlay = false;
            if (mediaPlayer != null && mediaPlayer.IsPlaying)
            {
                mediaPlayer.Pause();
            }
        }

        public void AlphaAnimation(float fromAlpha, float toAlpha, int duration) {
            preImgIv.Visibility = fromAlpha > 0 ? ViewStates.Gone : ViewStates.Visible;
            //Animation animation = new AlphaAnimation(fromAlpha, toAlpha);
            //animation.Duration = duration;
            //animation.SetAnimationListener(this);
            //preImgIv.Animation = animation;
        }

        public void ClickHomeButton()
        {
            AlphaAnimation(0, 1, 100);
            preImgIv.Visibility = ViewStates.Visible;
            mPath = null;
        }

        void ISurfaceHolderCallback.SurfaceCreated(ISurfaceHolder holder)
        {
            LogUtil.CustomLog("SurfaceCreated");
            preImgIv.Visibility = ViewStates.Visible;
            AlphaAnimation(0, 1, 100);
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
            preImgIv.Visibility = ViewStates.Visible;
            AlphaAnimation(0, 1, 100);
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
                AlphaAnimation(1, 0, 100);
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
            AlphaAnimation(1, 0, 100);
        }



        void Animation.IAnimationListener.OnAnimationEnd(Animation animation)
        {
            preImgIv.Visibility = preImgIv.Alpha == 0 ? ViewStates.Invisible : ViewStates.Visible;
        }

        void Animation.IAnimationListener.OnAnimationRepeat(Animation animation)
        {

        }

        void Animation.IAnimationListener.OnAnimationStart(Animation animation)
        {

        }
    }
}
