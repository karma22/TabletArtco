﻿
using System.Collections.Generic;
using System.IO;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace TabletArtco
{
    [Activity(Label = "MusicActivity")]
    public class MusicActivity : Activity, DataSource, Delegate
    {
        private int mItemW;
        private int mItemH;
        private int mIndex = 0;

        private string dirPath = UserDirectoryPath.userMusicPath;
        private string[] filePath = new string[0];
        private string[] fileName = new string[0];

        private SoundPlayer soundPlayer;
        private bool isPlay = false;
        private int position;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            Window.SetFlags(Android.Views.WindowManagerFlags.Fullscreen, Android.Views.WindowManagerFlags.Fullscreen);
            SetContentView(Resource.Layout.activity_grid);
            RequestedOrientation = Android.Content.PM.ScreenOrientation.Landscape;

            soundPlayer = new SoundPlayer(this);
            InitView();
        }

        protected override void OnPause()
        {
            base.OnPause();
            SoundPlayer.StopAll();
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
                Resource.Drawable.search_bg, Resource.Drawable.ss_nature_tab,
                Resource.Drawable.ss_life_tab, Resource.Drawable.ss_music_tab, Resource.Drawable.ss_etc_tab,
                Resource.Drawable.National_studies_tab, Resource.Drawable.User_tab
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
                    //LinearLayout.LayoutParams lp = new LinearLayout.LayoutParams(i == resIds.Length - 1 ? itemH : itemW, itemH);
                    LinearLayout.LayoutParams lp = new LinearLayout.LayoutParams(itemW, itemH);
                    lp.LeftMargin = margin / 3;
                    ImageView imgIv = new ImageView(this);
                    imgIv.LayoutParameters = lp;
                    imgIv.Tag = i;
                    imgIv.SetImageResource(resIds[i]);
                    topView.AddView(imgIv);
                    if (i == resIds.Length - 1)
                    {
                        imgIv.Click += (t, e) =>
                        {
                            SoundPlayer.StopAll();
                            int tag = (int)(((ImageView)t).Tag);
                            mIndex = tag - 1;

                            if (Directory.Exists(dirPath))
                            {
                                filePath = Directory.GetFiles(dirPath);

                                fileName = new string[filePath.Length];
                                for (int i = 0; i < fileName.Length; i++)
                                {
                                    fileName[i] = Path.GetFileNameWithoutExtension(filePath[i]);
                                }
                            }

                            UpdateView();
                        };
                    }
                    else
                    {
                        imgIv.Click += (t, e) =>
                        {
                            SoundPlayer.StopAll();
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
            List<List<Music>> musics = Music._bgms;
            if (0<= mIndex && mIndex < musics.Count)
            {
                return musics[mIndex].Count;
            }
            else
            {
                return filePath.Length;
            }
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
            List<List<Music>> musics = Music._bgms;

            if (mIndex == musics.Count) //user background tab
            {
                ViewHolder viewHolder = (ViewHolder)contentView.Tag;
                viewHolder.nameTv.Text = fileName[position];
                viewHolder.soundPlay.Tag = position;
            }
            else if (mIndex < musics.Count)
            {
                List<Music> list = musics[mIndex];
                Music sound = list[position];
                ViewHolder viewHolder = (ViewHolder)contentView.Tag;
                viewHolder.nameTv.Text = sound.name;
                viewHolder.soundPlay.Tag = position;
            }
        }

        public void ClickItem(int position, bool playSound)
        {
            List<List<Music>> musics = Music._bgms;
            Music music = null;

            if (mIndex < musics.Count)
            {
                List<Music> list = musics[mIndex];
                music = list[position];
            }
            else
            {
                music = new Music(fileName[position], filePath[position]);
            }
            
            if (playSound)
            {
                if(isPlay)
                {
                    isPlay = false;
                    SoundPlayer.StopAll();
                    if(this.position != position)
                    {
                        isPlay = true;
                        this.position = position;
                        soundPlayer.Play(music.path);
                    }
                }
                else
                {
                    isPlay = true;
                    this.position = position;
                    soundPlayer.Play(music.path);
                }
            }
            else
            {
                Intent intent = new Intent();
                Bundle bundle = new Bundle();
                bundle.PutString("model", music.path);
                intent.PutExtra("bundle", bundle);
                
                SetResult(Result.Ok, intent);
                Finish();
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