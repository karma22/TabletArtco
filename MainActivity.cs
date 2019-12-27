using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V4.View;
using Android.Runtime;
using Android.Widget;
using System.Collections.Generic;
using System;
using Android.Content;
using Android.Views;
using Com.Bumptech.Glide;

namespace TabletArtco
{
    [Activity(Theme = "@style/AppTheme")]
    public class MainActivity : AppCompatActivity, Delegate, DataSource
    {

        private List<ActivatedSprite> spritesList = SpriteManager.sprites;
        private List<Block> blocksList = new List<Block>();
        private Background mBackground = null;
        private List<ImageView> imgList = new List<ImageView>();
        private SpriteAdapter mSpriteAdapter;
        private GridAdapter mBlockAdapter;
        private int mSpriteIndex = -1;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            Window.SetFlags(Android.Views.WindowManagerFlags.Fullscreen, Android.Views.WindowManagerFlags.Fullscreen);
            RequestedOrientation = Android.Content.PM.ScreenOrientation.Landscape;
            SetContentView(Resource.Layout.activity_main);

            LoadResources();
            InitView();
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            switch (requestCode)
            {
                //Select Sprite callback
                case 0:
                    {
                        Bundle bundle = data.GetBundleExtra("bundle");
                        Sprite sprite = Sprite.ToSprite(bundle.GetString("model"));
                        if (sprite == null)
                        {
                            return;
                        }
                        spritesList.Add(new ActivatedSprite(sprite));
                        mSpriteIndex = spritesList.Count == 1 ? 0 : mSpriteIndex;                        
                        ListView listView = FindViewById<ListView>(Resource.Id.materailListView);
                        mSpriteAdapter.NotifyDataSetChanged();
                        break;
                    }
                case 1:
                    {
                        Bundle bundle = data.GetBundleExtra("bundle");
                        Background background = Background.ToBackground(bundle.GetString("model"));
                        if (background == null)
                        {
                            return;
                        }
                        mBackground = background;
                        break;
                    }
                case 2:
                    {
                        Bundle bundle = data.GetBundleExtra("bundle");
                        Background background = Background.ToBackground(bundle.GetString("model"));
                        if (background == null)
                        {
                            return;
                        }
                        mBackground = background;
                        if (mBackground.isPng)
                        {
                            
                        }
                        break;
                    }
                default:
                    break;
            }
        }

        public void LoadResources()
        {
            DBManager.LoadSprites();
            DBManager.LoadBackgrounds();
        }

        public void InitView()
        {
            InitTopButtonEvent();
            InitLeftButtonEvent();
            InitMainView();
            InitSpriteListView();
        }
        
        //Top tool button
        public void InitTopButtonEvent()
        {
            int[] btsResIds = {
                Resource.Id.bt_choice1, Resource.Id.bt_choice2, Resource.Id.bt_choice3, Resource.Id.bt_choice4, Resource.Id.bt_choice5,
                Resource.Id.bt_choice6, Resource.Id.bt_choice7, Resource.Id.bt_choice8, Resource.Id.bt_choice9
            };
            int height = (int)((ScreenUtil.ScreenHeight(this) - ScreenUtil.StatusBarHeight(this)) * 80 / 800.0 - 18);
            for (int i = 0; i < btsResIds.Length; i++)
            {
                ImageView imgBt = FindViewById<ImageView>(btsResIds[i]);
                imgBt.Tag = i;
                ViewUtil.SetViewSize(imgBt, (int)(height * 73 / 70.0), height);
                imgBt.Click += (t, e) =>
                {
                    switch ((int)((ImageView)t).Tag)
                    {
                        case 0:
                            {
                                Intent intent = new Intent(this, typeof(PictureActivity));
                                StartActivityForResult(intent, 0, null);
                                break;
                            }
                        case 1:
                            {
                                Intent intent = new Intent(this, typeof(EducationActivity));
                                StartActivityForResult(intent, 1, null);
                                break;
                            }
                        case 2:
                            {
                                Intent intent = new Intent(this, typeof(BackgroundActivity));
                                StartActivityForResult(intent, 2, null);
                                break;
                            }
                        case 3:
                            {
                                Intent intent = new Intent(this, typeof(SoundActivity));
                                StartActivityForResult(intent, 3, null);
                                break;
                            }
                        default:
                            break;
                    }

                };
            }
        }

