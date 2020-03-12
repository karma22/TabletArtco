
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Com.Bumptech.Glide;
using Java.Lang;

namespace TabletArtco
{
    [Activity(Label = "SettingActivity")]
    public class SettingActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            RequestedOrientation = Android.Content.PM.ScreenOrientation.Portrait;
            //Window.SetFlags(Android.Views.WindowManagerFlags.Fullscreen, Android.Views.WindowManagerFlags.Fullscreen);
            SetContentView(Resource.Layout.activity_setting);
            InitView();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public void InitView()
        {
            var version = VersionUtil.GetAppVersionName(this);
            TextView versionTv = FindViewById<TextView>(Resource.Id.versionTv);
            versionTv.Text = version == null ? "" : version;

            TextView cacheTv = FindViewById<TextView>(Resource.Id.cacheTv);
            cacheTv.Text = GlideUtil.GetCacheSize(Glide.GetPhotoCacheDir(this));

            FindViewById<RelativeLayout>(Resource.Id.cacheRl).Click += (t, e) =>
            {
                new Thread(new Runnable(() =>
                {
                    Glide.Get(this).ClearDiskCache();
                    RunOnUiThread(()=> {
                        ToastUtil.ShowToast(this, "缓存清除成功");
                        cacheTv.Text = GlideUtil.GetCacheSize(Glide.GetPhotoCacheDir(this));
                    });
                })).Start();
            };

            FindViewById<RelativeLayout>(Resource.Id.feedbackRl).Click += (t, e) => {
                Intent intent = new Intent(this, typeof(FeedbackActivity));
                StartActivity(intent);
            };
        }
    }
}
