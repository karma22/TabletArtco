﻿using Android.Content;
using Android.Media;
using Android.Runtime;
using Android.Views.Animations;
using Android.Widget;
using Com.Bumptech.Glide;
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
        public string mPath;
        private string mSound;

        private ImageView gifImageView;

        public VideoPlayer(VideoView videoView, ImageView preImgIv, Context context)
        {
            mVideoView = videoView;
            mPreImgIv = preImgIv;
            mCxt = context;
            init();
        }

        private void init() {
            gifImageView = ((Android.App.Activity)mCxt).FindViewById<ImageView>(Resource.Id.loading_imageview);
            Glide.With(mCxt).AsGif().Load(Resource.Drawable.gifLoding).Into(gifImageView);

            mVideoView.SetOnCompletionListener(this);
            mVideoView.SetOnPreparedListener(this);
            mVideoView.SetOnInfoListener(this);
            mVideoView.SetOnErrorListener(this);
        }

        public void PlayDefault() {
            if (ActivatedSprite.mIsFull)
                return;

            Android.Net.Uri url = Android.Net.Uri.Parse("android.resource://" + mCxt.PackageName + "/raw/" + Resource.Raw.default_video);
            mVideoView.SetVideoURI(url);
            mVideoView.Start();
            HidePreImg();
        }

        private void HidePreImg()
        {
            if (mPreImgIv == null) {
                return;
            }
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

        public void hideVideo()
        {
            mVideoView.Visibility = Android.Views.ViewStates.Invisible;
        }


        public void SetUri(int path, bool isRecycle)
        {
            mVideoView.Visibility = Android.Views.ViewStates.Visible;
            mVideoView.Pause();
            StopSound();
            mVideoView.StopPlayback();
            Android.Net.Uri url = Android.Net.Uri.Parse("android.resource://" + mCxt.PackageName + "/raw/" + path);
            mVideoView.SetVideoURI(url);
            HidePreImg();
            if (isPlay)
            {
                new Thread(new Runnable(() =>
                {
                    Thread.Sleep(100);
                    Play();
                })).Start();
            }
        }


        public void SetPath(string path, string img, string sound)
        {
            mVideoView.Visibility = Android.Views.ViewStates.Visible;
            mVideoView.Pause();
            StopSound();
            mVideoView.StopPlayback();

            mSound = sound;
            mPath = path;
            if (path != null)
            {
                mVideoView.SetVideoPath(mPath);                
                mVideoView.Start(); 
            }

           
            if(!isPlay && path != null)
            {
                gifImageView.Visibility = Android.Views.ViewStates.Visible;
            }

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
            mVideoView.Pause();
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
            gifImageView.Visibility = Android.Views.ViewStates.Invisible;

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
            //if(mp != soundPlayer)
            //{
            //    mp.SetVolume(0f, 0f);
            //}
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
                if(!isPlay && Project.currentBack != null)
                {                    
                    mVideoView.Pause();
                    mVideoView.SeekTo(0);
                }
                mPreImgIv.Visibility = Android.Views.ViewStates.Invisible;
                gifImageView.Visibility = Android.Views.ViewStates.Invisible;
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
