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
    public class MainActivity : AppCompatActivity, Delegate, DataSource, UpdateDelegate, View.IOnDragListener, View.IOnLongClickListener
    {
        private static string Tag = "MainActivity";
        private List<ActivatedSprite> spritesList = Project.mSprites;
        //private List<Block> blocksList = new List<Block>();
        private Background mBackground = null;
        private List<DragImgView> imgList = new List<DragImgView>();
        private SpriteAdapter mSpriteAdapter;
        private GridAdapter mBlockAdapter;
        private int mSpriteIndex = -1;
        private bool isPlay;
        private MediaManager mediaManager;
        private bool activateBlockScale = false;
        private View dragView;

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
                // select background callback
                case 1:
                    {
                        Bundle bundle = data.GetBundleExtra("bundle");
                        Background background = Background.ToBackground(bundle.GetString("model"));
                        if (background == null)
                        {
                            return;
                        }
                        mBackground = background;
                        mediaManager.SetPath(mBackground.remoteVideoPath, mBackground.remotePreviewImgPath);
                        break;
                    }
                // select background callback
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
                case 8:
                    {
                        UpdateBlockView();
                        break;
                    }
                case 9:
                    {
                        UpdateBlockView();
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
            DBManager.LoadSounds();
        }

        // init view
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
                                // Sprite select Activity
                                Intent intent = new Intent(this, typeof(PictureActivity));
                                StartActivityForResult(intent, 0, null);
                                break;
                            }
                        case 1:
                            {
                                // Education select activity
                                Intent intent = new Intent(this, typeof(EducationActivity));
                                StartActivityForResult(intent, 1, null);
                                break;
                            }
                        case 2:
                            {
                                // Background  select activity
                                Intent intent = new Intent(this, typeof(BackgroundActivity));
                                StartActivityForResult(intent, 2, null);
                                break;
                            }
                        case 3:
                            {
                                // Sound select activity
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
            FindViewById<ScrollView>(Resource.Id.left_blocks_view_wrapper).SetOnDragListener(this);
        }



        //change left blocks list
        public void ChangeLeftList(int index)
        {
            FrameLayout blockView = FindViewById<FrameLayout>(Resource.Id.left_blocks_view);
            blockView.RemoveAllViews();
            
            int[] resIds = index == 0 ? Block.blockTab0ResIds : index == 1 ? Block.blockTab1ResIds : index == 2 ? Block.blockTab2ResIds : Block.blockTab3ResIds;
            string[] resIdStrs = index == 0 ? Block.blockTab0ResIdStrs : index == 1 ? Block.blockTab1ResIdStrs : index == 2 ? Block.blockTab2ResIdStrs : Block.blockTab3ResIdStrs;
            int margin = 12;
            int padding = 4;
            double rowWidth = ScreenUtil.ScreenWidth(this) * 244.0 / 1280 - (12 * 2);
            int itemW = (int)((rowWidth - (margin * 2) - (padding * 2)) / 3);
            for (int i = 0; i < resIds.Length; i++)
            {
                FrameLayout.LayoutParams param = new FrameLayout.LayoutParams(itemW, itemW);
                param.LeftMargin = i % 3 * (itemW + padding) + margin;
                param.TopMargin = i / 3 * (itemW + padding) + padding;
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
                    if (resIdStrs[tempIndex] == "MoveEmpty")
                    {
                        return;
                    }
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
            int paddingT = (int)(19 / 549.0 * height);
            mainView.SetPadding(paddingL, paddingT, paddingL, 0);
            ViewUtil.SetViewHeight(mainView, (int)height);
            ViewUtil.SetViewHeight(centerView, (int)(481 / 549.0 * height));

            int[] btsResIds = { Resource.Id.bt_center1, Resource.Id.bt_center2, Resource.Id.bt_center3, Resource.Id.bt_center4 };
            int itemW = (int)(42 / 549.0 * height);
            // home、play、stop、full button
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
                                // home button click
                                break;
                            }
                        case 1:
                            {
                                // play button click
                                Android.Util.Log.Info(Tag, "Click play animation start");
                                if (isPlay)
                                {
                                    return;
                                }
                                isPlay = true;
                                Project.RunSprite();

                                //CrossMediaManager.Current.Play("https://672-3.vod.tv.itc.cn/sohu/v1/TmwGoKIsWBeHgB67yEW4gmdbW6c48KPLghAXeBvFhJXUyYbSoO27fSx.mp4?k=FtJelY&p=j9lvzSw3qm1UqpxUoLPUqSXiqpxmqpsdhRYRzSPWXZxIWhoGgY220Go70ScAZMx4gf&r=TUldziJCtpCmhWB3tSCGhWlvsmCUqmPWtWaizY&q=OpCBhW7IRYodRDvswmfCyY2sWhyHfhyt5G64fJXsWYeS0F2OfGWsWYdsZYoURDvsfp", MediaFileType.Video);
                                mediaManager.Play();
                                //VideoView videoView2 = FindViewById<VideoView>(Resource.Id.video_view);
                                //videoView2.Start();
                                break;
                            }
                        case 2:
                            {
                                // stop button click
                                Android.Util.Log.Info(Tag, "Click play animation stop");
                                if (!isPlay)
                                {
                                    return;
                                }
                                isPlay = false;
                                Project.StopSprite();
                                mediaManager.Stop();
                                //VideoView videoView1 = FindViewById<VideoView>(Resource.Id.video_view);
                                //videoView1.StopPlayback();
                                break;
                            }
                        case 3:
                            {
                                //full button click

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

            // video surfaceview
            //VideoView videoView = FindViewById<VideoView>(Resource.Id.video_view);
            SurfaceView surfaceView = FindViewById<SurfaceView>(Resource.Id.surfaceView);
            ImageView imgIv = FindViewById<ImageView>(Resource.Id.preimage);
            mediaManager = new MediaManager(surfaceView, imgIv, this);

            FrameLayout containerView = FindViewById<FrameLayout>(Resource.Id.ContainerView);
            RelativeLayout activate_block_wrapperview = FindViewById<RelativeLayout>(Resource.Id.activate_block_wrapperview);
            ScrollView activate_block_view = FindViewById<ScrollView>(Resource.Id.activate_block_view);
            
            FrameLayout blockView = FindViewById<FrameLayout>(Resource.Id.block_view);
            blockView.SetOnDragListener(this);
            ViewUtil.SetViewHeight(activate_block_wrapperview, (int)(ScreenUtil.ScreenHeight(this) * 175 / 800.0-ScreenUtil.dip2px(this, 8)));
            // scale button
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

            // add variable button
            FindViewById<ImageView>(Resource.Id.bt_add).Click += (t, e) =>
            {
                VariableInitDialog dialog = new VariableInitDialog(this, (name, value)=> {
                    Project.variableMap[name] = value;
                });
                dialog.Show();
            };

            // clear block button
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

        // main screen add animate sprite view
        public void addSpriteView() {
            FrameLayout containerView = FindViewById<FrameLayout>(Resource.Id.ContainerView);
            containerView.RemoveAllViews();
            imgList.RemoveRange(0, imgList.Count);
            for (int i = 0; i < spritesList.Count; i++)
            {
                ActivatedSprite activatedSprite = spritesList[i];

                DragImgView imgIv = new DragImgView(this);
                FrameLayout.LayoutParams layoutParams = new FrameLayout.LayoutParams(activatedSprite.curSize.Width, activatedSprite.curSize.Height);
                layoutParams.LeftMargin = activatedSprite.curPoint.X;
                layoutParams.TopMargin = activatedSprite.curPoint.Y;
                containerView.AddView(imgIv, layoutParams);
                imgIv.SetImageBitmap(activatedSprite.GetSpriteBit());
                imgList.Add(imgIv);
                imgIv.Tag = 100+i;
                imgIv.MoveAction += (t, x, y) =>
                {
                    if (!isPlay)
                    {
                        activatedSprite.AddToOriginPoint((int)x, (int)y);
                    }
                };
                imgIv.ClickAction += (t) =>
                {
                    if (!isPlay)
                    {
                        LongClickItem(i);
                    }
                };
            }
        }

        // update main screen animate sprite imageview location and visibility
        public void UpdateMainView()
        {
            FrameLayout containerView = FindViewById<FrameLayout>(Resource.Id.ContainerView);
            for (int i = 0; i < imgList.Count; i++)
            {
                ActivatedSprite activatedSprite = spritesList[i];
                DragImgView imgIv = imgList[i];
                FrameLayout.LayoutParams layoutParams = (FrameLayout.LayoutParams)imgIv.LayoutParameters;
                layoutParams.LeftMargin = activatedSprite.curPoint.X;
                layoutParams.TopMargin = activatedSprite.curPoint.Y;
                layoutParams.Width = activatedSprite.curSize.Width;
                layoutParams.Height = activatedSprite.curSize.Height;
                imgIv.SetImageBitmap(activatedSprite.GetSpriteBit());
                imgIv.Visibility = activatedSprite.isVisible ? ViewStates.Visible : ViewStates.Invisible;
                containerView.UpdateViewLayout(imgIv, layoutParams);
            }
        }

        // update bottom block views
        public void UpdateBlockView() {
            FrameLayout blockView = FindViewById<FrameLayout>(Resource.Id.block_view);
            int width = (int)(ScreenUtil.ScreenWidth(this) * 890 / 1280.0) - ScreenUtil.dip2px(this, 30);
            int GridViewH = (int)(ScreenUtil.ScreenHeight(this) * 175 / 800.0 - ScreenUtil.dip2px(this, 28));
            int margin = 10;
            int itemW = (int)((GridViewH - margin) / 2.0);
            int padding = 10;
            int column = (width - padding * 2) / (itemW + margin);
            padding = (width - column * itemW - (column - 1) * margin)/2;
            column = column - 1;
            if (mSpriteIndex > -1)
            {
                blockView.RemoveAllViews();
                ActivatedSprite activatedSprite = spritesList[mSpriteIndex];
                List<List<Block>> blockList = activatedSprite.mBlocks;

                int originY = 0;
                for (int i = 0; i < blockList.Count; i++)
                {
                    List<Block> list = blockList[i];
                    if (activatedSprite.curRow == i) 
                    {
                        ImageView view = new ImageView(this);
                        view.SetImageResource(Resource.Drawable.Revision_line);
                        FrameLayout.LayoutParams layoutParams = new FrameLayout.LayoutParams(itemW, itemW);
                        layoutParams.LeftMargin =  padding;
                        layoutParams.TopMargin = originY;
                        blockView.AddView(view, layoutParams);
                    }

                    for (int j = 0; j < list.Count; j++)
                    {
                        originY += (j > 0 && j % column == 0) ? itemW + margin : 0;
                        Block block = list[j];
                        FrameLayout view = new FrameLayout(this);
                        view.SetBackgroundResource(block.resourceId);
                        FrameLayout.LayoutParams layoutParams = new FrameLayout.LayoutParams(itemW, itemW);
                        layoutParams.LeftMargin = itemW + padding+margin + (itemW+margin)*(j%column);
                        layoutParams.TopMargin = originY;
                        blockView.AddView(view, layoutParams);
                        view.SetOnLongClickListener(this);
                        view.Tag =  i * 10000 + j;
                        List<Dictionary<string, string>> locationList = Block.TextViewLocations(block);
                        if (locationList != null && locationList.Count>0)
                        {
                            for (int k = 0; k < locationList.Count; k++)
                            {
                                Dictionary<string, string> dic = locationList[k];
                                float x = float.Parse(dic["x"]) * itemW;
                                float y = float.Parse(dic["y"]) * itemW;
                                float w = float.Parse(dic["w"]) * itemW;
                                float h = float.Parse(dic["h"]) * itemW;
                                TextView tv = new TextView(this);
                                FrameLayout.LayoutParams tvparams = new FrameLayout.LayoutParams((int)w,(int)h);
                                tvparams.LeftMargin = (int)x;
                                tvparams.TopMargin = (int)y;
                                tv.Text = dic["text"];
                                tv.TextSize = h/2.8f;
                                tv.SetPadding(0,0,0,0);
                                tv.TextAlignment = TextAlignment.Center;
                                view.AddView(tv, tvparams);
                            }
                        }
                        int clickType = Block.GetClickType(block);
                        if (clickType != -1)
                        {
                            // activateblock click event
                            view.Click += (t, e) =>
                            {
                                if (clickType == 3 || clickType == 5 || clickType == 6 || clickType == 10)
                                {
                                    activatedSprite.curRow = block.row;
                                    UpdateBlockView();
                                }
                                switch (clickType) {
                                    case 0:
                                        {
                                            List<string> varlist = new List<string>();
                                            foreach (string name in Project.variableMap.Keys)
                                            {
                                                varlist.Add(name);
                                            }
                                            VariableSelectDialog dialog = new VariableSelectDialog(this, (selectIndex, text)=> {
                                                if (selectIndex != -1)
                                                {
                                                    block.varName = varlist[selectIndex];
                                                }
                                                else
                                                {
                                                    block.text = text;
                                                }
                                                UpdateBlockView();
                                            });
                                            dialog.mList = varlist;
                                            dialog.Show();
                                            break;
                                        }
                                    case 1:
                                    case 2:
                                        {
                                            // change variable dialog, example set variable value or increase variable value
                                            if (Project.variableMap.Count<=0)
                                            {
                                                ToastUtil.ShowToast(this, "你还没有添加变量");
                                                return;
                                            }
                                            List<string> varlist = new List<string>();
                                            foreach (string name in Project.variableMap.Keys)
                                            {
                                                varlist.Add(name);
                                            }
                                            VariableChangeDialog dialog = new VariableChangeDialog(this, clickType>1, (selectIndex, value) =>
                                            {
                                                block.varName = varlist[selectIndex];
                                                block.varValue = value;
                                                UpdateBlockView();
                                            });
                                            dialog.mList = varlist;
                                            dialog.Show();
                                            break;
                                        }
                                    case 3:
                                    case 4:
                                        {

                                            SignalDialog dialog = new SignalDialog(this, (text) => {
                                                block.text = text;
                                                UpdateBlockView();
                                            });
                                            dialog.Show();
                                            break;
                                        }
                                    
                                    case 5:
                                    case 6:
                                        {
                                            // select collision image and select click image dialog
                                            if (clickType == 4 && Project.mSprites.Count == 1)
                                            {
                                                ToastUtil.ShowToast(this, "至少选择两个精灵才会有碰撞");
                                                return;
                                            }
                                            List<string> titlelist = new List<string>();
                                            List<string> imglist = new List<string>();
                                            List<string> idlist = new List<string>();
                                            for (int q = 0; q < Project.mSprites.Count; q++)
                                            {
                                                ActivatedSprite sprite = Project.mSprites[q];
                                                titlelist.Add(sprite.sprite.name);
                                                imglist.Add(sprite.sprite.remotePath);
                                                idlist.Add(sprite.activateSpriteId);
                                            }
                                            if (clickType == 4)
                                            {
                                                titlelist.RemoveRange(mSpriteIndex, 1);
                                                imglist.RemoveRange(mSpriteIndex, 1);
                                                idlist.RemoveRange(mSpriteIndex, 1);
                                            }

                                            ImageSelectDialog dialog = new ImageSelectDialog(this, clickType>4, (selectIndex) => {
                                                block.text = titlelist[selectIndex];
                                                block.activateSpriteId = idlist[selectIndex];
                                                UpdateBlockView();
                                            });
                                            dialog.mList = titlelist;
                                            dialog.mImgList = imglist;
                                            dialog.Show();
                                            break;
                                        }
                                    case 7: {
                                            // input speck text dialog
                                            SpeakDialog dialog = new SpeakDialog(this, (text) => {
                                                block.text = text;
                                                UpdateBlockView();
                                            });
                                            dialog.Show();
                                            break;
                                        }
                                    case 8:
                                    case 9: {
                                            // click activate block to select background
                                            Intent intent = new Intent(this, clickType == 8 ? typeof(SoundActivity) : typeof(BackgroundActivity));
                                            Bundle bundle = new Bundle();
                                            bundle.PutInt("index", mSpriteIndex);
                                            bundle.PutInt("row", block.row);
                                            bundle.PutInt("column", block.index);
                                            intent.PutExtra("bundle", bundle);
                                            StartActivityForResult(intent, clickType, null);
                                            break;
                                        }
                                    default:
                                        break;
                                }
                            };
                        }
                    }
                    originY += itemW + margin;
                }
            }
        }


        public void startAnimation() {
            
        }

        public void stopAnimation() {
            
        }

        //public override bool OnTouchEvent(MotionEvent e)
        //{
        //    LogUtil.CustomLog(e.ToString());
        //    return base.OnTouchEvent(e);
        //}

        /*
         * ActivateSprite interface
         */
        public void UpdateView() {
            RunOnUiThread(() => {
                UpdateMainView();
            });
        }


        bool View.IOnLongClickListener.OnLongClick(View v)
        {
            // 创建实现阴影的对象
            View.DragShadowBuilder builder = new View.DragShadowBuilder(v);
            //DragShadow Builder builder = new DragShadowBuilder(v);
            /*开始拖拽并把view对象传递给系统。作为响应startDrag()方法的一部分，
            *系统调用在View.DragShadowBuilder对象中定义的回调方法
            *来获取拖拽影子。
            */
            dragView = v;
            v.StartDragAndDrop(null, builder, null, (int)DragFlags.Opaque);
            v.Visibility = ViewStates.Invisible;
            return true;
        }

        /*
         *
         * OnDrag interface
         * 
         */
        bool View.IOnDragListener.OnDrag(View v, DragEvent e)
        {
            //获取拖拽的动作类型值
            DragAction action = e.Action;
            switch(action) {
                case DragAction.Started:
                    {
                        break;
                    }
                case DragAction.Location:
                    {
                        break;
                    }
                case DragAction.Drop:
                    {
                        if (v.Id == Resource.Id.left_blocks_view_wrapper)
                        {
                            int tag = (int)dragView.Tag;
                            int row = tag / 10000;
                            int column = tag - row * 10000;
                            ActivatedSprite activatedSprite = spritesList[mSpriteIndex];
                            activatedSprite.DeleteBlock(row, column);
                            UpdateBlockView();
                            dragView = null;
                        }
                        else if (v.Id == Resource.Id.block_view)
                        {
                            int tag = (int)dragView.Tag;
                            int row = tag / 10000;
                            int column = tag - row * 10000;
                            if (column != 0)
                            {
                                int width = (int)(ScreenUtil.ScreenWidth(this) * 890 / 1280.0) - ScreenUtil.dip2px(this, 30);
                                int GridViewH = (int)(ScreenUtil.ScreenHeight(this) * 175 / 800.0 - ScreenUtil.dip2px(this, 28));

                                int margin = 10;
                                int itemW = (int)((GridViewH - margin) / 2.0);
                                LogUtil.CustomLog("itemW="+itemW + ", width=" + width);
                                int padding = 10;
                                int columnNum = (width - padding * 2) / (itemW + margin);
                                padding = (width - columnNum * itemW - (columnNum - 1) * margin) / 2;
                                columnNum = columnNum - 1;
                                float x = e.GetX();
                                float y = e.GetY();
                                int firstX = itemW + padding + margin-margin/2;
                                int firstY = -margin/2;
                                int add = itemW + margin;
                                if (x>firstX)
                                {
                                    ActivatedSprite activatedSprite = spritesList[mSpriteIndex];
                                    List<List<Block>> blockList = activatedSprite.mBlocks;
                                    int r = Math.Round((y - firstY) / add);
                                    r = firstY + r * add < y ? r : r - 1;
                                    int c = Math.Round((x - firstX) / add);
                                    c = firstX + c * add < x ? c : c - 1;
                                    if (c<columnNum)
                                    {
                                        int cc = 0;
                                        bool isTrue = true;
                                        int i = 0;
                                        while (isTrue) {
                                            if (i < blockList.Count)
                                            {
                                                List<Block> list = blockList[i];
                                                int temp = list.Count / columnNum + (list.Count % columnNum == 0 ? 0 : 1);
                                                cc += temp;
                                                if (r < cc)
                                                {
                                                    c = (r - (cc - temp)) * columnNum + c;
                                                    r = i;
                                                    isTrue = false;
                                                }
                                            }
                                            else
                                            {
                                                isTrue = false;
                                            }
                                            i++;
                                        }
                                        if (activatedSprite.ExchangeBlock(row, column, r, c))
                                        {
                                            UpdateBlockView();
                                            dragView = null;
                                        }   
                                    }
                                }
                            }
                        }
                        break;
                    }
                case DragAction.Ended:
                    {
                        if (dragView != null)
                        {
                            dragView.Visibility = ViewStates.Visible;
                        }
                        break;
                    }
                case DragAction.Entered:
                    {
                        break;
                    }
                case DragAction.Exited:
                    {
                        break;
                    }
                default:
                    break;
            }
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