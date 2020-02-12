
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

namespace TabletArtco
{
    //[Activity(Theme = "@style/AppTheme", MainLauncher = true)]
    [Activity(Label = "TestActivity")]
    public class TestActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            Window.SetFlags(Android.Views.WindowManagerFlags.Fullscreen, Android.Views.WindowManagerFlags.Fullscreen);
            SetContentView(Resource.Layout.activity_test);
            RequestedOrientation = Android.Content.PM.ScreenOrientation.Landscape;
            
        }
    }
}
