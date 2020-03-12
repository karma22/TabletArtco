
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
using Java.Lang;

namespace TabletArtco
{
    [Activity(Label = "FeedbackActivity")]
    public class FeedbackActivity : Activity
    {
        private bool isSubmit = false;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            RequestedOrientation = Android.Content.PM.ScreenOrientation.Portrait;
            //Window.SetFlags(Android.Views.WindowManagerFlags.Fullscreen, Android.Views.WindowManagerFlags.Fullscreen);
            SetContentView(Resource.Layout.activity_feedback);
            InitView();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public void InitView()
        {
            EditText inputEt = FindViewById<EditText>(Resource.Id.inputEt);
            FindViewById<TextView>(Resource.Id.submitTv).Click += (t, e) => {
                if (inputEt.Text.Length <= 0) 
                {
                    ToastUtil.ShowToast(this, "提交内容不能为空");
                    return;
                }
                if (isSubmit)
                {
                    return;
                }
                inputEt.ClearFocus();
                
                isSubmit = true;
                new Thread(new Runnable(() =>
                {
                    Thread.Sleep(100);
                    RunOnUiThread(() => {
                        ToastUtil.ShowToast(this, "提交成功");
                        inputEt.Text = "";
                        isSubmit = false;
                    });
                })).Start();
            };
        }
    }
}