        //Left four select button
        public void InitLeftButtonEvent()
        {
            int[] btsResIds = {
                Resource.Id.bt_left_select1, Resource.Id.bt_left_select2, Resource.Id.bt_left_select3, Resource.Id.bt_left_select4
            };
            int itemW = (int)((ScreenUtil.ScreenWidth(this) * 244.0 / 1280 - (12 * 2)) / 4);
            for (int i = 0; i < btsResIds.Length; i++)
            {
                ImageView imgBt = FindViewById<ImageView>(btsResIds[i]);
                ViewUtil.SetViewSize(imgBt, itemW, (int)(itemW * 45.0 / 55));
                imgBt.Click += (t, e) =>
                {
                    int[] normalImgResId = {
                        Resource.Drawable.Button_coding1_Deactivation,
                        Resource.Drawable.Button_coding2_Deactivation,
                        Resource.Drawable.Button_control_Deactivation,
                        Resource.Drawable.Button_effect_Deactivation,
                    };
                    int[] selectImgResId = {
                        Resource.Drawable.Button_coding1_activation,
                        Resource.Drawable.Button_coding2_activation,
                        Resource.Drawable.Button_control_activation,
                        Resource.Drawable.Button_effect_activation,
                    };
                    for (int j = 0; j < 4; j++)
                    {
                        ImageView tempBt = FindViewById<ImageView>(btsResIds[j]);
                        if (tempBt == t)
                        {
                            tempBt.SetImageResource(selectImgResId[j]);
                            ChangeLeftList(j);
                        }
                        else
                        {
                            tempBt.SetImageResource(normalImgResId[j]);
                        }
                    }
                };
            }
            ChangeLeftList(0);
        }

        //change left blocks list
        public void ChangeLeftList(int index)
        {
            FrameLayout blockView = FindViewById<FrameLayout>(Resource.Id.left_blocks_view);
            blockView.RemoveAllViews();
            
            int[] resIds = index == 0 ? Block.blockTab1ResIds : index == 1 ? Block.blockTab3ResIds : index == 2 ? Block.blockTab3ResIds : Block.blockTab4ResIds;
            string[] resIdStrs = index == 0 ? Block.blockTab1ResIdStrs : index == 1 ? Block.blockTab3ResIdStrs : index == 2 ? Block.blockTab3ResIdStrs : Block.blockTab4ResIdStrs;
            int margin = 12;
            int padding = 4;
            double rowWidth = ScreenUtil.ScreenWidth(this) * 244.0 / 1280 - (12 * 2);
            int itemW = (int)((rowWidth - (margin * 2) - (padding * 2)) / 3);
            for (int i = 0; i < resIds.Length; i++)
            {
                FrameLayout.LayoutParams param = new FrameLayout.LayoutParams(itemW, itemW);
                param.LeftMargin = i % 3 * (itemW + padding) + margin;
                param.TopMargin = i / 3 * (itemW + padding) + padding + (index == 0 ? (i > 11 ? itemW / 4 : 0) : index == 1 ? (i > 8 ? itemW / 4 : 0) : 0);
                ImageView imgIv = new ImageView(this);
                imgIv.LayoutParameters = param;
                imgIv.SetImageResource(resIds[i]);
                blockView.AddView(imgIv);
                imgIv.Click += (t, e) =>
                {
                    Block block = new Block();
                    block.resourceId = resIds[i];
                    block.name = resIdStrs[i];
                    block.tabIndex = index;
                    block.index = i;
                    spritesList[mSpriteIndex].AddCode(block);
                    
                };
            }
        }

