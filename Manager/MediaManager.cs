using System;
using Android.Graphics;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Views;
using System.Collections.Generic;
namespace TabletArtco
{
    public class MediaManager: Java.Lang.Object ,ISurfaceHolderCallback, MediaPlayer.IOnPreparedListener, MediaPlayer.IOnCompletionListener, MediaPlayer.IOnErrorListener
    {
        private SurfaceView mSurfaceView;
        private MediaPlayer mediaPlayer;
        //http://username:password@host:8080/directory/file?query#ref:
        private string mPath = "http://103.120.226.173/artco/backgrounds/3%20Playgrounds.mp4";
        private bool isOnPrepared = false;
        private bool isPlay = false;
        private ISurfaceHolder surfaceHolder;

        public MediaManager(SurfaceView surfaceView)
        {
            mSurfaceView = surfaceView;
            InitPlayer();
            
            InitSurfaceView();
        }

        public void SetPath(string path) {
            mPath = path;
            if (mediaPlayer != null)
            {
                try {
                    //Android.Net.Uri.Builder builder = new Android.Net.Uri.Builder();
                    //builder.Path(mPath);
                    //Android.Net.Uri uri = builder.Build();
                    //IDictionary<string, string> dic = new Dictionary<string, string>();
                    //dic.Add("Authorization", GlideUtil.GetAuthorization());
                    //mediaPlayer.SetDataSource(mSurfaceView.Context, uri, dic);
                    mediaPlayer.SetDataSource(mPath);
                } catch (Exception e) {
                    LogUtil.CustomLog("Set path for mediaplayer error " + e.ToString());
                }
            }
        }

        private void InitSurfaceView() {
            mSurfaceView.SetZOrderOnTop(false);
            //mSurfaceView.Holder.SetType(SurfaceType.PushBuffers);
            mSurfaceView.Holder.AddCallback(this);
        }

        private void InitPlayer() {
            mediaPlayer = new MediaPlayer();
            mediaPlayer.SetOnCompletionListener(this);
            mediaPlayer.SetOnErrorListener(this);
            mediaPlayer.SetOnPreparedListener(this);
            if (mPath != null)
            {
                SetPath(mPath);
            }
        }

        public void Play()
        {
            if (mediaPlayer != null && !mediaPlayer.IsPlaying && mPath != null && isOnPrepared)
            {
                mediaPlayer.Start();
            }
            else
            {
                isPlay = true;
            }
        }

        public void Stop()
        {
            if (mediaPlayer != null && mediaPlayer.IsPlaying)
            {
                mediaPlayer.Pause();
            }
        }

        public void OnCompletion(MediaPlayer mp)
        {
            try
            {
                mp.SeekTo(0);
                mp.Start();
            }
            catch (Exception e)
            {
                LogUtil.CustomLog(e.ToString());
            }
            
        }

        public bool OnError(MediaPlayer mp, [GeneratedEnum] MediaError what, int extra)
        {
            return false;
        }

        public void SurfaceChanged(ISurfaceHolder holder, [GeneratedEnum] Format format, int width, int height)
        {
            
        }

        public void SurfaceCreated(ISurfaceHolder holder)
        {
            surfaceHolder = holder;
            mediaPlayer.SetDisplay(holder);
            try
            {
                mediaPlayer.PrepareAsync();
            }
            catch (Exception e)
            {
                LogUtil.CustomLog(e.ToString());
            }
            
        }

        public void SurfaceDestroyed(ISurfaceHolder holder)
        {
            surfaceHolder = null;
        }

        void MediaPlayer.IOnPreparedListener.OnPrepared(MediaPlayer mp)
        {
            isOnPrepared = true;
            if (isPlay)
            {
                mp.Start();
            }
        }


    }
}
