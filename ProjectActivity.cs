using System.Collections.Generic;
using System.IO;
using System.Security.AccessControl;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Service.Voice;
using Android.Views;
using Android.Widget;
using Com.Bumptech.Glide;
using Com.Bumptech.Glide.Manager;

namespace TabletArtco
{
    [Activity(Label = "ProjectActivity", LaunchMode = Android.Content.PM.LaunchMode.SingleTop)]
    public class ProjectActivity : Activity, DataSource, Delegate
    {
        private int mItemW;
        private int mItemH;
        private int mIndex;

        private List<List<string>> filePathList = new List<List<string>>();
        private List<List<string>> fileNameList = new List<List<string>>();

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
            string[] path = new string[]
            {
                UserDirectoryPath.projectPath,
                UserDirectoryPath.objectPath
            };

            for(int i= 0; i < path.Length; i++)
            {
                if (Directory.Exists(path[i]))
                {
                    string[] pathList = Directory.GetFiles(path[i]);
                    List<string> list = new List<string>();
                    for (int j=0; j<pathList.Length; j++) 
                    {
                        list.Add(pathList[j]);
                    }
                    filePathList.Add(list);


                    List<string> nameList = new List<string>();
                    for (int j = 0; j < pathList.Length; j++)
                    {
                        nameList.Add(Path.GetFileNameWithoutExtension(pathList[j]));
                    }
                    fileNameList.Add(nameList);
                }
                else
                {
                    filePathList.Add(new List<string>());
                    fileNameList.Add(new List<string>());
                }
            }

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
            //int[] resIds = {
            //    Resource.Drawable.search_bg, Resource.Drawable.PO_ProjectOpen_tab, Resource.Drawable.PO_PictureOpen_tab
            //};
            int[] resIds = {
                Resource.Drawable.PO_ProjectOpen_tab, Resource.Drawable.PO_PictureOpen_tab
            };
            int editTvH = (int)(45 / 90.0 * topH);
            int editTvW = (int)(166 / 35.0 * editTvH);
            int itemH = (int)(50 / 90.0 * topH); 
            int itemW = (int)(179 / 50.0 * itemH);
            for (int i = 0; i < resIds.Length; i++)
            {
                //if (i == 0)
                //{
                //    LinearLayout.LayoutParams lp = new LinearLayout.LayoutParams(editTvW, editTvH);
                //    EditText searchEt = new EditText(this);
                //    searchEt.LayoutParameters = lp;
                //    searchEt.SetPadding((int)(30 / 166.0 * editTvW), 0, 0, 0);
                //    searchEt.Gravity = GravityFlags.CenterVertical;
                //    searchEt.SetBackgroundResource(resIds[i]);
                //    topView.AddView(searchEt);
                //}
                //else
                //{
                    LinearLayout.LayoutParams lp = new LinearLayout.LayoutParams(itemW + 2*padding, itemH + 2*padding);

                    ImageView imgIv = new ImageView(this);
                    imgIv.SetImageResource(resIds[i]);

                    FrameLayout frameLayout = new FrameLayout(this);
                    frameLayout.LayoutParameters = lp;
                    frameLayout.SetPadding(padding, padding, padding, padding);
                    frameLayout.Tag = i;
                    if (i == 0)
                        frameLayout.SetBackgroundResource(Resource.Drawable.tab_select);

                    frameLayout.AddView(imgIv);
                    topView.AddView(frameLayout);

                    frameLayout.Click += (t, e) =>
                    {
                        int tag = (int)(((FrameLayout)t).Tag);
                        mIndex = tag;

                        for (int j = 0; j < resIds.Length; j++)
                        {
                            FrameLayout fl = (FrameLayout)topView.GetChildAt(j);
                            fl.Background = null;
                        }
                        ((FrameLayout)t).SetBackgroundResource(Resource.Drawable.tab_select);

                        UpdateView();
                    };
                //}
            }

            int spacing = 20;
            LinearLayout conView = FindViewById<LinearLayout>(Resource.Id.grid_wrapper_view);
            LinearLayout.LayoutParams conParams = (LinearLayout.LayoutParams)conView.LayoutParameters;
            conParams.Width = w;
            conParams.Height = conH;
            conParams.LeftMargin = margin;
            conView.LayoutParameters = conParams;
            conView.SetPadding(spacing, spacing, spacing, spacing);

            int columnCount = 7;
            mItemW = (w - (columnCount + 1) * spacing) / columnCount;
            mItemH = mItemW;

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
            return fileNameList[mIndex].Count;
        }

        public View GetItemView(Java.Lang.Object adapter, ViewGroup parent)
        {
            View convertView = LayoutInflater.From(this).Inflate(Resource.Layout.item_background, parent, false);
            ViewUtil.SetViewSize(convertView, mItemW, mItemH);
            ViewHolder holder = new ViewHolder();
            holder.bgIv = convertView.FindViewById<ImageView>(Resource.Id.selected_material_bgIv);
            holder.imgIv = convertView.FindViewById<ImageView>(Resource.Id.selected_material_imgIv);
            holder.txtTv = convertView.FindViewById<TextView>(Resource.Id.sprite_tv);
            holder.delete_fl = convertView.FindViewById<FrameLayout>(Resource.Id.delete_fl);
            holder.delete_fl.Visibility = ViewStates.Visible;
            convertView.Tag = holder;
            convertView.Click += (t, e) =>
            {
                ViewHolder viewHolder = (ViewHolder)(((View)t).Tag);
                int position = (int)viewHolder.txtTv.Tag;
                ClickItem(position);
            };
            holder.delete_fl.Click += (t, e) =>
            {
                FrameLayout view = (FrameLayout)t;
                int position = (int)view.Tag;
                DeleteItem(position);
            };
            return convertView;
        }

        public void UpdateItemView(Java.Lang.Object adapter, View contentView, int position)
        {
            ViewHolder viewHolder = (ViewHolder)contentView.Tag;
            if(mIndex==0)
                viewHolder.imgIv.SetImageResource(Resource.Drawable.PO_ProjectBack);
            else
                viewHolder.imgIv.SetImageResource(Resource.Drawable.PO_SpriteBack);
            viewHolder.txtTv.Text = fileNameList[mIndex][position];
            viewHolder.txtTv.Tag = position;
            viewHolder.delete_fl.Tag = position;
        }

        public void ClickItem(int position)
        {
            bool result = false;
            if (mIndex == 0)
            {
                result = new ArtcoProject(this).LoadProject(filePathList[mIndex][position]);
            }
            else
            { 
                result = new ArtcoObject(this).LoadObject(filePathList[mIndex][position]);
            }

            Intent intent = new Intent();
            if (result)
                SetResult(Result.Ok, intent);
            else
                SetResult(Result.Canceled);

            Finish();
        }

        public void DeleteItem(int position)
        {
            List<string> pathList = filePathList[mIndex];
            List<string> nameList = fileNameList[mIndex];

            string path = pathList[position];
            string name = nameList[position];

            Java.IO.File file = new Java.IO.File(path);
            if (file.IsFile && file.Exists())
            {
                file.Delete();
            }

            pathList.RemoveAt(position);
            nameList.RemoveAt(position);
            UpdateView();
        }

        //定义ViewHolder内部类，用于对控件实例进行缓存
        class ViewHolder : Java.Lang.Object
        {
            public ImageView bgIv;
            public ImageView imgIv;
            public TextView txtTv;
            public FrameLayout delete_fl;
        }
    }
}
