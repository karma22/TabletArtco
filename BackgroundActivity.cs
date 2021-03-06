﻿using System.Collections.Generic;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Com.Bumptech.Glide;
using Android.Content;
using System.IO;
using Android.Runtime;
using Android.Net;
using Java.Lang;
using Android.Views.InputMethods;

namespace TabletArtco
{
    [Activity(Label = "BackgroundActivity", LaunchMode = Android.Content.PM.LaunchMode.SingleTop)]
    public class BackgroundActivity : Activity, DataSource, Delegate, TextView.IOnEditorActionListener
    {

        private int mItemW;
        private int mItemH;
        private int mIndex = 5;

        private EditText searchEt;
        private bool isSearch = false;
        private List<Background> searchList = new List<Background>();

        private Block block;

        private string dirPath = UserDirectoryPath.userBackgroundPath;
        private string[] filePath = new string[0];
        private string[] fileName = new string[0];

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            Window.SetFlags(WindowManagerFlags.Fullscreen, WindowManagerFlags.Fullscreen);
            SetContentView(Resource.Layout.activity_grid);
            RequestedOrientation = Android.Content.PM.ScreenOrientation.Landscape;
            InitData();
            InitView();
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (data == null)
            {
                return;
            }

            if(resultCode == Result.Ok)
            {
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }

                Uri uri = data.Data;
                string from = IOUtil.GetRealPathFromURI(this, uri);

                ProgressDialog progressDialog = new ProgressDialog(this);
                progressDialog.SetProgressStyle(ProgressDialogStyle.Spinner);
                progressDialog.SetMessage("Loading...");
                progressDialog.SetCancelable(false);
                progressDialog.Show();

