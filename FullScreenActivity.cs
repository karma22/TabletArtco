
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Net;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Com.Bumptech.Glide;
using Com.Bumptech.Glide.Request;
using Java.Lang;

namespace TabletArtco
{
    [Activity(Label = "FullScreenActivity", LaunchMode = Android.Content.PM.LaunchMode.SingleTop)]
    public class FullScreenActivity : Activity, UpdateDelegate, IServiceConnection, RecordUtil.RecordListener
    {
        private static string Tag = "FullScreenActivity";
        private List<DragImgView> imgList = new List<DragImgView>();
        private List<SpeakView> speakViewList = new List<SpeakView>();
        private bool isRecord;
        private bool isPlay;
        private string name;
        //private MediaManager mediaManager;
        private VideoPlayer videoPlayer;
        private SoundPlayer bgmPlayer;
        private ImageView recordBt;
        private TextView timeTv;
        int mRequestCode = 1;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            Window.SetFlags(Android.Views.WindowManagerFlags.Fullscreen, Android.Views.WindowManagerFlags.Fullscreen);
            RequestedOrientation = Android.Content.PM.ScreenOrientation.Landscape;
            SetContentView(Resource.Layout.activity_full);
            InitView();
        }

        protected override void OnResume()
        {
            base.OnResume();
            ActivatedSprite.mUpdateDelegate = this;
            ActivatedSprite.SoundAction = (sound) =>
            {
                RunOnUiThread(() => {
                    if (isPlay)
                    {
                        new SoundPlayer(this).Play(sound);
                    }
                });
            };
            if (Project.currentBack != null)
            {
                Glide.With(this)
                    .Load(Project.currentBack.remotePreviewImgPath.Equals("") ? Project.currentBack.remoteVideoPath : Project.currentBack.remotePreviewImgPath)
                    .Apply(new RequestOptions().Placeholder(Resource.Drawable.home_bg))
                    .Into(FindViewById<ImageView>(Resource.Id.preImg));
                videoPlayer.SetPath(Project.currentBack.remoteVideoPath, Project.currentBack.remotePreviewImgPath, null);
            }
            AddSpriteView();   
        }

        protected override void OnPause()
        {
            base.OnPause();
            Project.ChangeMode(false);

            if (!isPlay)
            {
                return;
            }
            isPlay = false;

            Project.StopSprite();
            if (Project.currentBack != null)
            {
                //videoPlayer.SetPath(Project.currentBack.remoteVideoPath, Project.currentBack.remotePreviewImgPath, null);
            }
            videoPlayer.Stop();
            SoundPlayer.StopAll();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (isRecord)
            {
                RecordUtil.stopScreenRecord(this);
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            foreach (Android.Content.PM.Permission permission in grantResults) {
                if (permission == Android.Content.PM.Permission.Denied)
                {
                    Android.App.AlertDialog.Builder builder = new AlertDialog.Builder(this);
                    builder.SetTitle("申请权限");
                    builder.SetMessage("这些权限很重要");
                    builder.SetNegativeButton("取消",(s, e)=> { 
                    
                    });
                    builder.SetPositiveButton("设置", (s, e) => {
                        Intent intent = new Intent();
                        intent.SetAction(Android.Provider.Settings.ActionApplicationDetailsSettings);
                        intent.SetData(Uri.Parse("package:" + PackageName));
                        StartActivity(intent);
                    });

                    AlertDialog dialog = builder.Create();
                    dialog.Show();
                    break;
                }
            }
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == mRequestCode && resultCode == Result.Ok)
            {
                try
                {
                    RecordUtil.setUpData((int)resultCode, data);
                    changeRecordState(true);
                    startAnimation();
                }
                catch (Exception e)
                {
                    e.PrintStackTrace();
                }
            }
            else
            {
                ToastUtil.ShowToast(this, "拒绝录屏");
                startAnimation();
            }
        }

