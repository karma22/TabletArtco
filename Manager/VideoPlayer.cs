using System;
using Android.Content;
using Android.Graphics;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Views.Animations;
using Android.Widget;
using Com.Bumptech.Glide;
using Com.Bumptech.Glide.Request;
using Java.Lang;

namespace TabletArtco
{
    public class VideoPlayer : Java.Lang.Object, MediaPlayer.IOnPreparedListener, MediaPlayer.IOnCompletionListener, MediaPlayer.IOnErrorListener, MediaPlayer.IOnInfoListener, MediaPlayer.IOnSeekCompleteListener, Animation.IAnimationListener
    {
        private VideoView mVideoView;
        private ImageView mPreImgIv;
        private Context mCxt;
        private MediaPlayer soundPlayer;
        private bool isPlay;
        private string mPath;
        private string mSound;

        public VideoPlayer(VideoView videoView, ImageView preImgIv, Context context)
        {
            mVideoView = videoView;
            mPreImgIv = preImgIv;
            mCxt = context;
            init();
        }

        private void init() {
            mVideoView.SetOnCompletionListener(this);
            mVideoView.SetOnPreparedListener(this);
            mVideoView.SetOnInfoListener(this);
            mVideoView.SetOnErrorListener(this);
            PlayDefault();
        }

        private void PlayDefault() {
            Android.Net.Uri url = Android.Net.Uri.Parse("android.resource://" + mCxt.PackageName + "/raw/" + Resource.Raw.default_video);
            mVideoView.SetVideoURI(url);
            mVideoView.Start();
            HidePreImg();
        }

        private void HidePreImg()
        {
            new Thread(new Runnable(() =>
            {
                while (true)
                {
                    if (mVideoView.Duration > 4)
                    {
                        Animation animation = new AlphaAnimation(1, 0);
                        animation.Duration = 1000;
                        animation.SetAnimationListener(this);
                        mPreImgIv.Animation = animation;

                        break;
                    }
                }
            })).Start();
        }

        public void PreImageViewVisible()
        {
            mPreImgIv.ClearAnimation();
            mPreImgIv.Alpha = 1;
            mPreImgIv.Visibility = Android.Views.ViewStates.Visible;
        }

        public void SetPath(string path, string img, string sound)
        {
            mVideoView.Pause();
            StopSound();
            mVideoView.StopPlayback();

            mSound = sound;

            RequestOptions options = new RequestOptions().Placeholder(Resource.Drawable.home_bg).Frame(0);
            Glide.With(mCxt)
                .AsBitmap()
                .Load(path)
                .Apply(options)
                .Into(mPreImgIv);

            if (isPlay && mPath != null)
            {
                mPreImgIv.Visibility = Android.Views.ViewStates.Visible;
                if (path != null)
                {
                    Glide.With(mCxt).Load(GlideUtil.GetGlideUrl(path)).Apply(new Com.Bumptech.Glide.Request.RequestOptions().Placeholder(Resource.Drawable.home_bg)).Into(mPreImgIv);
                }
                else if (img != null)
                {
                    Glide.With(mCxt).Load(GlideUtil.GetGlideUrl(path)).Apply(new Com.Bumptech.Glide.Request.RequestOptions().Placeholder(Resource.Drawable.home_bg)).Into(mPreImgIv);
                }
            }

            mPath = path;
            if (path != null)
            {
                mVideoView.SetVideoPath(mPath);
            }
            mVideoView.SetVideoPath(mPath);

            if (isPlay)
            {
                new Thread(new Runnable(() =>
                {
                    Thread.Sleep(100);
                    Play();
                })).Start();
            }
        }

        public void ClickHomeBt() {
            mPath = null;
            PreImageViewVisible();
            mPreImgIv.SetImageResource(Resource.Drawable.home_bg);
            if (mVideoView.IsPlaying)
            {
                mVideoView.Pause();
            }
        }

        // play media
        public void Play()
        {
            isPlay = true;
            PlaySound();
            
            if (mPath == null)
            {
                return;
            }
            mVideoView.Start();
            //HidePreImg();
        }

        // Stop media
        public void Stop()
        {
            isPlay = false;
            StopSound();
            if (mPath == null)
            {
                return;
            }
            mVideoView.Pause();
            if (mVideoView.CanSeekBackward()) {
                mVideoView.SeekTo(0);
            }
        }

        public void PlaySound() {
            if (mSound != null && mSound.Length>0)
            {
                try {
                    soundPlayer?.Reset();
                    soundPlayer?.Release();
                    soundPlayer = new MediaPlayer();
                    soundPlayer.SetDataSource(mSound);
                    soundPlayer.Prepare();
                    soundPlayer.Start();
                } catch (Java.Lang.Exception e) {

                }
                
            }
        }

        public void StopSound() {
            soundPlayer?.Reset();
            soundPlayer?.Release();
            soundPlayer = null;
        }

        public void OnPrepared(MediaPlayer mp)
        {
            if(mp != soundPlayer)
            {
                mp.SetVolume(0f, 0f);
            }
            LogUtil.CustomLog("OnPrepared");
        }

        public void OnCompletion(MediaPlayer mp)
        {
            LogUtil.CustomLog("OnCompletion");
            if (isPlay)
            {
                if (mp == soundPlayer)
                {
                    PlaySound();
                }
                else {
                    if (mVideoView.CanSeekBackward())
                    {
                        mVideoView.SeekTo(0);
                    }
                    Play();
                }
            }
        }

        public bool OnError(MediaPlayer mp, [GeneratedEnum] MediaError what, int extra)
        {
            LogUtil.CustomLog("OnError");
            return false;
        }

        public bool OnInfo(MediaPlayer mp, [GeneratedEnum] MediaInfo what, int extra)
        {
            LogUtil.CustomLog("OnInfo");
            if(what == MediaInfo.VideoRenderingStart)
            {
                mPreImgIv.Visibility = Android.Views.ViewStates.Invisible;
            }
            return true;
        }

        public void OnSeekComplete(MediaPlayer mp)
        {
            LogUtil.CustomLog("OnSeekComplete");
        }

        public void OnAnimationEnd(Animation animation)
        {
            mPreImgIv.Visibility = Android.Views.ViewStates.Invisible;
        }

        public void OnAnimationRepeat(Animation animation)
        {
            
        }

        public void OnAnimationStart(Animation animation)
        {
            
        }
    }
}
