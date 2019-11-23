using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Collections.Generic;
using System;
using Android.Util;
using Android.Graphics;


namespace TabletArtco {
    [Activity(Theme = "@style/AppTheme")]
    public class MainActivity : AppCompatActivity {
        private RecyclerView recyclerView;
        private List<ImageView> btList =  new List<ImageView>();

        protected override void OnCreate(Bundle savedInstanceState) {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);
            RequestedOrientation = Android.Content.PM.ScreenOrientation.Landscape;
            initView();
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults) {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public void initView() {
            int[] btsResIds = {
                Resource.Id.bt_choice1,
                Resource.Id.bt_choice2,
                Resource.Id.bt_choice3,
                Resource.Id.bt_choice4,
                Resource.Id.bt_choice5,
                Resource.Id.bt_choice6,
                Resource.Id.bt_choice7,
                Resource.Id.bt_choice8,
                Resource.Id.bt_choice9
            };

            int height = (int)((ScreenUtil.ScreenHeight(this)-ScreenUtil.StatusBarHeight(this)) * 82 / 800.0-18);           
            for (int i = 0; i < 9; i++) {
                ImageView imgBt = FindViewById<ImageView>(btsResIds[i]);
                ViewUtil.setViewSize(imgBt, (int)(height*73/70.0), height);
                btList.Add(imgBt);
            }
            //recyclerView = FindViewById<RecyclerView>(Resource.Id.topRecyclerView);
            //LinearLayoutManager manager = new LinearLayoutManager(this);
            //manager.Orientation = LinearLayoutManager.Horizontal;
            //manager.ReverseLayout = true;
            //TopBarAdapter topBarAdapter = new TopBarAdapter(this);
            //recyclerView.SetLayoutManager(manager);
            //recyclerView.SetAdapter(topBarAdapter);
            //topBarAdapter.NotifyDataSetChanged();
        }

    }
}