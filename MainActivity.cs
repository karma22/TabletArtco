using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V4.View;
using Android.Runtime;
using Android.Widget;
using System.Collections.Generic;
using System;
using Android.Content;

namespace TabletArtco
{
    [Activity(Theme = "@style/AppTheme")]
    public class MainActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            Window.SetFlags(Android.Views.WindowManagerFlags.Fullscreen, Android.Views.WindowManagerFlags.Fullscreen);
            RequestedOrientation = Android.Content.PM.ScreenOrientation.Landscape;
            SetContentView(Resource.Layout.activity_main);
            
            InitView();
            LoadResources();
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public void InitView()
        {
            InitTopButtonEvent();
            InitLeftButtonEvent();
            InitMainView();
            InitAniBlocksView();
            InitMaterailListView();
        }
        
        public void LoadResources()
        {
            DBManager.LoadSprites();
            DBManager.LoadBackgrounds();
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
                                StartActivity(intent);
                                break;
                            }
                        case 1:
                            {
                                Intent intent = new Intent(this, typeof(EducationActivity));
                                StartActivity(intent);
                                break;
                            }
                        case 2:
                            {
                                Intent intent = new Intent(this, typeof(BackgroundActivity));
                                StartActivity(intent);
                                break;
                            }
                        case 3:
                            {
                                Intent intent = new Intent(this, typeof(SoundActivity));
                                StartActivity(intent);
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
            int[] codeingOneResIds = {
                Resource.Drawable.Moveblock_Down1, Resource.Drawable.Moveblock_Down5, Resource.Drawable.Moveblock_Down10,
                Resource.Drawable.Moveblock_Left1, Resource.Drawable.Moveblock_Left5, Resource.Drawable.Moveblock_Left10,
                Resource.Drawable.Moveblock_Right1, Resource.Drawable.Moveblock_Right5, Resource.Drawable.Moveblock_Right10,
                Resource.Drawable.Moveblock_Up1, Resource.Drawable.Moveblock_Up5, Resource.Drawable.Moveblock_Up10,
                Resource.Drawable.Actblock_Slow, Resource.Drawable.Actblock_Fast, Resource.Drawable.Actblock_Flash,
                Resource.Drawable.Actblock_RRotate, Resource.Drawable.Actblock_LRotate, Resource.Drawable.Actblock_RotateLoop,
                Resource.Drawable.Actblock_Wave, Resource.Drawable.Actblock_TWave, Resource.Drawable.Actblock_RandomMove,
                Resource.Drawable.Actblock_Zigzag, Resource.Drawable.Actblock_TZigzag, Resource.Drawable.Actblock_Bounce,
                Resource.Drawable.Actblock_Jump, Resource.Drawable.Actblock_RLJump, Resource.Drawable.Actblock_Animate
            };
            int[] codeingTwoResIds = {
                Resource.Drawable.Moveblock_LUpN, Resource.Drawable.Moveblock_UpN, Resource.Drawable.Moveblock_RUpN,
                Resource.Drawable.Moveblock_LeftN, Resource.Drawable.Moveblock_Empty, Resource.Drawable.Moveblock_RightN,
                Resource.Drawable.Moveblock_LDownN, Resource.Drawable.Moveblock_DownN, Resource.Drawable.Moveblock_RDownN,
                Resource.Drawable.Actblock_Slow, Resource.Drawable.Actblock_Fast, Resource.Drawable.Actblock_Flash,
                Resource.Drawable.Actblock_RRotateN, Resource.Drawable.Actblock_LRotateN, Resource.Drawable.Actblock_RotateLoop,
                Resource.Drawable.Actblock_Wave, Resource.Drawable.Actblock_TWave, Resource.Drawable.Actblock_RandomMove,
                Resource.Drawable.Actblock_Zigzag, Resource.Drawable.Actblock_TZigzag, Resource.Drawable.Actblock_Bounce,
                Resource.Drawable.Actblock_Jump, Resource.Drawable.Actblock_RLJump, Resource.Drawable.Actblock_Animate
            };
            int[] controlResIds = {
                Resource.Drawable.Contblock_Time1, Resource.Drawable.Contblock_Time2, Resource.Drawable.Contblock_TimeN,
                Resource.Drawable.Contblock_LoopN, Resource.Drawable.Contblock_loop, Resource.Drawable.Contblock_Flag,
                Resource.Drawable.Contblock_FlipX, Resource.Drawable.Contblock_FlipY, Resource.Drawable.Contblock_NextSprite,
                Resource.Drawable.Contblock_Show, Resource.Drawable.Contblock_Hide, Resource.Drawable.Contblock_Sound,
                Resource.Drawable.Contblock_AdditionBackground, Resource.Drawable.Contblock_SendSignal, Resource.Drawable.Contblock_ReceiveSignal,
                Resource.Drawable.Contblock_Speak
            };
            int[] educationResIds = {
                Resource.Drawable.Edublock_Down, Resource.Drawable.Edublock_Jump, Resource.Drawable.Edublock_Up,
                Resource.Drawable.Edublock_Left, Resource.Drawable.Edublock_LoopN, Resource.Drawable.Edublock_Right
            };
            int[] resIds = index == 0 ? codeingOneResIds : index == 1 ? codeingTwoResIds : index == 2 ? controlResIds : educationResIds;
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
            }
        }

        public void InitMainView()
        {
            LinearLayout mainView = FindViewById<LinearLayout>(Resource.Id.mainView);
            FrameLayout centerView = FindViewById<FrameLayout>(Resource.Id.centerView);
            double width = ScreenUtil.ScreenWidth(this) * 890 / 1280.0;
            double height = ScreenUtil.ScreenHeight(this) * 545 / 800.0;
            int paddingL = (int)(18 / 913.0 * width);
            mainView.SetPadding(paddingL, (int)(19 / 549.0 * height), paddingL, 0);
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
        }

        public void InitAniBlocksView()
        {
            int width = (int)(ScreenUtil.ScreenWidth(this) * 890 / 1280.0);
            double height = (ScreenUtil.ScreenHeight(this) * 175 / 800.0 - 10 - ScreenUtil.dip2px(this, 4));
            int itemH = (int)((height - 40) / 3.0);
            int itemW = (int)(itemH * 168 / 50.0);
            Android.Util.Log.Info("tag", "statusHeight:" + ScreenUtil.StatusBarHeight(this));
            int[] btsResIds = { Resource.Id.bt_delete1, Resource.Id.bt_delete2, Resource.Id.bt_delete3 };
            for (int i = 0; i < btsResIds.Length; i++)
            {
                ImageView imgBt = FindViewById<ImageView>(btsResIds[i]);
                ViewUtil.SetViewSize(imgBt, itemW, itemH);
                imgBt.Click += (t, e) =>
                {

                };
            }
            List<String> list = new List<String>();
            list.Add("ff");
            list.Add("ff");
            BlockAdapter adapter = new BlockAdapter(this, list);
            ViewPager viewPager = FindViewById<ViewPager>(Resource.Id.viewpager);
            ViewUtil.SetViewWidth(viewPager, width - itemW);
            viewPager.Adapter = adapter;

        }

        public void InitMaterailListView()
        {
            int itemW = (int)(ScreenUtil.ScreenWidth(this) * 146 / 1280.0 - ScreenUtil.dip2px(this, 24));
            ListView listView = FindViewById<ListView>(Resource.Id.materailListView);
            listView.Adapter = new MaterailAdapter(this, itemW);
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

    }
}