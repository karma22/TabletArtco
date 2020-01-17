using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using System.Collections.Generic;
using Android.Content;
using Android.Views;
using Com.Bumptech.Glide;
using Java.Lang;
using Android.Graphics;

namespace TabletArtco
{
    [Activity(Theme = "@style/AppTheme")]
    public class MainActivity : AppCompatActivity, Delegate, DataSource, UpdateDelegate, View.IOnDragListener
    {
        private static string Tag = "MainActivity";
        private List<ActivatedSprite> spritesList = Project.mSprites;
        //private List<Block> blocksList = new List<Block>();
        private Background mBackground = null;
        private List<ImageView> imgList = new List<ImageView>();
        private SpriteAdapter mSpriteAdapter;
        private GridAdapter mBlockAdapter;
        private int mSpriteIndex = -1;
        private bool isPlay;
        private MediaManager mediaManager;
        private bool activateBlockScale = false;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            Window.SetFlags(Android.Views.WindowManagerFlags.Fullscreen, Android.Views.WindowManagerFlags.Fullscreen);
            RequestedOrientation = Android.Content.PM.ScreenOrientation.Landscape;
            SetContentView(Resource.Layout.activity_main);
            //IWindowManager mWindowManager = (IWindowManager)this.GetSystemService(Context.WindowService);
            
            LoadResources();
            InitView();
        }