        // center main view
        public void InitMainView()
        {
            LinearLayout mainView = FindViewById<LinearLayout>(Resource.Id.mainView);
            FrameLayout centerView = FindViewById<FrameLayout>(Resource.Id.centerView);
            double width = ScreenUtil.ScreenWidth(this) * 890 / 1280.0;
            double height = ScreenUtil.ScreenHeight(this) * 545 / 800.0;
            int paddingL = (int)(18 / 913.0 * width);
            mainView.SetPadding(paddingL, (int)(19 / 549.0 * height), paddingL, 0);
            ViewUtil.SetViewHeight(mainView, (int)height);
            ViewUtil.SetViewHeight(centerView, (int)(481 / 549.0 * height));
            int[] btsResIds = { Resource.Id.bt_center1, Resource.Id.bt_center2, Resource.Id.bt_center3, Resource.Id.bt_center4 };
            int itemW = (int)(42 / 549.0 * height);
            for (int i = 0; i < btsResIds.Length; i++)
            {
                ImageView imgBt = FindViewById<ImageView>(btsResIds[i]);
                ViewUtil.SetViewSize(imgBt, itemW, itemW);
                imgBt.Click += (t, e) =>
                {

                };
            }

            //videoView
            //ContainerView

            RelativeLayout activate_block_view = FindViewById<RelativeLayout>(Resource.Id.activate_block_view);
            ViewUtil.SetViewHeight(activate_block_view, (int)(ScreenUtil.ScreenHeight(this) * 175 / 800.0));

            //int columnCount = 4;
            //mItemW = (int)((w - (columnCount + 1) * spacing * 1.0) / columnCount);
            //mItemH = (int)(mItemW * 170.0 / 250);

            //int width = (int)(ScreenUtil.ScreenWidth(view.Context) * 890 / 1280.0);
            int GridViewH = (int)(ScreenUtil.ScreenHeight(this) * 175 / 800.0 - 10 - ScreenUtil.dip2px(this, 4));
            
            int margin = 20;
            int itemH = (int)((GridViewH - margin) / 2.0);
            int column = (int)(width / itemH);
            int start = (int)((width - column * itemH) / 2.0);

            GridView gridView = FindViewById<GridView>(Resource.Id.gridview);
            gridView.SetColumnWidth(200);
            gridView.SetNumColumns(column);
            gridView.SetVerticalSpacing(margin);
            gridView.SetHorizontalSpacing(margin);
            mBlockAdapter = new GridAdapter((DataSource)this, (Delegate)this);
            gridView.Adapter = mBlockAdapter;
            
            //List<String> list = new List<String>();
            //list.Add("ff");
            //list.Add("ff");
            //BlockAdapter adapter = new BlockAdapter(this, list);
            //ViewPager viewPager = FindViewById<ViewPager>(Resource.Id.viewpager);
            //ViewUtil.SetViewWidth(viewPager, (int)width);
            //viewPager.Adapter = adapter;
        }

       
        // Right sprite list
        public void InitSpriteListView()
        {
            int itemW = (int)(ScreenUtil.ScreenWidth(this) * 146 / 1280.0 - ScreenUtil.dip2px(this, 24));
            ListView listView = FindViewById<ListView>(Resource.Id.materailListView);
            mSpriteAdapter = new SpriteAdapter(this, this);
            listView.Adapter = mSpriteAdapter;
            listView.ItemClick += ListView_ItemClick;
        }

        private void ListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            // Enter main page
            Intent intent = new Intent(this, typeof(EditActivity));
            StartActivity(intent);
        }


        public void UpdateMainView()
        {

        }

        public void startAnimation() {

        }

        public void stopAnimation() {

        }


        /*
         * Delegate
         * DataSource 
         * interface
        */
        public int GetItemsCount(Java.Lang.Object adapter)
        {
            if (adapter == mSpriteAdapter)
            {
                return spritesList.Count;
            }
            else
            {
                return blocksList.Count;
            }
        }

        public View GetItemView(Java.Lang.Object adapter, ViewGroup parent)
        {
            View convertView = LayoutInflater.From(this).Inflate(Resource.Layout.item_sprite, parent, false);
            int itemW = (int)(ScreenUtil.ScreenWidth(this) * 146 / 1280.0 - ScreenUtil.dip2px(this, 24));
            ViewUtil.SetViewHeight(convertView, itemW);
            ViewHolder holder = new ViewHolder();
            holder.bgIv = convertView.FindViewById<ImageView>(Resource.Id.selected_material_bgIv);
            holder.imgIv = convertView.FindViewById<ImageView>(Resource.Id.selected_material_imgIv);
            holder.bgIv.SetBackgroundResource(Resource.Drawable.xml_gridview_bg);
            convertView.Tag = holder;
            convertView.Click += (t, e) =>
            {
                ViewHolder viewHolder = (ViewHolder)(((View)t).Tag);
                //ClickItem(position);
            };
            return convertView;
        }

        public void UpdateItemView(Java.Lang.Object adapter, View contentView, int position)
        {
            ActivatedSprite sprite = spritesList[position];
            ViewHolder viewHolder = (ViewHolder)contentView.Tag;
            Glide.With(this).Load(sprite._sprite.remotePath).Into(viewHolder.imgIv);
        }

        public void ClickItem(int position)
        {
           
        }

        //定义ViewHolder内部类，用于对控件实例进行缓存
        class ViewHolder : Java.Lang.Object
        {
            public ImageView bgIv;
            public ImageView imgIv;
        }
    }
}