        // init view
        public void InitView()
        {
            Project.ChangeMode(true); 
            VideoView videoView = FindViewById<VideoView>(Resource.Id.videoview);
            ImageView imgIv = FindViewById<ImageView>(Resource.Id.preImg);
            videoPlayer = new VideoPlayer(videoView, imgIv, this);
            
            if (Project.currentBack != null)
            {
                Glide.With(this)
                    .Load(Project.currentBack.remotePreviewImgPath.Equals("") ? Project.currentBack.remoteVideoPath : Project.currentBack.remotePreviewImgPath)
                    .Apply(new RequestOptions().Placeholder(Resource.Drawable.home_bg))
                    .Into(imgIv);

                videoPlayer.SetPath(Project.currentBack.remoteVideoPath, Project.currentBack.remotePreviewImgPath, null);
            }

            bgmPlayer = new SoundPlayer(this);
            recordBt = FindViewById<ImageView>(Resource.Id.recordBt);
            FindViewById<ImageView>(Resource.Id.recordBt).Click += (t, e) => {

                if (!isRecord) {
                    SaveDialog dialog = new SaveDialog(this, (text) =>
                    {
                        name = text;
                    });
                    dialog.Show();
                }
            };

            FindViewById<ImageView>(Resource.Id.playBt).Click += (t, e) => {
                Android.Util.Log.Info(Tag, "Click play animation start");
                if (isPlay)
                {
                    return;
                }

                if (name != null)
                {
                    RecordUtil.startScreenRecord(this, 1, name);
                }
                else {
                    startAnimation();
                }
            };

            FindViewById<ImageView>(Resource.Id.stopBt).Click += (t, e) => {
                Android.Util.Log.Info(Tag, "Click play animation stop");
                changeRecordState(false);
                if (!isPlay)
                {
                    return;
                }
                isPlay = false;
                Project.StopSprite();
                if (Project.currentBack != null)
                {
                    //videoPlayer.SetPath(Project.currentBack.remoteVideoPath, Project.currentBack.remotePreviewImgPath, null);
                }
                videoPlayer.Stop();
                SoundPlayer.StopAll();
            };

            FindViewById<ImageView>(Resource.Id.closeBt).Click += (t, e) => {
                FindViewById<ImageView>(Resource.Id.stopBt).PerformClick();
                Finish();
            };

            Intent service = new Intent(this, typeof(ScreenRecordService));
            //StartService(service);
            ApplicationContext.BindService(service, this, Bind.AutoCreate);
            RecordUtil.addRecordListener(this);
            PermissionUtil.checkPermission(this);

            AddSpriteView();
        }

        private void startAnimation() {
            if (!Project.RunSprite(this))
                return;

            isPlay = true;            
            videoPlayer.Play();
            bgmPlayer.Play(SoundPlayer.bgmPath);
        }

        // main screen add animate sprite view
        public void AddSpriteView()
        {
            FrameLayout containerView = FindViewById<FrameLayout>(Resource.Id.FullContainerView);
            containerView.RemoveAllViews();
            imgList.RemoveRange(0, imgList.Count);
            speakViewList.RemoveRange(0, speakViewList.Count);
            for (int i = 0; i < Project.mSprites.Count; i++)
            {
                ActivatedSprite activatedSprite = Project.mSprites[i];
                DragImgView imgIv = new DragImgView(this);
                imgIv.SetImageBitmap(activatedSprite.GetSpriteBit());
                FrameLayout.LayoutParams layoutParams = new FrameLayout.LayoutParams(activatedSprite.curSize.Width, activatedSprite.curSize.Height);
                layoutParams.LeftMargin = activatedSprite.curPoint.X;
                layoutParams.TopMargin = activatedSprite.curPoint.Y;
                containerView.AddView(imgIv, layoutParams);
                imgList.Add(imgIv);
                imgIv.Tag = 100 + i;
                imgIv.MoveAction += (t, x, y) =>
                {
                    if (!isPlay)
                    {
                        activatedSprite.AddToOriginPoint((int)x, (int)y);
                    }
                };
                imgIv.ClickAction += (t) =>
                {
                    int tag = (int)((DragImgView)t).Tag;
                    if (!isPlay)
                    {
                        //Intent intent = new Intent(this, typeof(EditActivity));
                        //Bundle bundle = new Bundle();
                        //bundle.PutInt("position", tag - 100);
                        //intent.PutExtra("bundle", bundle);
                        //StartActivityForResult(intent, 10, null);
                    }
                    else
                    {
                        if (tag - 100 < Project.mSprites.Count)
                        {
                            Project.mSprites[tag - 100].ReceiveClickSignal();
                        }
                    }
                };
                SpeakView speakView = new SpeakView(this);
                FrameLayout.LayoutParams p = new FrameLayout.LayoutParams(FrameLayout.LayoutParams.WrapContent, FrameLayout.LayoutParams.WrapContent);
                containerView.AddView(speakView, p);
                speakViewList.Add(speakView);
            }
        }

