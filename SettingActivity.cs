
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
    [Activity(Label = "SettingActivity", LaunchMode = Android.Content.PM.LaunchMode.SingleTop)]
    public class SettingActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            RequestedOrientation = Android.Content.PM.ScreenOrientation.Landscape;
            Window.SetFlags(Android.Views.WindowManagerFlags.Fullscreen, Android.Views.WindowManagerFlags.Fullscreen);
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
            Intent intent = new Intent();
            Bundle bundle = new Bundle();

            FindViewById<ImageView>(Resource.Id.bg_blue).Click += (t, e) => {    
                bundle.PutString("model", "blue");
                intent.PutExtra("bundle", bundle);

                SetResult(Result.Ok, intent);
                Finish();
            };

            FindViewById<ImageView>(Resource.Id.bg_red).Click += (t, e) => {
                bundle.PutString("model", "red");
                intent.PutExtra("bundle", bundle);

                SetResult(Result.Ok, intent);
                Finish();
            };

            FindViewById<ImageView>(Resource.Id.bg_yellow).Click += (t, e) => {
                bundle.PutString("model", "yellow");
                intent.PutExtra("bundle", bundle);

                SetResult(Result.Ok, intent);
                Finish();
            };

            FindViewById<ImageView>(Resource.Id.bg_black).Click += (t, e) => {
                bundle.PutString("model", "black");
                intent.PutExtra("bundle", bundle);

                SetResult(Result.Ok, intent);
                Finish();
            };

        }
    }
}