        protected override void OnPause()
        {
            base.OnPause();
            mediaManager.Stop();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        [System.Obsolete]
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (data == null)
            {
                return;
            }
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

                        new Thread(new Runnable(() =>
                        {
                            Bitmap bitmap = (Bitmap)Glide.With(this).AsBitmap().Load(GlideUtil.GetGlideUrl(sprite.remotePath)).Into(100, 100).Get();
                            sprite.bitmap = bitmap;
                            RunOnUiThread(() => {
                                Project.AddSprite(sprite);
                                mSpriteIndex = spritesList.Count - 1;
                                ListView listView = FindViewById<ListView>(Resource.Id.materailListView);
                                mSpriteAdapter.NotifyDataSetChanged();
                                UpdateBlockView();
                                addSpriteView();
                            });
                        })).Start();
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
                        mediaManager.SetPath(mBackground.remoteVideoPath);
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
                case 10:
                    {

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
            
            int[] resIds = index == 0 ? Block.blockTab1ResIds : index == 1 ? Block.blockTab2ResIds : index == 2 ? Block.blockTab3ResIds : Block.blockTab4ResIds;
            string[] resIdStrs = index == 0 ? Block.blockTab1ResIdStrs : index == 1 ? Block.blockTab2ResIdStrs : index == 2 ? Block.blockTab3ResIdStrs : Block.blockTab4ResIdStrs;
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
                imgIv.Tag = index * 1000 + i;
                imgIv.SetImageResource(resIds[i]);
                blockView.AddView(imgIv);
                imgIv.Click += (t, e) =>
                {
                    if (mSpriteIndex<0 || mSpriteIndex>=spritesList.Count)
                    {
                        return;
                    }
                    int tag = (int)(((ImageView)t).Tag);
                    int tabIndex = tag / 1000;
                    int tempIndex = tag - tabIndex * 1000;
                    //int[] resIds = tabIndex == 0 ? Block.blockTab1ResIds : tabIndex == 1 ? Block.blockTab3ResIds : tabIndex == 2 ? Block.blockTab3ResIds : Block.blockTab4ResIds;
                    //string[] resIdStrs = tabIndex == 0 ? Block.blockTab1ResIdStrs : tabIndex == 1 ? Block.blockTab3ResIdStrs : tabIndex == 2 ? Block.blockTab3ResIdStrs : Block.blockTab4ResIdStrs;
                    Block block = new Block();
                    block.resourceId = resIds[tempIndex];
                    block.name = resIdStrs[tempIndex];
                    block.tabIndex = tabIndex;
                    block.index = tempIndex;
                    spritesList[mSpriteIndex].AddBlock(block);
                    UpdateBlockView();
                    //mBlockAdapter.NotifyDataSetChanged();
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
            int paddingB = (int)(19 / 549.0 * height);
            mainView.SetPadding(paddingL, paddingB, paddingL, 0);
            ViewUtil.SetViewHeight(mainView, (int)height);
            ViewUtil.SetViewHeight(centerView, (int)(481 / 549.0 * height));
            int[] btsResIds = { Resource.Id.bt_center1, Resource.Id.bt_center2, Resource.Id.bt_center3, Resource.Id.bt_center4 };
            int itemW = (int)(42 / 549.0 * height);
            for (int i = 0; i < btsResIds.Length; i++)
            {
                ImageView imgBt = FindViewById<ImageView>(btsResIds[i]);
                ViewUtil.SetViewSize(imgBt, itemW, itemW);
                imgBt.Tag = i;
                imgBt.Click += (t, e) =>
                {
                    int tag = (int)((ImageView)t).Tag;
                    switch (tag)
                    {
                        case 0:
                            {

                                break;
                            }
                        case 1:
                            {
                                Android.Util.Log.Info(Tag, "Click play animation start");
                                if (isPlay)
                                {
                                    return;
                                }
                                isPlay = true;
                                Project.RunSprite();

                                //CrossMediaManager.Current.Play("https://672-3.vod.tv.itc.cn/sohu/v1/TmwGoKIsWBeHgB67yEW4gmdbW6c48KPLghAXeBvFhJXUyYbSoO27fSx.mp4?k=FtJelY&p=j9lvzSw3qm1UqpxUoLPUqSXiqpxmqpsdhRYRzSPWXZxIWhoGgY220Go70ScAZMx4gf&r=TUldziJCtpCmhWB3tSCGhWlvsmCUqmPWtWaizY&q=OpCBhW7IRYodRDvswmfCyY2sWhyHfhyt5G64fJXsWYeS0F2OfGWsWYdsZYoURDvsfp", MediaFileType.Video);
                                mediaManager.Play();
                                break;
                            }
                        case 2:
                            {
                                Android.Util.Log.Info(Tag, "Click play animation stop");
                                if (!isPlay)
                                {
                                    return;
                                }
                                isPlay = false;
                                Project.StopSprite();
                                mediaManager.Stop();
                                break;
                            }
                        case 3:
                            {

                                break;
                            }
                        default:
                            break;
                    }
                };
            }

            ActivatedSprite.notFullSize = new Android.Util.Size((int)width-paddingL*2, (int)(481 / 549.0 * height));
            ActivatedSprite.fullSize = new Android.Util.Size(ScreenUtil.ScreenWidth(this), ScreenUtil.ScreenHeight(this));
            ActivatedSprite.mUpdateDelegate = this;
            //videoView
            //surfaceView
            
            SurfaceView surfaceView = FindViewById<SurfaceView>(Resource.Id.surfaceView);
            mediaManager = new MediaManager(surfaceView);

            //Stream input = Assets.Open("Stage_Default.mp4");
            //Java.IO.InputStream input = (Java.IO.InputStream)Assets.Open("Stage_Default.mp4");

            //InputStream inputStream;
            //LogUtil.CustomLog(Assets.OpenFd("Stage_Default.mp4").ToString());
            //File file = Resources.Assets.Open();
            //Resource.
            //Resources.OpenRawResource
            //mediaManager.SetPath();
            //ContainerView

            RelativeLayout activate_block_wrapperview = FindViewById<RelativeLayout>(Resource.Id.activate_block_wrapperview);
            ScrollView activate_block_view = FindViewById<ScrollView>(Resource.Id.activate_block_view);
            ViewUtil.SetViewHeight(activate_block_wrapperview, (int)(ScreenUtil.ScreenHeight(this) * 175 / 800.0-ScreenUtil.dip2px(this, 8)));

            FindViewById<ImageView>(Resource.Id.bt_scale).Click += (t, e) =>
            {
                activateBlockScale = !activateBlockScale;
                if (activateBlockScale)
                {
                    ViewUtil.SetViewHeight(activate_block_wrapperview, (int)(ScreenUtil.ScreenHeight(this) * 175 * 3 / 800.0 ));
                    activate_block_wrapperview.SetBackgroundResource(Resource.Drawable.BlockGlass_large);
                }
                else
                {
                    ViewUtil.SetViewHeight(activate_block_wrapperview, (int)(ScreenUtil.ScreenHeight(this) * 175 / 800.0 - ScreenUtil.dip2px(this, 8)));
                    activate_block_wrapperview.SetBackgroundResource(Resource.Drawable.BlockGlass_small);
                }
            };

            FindViewById<ImageView>(Resource.Id.bt_clear_block).Click += (t, e) =>
            {
                if (mSpriteIndex > -1)
                {
                    ActivatedSprite activatedSprite = spritesList[mSpriteIndex];
                    activatedSprite.ClearCode();
                    UpdateBlockView();
                }
            };
        }

        // Right sprite list
        public void InitSpriteListView()
        {
            int itemW = (int)(ScreenUtil.ScreenWidth(this) * 146 / 1280.0 - ScreenUtil.dip2px(this, 24));
            ListView listView = FindViewById<ListView>(Resource.Id.materailListView);
            mSpriteAdapter = new SpriteAdapter(this, this);
            listView.Adapter = mSpriteAdapter;
        }

        public void addSpriteView() {
            FrameLayout containerView = FindViewById<FrameLayout>(Resource.Id.ContainerView);
            containerView.RemoveAllViews();
            imgList.RemoveRange(0, imgList.Count);
            for (int i = 0; i < spritesList.Count; i++)
            {
                ActivatedSprite activatedSprite = spritesList[i];
                ImageView imgIv = new ImageView(this);
                FrameLayout.LayoutParams layoutParams = new FrameLayout.LayoutParams(activatedSprite.curSize.Width, activatedSprite.curSize.Height);
                layoutParams.LeftMargin = activatedSprite.curPoint.X;
                layoutParams.TopMargin = activatedSprite.curPoint.Y;
                containerView.AddView(imgIv, layoutParams);
                imgIv.SetImageBitmap(activatedSprite.GetSpriteBit());
                imgList.Add(imgIv);
                imgIv.SetOnDragListener(this);
            }
        }

        public void UpdateMainView()
        {
            FrameLayout containerView = FindViewById<FrameLayout>(Resource.Id.ContainerView);
            for (int i = 0; i < imgList.Count; i++)
            {
                ActivatedSprite activatedSprite = spritesList[i];
                ImageView imgIv = imgList[i];
                FrameLayout.LayoutParams layoutParams = (FrameLayout.LayoutParams)imgIv.LayoutParameters;
                layoutParams.LeftMargin = activatedSprite.curPoint.X;
                layoutParams.TopMargin = activatedSprite.curPoint.Y;
                layoutParams.Width = activatedSprite.curSize.Width;
                layoutParams.Height = activatedSprite.curSize.Height;
                imgIv.SetImageBitmap(activatedSprite.GetSpriteBit());
                imgIv.Visibility = activatedSprite.isVisible ? ViewStates.Visible : ViewStates.Invisible;
                containerView.UpdateViewLayout(imgIv, layoutParams);
            }
            LogUtil.CustomLog("UpdateMainView");
        }

        public void UpdateBlockView() {
            FrameLayout blockView = FindViewById<FrameLayout>(Resource.Id.block_view);
            int width = (int)(ScreenUtil.ScreenWidth(this) * 890 / 1280.0) - ScreenUtil.dip2px(this, 30);
            int GridViewH = (int)(ScreenUtil.ScreenHeight(this) * 175 / 800.0 - ScreenUtil.dip2px(this, 28));
            int margin = 10;
            int itemW = (int)((GridViewH - margin) / 2.0);
            int padding = 10;
            int column = (width - padding * 2) / (itemW + margin);
            padding = (width - column * itemW - (column - 1) * margin)/2;
            if (mSpriteIndex > -1)
            {
                blockView.RemoveAllViews();
                ActivatedSprite activatedSprite = spritesList[mSpriteIndex];
                List<List<Block>> blockList = activatedSprite.mBlocks;

                int originY = 0;
                for (int i = 0; i < blockList.Count; i++)
                {
                    List<Block> list = blockList[i];
                    for (int j = 0; j < list.Count; j++)
                    {
                        originY += (j > 0 && j % column == 0) ? itemW + margin : 0;
                        Block block = list[j];
                        FrameLayout view = new FrameLayout(this);
                        view.SetBackgroundResource(block.resourceId);
                        FrameLayout.LayoutParams layoutParams = new FrameLayout.LayoutParams(itemW, itemW);
                        layoutParams.LeftMargin = padding + (itemW+margin)*(j%column);
                        layoutParams.TopMargin = originY;
                        blockView.AddView(view, layoutParams);

                        view.Click += (t, e) =>
                        {

                            DialogView dialogView = new DialogView(this, DialogStyle.Signal);
                            dialogView.Show();
                        };
                    }
                    originY += itemW + margin;
                }
            }
        }

        public void startAnimation() {

        }

        public void stopAnimation() {
            
        }

        /*
         * ActivateSprite interface
         */
        public void UpdateView() {
            RunOnUiThread(() => {
                UpdateMainView();
            });
        }

        /*
         *
         * OnDrag interface
         * 
         */
        bool View.IOnDragListener.OnDrag(View v, DragEvent e)
        {
            LogUtil.CustomLog(e.ToString());
            return true;
        }

        /*
         * 
         * Delegate and DataSource interface 
         * 
        */
        public int GetItemsCount(Java.Lang.Object adapter)
        {
            return spritesList.Count;
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
                int position = (int)viewHolder.bgIv.Tag;
                ClickItem(position);
            };
            convertView.LongClick += (t, e) =>
            {
                ViewHolder viewHolder = (ViewHolder)(((View)t).Tag);
                int position = (int)viewHolder.bgIv.Tag;
                LongClickItem(position);
            };
            return convertView;
        }

        public void UpdateItemView(Java.Lang.Object adapter, View contentView, int position)
        {
            ActivatedSprite sprite = spritesList[position];
            ViewHolder viewHolder = (ViewHolder)contentView.Tag;
            viewHolder.bgIv.Tag = position;
            Glide.With(this).Load(GlideUtil.GetGlideUrl(sprite.sprite.remotePath)).Into(viewHolder.imgIv);
        }

        public void ClickItem(int position)
        {
            mSpriteIndex = position;
            UpdateBlockView();
        }

        public void LongClickItem(int position)
        {
            mSpriteIndex = position;
            Intent intent = new Intent(this, typeof(EditActivity));
            Bundle bundle = new Bundle();
            bundle.PutInt("position", position);
            intent.PutExtra("bundle", bundle);
            StartActivityForResult(intent, 10, null);
        }

        //定义ViewHolder内部类，用于对控件实例进行缓存
        class ViewHolder : Java.Lang.Object
        {
            public ImageView bgIv;
            public ImageView imgIv;
        }
    }
}