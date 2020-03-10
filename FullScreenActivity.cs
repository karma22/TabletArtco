
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace TabletArtco
{
    [Activity(Label = "FullScreenActivity")]
    public class FullScreenActivity : Activity, UpdateDelegate
    {
        private static string Tag = "FullScreenActivity";
        private List<DragImgView> imgList = new List<DragImgView>();
        private List<SpeakView> speakViewList = new List<SpeakView>();
        private bool isPlay;
        private MediaManager mediaManager;
        

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
            AddSpriteView();   
        }

        protected override void OnPause()
        {
            base.OnPause();
            mediaManager.Stop();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        // init view
        public void InitView()
        {

        }

        // main screen add animate sprite view
        public void AddSpriteView()
        {
            FrameLayout containerView = FindViewById<FrameLayout>(Resource.Id.ContainerView);
            containerView.RemoveAllViews();
            imgList.RemoveRange(0, imgList.Count);
            speakViewList.RemoveRange(0, speakViewList.Count);
            for (int i = 0; i < Project.mSprites.Count; i++)
            {
                ActivatedSprite activatedSprite = Project.mSprites[i];
                DragImgView imgIv = new DragImgView(this);
                FrameLayout.LayoutParams layoutParams = new FrameLayout.LayoutParams(activatedSprite.curSize.Width, activatedSprite.curSize.Height);
                layoutParams.LeftMargin = activatedSprite.curPoint.X;
                layoutParams.TopMargin = activatedSprite.curPoint.Y;
                containerView.AddView(imgIv, layoutParams);
                imgIv.SetImageBitmap(activatedSprite.GetSpriteBit());
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
                        Intent intent = new Intent(this, typeof(EditActivity));
                        Bundle bundle = new Bundle();
                        bundle.PutInt("position", tag - 100);
                        intent.PutExtra("bundle", bundle);
                        StartActivityForResult(intent, 10, null);
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
            FrameLayout containerView = FindViewById<FrameLayout>(Resource.Id.ContainerView);
            for (int i = 0; i < imgList.Count; i++)
            {
                ActivatedSprite activatedSprite = Project.mSprites[i];
                DragImgView imgIv = imgList[i];
                FrameLayout.LayoutParams layoutParams = (FrameLayout.LayoutParams)imgIv.LayoutParameters;
                layoutParams.LeftMargin = activatedSprite.curPoint.X;
                layoutParams.TopMargin = activatedSprite.curPoint.Y;
                layoutParams.Width = activatedSprite.curSize.Width;
                layoutParams.Height = activatedSprite.curSize.Height;
                imgIv.SetImageBitmap(activatedSprite.GetSpriteBit());
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
            
        }
    }
}
