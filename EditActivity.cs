
using System;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Graphics;

namespace TabletArtco
{
    [Activity(Label = "EditActivity")]
    public class EditActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            Window.SetFlags(Android.Views.WindowManagerFlags.Fullscreen, Android.Views.WindowManagerFlags.Fullscreen);
            SetContentView(Resource.Layout.activity_edit);
            RequestedOrientation = Android.Content.PM.ScreenOrientation.Landscape;
            InitView();
        }

        private void InitView()
        {
            InitTopView();
            InitMaterialList();
            InitToolView();
            InitOperateView();
        }

        private void InitTopView()
        {
            int h = (int)(ScreenUtil.ScreenHeight(this) * 57 / 800.0);
            int logoW = (int)(70 / 43.0 * h);
            int titleW = (int)(242 / 35.0 * h);
            ImageView logoIv = FindViewById<ImageView>(Resource.Id.edit_logo_view);
            ImageView titleIv = FindViewById<ImageView>(Resource.Id.edit_title_view);
            ViewUtil.SetViewWidth(logoIv, logoW);
            ViewUtil.SetViewWidth(titleIv, titleW);
        }

        private void InitMaterialList()
        {
            int sw = ScreenUtil.ScreenWidth(this);
            double width = sw * 190 / 1280.0;
            LinearLayout editListWrapper = FindViewById<LinearLayout>(Resource.Id.edit_list_wrapper);
            editListWrapper.SetPadding((int)(28.0 / 1280.0 * sw), 0, (int)(4 / 1280.0 * sw), (int)(8 / 1280.0 * sw));
            int listW = (int)((190 - 60) / 1280.0 * sw);
            int itemW = listW - 30;
            ListView listView = FindViewById<ListView>(Resource.Id.edit_material_list);
            listView.SetPadding(30, 28, 45, 50);
            listView.Adapter = new MaterailAdapter(this, itemW);

        }

        private void InitToolView()
        {
            int w = (int)(ScreenUtil.ScreenWidth(this) * 1055 / 1280.0);
            int h = (int)(ScreenUtil.ScreenHeight(this) * 6 / 8.0);
            //1090/1280.0
            //600 143
            //145 /543
            FrameLayout wrapperView = FindViewById<FrameLayout>(Resource.Id.area_wrapper_view);
            LinearLayout.LayoutParams layoutParams = (LinearLayout.LayoutParams)wrapperView.LayoutParameters;
            layoutParams.RightMargin = (int)(12 / 1280.0 * ScreenUtil.ScreenWidth(this));
            wrapperView.LayoutParameters = layoutParams;

            int topMargin = (int)(13 / 600.0 * h);
            int wrapperH = (int)(543 / 600.0 * h);
            int toolW = (int)(145 / 543.0 * wrapperH);
            int areaW = w - toolW - topMargin * 4;
            //140 //28 26
            int colorW = (int)(28 / 140.0 * toolW);
            int colorH = (int)(13 / 14.0 * colorW);
            int colorMargin = (int)((toolW - (colorW * 4)) / 5.0);

            //center edit area
            FrameLayout areaView = FindViewById<FrameLayout>(Resource.Id.edit_area_view);
            FrameLayout.LayoutParams areaParams = (FrameLayout.LayoutParams)areaView.LayoutParameters;
            areaParams.Height = wrapperH - colorMargin;
            areaParams.Width = areaW - (colorMargin * 2);
            areaParams.TopMargin = topMargin + colorMargin;
            areaParams.LeftMargin = topMargin + colorMargin;
            areaView.LayoutParameters = areaParams;
            areaView.SetBackgroundResource(Resource.Drawable.xml_edit_bg);

            //right tool area
            FrameLayout toolView = FindViewById<FrameLayout>(Resource.Id.tool_wrapper_view);
            FrameLayout.LayoutParams toolParams = (FrameLayout.LayoutParams)toolView.LayoutParameters;
            toolParams.Height = wrapperH;
            toolParams.Width = toolW;
            toolParams.TopMargin = topMargin;
            toolParams.LeftMargin = (int)(w - toolW - (topMargin * 2));
            toolView.LayoutParameters = toolParams;

            String[] colors = {
                "#000000","", "", "",
                "#880016", "#b97a57", "#300088", "#ab57b9",
                "#ed1b24", "#feaec9", "#7c1ded", "#bbaefe",
                "#ff7f26", "#ffc90d", "#ec26ff", "#ff0ebd",
                "#fef200", "#efe3af", "#ff008e", "#efb1da",
                "#23b14d", "#b5e51d", "#b19321", "#e61d51",
                "#00a3e8", "#9ad9ea", "#2ee800", "#b1ea99",
                "#3f47cc", "#7092bf", "#3fcc7d", "#70bd75",
                "#a349a3", "#c7bfe8", "#4978a4", "#bfe7dc"
            };
            for (int i = 0; i < 36; i++)
            {
                if (0 < i && i < 4)
                {
                    continue;
                }
                FrameLayout.LayoutParams colorParams = new FrameLayout.LayoutParams(colorW, colorH);
                colorParams.TopMargin = colorMargin + (i / 4 * (colorH + colorMargin));
                colorParams.LeftMargin = colorMargin + (i % 4 * (colorW + colorMargin));
                ImageView imgIv = new ImageView(this);
                imgIv.LayoutParameters = colorParams;
                imgIv.SetBackgroundColor(Color.ParseColor(colors[i]));
                toolView.AddView(imgIv);
            }

            //The center of two button
            int cW = (int)((toolW - topMargin * 3) / 2.0);
            int cMargin = (int)(295 / 543.0 * wrapperH);
            int[] cIds = { Resource.Drawable.Button_pallet, Resource.Drawable.Button_spoid };
            for (int i = 0; i < cIds.Length; i++)
            {
                FrameLayout.LayoutParams btParams = new FrameLayout.LayoutParams(cW, cW);
                btParams.TopMargin = cMargin;
                btParams.LeftMargin = topMargin + i * (topMargin + cW);
                ImageView imgIv = new ImageView(this);
                imgIv.LayoutParameters = btParams;
                imgIv.SetImageResource(cIds[i]);
                toolView.AddView(imgIv);
            }

            //366
            int sTopMargin = (int)(366 / 543.0 * wrapperH);
            int sW = (int)(((colorW * 4) + (colorMargin * 2)) / 2.0);
            int sH = (int)(28 / 63.0 * sW);
            for (int i = 0; i < cIds.Length; i++)
            {
                FrameLayout.LayoutParams btParams = new FrameLayout.LayoutParams(sW, sH);
                btParams.TopMargin = sTopMargin;
                btParams.LeftMargin = colorMargin + i * (colorMargin + sW);
                if (i == 0)
                {
                    TextView textTv = new TextView(this);
                    textTv.LayoutParameters = btParams;
                    textTv.SetBackgroundResource(Resource.Drawable.edit_size_bg);
                    textTv.Text = "10";
                    textTv.Gravity = GravityFlags.CenterVertical;
                    textTv.SetPadding(4, 0, 4, 0);
                    textTv.TextSize = (float)(sH / 4.0);
                    toolView.AddView(textTv);
                }
                else
                {
                    ImageView imgIv = new ImageView(this);
                    imgIv.LayoutParameters = btParams;
                    imgIv.SetBackgroundColor(Color.ParseColor("#000000"));
                    toolView.AddView(imgIv);
                }
            }

            //The bottom of three button
            int[] resIds = { Resource.Drawable.Button_PictureCopy, Resource.Drawable.Button_PictureOpen, Resource.Drawable.Button_Restore };
            int btH = (int)(46 / 144.0 * toolW);
            int btTopMargin = (int)(2 / 144.0 * toolW);
            for (int i = 0; i < resIds.Length; i++)
            {
                FrameLayout.LayoutParams btParams = new FrameLayout.LayoutParams(toolW, btH);
                btParams.TopMargin = wrapperH - ((2 - i) * (btH + btTopMargin)) - btH;
                ImageView imgIv = new ImageView(this);
                imgIv.LayoutParameters = btParams;
                imgIv.SetImageResource(resIds[i]);
                toolView.AddView(imgIv);
            }
        }

        private void InitOperateView()
        {
            int[] resIds = {
                Resource.Drawable.FlipXBtn, Resource.Drawable.FlipYBtn, Resource.Drawable.RRotate,
                Resource.Drawable.LRotate, Resource.Drawable.SizeIncreseBtn, Resource.Drawable.SizeDecreseBtn,
                Resource.Drawable.EraseBtn, Resource.Drawable.BrushBtn, Resource.Drawable.MagicBackBtn,
                Resource.Drawable.RectCutBtn, Resource.Drawable.FreeCutBtn, Resource.Drawable.EditAddSprite
            };
            int sw = (int)(ScreenUtil.ScreenWidth(this) * 931 / 1280.0);
            FrameLayout contentView = FindViewById<FrameLayout>(Resource.Id.edit_bt_content_view);
            int itemW = (int)(sw / 12.0);
            int imgW = (int)(10.0 / 11 * itemW);
            for (int i = 0; i < resIds.Length; i++)
            {
                FrameLayout.LayoutParams layoutParams = new FrameLayout.LayoutParams(imgW, imgW);
                layoutParams.LeftMargin = i * itemW + 8;
                layoutParams.TopMargin = 8;
                ImageView imgBt = new ImageView(this);
                imgBt.SetBackgroundResource(resIds[i]);
                imgBt.LayoutParameters = layoutParams;
                contentView.AddView(imgBt);
            }
        }
    }
}