                new Thread(new Runnable(() =>
                {
                    bool result = IOUtil.CopyFile(from, dirPath);
                    if (result)
                    {
                        filePath = Directory.GetFiles(dirPath);

                        fileName = new string[filePath.Length];
                        for (int i = 0; i < fileName.Length; i++)
                        {
                            fileName[i] = Path.GetFileNameWithoutExtension(filePath[i]);
                        }

                        RunOnUiThread(()=>
                        {
                            progressDialog.Dismiss();
                            UpdateView();
                        });
                    }
                })).Start();
            }
        }


        private void InitData()
        {
            Intent intent = this.Intent;
            Bundle bundle = intent.GetBundleExtra("bundle");
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
            int padding = 4;
            int w = width - margin * 2;
            int topH = (int)(90 / 975.0 * height);
            int conH = height - topH - margin;

            ScrollView scrollView = FindViewById<ScrollView>(Resource.Id.scrollView);
            LinearLayout.LayoutParams svParams = (LinearLayout.LayoutParams)scrollView.LayoutParameters;
            svParams.Width = width;
            svParams.Height = topH;
            scrollView.LayoutParameters = svParams;
            scrollView.SetPadding(margin, (int)(10 / 975.0 * height), margin, 0);
            LinearLayout topView = FindViewById<LinearLayout>(Resource.Id.grid_top_view);
            int[] resIds = {
                Resource.Drawable.search_bg, Resource.Drawable.bs_themeBackground_tab, Resource.Drawable.bs_cartoonbackground_tab,
                Resource.Drawable.bs_reallifebackground_tab, Resource.Drawable.bs_planeBackground_tab ,Resource.Drawable.momochung_tab,
                Resource.Drawable.National_studies_tab, Resource.Drawable.User_tab, Resource.Drawable.OpenDirectory,
            };
            int editTvH = (int)(45 / 90.0 * topH);
            int editTvW = (int)(166 / 35.0 * editTvH);
            int itemH = (int)(50 / 90.0 * topH);
            int itemW = (int)(92 / 35.0 * itemH);
            for (int i = 0; i < resIds.Length; i++)
            {
                if (i == 0)
                {
                    LinearLayout.LayoutParams lp = new LinearLayout.LayoutParams(editTvW, editTvH);
                    searchEt = new EditText(this);
                    searchEt.LayoutParameters = lp;
                    searchEt.SetPadding((int)(30/166.0 * editTvW), 0, 0, 0);
                    searchEt.Gravity = GravityFlags.CenterVertical;
                    searchEt.SetBackgroundResource(resIds[i]);
                    searchEt.Hint = "搜索";
                    searchEt.TextSize = 14;
                   
                    searchEt.SetSingleLine(true);
                    searchEt.ImeOptions = Android.Views.InputMethods.ImeAction.Search;
                    searchEt.SetOnEditorActionListener(this);
                    topView.AddView(searchEt);
                    TextViewUtil.setMaxLength(searchEt, 32);
                }
                else
                {
                    LinearLayout.LayoutParams lp = new LinearLayout.LayoutParams(i == resIds.Length - 1 ? itemH + 2*padding : itemW + 2*padding, itemH + 2*padding);

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

                    if(i == resIds.Length - 1)
                    {
                        frameLayout.Click += (t, e) =>
                        {
                            Intent intent = new Intent();
                            intent.SetAction(Intent.ActionGetContent);
                            intent.SetType("video/*");
                            StartActivityForResult(Intent.CreateChooser(intent, "Open"), 0);
                        };
                    }
                    else
                    {
                        frameLayout.Click += (t, e) =>
                        {
                            int tag = (int)(((FrameLayout)t).Tag);
                            mIndex = tag + 4;

                            for (int j = 1; j < resIds.Length; j++)
                            {
                                FrameLayout fl = (FrameLayout)topView.GetChildAt(j);
                                fl.Background = null;
                            }
                            ((FrameLayout)t).SetBackgroundResource(Resource.Drawable.tab_select);

                            if (tag == resIds.Length - 2)
                            {
                                if (Directory.Exists(dirPath))
                                {
                                    filePath = Directory.GetFiles(dirPath);

                                    fileName = new string[filePath.Length];
                                    for (int i = 0; i < fileName.Length; i++)
                                    {
                                        fileName[i] = Path.GetFileNameWithoutExtension(filePath[i]);
                                    }
                                }
                            }
                            searchEt.Text = "";
                            isSearch = false;
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
            mItemH = (int)(mItemW * 170.0 / 250);

            GridView gridView = FindViewById<GridView>(Resource.Id.gridview);
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
        public int GetItemsCount(Java.Lang.Object adapter)
        {
            if (isSearch) {
                return searchList.Count;
            }

            List<List<Background>> backgrounds = Background._backgrounds;
            if (mIndex < backgrounds.Count)
            {
                return backgrounds[mIndex].Count;
            }
            else
            {
                return filePath.Length;
            }
        }

        public View GetItemView(Java.Lang.Object adapter, ViewGroup parent)
        {
            View convertView = LayoutInflater.From(this).Inflate(Resource.Layout.item_background, parent, false);
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
            if (isSearch) {
                Background background = searchList[position];
                ViewHolder viewHolder = (ViewHolder)contentView.Tag;
                Glide.With(this).Load(GlideUtil.GetGlideUrl(background.remotePreviewImgPath)).Into(viewHolder.imgIv);
                viewHolder.txtTv.Text = background.name;
                viewHolder.txtTv.Tag = position;
                return;
            }

            List<List<Background>> backgrounds = Background._backgrounds;

            if (mIndex == backgrounds.Count) //user background tab
            {
                ViewHolder viewHolder = (ViewHolder)contentView.Tag;
                Glide.With(this).Load(filePath[position]).Into(viewHolder.imgIv);
                viewHolder.txtTv.Text = fileName[position];
                viewHolder.txtTv.Tag = position;
            }
            else if(mIndex < backgrounds.Count)
            {
                List<Background> list = backgrounds[mIndex];
                Background background = list[position];
                ViewHolder viewHolder = (ViewHolder)contentView.Tag;
                Glide.With(this).Load(GlideUtil.GetGlideUrl(background.remotePreviewImgPath)).Into(viewHolder.imgIv);
                viewHolder.txtTv.Text = background.name;
                viewHolder.txtTv.Tag = position;
            }
        }

        public void ClickItem(int position)
        {
            List<List<Background>> backgrounds = Background._backgrounds;
            Background background = null;
            if (isSearch) {
                background = searchList[position];
            } else {
                if (mIndex < backgrounds.Count)
                {
                    List<Background> list = backgrounds[mIndex];
                    background = list[position];
                }
                else
                {
                    background = new Background()
                    {
                        name = fileName[position],
                        idx = position,
                        category = backgrounds.Count,
                        mode = 0,
                        remoteVideoPath = filePath[position],
                        remotePreviewImgPath = "",
                        level = 0,
                    };
                }
            }
            Intent intent = new Intent();
            Bundle bundle = new Bundle();
            bundle.PutString("model", background.ToString());
            intent.PutExtra("bundle", bundle);
            if (block != null)
            {
                block.text = background.name;
                block.backgroundName = background.name;
                block.varName = background.remotePreviewImgPath;
                block.varValue = background.remoteVideoPath;
                if (!Project.backgroundsList.ContainsKey(background.name)) {
                    Project.backgroundsList.Add(background.name, background);
                }
               
            }
            SetResult(Result.Ok, intent);
            Finish();
        }

        public bool OnEditorAction(TextView v, [GeneratedEnum] ImeAction actionId, KeyEvent e)
        { 
            if (v.Text.Length == 0)
            {
                ToastUtil.ShowToast(this, "搜索内容不能为空");
                return false;
            }
            if (actionId == ImeAction.Search)
            {
                string text = searchEt.Text;
                isSearch = true;
                searchList.RemoveRange(0, searchList.Count);
                List<List<Background>> list = Background._backgrounds;
                for (int i = 0; i < list.Count; i++)
                {
                    List<Background> subList = list[i];
                    for (int j = 0; j < subList.Count; j++)
                    {
                        Background bg = subList[j];
                        if (bg.name.Contains(text))
                        {
                            searchList.Add(bg);
                        }
                    }
                }
                UpdateView();
                // 当按了搜索之后关闭软键盘
                Keyboard.hideKeyboard(v);
                return true;
            }

            return false;
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
