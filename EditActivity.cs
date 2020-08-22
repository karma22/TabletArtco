using System;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using System.Collections.Generic;
using Android.Content;
using Android.Util;
using Android.Runtime;
using Java.Lang;
using Com.Bumptech.Glide;
using Android.Text;
using Android.Views.InputMethods;

namespace TabletArtco
{
    //[Activity(Label = "EditActivity", MainLauncher = true)]

    // Edit Image Activity
    [Activity(Theme = "@style/AppTheme", LaunchMode = Android.Content.PM.LaunchMode.SingleTop)]
    public class EditActivity : Activity, Delegate, DataSource, PopupMenu.IOnMenuItemClickListener, Android.Text.ITextWatcher, TextView.IOnEditorActionListener
    {
        private List<Bitmap> originList = new List<Bitmap>();
        private List<Bitmap> operateList = new List<Bitmap>();
        private List<float> scaleList = new List<float>();
        private int mIndex = -1;
        private EditView editView;
        private SpriteAdapter mAdapter;
        private ImageView curColorIv;

        ActivatedSprite mActivatedSprite = null;

        private EditText widthEt;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            Window.SetFlags(Android.Views.WindowManagerFlags.Fullscreen, Android.Views.WindowManagerFlags.Fullscreen);
            SetContentView(Resource.Layout.activity_edit);
            RequestedOrientation = Android.Content.PM.ScreenOrientation.Landscape;
            initData();
            InitView();
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
                            RunOnUiThread(() =>
                            {
                                originList.Add(bitmap);
                                operateList.Add(Bitmap.CreateBitmap(bitmap));
                                scaleList.Add(1);
                                mIndex = operateList.Count - 1;
                                editView.SetSrcBitmap(bitmap);
                                mAdapter.NotifyDataSetChanged();
                            });
                        })).Start();
                        break;
                    }
                case 1:
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
                            RunOnUiThread(() =>
                            {
                                editView.InsertBitmap(bitmap);
                            });
                        })).Start();
                        break;
                    }
                
                default:
                    break;
            }
        }

        public override bool OnKeyDown([GeneratedEnum] Keycode keyCode, KeyEvent e)
        {
            if (keyCode == Keycode.Back)
            {
                ConfirmDialog dialog = new ConfirmDialog(this, true);
                dialog.SetMessage("是否保存对当前图片的更改？");
                dialog.callbackAction = (confirm) =>
                {
                    if (confirm)
                    {
                        Bitmap oldB = operateList[mIndex];
                        operateList.Remove(oldB);
                        Bitmap bitmap = editView.CurBitmap();
                        operateList.Insert(mIndex, bitmap);
                        scaleList.RemoveAt(mIndex);
                        scaleList.Insert(mIndex, editView.CurScale());
                        mActivatedSprite.SetSrcBitmapList(operateList);
                        mActivatedSprite.scaleList = scaleList;
                        LogUtil.CustomLog(scaleList.ToString());
                        for (int i = 0; i < originList.Count; i++)
                        {
                            originList[i].Recycle();
                        }
                    }
                    Finish();
                };
                dialog.Show();
                return false;
            }
            return false;
        }

        // image List
        private void initData() {
            Intent intent = this.Intent;
            Bundle bundle = intent.GetBundleExtra("bundle");
            int position = bundle.GetInt("position");
            if (position<Project.mSprites.Count)
            {
                mActivatedSprite = Project.mSprites[position];
                for (int i = 0; i < mActivatedSprite.originBitmapList.Count; i++)
                {
                    Bitmap bitmap = Bitmap.CreateBitmap(mActivatedSprite.originBitmapList[i]);
                    originList.Add(bitmap);
                    operateList.Add(Bitmap.CreateBitmap(bitmap));
                    scaleList.Add(mActivatedSprite.scaleList[i]);
                }
                if (mActivatedSprite.originBitmapList.Count>0)
                {
                    mIndex = 0;
                }
            }               
        }

        // init view
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
            //ImageView logoIv = FindViewById<ImageView>(Resource.Id.edit_logo_view);
            ImageView titleIv = FindViewById<ImageView>(Resource.Id.edit_title_view);
            //ViewUtil.SetViewWidth(logoIv, logoW);
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
            mAdapter = new SpriteAdapter(this, this);
            listView.Adapter = mAdapter;
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
            RelativeLayout areaView = FindViewById<RelativeLayout>(Resource.Id.edit_area_view);
            FrameLayout.LayoutParams areaParams = (FrameLayout.LayoutParams)areaView.LayoutParameters;
            areaParams.Height = wrapperH - colorMargin;
            areaParams.Width = areaW - (colorMargin * 2);
            areaParams.TopMargin = topMargin + colorMargin;
            areaParams.LeftMargin = topMargin + colorMargin;
            areaView.LayoutParameters = areaParams;
            areaView.SetBackgroundResource(Resource.Drawable.xml_edit_bg);

            editView = new EditView(this);
            editView.colorAction = (color) =>
            {
                curColorIv?.SetBackgroundColor(Color.ParseColor(color));
            };
            editView.SetSrcBitmap(Bitmap.CreateBitmap(operateList[0]));
            RelativeLayout.LayoutParams conParams = new RelativeLayout.LayoutParams(areaParams.Width, areaParams.Height);
            conParams.AddRule(LayoutRules.CenterInParent);
            areaView.AddView(editView, conParams);

            //right tool area
            FrameLayout toolView = FindViewById<FrameLayout>(Resource.Id.tool_wrapper_view);
            FrameLayout.LayoutParams toolParams = (FrameLayout.LayoutParams)toolView.LayoutParameters;
            toolParams.Height = wrapperH;
            toolParams.Width = toolW;
            toolParams.TopMargin = topMargin;
            toolParams.LeftMargin = (int)(w - toolW - (topMargin * 2));
            toolView.LayoutParameters = toolParams;

            System.String[] colors = {
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
                imgIv.Tag = i;
                imgIv.LayoutParameters = colorParams;
                imgIv.SetBackgroundColor(Color.ParseColor(colors[i]));
                toolView.AddView(imgIv);
                if (i == 0)
                {
                    curColorIv = imgIv;
                }
                else
                {
                    imgIv.Click += (t, e) =>
                    {
                        int position = (int)((ImageView)t).Tag;
                        string color = colors[position];
                        curColorIv.SetBackgroundColor(Color.ParseColor(color));
                        editView.curColor = color;
                    };
                }
            }

            //The center of two button  色盘 吸管
            int cW = (int)((toolW - topMargin * 3) / 2.0);
            int cMargin = (int)(295 / 543.0 * wrapperH);
            int[] cIds = { Resource.Drawable.Button_pallet, Resource.Drawable.Button_spoid };
            for (int i = 0; i < cIds.Length; i++)
            {
                FrameLayout.LayoutParams btParams = new FrameLayout.LayoutParams(cW, cW);
                btParams.TopMargin = cMargin;
                btParams.LeftMargin = topMargin + i * (topMargin + cW);
                ImageView imgIv = new ImageView(this);
                imgIv.Tag = i;
                imgIv.LayoutParameters = btParams;
                imgIv.SetImageResource(cIds[i]);
                imgIv.Click += (t, e) =>
                {
                    int tag = (int)((ImageView)t).Tag;
                    switch (tag) {
                        case 0:
                            {
                                ColorPickerDialog dialog = new ColorPickerDialog(this, Color.ParseColor(editView.curColor));
                                dialog.colorAction = (color) =>
                                {
                                    curColorIv.SetBackgroundColor(Color.ParseColor(color));
                                    editView.curColor = color;
                                };
                                dialog.Show();
                                break;
                            }
                        case 1:
                            {
                                editView.Operate(OperateType.Straw);
                                break;
                            }
                        default: {
                                break;//nothing
                            }
                    }
                };
                toolView.AddView(imgIv);
            }

            //366 宽度 颜色
            int sTopMargin = (int)(366 / 543.0 * wrapperH);
            int sW = (int)(((colorW * 4) + (colorMargin * 2)) / 2.0);
            int sH = (int)(28 / 63.0 * sW);
            sW = sW * 2;
            for (int i = 0; i < 1; i++)
            {
                FrameLayout.LayoutParams btParams = new FrameLayout.LayoutParams(sW, sH);
                btParams.TopMargin = sTopMargin;
                btParams.LeftMargin = colorMargin + i * (colorMargin + sW);
                if (i == 0)
                {
                    widthEt = new EditText(this);
                    widthEt.LayoutParameters = btParams;
                    widthEt.SetBackgroundResource(Resource.Drawable.edit_size_bg);
                    widthEt.Text = "10";
                    widthEt.Gravity = GravityFlags.CenterVertical;
                    widthEt.SetPadding(4, 0, 4, 0);
                    widthEt.TextSize = (float)(sH / 4.0);
                    widthEt.InputType = InputTypes.ClassNumber;
                    toolView.Focusable = true;
                    toolView.FocusableInTouchMode = true;
                    toolView.AddView(widthEt);
                    widthEt.FocusChange += (v, hasFocus) =>
                    {
                        if (!widthEt.HasFocus)
                        {
                            widthEt.Text = widthEt.Text.Length <= 0 ? "10" : widthEt.Text;
                            editView.curStrokeW = int.Parse(widthEt.Text);
                        }
                    };
                    widthEt.SetOnEditorActionListener(this);
                    
                    widthEt.AddTextChangedListener(this);

                    FrameLayout.LayoutParams imgParams = new FrameLayout.LayoutParams(sH, sH);
                    imgParams.TopMargin = sTopMargin;
                    imgParams.LeftMargin = colorMargin + i * (colorMargin + sW) + sW-sH;
                    ImageView imageIv = new ImageView(this);
                    imageIv.LayoutParameters = imgParams;
                    imageIv.SetPadding(3, 3, 3, 3);
                    imageIv.SetImageResource(Resource.Drawable.edit_down);
                    toolView.AddView(imageIv);

                    imageIv.Click += (t, e) =>
                    {
                        PopupMenu popupMenu = new PopupMenu(imageIv.Context, imageIv);
                        popupMenu.Menu.Add("5");
                        popupMenu.Menu.Add("10");
                        popupMenu.Menu.Add("15");
                        popupMenu.Menu.Add("20");
                        popupMenu.Menu.Add("25");
                        popupMenu.Menu.Add("30");
                        popupMenu.Menu.Add("35");
                        popupMenu.Menu.Add("40");
                        popupMenu.Menu.Add("45");
                        popupMenu.Menu.Add("50");
                        popupMenu.SetOnMenuItemClickListener(this);
                        popupMenu.Show();
                    };
                }
                else
                {
                    ImageView imgIv = new ImageView(this);
                    imgIv.LayoutParameters = btParams;
                    imgIv.SetBackgroundColor(Color.ParseColor("#000000"));
                    toolView.AddView(imgIv);
                }
            }

            //The bottom of three button  复制 打开图片 恢复
            int[] resIds = { Resource.Drawable.Button_PictureCopy, Resource.Drawable.Button_PictureOpen, Resource.Drawable.Button_Restore };
            int btH = (int)(46 / 144.0 * toolW);
            int btTopMargin = (int)(2 / 144.0 * toolW);
            for (int i = 0; i < resIds.Length; i++)
            {
                FrameLayout.LayoutParams btParams = new FrameLayout.LayoutParams(toolW, btH);
                btParams.TopMargin = wrapperH - ((2 - i) * (btH + btTopMargin)) - btH;
                ImageView imgIv = new ImageView(this);
                imgIv.Tag = i;
                imgIv.LayoutParameters = btParams;
                imgIv.SetImageResource(resIds[i]);
                toolView.AddView(imgIv);
                imgIv.Click += (t, e) =>
                {
                    int tag = (int)((ImageView)t).Tag;
                    switch (tag)
                    {
                        case 0:
                            {
                                Bitmap oldB = operateList[mIndex];
                                operateList.Remove(oldB);
                                Bitmap bitmap = editView.CurBitmap();
                                operateList.Insert(mIndex, bitmap);
                                scaleList.Insert(mIndex, editView.CurScale());
                                operateList.Add(Bitmap.CreateBitmap(bitmap));
                                originList.Add(Bitmap.CreateBitmap(bitmap));
                                mIndex = operateList.Count - 1;
                                editView.SetSrcBitmap(bitmap);
                                oldB.Recycle();
                                mAdapter.NotifyDataSetChanged();
                                break;
                            }
                        case 1:
                            {
                                Intent intent = new Intent(this, typeof(PictureActivity));
                                StartActivityForResult(intent, 0, null);
                                break;
                            }
                        case 2:
                            {
                                Bitmap srcB = originList[mIndex];
                                Bitmap oldB = operateList[mIndex];
                                operateList.Remove(oldB);
                                operateList.Insert(mIndex, Bitmap.CreateBitmap(srcB));
                                editView.SetSrcBitmap(srcB);
                                mAdapter.NotifyDataSetChanged();
                                oldB.Recycle();
                                break;
                            }

                        default:
                            break;
                    }

                };
            }
        }

        private void InitOperateView()
        {
            int[] resIds = {
                Resource.Drawable.FlipXBtn, Resource.Drawable.FlipYBtn, Resource.Drawable.RRotate,
                Resource.Drawable.LRotate, Resource.Drawable.SizeIncreseBtn, Resource.Drawable.SizeDecreseBtn,
                Resource.Drawable.EraseBtn, Resource.Drawable.BrushBtn,
                Resource.Drawable.RectCutBtn, Resource.Drawable.FreeCutBtn, Resource.Drawable.EditAddSprite,Resource.Drawable.previous,Resource.Drawable.next
            };
            int sw = (int)(ScreenUtil.ScreenWidth(this) * 931 / 1280.0);
            FrameLayout contentView = FindViewById<FrameLayout>(Resource.Id.edit_bt_content_view);
            int itemW = (int)(sw / 13.0);
            int imgW = (int)(10.0 / 11 * itemW);
            for (int i = 0; i < resIds.Length; i++)
            {
                FrameLayout.LayoutParams layoutParams = new FrameLayout.LayoutParams(imgW, imgW);
                layoutParams.LeftMargin = i * itemW + 8;
                layoutParams.TopMargin = 8;
                ImageView imgBt = new ImageView(this);
                imgBt.SetImageResource(resIds[i]);
                imgBt.LayoutParameters = layoutParams;
                imgBt.Tag = i;
                imgBt.SetPadding(5,5,0,0);
                contentView.AddView(imgBt);
                imgBt.Click += (t, e) =>
                {
                    int tag = (int) ((ImageView)t).Tag;
                    UpdateOperateViews(tag);
                    OperateType[] types = { OperateType.FlipX, OperateType.FlipY, OperateType.RotateR, OperateType.RotateL,
                                            OperateType.Enlarge, OperateType.Narrow, OperateType.Eraser, OperateType.Draw,
                                            OperateType.RectCut, OperateType.FreeCut, OperateType.Add, OperateType.Previous, OperateType.Next
                                            };
                    OperateType type = types[tag];
                    if (type == OperateType.Add)
                    {
                        Intent intent = new Intent(this, typeof(PictureActivity));
                        StartActivityForResult(intent, 1, null);
                    }
                    else {
                        editView.Operate(types[tag]);
                    }
                };
            }
        }

        private void UpdateOperateViews(int tag)
        {
            FrameLayout contentView = FindViewById<FrameLayout>(Resource.Id.edit_bt_content_view);
            for (int i = 0; i < contentView.ChildCount; i++)
            {
                ImageView imgBt = (ImageView)contentView.GetChildAt(i);
                imgBt.SetBackgroundResource(tag!=i ? 0 : Resource.Drawable.xml_edit_bt_bg);
            }
        }

        public void AfterTextChanged(IEditable s)
        {
            if (widthEt.Text.Length>4)
            {
                widthEt.Text = widthEt.Text.Substring(0, 4);
                widthEt.SetSelection(widthEt.Text.Length);
            }
            while (widthEt.Text.Length>0  && widthEt.Text.Substring(0, 1) == "0")
            {
                widthEt.Text = widthEt.Text.Substring(1);
            }
            widthEt.SetSelection(widthEt.Text.Length);
            if (widthEt.Text.Length>0)
            {
                editView.curStrokeW = int.Parse(widthEt.Text);
            }
            LogUtil.CustomLog(widthEt.Text);
        }

        public void BeforeTextChanged(ICharSequence s, int start, int count, int after)
        {
            
        }

        public void OnTextChanged(ICharSequence s, int start, int before, int count)
        {
            
        }

        bool TextView.IOnEditorActionListener.OnEditorAction(TextView v, ImeAction actionId, KeyEvent e)
        {
            if (actionId == ImeAction.Done)
            {
                v.ClearFocus();
                return true;
            }
            return false;
        }

        /*PopMenuView interface*/
        public bool OnMenuItemClick(IMenuItem item)
        {
            string title = item.ToString();
            widthEt.Text = title;
            editView.curStrokeW = int.Parse(widthEt.Text);
            return true;
        }

        /*
         * 
         * Delegate and DataSource 
         * 
        */
        public int GetItemsCount(Java.Lang.Object adapter)
        {
            return operateList.Count;
        }

        public View GetItemView(Java.Lang.Object adapter, ViewGroup parent)
        {

            View convertView = LayoutInflater.From(this).Inflate(Resource.Layout.item_list_sprite, parent, false);
            //int itemW = (int)(ScreenUtil.ScreenWidth(this) * 146 / 1280.0 - ScreenUtil.dip2px(this, 24));

            int sw = ScreenUtil.ScreenWidth(this);
            double width = sw * 190 / 1280.0;
            int listW = (int)((190 - 60) / 1280.0 * sw);
            int itemW = listW - 30;

            ViewUtil.SetViewHeight(convertView, itemW);
            ViewHolder holder = new ViewHolder();
            holder.bgIv = convertView.FindViewById<ImageView>(Resource.Id.selected_material_bgIv);
            holder.imgIv = convertView.FindViewById<ImageView>(Resource.Id.selected_material_imgIv);
            holder.nameTv = convertView.FindViewById<TextView>(Resource.Id.sprite_tv);
            holder.deleteFl = convertView.FindViewById<FrameLayout>(Resource.Id.delete_fl);
            convertView.Tag = holder;
            convertView.Click += (t, e) =>
            {
                ViewHolder viewHolder = (ViewHolder)(((View)t).Tag);
                int position = (int)viewHolder.bgIv.Tag;
                ClickItem(position);
            };
            holder.deleteFl.Click += (t, e) =>
            {
                int position = (int)(((FrameLayout)t).Tag);
                ClickDeleteView(position);
            };
            return convertView;
        }

        public void UpdateItemView(Java.Lang.Object adapter, View contentView, int position)
        {
            Bitmap bitmap = operateList[position];
            ViewHolder viewHolder = (ViewHolder)contentView.Tag;
            viewHolder.imgIv.SetImageBitmap(bitmap);
            viewHolder.bgIv.Tag = position;
            viewHolder.deleteFl.Tag = position;
            viewHolder.nameTv.Text = "" + (position + 1);
            viewHolder.bgIv.SetBackgroundResource(position == mIndex ? Resource.Drawable.xml_asprite_item_bg_s : Resource.Drawable.xml_asprite_item_bg_n);
            viewHolder.deleteFl.Visibility = position == 0 ? ViewStates.Gone : ViewStates.Visible;
        }

        public void ClickItem(int position)
        {
            if (mIndex == position)
            {
                return;
            }
            Bitmap bitmap = operateList[mIndex];
            operateList.Remove(bitmap);
            operateList.Insert(mIndex, editView.CurBitmap());
            scaleList.RemoveAt(mIndex);
            scaleList.Insert(mIndex, editView.CurScale());
            bitmap.Recycle();
            editView.SetSrcBitmap(operateList[position]);
            mIndex = position;
            mAdapter.NotifyDataSetChanged();
        }

        public void ClickDeleteView(int position) {
            Bitmap src = originList[position];
            Bitmap bitmap = operateList[position];
            originList.Remove(src);
            operateList.Remove(bitmap);
            scaleList.RemoveAt(position);
            src.Recycle();
            bitmap.Recycle();
            if (position == mIndex)
            {
                editView.SetSrcBitmap(operateList[position - 1]);
                mIndex = position - 1;
            }
            mAdapter.NotifyDataSetChanged();
        }

        //定义ViewHolder内部类，用于对控件实例进行缓存
        class ViewHolder : Java.Lang.Object
        {
            public ImageView bgIv;
            public ImageView imgIv;
            public TextView nameTv;
            public FrameLayout deleteFl;
        }
    }

}