        // update main screen animate sprite imageview location and visibility
        public void UpdateMainView()
        {
            FrameLayout containerView = FindViewById<FrameLayout>(Resource.Id.FullContainerView);
            for (int i = 0; i < imgList.Count; i++)
            {
                ActivatedSprite activatedSprite = Project.mSprites[i];
                DragImgView imgIv = imgList[i];
                imgIv.SetImageBitmap(activatedSprite.GetSpriteBit());
                FrameLayout.LayoutParams layoutParams = (FrameLayout.LayoutParams)imgIv.LayoutParameters;
                layoutParams.LeftMargin = activatedSprite.curPoint.X;
                layoutParams.TopMargin = activatedSprite.curPoint.Y;
                layoutParams.Width = activatedSprite.curSize.Width;
                layoutParams.Height = activatedSprite.curSize.Height;
                imgIv.Visibility = activatedSprite.isVisible ? ViewStates.Visible : ViewStates.Invisible;
                containerView.UpdateViewLayout(imgIv, layoutParams);

                SpeakView speakView = speakViewList[i];
                speakView.SetSpeakText(activatedSprite.speakText);
                if (speakView.Visibility == ViewStates.Visible)
                {
                    FrameLayout.LayoutParams p = (FrameLayout.LayoutParams)speakView.LayoutParameters;
                    p.LeftMargin = activatedSprite.curPoint.X;
                    p.TopMargin = activatedSprite.curPoint.Y - 100;
                    containerView.UpdateViewLayout(speakView, p);
                }
            }
            JudgeCollision();
        }

        // check Collision
        public void JudgeCollision()
        {
            if (!isPlay || imgList.Count <= 1)
            {
                return;
            }
            new Java.Lang.Thread(new Java.Lang.Runnable(() =>
            {
                for (int i = 0; i < imgList.Count; i++)
                {
                    DragImgView firstIv = imgList[i];
                    for (int j = i + 1; j < imgList.Count; j++)
                    {
                        DragImgView twoIv = imgList[j];
                        FrameLayout.LayoutParams p1 = (FrameLayout.LayoutParams)firstIv.LayoutParameters;
                        FrameLayout.LayoutParams p2 = (FrameLayout.LayoutParams)twoIv.LayoutParameters;
                        Rect r1 = new Rect(p1.LeftMargin, p1.TopMargin, p1.LeftMargin + firstIv.Width, p1.TopMargin + firstIv.Height);
                        Rect r2 = new Rect(p2.LeftMargin, p2.TopMargin, p2.LeftMargin + twoIv.Width, p2.TopMargin + twoIv.Height);
                        if (RectUtil.intersect(r1, r2))
                        {
                            if (i < Project.mSprites.Count && j < Project.mSprites.Count)
                            {
                                // send collision signal
                                ActivatedSprite activatedSprite1 = Project.mSprites[i];
                                ActivatedSprite activatedSprite2 = Project.mSprites[j];
                                activatedSprite1.ReceiveCollisionSignal(activatedSprite2.activateSpriteId);
                                activatedSprite2.ReceiveCollisionSignal(activatedSprite1.activateSpriteId);
                            }
                        }
                    }
                }
            })).Start();
        }

        /*
         * ActivateSprite interface
         */
        public void UpdateView()
        {
            RunOnUiThread(() => {
                UpdateMainView();
            });
        }

        public void UpdateBlockViewDelegate()
        {
            // do nothing
        }

        public void UpdateBackground(string backgroundId)
        {
            RunOnUiThread(() => {
                Background background = Project.backgroundsList[backgroundId];
                if (background != null)
                {
                    videoPlayer.SetPath(background.remoteVideoPath, background.remotePreviewImgPath, null);
                }
            });
        }

        private void changeRecordState(bool isStart) {
            if (isStart)
            {
                isRecord = true;
                
            }
            else {
                if (isRecord)
                {
                    recordBt.Visibility = ViewStates.Visible;
                    FindViewById<TextView>(Resource.Id.timeTv).Text = "";
                    isRecord = false;
                    RecordUtil.stopScreenRecord(this);
                    name = null;
                }
            }
        }

        public void OnServiceConnected(ComponentName name, IBinder service)
        {
            ScreenRecordService.RecordBinder recordBinder = (ScreenRecordService.RecordBinder)service;
            ScreenRecordService screenRecordService = recordBinder.getRecordService();
            RecordUtil.setScreenService(screenRecordService);
        }

        public void OnServiceDisconnected(ComponentName name)
        {
           
        }

        void RecordUtil.RecordListener.onStartRecord()
        {
           
        }

        void RecordUtil.RecordListener.onPauseRecord()
        {
           
        }

        void RecordUtil.RecordListener.onResumeRecord()
        {
           
        }

        void RecordUtil.RecordListener.onStopRecord(string stopTip)
        {
            ToastUtil.ShowToast(this, stopTip);
        }

        void RecordUtil.RecordListener.onRecording(string timeTip)
        {
            FindViewById<TextView>(Resource.Id.timeTv).Text = timeTip;
            if (isRecord)
            {
                recordBt.Visibility = recordBt.Visibility == ViewStates.Visible ? ViewStates.Invisible : ViewStates.Visible;
            }
            else {
                recordBt.Visibility = ViewStates.Visible;
            }
        }

        public void RowAnimateComplete()
        {
            
        }
    }
}
 