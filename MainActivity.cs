using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Collections.Generic;
using System;
using Android.Util;
using Android.Graphics;


namespace TabletArtco {
    [Activity(Theme = "@style/AppTheme")]
    public class MainActivity : AppCompatActivity {
         
        protected override void OnCreate(Bundle savedInstanceState) {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            RequestedOrientation = Android.Content.PM.ScreenOrientation.Landscape;
            initView();
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults) {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public void initView() {
            addTopButtonEvent();
            addLeftButtonEvent();
        }

        //Top tool button
        public void addTopButtonEvent() {
            int[] btsResIds = {
                Resource.Id.bt_choice1, Resource.Id.bt_choice2, Resource.Id.bt_choice3, Resource.Id.bt_choice4, Resource.Id.bt_choice5,
                Resource.Id.bt_choice6, Resource.Id.bt_choice7, Resource.Id.bt_choice8, Resource.Id.bt_choice9
            };
            int height = (int)((ScreenUtil.ScreenHeight(this) - ScreenUtil.StatusBarHeight(this)) * 82 / 800.0 - 18);
            for (int i = 0; i < 9; i++) {
                ImageView imgBt = FindViewById<ImageView>(btsResIds[i]);
                ViewUtil.setViewSize(imgBt, (int)(height * 73 / 70.0), height);
                imgBt.Click += (t, e) => {

                }; 
            }
        }

        //Left four select button
        public void addLeftButtonEvent() {
            int[] btsResIds = {
                Resource.Id.bt_left_select1, Resource.Id.bt_left_select2, Resource.Id.bt_left_select3, Resource.Id.bt_left_select4
            };
            int itemW = (int)((ScreenUtil.ScreenWidth(this)*244.0/1280-12)/4);
            for (int i = 0; i < 4; i++) {
                ImageView imgBt = FindViewById<ImageView>(btsResIds[i]);
                ViewUtil.setViewSize(imgBt, itemW, (int)(itemW*45.0/55));
                imgBt.Click += (t, e) => {
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
                    for (int j = 0; j < 4; j++) {
                        ImageView tempBt = FindViewById<ImageView>(btsResIds[j]);
                        if (tempBt == t) {
                            tempBt.SetImageResource(selectImgResId[j]);
                            changeLeftList(j);
                        } else {
                            tempBt.SetImageResource(normalImgResId[j]);
                        }
                    }
                };
            }
            changeLeftList(0);
        }

        //change left blocks list
        public void changeLeftList(int index) {
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
            int itemW = (int)((ScreenUtil.ScreenWidth(this) * 244.0 / 1280 - 12) / 3);
            for (int i = 0; i < resIds.Length; i++) {
                FrameLayout.LayoutParams param = new FrameLayout.LayoutParams(itemW, itemW);
                param.LeftMargin = i % 3 * itemW;
                param.TopMargin = i / 3 * itemW + (index==0 ? (i>11 ? itemW/3 : 0) : index==1 ? (i > 8 ? itemW / 3 : 0) : 0);
                ImageView imgIv = new ImageView(this);
                imgIv.LayoutParameters = param;
                imgIv.SetImageResource(resIds[i]);
                blockView.AddView(imgIv);
            }
        }


    }
}