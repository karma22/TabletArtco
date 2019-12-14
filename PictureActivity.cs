
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
using Android.Util;
using System.Net;
using System.IO;
using Android.Graphics;
using Java.Lang;
using Com.Bumptech.Glide;


namespace TabletArtco
{
    [Activity(Label = "PictureActivity")]
    public class PictureActivity : Activity, DataSource, Delegate
    {

        private int mItemW;
        private int mItemH;
        private int mIndex;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            Window.SetFlags(Android.Views.WindowManagerFlags.Fullscreen, Android.Views.WindowManagerFlags.Fullscreen);
            SetContentView(Resource.Layout.activity_grid);
            RequestedOrientation = Android.Content.PM.ScreenOrientation.Landscape;
            InitView();
            GetData();
        }

        private void InitView()
        {
            int width = ScreenUtil.ScreenWidth(this);
            int height = ScreenUtil.ScreenHeight(this);
            int margin = (int)(20 / 1280.0 * width);
            int w = width - margin * 2;
            int topH = (int)(60 / 800.0 * height);
            int conH = height - topH - margin;

            ScrollView scrollView = FindViewById<ScrollView>(Resource.Id.scrollView);
            LinearLayout.LayoutParams svParams = (LinearLayout.LayoutParams)scrollView.LayoutParameters;
            svParams.Width = width;
            svParams.Height = topH;
            scrollView.LayoutParameters = svParams;
            scrollView.SetPadding(margin, 0, margin, 0);
            LinearLayout topView = FindViewById<LinearLayout>(Resource.Id.grid_top_view);
            int[] resIds = {
                Resource.Drawable.search_bg, Resource.Drawable.ps_user_tab, Resource.Drawable.ps_momochung_tab,
                Resource.Drawable.ps_sea_tab, Resource.Drawable.ps_animal_tab, Resource.Drawable.ps_plants_tab,
                Resource.Drawable.ps_insect_tab, Resource.Drawable.ps_character_tab, Resource.Drawable.ps_food_tab,
                Resource.Drawable.ps_traffic_tab, Resource.Drawable.ps_object_tab, Resource.Drawable.ps_etc_tab
            };
            int editTvH = (int)(30 / 60.0 * topH);
            int editTvW = (int)(166 / 35.0 * editTvH);
            int itemH = (int)(34 / 60.0 * topH);
            int itemW = (int)(92 / 35.0 * itemH);
            for (int i = 0; i < resIds.Length; i++)
            {
                if (i == 0)
                {
                    LinearLayout.LayoutParams lp = new LinearLayout.LayoutParams(editTvW, editTvH);
                    EditText searchEt = new EditText(this);
                    searchEt.LayoutParameters = lp;
                    searchEt.SetBackgroundResource(resIds[i]);
                    topView.AddView(searchEt);
                }
                else
                {
                    LinearLayout.LayoutParams lp = new LinearLayout.LayoutParams(itemW, itemH);
                    lp.LeftMargin = margin / 3;
                    ImageView imgIv = new ImageView(this);
                    imgIv.Tag = i;
                    imgIv.LayoutParameters = lp;
                    imgIv.SetImageResource(resIds[i]);
                    topView.AddView(imgIv);
                    imgIv.Click += (t, e) =>
                    {
                        int tag = (int)(((ImageView)t).Tag);
                        mIndex = tag - 1;
                        updateView();
                    };
                }
            }

            int spacing = 20;
            LinearLayout conView = FindViewById<LinearLayout>(Resource.Id.grid_wrapper_view);
            LinearLayout.LayoutParams conParams = (LinearLayout.LayoutParams)conView.LayoutParameters;
            conParams.Width = w;
            conParams.Height = conH;
            conParams.LeftMargin = margin;
            conView.LayoutParameters = conParams;
            conView.SetPadding(spacing, spacing, spacing, spacing);

            int columnCount = 4;
            mItemW = (int)((w - (columnCount + 1) * spacing * 1.0) / columnCount);
            mItemH = mItemW / 2;

            GridView gridView = FindViewById<GridView>(Resource.Id.gridview);
            gridView.SetColumnWidth(200);
            gridView.SetNumColumns(columnCount);
            gridView.SetVerticalSpacing(spacing);
            gridView.SetHorizontalSpacing(spacing);
            gridView.Adapter = new GridAdapter((DataSource)this, (Delegate)this);
            GridAdapter adapter = new GridAdapter((DataSource)this, (Delegate)this);

        }

        private void updateView() {
            GridView gridView = FindViewById<GridView>(Resource.Id.gridview);
            GridAdapter adapter = (GridAdapter)gridView.Adapter;
            adapter.NotifyDataSetChanged();
        }

        private void GetData()
        {
            //Log.Info("====", "==============");

            //List<List<Sprite>> sprites = Sprite._sprites;
            
            //for (int i = 0; i < sprites.Count; i++)
            //{
            //    List<Sprite> l = sprites[i];
            //    for (int j = 0; j < l.Count; j++)
            //    {
            //        Sprite s = l[j];
            //        Log.Info("tag", "==========" + s.name);
            //    }
            //}
            
        }

        // Delegate interface
        public int GetItemsCount()
        {
            List<List<Sprite>> sprites = Sprite._sprites;
            if (mIndex<sprites.Count)
            {
                return sprites[mIndex].Count;
            }
            return 0;
        }

        public View GetItemView(ViewGroup parent)
        {
            View convertView = LayoutInflater.From(this).Inflate(Resource.Layout.selected_material_item, parent, false);
            ViewUtil.SetViewSize(convertView, mItemW, mItemH);
            ViewHolder holder = new ViewHolder();
            holder.bgIv = convertView.FindViewById<ImageView>(Resource.Id.selected_material_bgIv);
            holder.imgIv = convertView.FindViewById<ImageView>(Resource.Id.selected_material_imgIv);
            convertView.Tag = holder;
            return convertView;
        }

        public void UpdateItemView(View contentView, int position)
        {
            List<List<Sprite>> sprites = Sprite._sprites;   
            if (mIndex >= sprites.Count)
            {
                return;
            }
            List<Sprite> list = sprites[mIndex];
            Sprite sprite = list[position];
            ViewHolder viewHolder = (ViewHolder)contentView.Tag;
            contentView.SetBackgroundColor(Android.Graphics.Color.Red);
            Glide.With(this).Load(sprite.remotePath).Into(viewHolder.bgIv);
            //new Thread(new Runnable(() =>
            //{
            //    Stream stream = FTPManager.GetStreamFromFTP(sprite.remotePath);
            //    RunOnUiThread(() =>
            //    {
            //        if (stream != null)
            //        {
            //            Log.Info("====tag====", stream.ToString());
            //            viewHolder.bgIv.SetImageBitmap(BitmapFactory.DecodeStream(stream));
            //        }
            //    });
            //})).Start();
        }

        private void action() {

        }

        //定义ViewHolder内部类，用于对控件实例进行缓存
        class ViewHolder : Java.Lang.Object
        {
            public ImageView bgIv;
            public ImageView imgIv;
        }
    }
}
