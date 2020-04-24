
using System;
using System.Collections.Generic;

using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Com.Bumptech.Glide;
using Android.Content;

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
            RequestedOrientation = Android.Content.PM.ScreenOrientation.Landscape;
            Window.SetFlags(Android.Views.WindowManagerFlags.Fullscreen, Android.Views.WindowManagerFlags.Fullscreen);
            SetContentView(Resource.Layout.activity_grid);
            InitView();
        }

        // init view
        private void InitView()
        {
            int width = ScreenUtil.ScreenWidth(this);
            int height = ScreenUtil.ScreenHeight(this);
            int margin = (int)(20 / 1280.0 * width);
            int padding = 4;
            int w = width - margin * 2;
            //int topH = (int)(60 / 800.0 * height);
            int topH = (int)(90 / 975.0 * height);
            int conH = height - topH - margin;

            ScrollView scrollView = FindViewById<ScrollView>(Resource.Id.scrollView);
            LinearLayout.LayoutParams svParams = (LinearLayout.LayoutParams)scrollView.LayoutParameters;
            svParams.Width = width;
            svParams.Height = topH;
            scrollView.LayoutParameters = svParams;
            scrollView.SetPadding(margin, (int)(10/975.0*height), margin, 0);
            LinearLayout topView = FindViewById<LinearLayout>(Resource.Id.grid_top_view);
            int[] resIds = {
                Resource.Drawable.search_bg, Resource.Drawable.User_tab, Resource.Drawable.momochung_tab,
                Resource.Drawable.ps_sea_tab, Resource.Drawable.ps_animal_tab, Resource.Drawable.ps_plants_tab,
                Resource.Drawable.ps_insect_tab, Resource.Drawable.ps_character_tab, Resource.Drawable.ps_food_tab,
                Resource.Drawable.ps_traffic_tab, Resource.Drawable.ps_object_tab, Resource.Drawable.National_studies_tab
            };
            int editTvH = (int)(38 / 90.0 * topH);
            int editTvW = (int)(166 / 35.0 * editTvH);
            int itemH = (int)(42 / 90.0 * topH);
            int itemW = (int)(92 / 35.0 * itemH);
            for (int i = 0; i < resIds.Length; i++)
            {
                if (i == 0)
                {
                    LinearLayout.LayoutParams lp = new LinearLayout.LayoutParams(editTvW, editTvH);
                    EditText searchEt = new EditText(this);
                    searchEt.LayoutParameters = lp;
                    searchEt.SetPadding((int)(30 / 166.0 * editTvW), 0, 0, 0);
                    searchEt.Gravity = GravityFlags.CenterVertical;
                    searchEt.SetBackgroundResource(resIds[i]);
                    topView.AddView(searchEt);
                }
                else
                {
                    LinearLayout.LayoutParams lp = new LinearLayout.LayoutParams(itemW + 2*padding, itemH + 2*padding);

                    ImageView imgIv = new ImageView(this);
                    imgIv.SetImageResource(resIds[i]);

                    FrameLayout frameLayout = new FrameLayout(this);
                    frameLayout.LayoutParameters = lp;
                    frameLayout.SetPadding(padding, padding, padding, padding);
                    frameLayout.Tag = i;
                    if (i == 1)
                        frameLayout.SetBackgroundResource(Resource.Drawable.tab_select);

                    frameLayout.AddView(imgIv);
                    topView.AddView(frameLayout);

                    frameLayout.Click += (t, e) =>
                    {
                        int tag = (int)(((FrameLayout)t).Tag);
                        mIndex = tag - 1;

                        for (int j = 1; j < resIds.Length; j++)
                        {
                            FrameLayout fl = (FrameLayout)topView.GetChildAt(j);
                            fl.Background = null;
                        }
                        ((FrameLayout)t).SetBackgroundResource(Resource.Drawable.tab_select);

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

            int columnCount = 8;
            mItemW = (int)((w - (columnCount + 1) * spacing * 1.0) / columnCount);
            mItemH = mItemW; // 2;
            
            GridView gridView = FindViewById<GridView>(Resource.Id.gridview);
            gridView.SetColumnWidth(200);
            gridView.SetNumColumns(columnCount);
            gridView.SetVerticalSpacing(spacing*2);
            gridView.SetHorizontalSpacing(spacing);
            gridView.Adapter = new GridAdapter((DataSource)this, (Delegate)this);
            GridAdapter adapter = new GridAdapter((DataSource)this, (Delegate)this);

        }

        // update listview
        private void UpdateView()
        {
            GridView gridView = FindViewById<GridView>(Resource.Id.gridview);
            GridAdapter adapter = (GridAdapter)gridView.Adapter;
            adapter.NotifyDataSetChanged();
        }

        /*
         *
         * Delegate interface
         * 
         */
        public int GetItemsCount(Java.Lang.Object adapter)
        {
            List<List<Sprite>> sprites = Sprite._sprites;
            if (mIndex<sprites.Count)
            {
                return sprites[mIndex].Count;
            }
            return 0;
        }

        public View GetItemView(Java.Lang.Object adapter, ViewGroup parent)
        {
            View convertView = LayoutInflater.From(this).Inflate(Resource.Layout.item_sprite, parent, false);
            ViewUtil.SetViewSize(convertView, mItemW, mItemH);
            ViewHolder holder = new ViewHolder();
            holder.bgIv = convertView.FindViewById<ImageView>(Resource.Id.selected_material_bgIv);
            holder.imgIv = convertView.FindViewById<ImageView>(Resource.Id.selected_material_imgIv);
            holder.txtTv = convertView.FindViewById<TextView>(Resource.Id.sprite_tv);
            convertView.Tag = holder;
            convertView.Click += (t, e) =>
            {
                ViewHolder viewHolder = (ViewHolder)(((View)t).Tag);
                int position = (int)viewHolder.txtTv.Tag;
                ClickItem(position);
            };
            return convertView;
        }

        public void UpdateItemView(Java.Lang.Object adapter, View contentView, int position)
        {
            List<List<Sprite>> sprites = Sprite._sprites;   
            if (mIndex >= sprites.Count)
            {
                return;
            }
            List<Sprite> list = sprites[mIndex];
            Sprite sprite = list[position];
            ViewHolder viewHolder = (ViewHolder)contentView.Tag;

            Glide.With(this).Load(GlideUtil.GetGlideUrl(sprite.remotePath)).Into(viewHolder.imgIv);
            viewHolder.txtTv.Text = sprite.name;
            viewHolder.txtTv.Tag = position;
        }

        // select sprite then back
        public void ClickItem(int position)
        {
            
            List<List<Sprite>> sprites = Sprite._sprites;
            if (mIndex >= sprites.Count)
            {
                return;
            }
            Android.Util.Log.Info("position", "position===" + position);
            List<Sprite> list = sprites[mIndex];
            Sprite sprite = list[position];
            Intent intent = new Intent();
            Bundle bundle = new Bundle();
            bundle.PutString("model", sprite.ToString());
            intent.PutExtra("bundle", bundle);
            SetResult(Result.Ok, intent);
            Finish();
        }

        //定义ViewHolder内部类，用于对控件实例进行缓存
        class ViewHolder : Java.Lang.Object
        {
            public ImageView bgIv;
            public ImageView imgIv;
            public TextView txtTv;
        }
    }
}

