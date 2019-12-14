
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
using Android.Graphics;

namespace TabletArtco
{
    [Activity(Label = "EducationActivity")]
    public class EducationActivity : Activity, DataSource, Delegate
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
                Resource.Drawable.search_bg, Resource.Drawable.ebs_codingpractice_tab, Resource.Drawable.ebs_codingapplied1_tab,
                Resource.Drawable.ebs_codingapplied2_tab, Resource.Drawable.ebs_movementblockpractice_tab, Resource.Drawable.ebs_controlblockpractice_tab
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
                    LinearLayout.LayoutParams lp = new LinearLayout.LayoutParams(i + 2 >= resIds.Length ? (int)(125 / 92.0 * itemW) : itemW, itemH);
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
                        UpdateView();
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
        }

        private void UpdateView()
        {
            GridView gridView = FindViewById<GridView>(Resource.Id.gridview);
            GridAdapter adapter = (GridAdapter)gridView.Adapter;
            adapter.NotifyDataSetChanged();
        }

        // Delegate interface
        public int GetItemsCount()
        {
            List<List<Background>> backgrounds = Background._backgrounds;
            Android.Util.Log.Info("fff", backgrounds+"");
            //for (int i = 0; i < backgrounds.Count; i++)
            //{
            //    Android.Util.Log.Info("fff", i + ""); 
            //    Android.Util.Log.Info("fff", backgrounds[i] + "");
            //}
            if (mIndex < backgrounds.Count)
            {
                return backgrounds[mIndex].Count;
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
            List<List<Background>> backgrounds = Background._backgrounds;
            if (mIndex >= backgrounds.Count)
            {
                return;
            }
            List<Background> list = backgrounds[mIndex];
            Background background = list[position];
            //contentView.SetBackgroundColor(Color.Red);
            ViewHolder viewHolder = (ViewHolder)contentView.Tag;
            Android.Util.Log.Info("background", background.remotePreviewImgPath+"");
            Glide.With(this).Load(background.remotePreviewImgPath).Into(viewHolder.bgIv);
        }

        //定义ViewHolder内部类，用于对控件实例进行缓存
        class ViewHolder : Java.Lang.Object
        {
            public ImageView bgIv;
            public ImageView imgIv;
        }
    }
}
