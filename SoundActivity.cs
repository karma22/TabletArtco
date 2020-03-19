
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace TabletArtco
{
    [Activity(Label = "SoundActivity")]
    public class SoundActivity : Activity, DataSource, Delegate
    {

        private int mItemW;
        private int mItemH;
        private int mIndex = 0;

        private Block block;
        private Intent mIntent;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            Window.SetFlags(Android.Views.WindowManagerFlags.Fullscreen, Android.Views.WindowManagerFlags.Fullscreen);
            SetContentView(Resource.Layout.activity_grid);
            RequestedOrientation = Android.Content.PM.ScreenOrientation.Landscape;
            InitData();
            InitView();
        }

        private void InitData()
        {
            mIntent = this.Intent;
            Bundle bundle = mIntent.GetBundleExtra("bundle");
            if (bundle != null)
            {
                int index = bundle.GetInt("index");
                int row = bundle.GetInt("row");
                int column = bundle.GetInt("column");
                if (index != -1 && index < Project.mSprites.Count)
                {
                    ActivatedSprite activatedSprite = Project.mSprites[index];
                    if (row != -1 && row < activatedSprite.mBlocks.Count)
                    {
                        List<Block> list = activatedSprite.mBlocks[row];
                        if (column != -1 && column < list.Count)
                        {
                            block = list[column];
                        }
                    }
                }
            }
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
                Resource.Drawable.search_bg, Resource.Drawable.ss_animal_tab, Resource.Drawable.ss_nature_tab,
                Resource.Drawable.ss_life_tab, Resource.Drawable.ss_music_tab, Resource.Drawable.ss_etc_tab,
                Resource.Drawable.User_tab, Resource.Drawable.storage_recorder_button
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
                    LinearLayout.LayoutParams lp = new LinearLayout.LayoutParams(i == resIds.Length - 1 ? itemH : itemW, itemH);
                    lp.LeftMargin = margin / 3;
                    ImageView imgIv = new ImageView(this);
                    imgIv.LayoutParameters = lp;
                    imgIv.Tag = i;
                    imgIv.SetImageResource(resIds[i]);
                    topView.AddView(imgIv);
                    if(i == resIds.Length - 1)
                    {
                        imgIv.Click += (t, e) =>
                        {
                            new RecordDialog(this, () =>
                            {

                            }).Show();
                        };
                    }
                    else
                    {
                        imgIv.Click += (t, e) =>
                        {
                            int tag = (int)(((ImageView)t).Tag);
                            mIndex = tag - 1;
                            UpdateView();
                        };
                    }
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
            mItemH = mItemW;

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


        public int GetItemsCount(Java.Lang.Object adapter)
        {
            List<List<Sound>> sounds = Sound._sounds;
            if (0<= mIndex && mIndex < sounds.Count)
            {
                return sounds[mIndex].Count;
            }
            return 0;
        }

        public View GetItemView(Java.Lang.Object adapter, ViewGroup parent)
        {
            View convertView = LayoutInflater.From(this).Inflate(Resource.Layout.item_sound, parent, false);
            ViewUtil.SetViewSize(convertView, mItemW, mItemH);
            
            ViewHolder holder = new ViewHolder();
            holder.playIv = convertView.FindViewById<ImageView>(Resource.Id.iv_play);
            holder.nameTv = convertView.FindViewById<TextView>(Resource.Id.tv_name);
            holder.soundPlay = convertView.FindViewById<FrameLayout>(Resource.Id.play_sound);
            ViewUtil.SetViewSize(holder.soundPlay, (int)(mItemW/4), (int)(mItemH/4));

            convertView.Tag = holder;

            convertView.Click += (t, e) =>
            {
                ViewHolder viewHolder = (ViewHolder)(((View)t).Tag);
                int position = (int)viewHolder.soundPlay.Tag;
                ClickItem(position, false);
            };
            holder.soundPlay.Click += (t, e) =>
            {
                int position = (int)((View)t).Tag;
                ClickItem(position, true);
            };

            return convertView;
        }

        public void UpdateItemView(Java.Lang.Object adapter, View contentView, int position)
        {
            List<List<Sound>> sounds = Sound._sounds;
            if (mIndex >= sounds.Count)
            {
                return;
            }
            List<Sound> list = sounds[mIndex];
            Sound sound = list[position];
            ViewHolder viewHolder = (ViewHolder)contentView.Tag;
            viewHolder.nameTv.Text = sound.name;

            viewHolder.soundPlay.Tag = position;
        }

        public void ClickItem(int position, bool playSound)
        {
            List<List<Sound>> sounds = Sound._sounds;
            if (mIndex >= sounds.Count)
            {
                return;
            }

            List<Sound> list = sounds[mIndex];
            Sound sound = list[position];
            Intent intent = new Intent();
            
            if (block != null)
            {
                block.text = sound.name;
                block.varName = sound.localPath;
                block.varValue = sound.localPath;
            }
            
            if (playSound)
            {
                new SoundPlayer(this).Play(sound.localPath);
            }
            else
            {
                Bundle bundle = mIntent.GetBundleExtra("bundle");
                if (bundle != null)
                {
                    SetResult(Result.Ok, intent);
                    Finish();
                }
            }
        }

        //定义ViewHolder内部类，用于对控件实例进行缓存
        class ViewHolder : Java.Lang.Object
        {
            public ImageView playIv;
            public TextView nameTv;
            public FrameLayout soundPlay;
        }
    }
}
