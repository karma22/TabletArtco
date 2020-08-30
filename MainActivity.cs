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
using System.Linq;
using System.IO;
using Com.Bumptech.Glide.Request;
using System.Drawing.Printing;

namespace TabletArtco
{

    [Activity(Theme = "@style/AppTheme", LaunchMode = Android.Content.PM.LaunchMode.SingleTop)]
    public class MainActivity : AppCompatActivity, Delegate, DataSource, UpdateDelegate, View.IOnDragListener, View.IOnLongClickListener, PopupMenu.IOnMenuItemClickListener
    {
        private static string Tag = "MainActivity";
        private List<ActivatedSprite> spritesList = Project.mSprites;
        private List<DragImgView> imgList = new List<DragImgView>();
        private List<SpeakView> speakViewList = new List<SpeakView>();
        private SpriteAdapter mSpriteAdapter;
        private VariableAdapter mVariableAdapter;
        private int mSpriteIndex = -1;
        private int mLongPressSpriteIndex = -1;
        private bool isPlay;
        private VideoPlayer videoPlayer;
        private VideoPlayer p_videoPlayer;
        private bool isMute = false;
        private SoundPlayer bgmPlayer;
        private bool activateBlockScale = false;
        private View dragView;
        private bool isPractice = false;
        private Practice practice;
        private bool isPracticeStart = false;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            Window.SetFlags(Android.Views.WindowManagerFlags.Fullscreen, Android.Views.WindowManagerFlags.Fullscreen);
            RequestedOrientation = Android.Content.PM.ScreenOrientation.Landscape;
            SetContentView(Resource.Layout.activity_main);
            PermissionUtil.checkPermission(this);
            InitView();
        }

        protected override void OnResume()
        {
            base.OnResume();
            UpdateMainView();
            AddSpriteView();
            mSpriteAdapter.NotifyDataSetChanged();

            ActivatedSprite.mUpdateDelegate = this;
            ActivatedSprite.SoundAction = (sound) =>
            {
                RunOnUiThread(() =>
                {
                    if (isPlay)
                    {
                        new SoundPlayer(this).Play(sound);
                    }
                });
            };

            if (Project.currentBack != null)
            {
                if (!isPractice)
                {
                    ImageView imgIv = FindViewById<ImageView>(Resource.Id.preimage);
                    Glide.With(this)
                        .Load(Project.currentBack.remotePreviewImgPath.Equals("") ? Project.currentBack.remoteVideoPath : Project.currentBack.remotePreviewImgPath)
                        .Apply(new RequestOptions().Placeholder(Resource.Drawable.home_bg))
                        .Into(imgIv);

                    //mediaManager.SetPath(Project.currentBack.remoteVideoPath, Project.currentBack.remotePreviewImgPath, Project.currentBack.remoteSoundPath);
                    videoPlayer.SetPath(Project.currentBack.remoteVideoPath, Project.currentBack.remotePreviewImgPath, null);
                }
                else
                {
                    if (!isPracticeStart && practice != null)
                    {
                        videoPlayer.SetUri(practice.explainId, false);
                        videoPlayer.Play();
                    }
                    else
                    {
                        ImageView imgIv = FindViewById<ImageView>(Resource.Id.preimage);
                        Glide.With(this)
                            .Load(Project.currentBack.remotePreviewImgPath.Equals("") ? Project.currentBack.remoteVideoPath : Project.currentBack.remotePreviewImgPath)
                            .Apply(new RequestOptions().Placeholder(Resource.Drawable.home_bg))
                            .Into(imgIv);
                        //mediaManager.SetPath(Project.currentBack.remoteVideoPath, Project.currentBack.remotePreviewImgPath, Project.currentBack.remoteSoundPath);
                        videoPlayer.SetPath(Project.currentBack.remoteVideoPath, Project.currentBack.remotePreviewImgPath, null);
                        new Java.Lang.Thread(new Java.Lang.Runnable(() =>
                        {
                            videoPlayer.SetPath(Project.currentBack.remoteVideoPath, Project.currentBack.remotePreviewImgPath, null);
                            Thread.Sleep(10);
                            videoPlayer.Play();
                            new Java.Lang.Thread(new Java.Lang.Runnable(() =>
                            {
                                Thread.Sleep(10);
                                RunOnUiThread(() =>
                                {
                                    videoPlayer.Stop();
                                });
                            })).Start();
                        })).Start();
                    }
                }
            }
            else
            {
                ImageView imgIv = FindViewById<ImageView>(Resource.Id.preimage);
                imgIv.SetImageResource(Resource.Drawable.home_bg);
                imgIv.Visibility = ViewStates.Visible;
            }
        }

        protected override void OnPause()
        {
            base.OnPause();
            videoPlayer.Stop();
            SoundPlayer.StopAll();
            //mediaManager.Stop();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            // initialize ActivatedSprite
            Project.mSprites.RemoveRange(0, Project.mSprites.Count);
            mSpriteIndex = -1;

            // initialize Background
            Project.currentBack = null;
            videoPlayer.ClickHomeBt();
            SoundPlayer.bgmPath = null;

            // initialize Variavles
            Variable.ClearVariable();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        [System.Obsolete]
        public Bitmap GetBitmap(Sprite sprite)
        {
            if(sprite.category == -1)
            {
                return (Bitmap)Glide.With(this).AsBitmap().Load(sprite.remotePath).Into(100, 100).Get();
            }            
            return (Bitmap)Glide.With(this).AsBitmap().Load(GlideUtil.GetGlideUrl(sprite.remotePath)).Into(100, 100).Get();
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
                        if (isPractice)
                        {
                            break;
                        }
                        Bundle bundle = data.GetBundleExtra("bundle");
                        Sprite sprite = Sprite.ToSprite(bundle.GetString("model"));
                        if (sprite == null)
                            return;

                        new Thread(new Runnable(() =>
                        {
                            //Bitmap bitmap = (Bitmap)Glide.With(this).AsBitmap().Load(GlideUtil.GetGlideUrl(sprite.remotePath)).Into(100, 100).Get();
                            Bitmap bitmap = GetBitmap(sprite);
                            bitmap.HasAlpha = true;

                            sprite.bitmap = bitmap;
                            Project.AddSprite(sprite);
                            mSpriteIndex = spritesList.Count - 1;

                            RunOnUiThread(() =>
                            {
                                ListView listView = FindViewById<ListView>(Resource.Id.materailListView);
                                mSpriteAdapter.NotifyDataSetChanged();
                                UpdateBlockView();
                                AddSpriteView();
                            });
                        })).Start();
                        break;
                    }
                // select education callback
                case 1:
                    {
                        Bundle bundle = data.GetBundleExtra("bundle");
                        Background background = Background.ToBackground(bundle.GetString("model"));
                        if (isPractice && !bundle.GetBoolean("isPractice"))
                        {
                            FindViewById<RelativeLayout>(Resource.Id.p_wrapper_view).Visibility = ViewStates.Invisible;
                            ChangeLeftList(1);
                            changeLeftTopButtonImage(1);
                            Project.mSprites.RemoveRange(0, Project.mSprites.Count);
                            mSpriteIndex = -1;
                            mSpriteAdapter.NotifyDataSetChanged();
                            AddSpriteView();
                            UpdateBlockView();
                        }

                        if (background == null)
                        {
                            return;
                        }

                        ImageView imgIv = FindViewById<ImageView>(Resource.Id.preimage);
                        Glide.With(this).Load(background.remotePreviewImgPath.Equals("") ? background.remoteVideoPath : background.remotePreviewImgPath).Apply(new RequestOptions().Placeholder(Resource.Drawable.home_bg)).Into(imgIv);
                        Project.currentBack = background;
                        videoPlayer.SetPath(Project.currentBack.remoteVideoPath, Project.currentBack.remotePreviewImgPath, null);

                        isPractice = bundle.GetBoolean("isPractice");
                        if (isPractice)
                        {
                            isPracticeStart = false;
                            LoadPracticeProject(bundle);
                        }
                        break;
                    }
                // select background callback
                case 2:
                    {
                        Bundle bundle = data.GetBundleExtra("bundle");
                        if (isPractice && !bundle.GetBoolean("isPractice"))
                        {
                            FindViewById<RelativeLayout>(Resource.Id.p_wrapper_view).Visibility = ViewStates.Invisible;
                            ChangeLeftList(1);
                            changeLeftTopButtonImage(1);
                            Project.mSprites.RemoveRange(0, Project.mSprites.Count);
                            mSpriteIndex = -1;
                            mSpriteAdapter.NotifyDataSetChanged();
                            AddSpriteView();
                            UpdateBlockView();
                        }
                        Background background = Background.ToBackground(bundle.GetString("model"));
                        if (background == null)
                        {
                            return;
                        }

                        ImageView imgIv = FindViewById<ImageView>(Resource.Id.preimage);
                        Glide.With(this).Load(background.remotePreviewImgPath.Equals("") ? background.remoteVideoPath : background.remotePreviewImgPath).Apply(new RequestOptions().Placeholder(Resource.Drawable.home_bg)).Into(imgIv);
                        isPractice = false;
                        Project.currentBack = background;
                        videoPlayer.SetPath(Project.currentBack.remoteVideoPath, Project.currentBack.remotePreviewImgPath, null);

                        break;
                    }
                // select BGM callback
                case 4:
                    {
                        Bundle bundle = data.GetBundleExtra("bundle");
                        SoundPlayer.bgmPath = bundle.GetString("model");

                        break;
                    }
                // select background theme
                case 5:
                    {
                        Bundle bundle = data.GetBundleExtra("bundle");
                        string bg = bundle.GetString("model");
                        Editor.PutString("model", bg).Commit();
                        initBg();
                        break;
                    }
                // load sprite or project
                case 6:
                    {
                        if (resultCode == Result.Ok)
                        {
                            mSpriteIndex = spritesList.Count - 1;
                            mSpriteAdapter.NotifyDataSetChanged();
                            UpdateBlockView();
                            AddSpriteView();
                        }
                        break;
                    }
                // block select sound 
                case 7:
                    {
                        UpdateBlockView();
                        break;
                    }
                // block select background
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
                // EditActivity callback
                case 10:
                    {
                        break;
                    }
                // camera callback
                case 11:
                    {
                        byte[] bitmapData;

                        var image = (Bitmap)data.Extras.Get("data");
                        //Bitmap bm = Bitmap.CreateScaledBitmap(image, (int)(image.Width * 0.06), (int)(image.Height * 0.06), true);

                        var stream = new MemoryStream();
                        image.Compress(Bitmap.CompressFormat.Png, 0, stream);
                        bitmapData = stream.ToArray();

                        InputFileNameDlg(new MemoryStream(bitmapData));
                        break;
                    }
                default:
                    break;
            }
        }

        public void LoadPracticeProject(Bundle bundle)
        {
            ChangeLeftList(4);
            //changeLeftTopButtonImage(4);
            Project.mSprites.RemoveRange(0, Project.mSprites.Count);
            mSpriteIndex = -1;
            mSpriteAdapter.NotifyDataSetChanged();
            AddSpriteView();
            UpdateBlockView();
            int row = bundle.GetInt("row");
            int column = bundle.GetInt("column");
            if (row == 0)
            {
                int h = (int)(ActivatedSprite.notFullSize.Height * 100.0 / 548);
                int w = (int)(69.0 / 99 * h);
                Bitmap bitmap = BitmapFactory.DecodeResource(Resources, Resource.Drawable.figure_front);
                Sprite sprite = new Sprite();
                sprite.bitmap = Bitmap.CreateScaledBitmap(bitmap, w, h, false);
                sprite.name = "小飞";

                practice = Practice.createPracticeMode("" + (column + 1));
                if (practice == null)
                {
                    return;
                }
                Project.AddSprite(sprite);
                int[] srcs = {
                    Resource.Drawable.figure_right, Resource.Drawable.figure_Left,
                    Resource.Drawable.figure_front, Resource.Drawable.figure_Back,
                    Resource.Drawable.figure_Jump_Left, Resource.Drawable.figure_Jump_right };
                List<Bitmap> list = new List<Bitmap>();
                for (int i = 0; i < 6; i++)
                {
                    Bitmap b = BitmapFactory.DecodeResource(Resources, srcs[i]);
                    b = Bitmap.CreateScaledBitmap(b, w, h, false);
                    list.Add(b);
                }
                int x = (int)(practice.pointsList[0].X / 1000.0 * ActivatedSprite.notFullSize.Width);
                int y = (int)(practice.pointsList[0].Y / 548.0 * ActivatedSprite.notFullSize.Height);
                Project.mSprites[0].SetSrcBitmapList(list);                
                Project.mSprites[0].Location((int)(x - w / 6.0), y - 6);
                Project.mSprites[0].isVisible = false;
                mSpriteIndex = 0;
                ListView listView = FindViewById<ListView>(Resource.Id.materailListView);
                mSpriteAdapter.NotifyDataSetChanged();
                AddSpriteView();
                UpdateMainView();
                UpdateBlockView();
                videoPlayer.SetUri(practice.explainId, false);
                videoPlayer.Play();

                FindViewById<RelativeLayout>(Resource.Id.p_wrapper_view).Visibility = ViewStates.Visible;
                FindViewById<LinearLayout>(Resource.Id.successView).Visibility = ViewStates.Invisible;
                FindViewById<ImageView>(Resource.Id.p_start).Visibility = ViewStates.Visible;
            }
            else
            {
                isPracticeStart = true;
                practice = null;
            }
        }

        public void InputFileNameDlg(Stream stream)
        {
            EditText editText = new EditText(this);

            Android.App.AlertDialog.Builder builder = new Android.App.AlertDialog.Builder(this);
            builder.SetTitle("Artco");
            builder.SetMessage("请输入图片名称.");
            builder.SetView(editText);
            builder.SetPositiveButton("OK", (sender, args) =>
            {
                string fileName = editText.Text + ".png";
                if (FTPManager.ftpManager.UploadResource(stream, fileName))
                {
                    Toast.MakeText(Application, "Upload Succeeded", ToastLength.Short).Show();

                    Sprite sprite = new Sprite()
                    {
                        name = editText.Text,
                        category = 0,
                        remotePath = DBManager.imgPath + "sprites/" + GlideUtil.username + "/" + fileName
                    };
                    Sprite._sprites[0].Add(sprite);
                }
                else
                {
                    Toast.MakeText(Application, "Upload Failed", ToastLength.Short).Show();
                }
            });
            builder.SetNegativeButton("Cancel", (sender, args) =>
            {

            });

            builder.Show();
        }

        // init view
        public void InitView()
        {
            initBg();
            InitTopButtonEvent();
            InitLeftButtonEvent();
            InitMainView();
            InitSpriteListView();
        }
        public void initBg()
        {
            string bg = SharedPres.GetString("model", "blue");
            if (bg.Equals("blue"))
                FindViewById<LinearLayout>(Resource.Id.ll_main).SetBackgroundResource(Resource.Drawable.Background);
            else if (bg.Equals("red"))
                FindViewById<LinearLayout>(Resource.Id.ll_main).SetBackgroundResource(Resource.Drawable.Background_red);
            else if (bg.Equals("yellow"))
                FindViewById<LinearLayout>(Resource.Id.ll_main).SetBackgroundResource(Resource.Drawable.Background_yellow);
            else if (bg.Equals("black"))
                FindViewById<LinearLayout>(Resource.Id.ll_main).SetBackgroundResource(Resource.Drawable.Background_black);
        }


        //Top tool button
        public void InitTopButtonEvent()
        {
            int[] btsResIds = {
                Resource.Id.bt_choice1, Resource.Id.bt_choice2, Resource.Id.bt_choice3, Resource.Id.bt_choice4, Resource.Id.bt_choice5,
                Resource.Id.bt_choice6, Resource.Id.bt_choice7, Resource.Id.bt_choice8, Resource.Id.bt_choice9, Resource.Id.bt_choice10
            };
            int height = (int)((ScreenUtil.ScreenHeight(this) - ScreenUtil.StatusBarHeight(this)) * 80 / 800.0 - 18);
            for (int i = 0; i < btsResIds.Length; i++)
            {
                ImageView imgBt = FindViewById<ImageView>(btsResIds[i]);
                imgBt.Tag = i;
                ViewUtil.SetViewSize(imgBt, (int)(height * 73 / 70.0), height);
                imgBt.Click += (t, e) =>
                {
                    new SoundPlayer(this).PlayLocal(SoundPlayer.mouse_click);
                    if (isPlay)
                    {
                        return;
                    }

                    switch ((int)((ImageView)t).Tag)
                    {
                        case 0:
                            {
                                // Background  select activity
                                Intent intent = new Intent(this, typeof(BackgroundActivity));
                                intent.AddFlags(ActivityFlags.ReorderToFront);
                                StartActivityForResult(intent, 2, null);
                                break;
                            }
                        case 1:
                            {
                                // Sprite select Activity
                                Intent intent = new Intent(this, typeof(PictureActivity));
                                intent.AddFlags(ActivityFlags.ReorderToFront);
                                StartActivityForResult(intent, 0, null);
                                break;
                            }
                        case 2:
                            {
                                // Education select activity
                                Intent intent = new Intent(this, typeof(EducationActivity));
                                intent.AddFlags(ActivityFlags.ReorderToFront);
                                StartActivityForResult(intent, 1, null);
                                break;
                            }
                        case 3:
                            {
                                // Sound select activity
                                Intent intent = new Intent(this, typeof(SoundActivity));
                                intent.AddFlags(ActivityFlags.ReorderToFront);
                                StartActivityForResult(intent, 3, null);
                                break;
                            }
                        case 4:
                            {
                                // BGM select activity
                                Intent intent = new Intent(this, typeof(MusicActivity));
                                intent.AddFlags(ActivityFlags.ReorderToFront);
                                StartActivityForResult(intent, 4, null);
                                break;
                            }
                        case 5:
                            {
                                // save project
                                SaveDialog dialog = new SaveDialog(this, (text) =>
                                {
                                    new ArtcoProject(this).SaveProject(text);
                                });
                                dialog.Show();

                                break;
                            }
                        case 6:
                            {
                                // to project activity
                                Intent intent = new Intent(this, typeof(ProjectActivity));
                                intent.AddFlags(ActivityFlags.ReorderToFront);
                                StartActivityForResult(intent, 6, null);

                                break;
                            }
                        case 7:
                            {
                                // to setting activity
                                Intent intent = new Intent(this, typeof(SettingActivity));
                                intent.AddFlags(ActivityFlags.ReorderToFront);
                                StartActivityForResult(intent, 5, null);
                                break;
                            }
                        case 8:
                            {
                                // to aboutus activity
                                Intent intent = new Intent(this, typeof(AboutUsActivity));
                                intent.AddFlags(ActivityFlags.ReorderToFront);
                                StartActivity(intent);
                                break;
                            }
                        case 9:
                            {
                                Intent intent = new Intent(Android.Provider.MediaStore.ActionImageCapture);
                                intent.AddFlags(ActivityFlags.ReorderToFront);
                                StartActivityForResult(intent, 11, null);
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
                Resource.Id.bt_left_select1, Resource.Id.bt_left_select2, Resource.Id.bt_left_select3, Resource.Id.bt_left_select4, Resource.Id.bt_left_select5
            };
            //int itemW = (int)((ScreenUtil.ScreenWidth(this) * 244.0 / 1280 - (12 * 2)) / 4);
            for (int i = 0; i < btsResIds.Length - 1; i++)
            {
                ImageView imgBt = FindViewById<ImageView>(btsResIds[i]);
                imgBt.Tag = i;
                //ViewUtil.SetViewSize(imgBt, itemW, (int)(itemW * 45.0 / 55));
                imgBt.Click += (t, e) =>
                {
                    new SoundPlayer(this).PlayLocal(SoundPlayer.mouse_click);
                    if (isPlay || isPractice)
                    {
                        return;
                    }
                    int tag = (int)((ImageView)t).Tag;
                    changeLeftTopButtonImage(tag);
                };
            }
            ChangeLeftList(1);
            FindViewById<ScrollView>(Resource.Id.left_blocks_view_wrapper).SetOnDragListener(this);
        }

        public void changeLeftTopButtonImage(int index)
        {
            int[] btsResIds = {
                Resource.Id.bt_left_select1, Resource.Id.bt_left_select2, Resource.Id.bt_left_select3, Resource.Id.bt_left_select4, Resource.Id.bt_left_select5
            };
            int[] normalImgResId = {
                        Resource.Drawable.Button_event_Deactivation,
                        Resource.Drawable.Button_coding1_Deactivation,
                        Resource.Drawable.Button_coding2_Deactivation,
                        Resource.Drawable.Button_control_Deactivation,
                        Resource.Drawable.Button_AI_Deactivation,
                    };
            int[] selectImgResId = {
                        Resource.Drawable.Button_event_activation,
                        Resource.Drawable.Button_coding1_activation,
                        Resource.Drawable.Button_coding2_activation,
                        Resource.Drawable.Button_control_activation,
                        Resource.Drawable.Button_AI_activation,
                    };
            for (int j = 0; j < 5; j++)
            {
                ImageView tempBt = FindViewById<ImageView>(btsResIds[j]);
                if (index == j)
                {
                    tempBt.SetImageResource(selectImgResId[j]);
                    ChangeLeftList(j);
                }
                else
                {
                    tempBt.SetImageResource(normalImgResId[j]);
                }
            }
        }

        //change left blocks list
        public void ChangeLeftList(int index)
        {
            FrameLayout blockView = FindViewById<FrameLayout>(Resource.Id.left_blocks_view);
            blockView.RemoveAllViews();

            var blocks = Block._blocks[index];

            int margin = 12;
            int padding = 4;
            double rowWidth = ScreenUtil.ScreenWidth(this) * 244.0 / 1280 - (12 * 2);
            int itemW = (int)((rowWidth - (margin * 2) - (padding * 2)) / 3);
            for (int i = 0; i < blocks.Count; i++)
            {
                FrameLayout.LayoutParams param = new FrameLayout.LayoutParams(itemW, itemW);
                param.LeftMargin = i % 3 * (itemW + padding) + margin;
                param.TopMargin = i / 3 * (itemW + padding) + padding;
                ImageView imgIv = new ImageView(this);
                imgIv.LayoutParameters = param;
                imgIv.Tag = index * 1000 + i;
                imgIv.SetImageResource(blocks[i].resourceId);
                blockView.AddView(imgIv);
                imgIv.Click += (t, e) =>
                {
                    new SoundPlayer(this).PlayLocal(SoundPlayer.mouse_click);
                    if (isPlay)
                    {
                        return;
                    }
                    if (mSpriteIndex < 0 || mSpriteIndex >= spritesList.Count)
                    {
                        return;
                    }

                    int tag = (int)(((ImageView)t).Tag);
                    int tabIndex = tag / 1000;
                    int tempIndex = tag - tabIndex * 1000;
                    if (Block._blocks[tabIndex][tempIndex].name.Equals("MoveEmpty"))
                    {
                        return;
                    }

                    if (spritesList[mSpriteIndex].mBlocks.Count == 0 && tabIndex != 0)
                    {
                        Block b = new Block(Block._blocks[0][0]);
                        spritesList[mSpriteIndex].AddBlock(b);
                    }
                    
                    Block block = new Block(Block._blocks[tabIndex][tempIndex]);
                    if (Block._blocks[tabIndex][tempIndex].category == 2)
                    {
                        block.text = "1";
                    }

                    if (isPractice)
                    {
                        List<Block> list = spritesList[mSpriteIndex].mBlocks[0];
                        if (practice.solutionList.Count > list.Count)
                        {
                            // 练习block匹配才加入
                            if (practice.solutionList[list.Count].Contains(block.name))
                            {
                                LogUtil.CustomLog("=========" + practice.solutionList[list.Count]);
                                var arr = practice.solutionList[list.Count].Split(":");
                                if (arr.Length > 1)
                                {
                                    block.text = arr[1];
                                };
                                spritesList[mSpriteIndex].AddBlock(block);
                                UpdateBlockView();
                            }
                        }
                    }
                    else
                    {
                        bool ret = spritesList[mSpriteIndex].AddBlock(block);
                        if(ret)
                        {
                            UpdateBlockView();
                        }
                        else
                        {
                            ConfirmDialog confirmDialog = new ConfirmDialog(this);
                            confirmDialog.SetMessage("因为动作积木默认为无限循环，所以动作积木后不能放置其他积木");
                            confirmDialog.Show();                            
                        }
                    }
                };
            }

            if (index == 3)
            {
                int tempW = (int)((ScreenUtil.ScreenWidth(this) * 244.0 / 1280 - (12 * 2)) / 4);

                int tempH = (int)(720 / 800.0 * ScreenUtil.ScreenHeight(this)) - (int)(tempW * 45.0 / 55);

                int w = (int)((rowWidth - margin * 3) / 2);
                int h = (int)(22 / 72.0 * w);
                for (int i = 0; i < 2; i++)
                {
                    FrameLayout.LayoutParams param = new FrameLayout.LayoutParams(w, h);
                    param.LeftMargin = margin + (w + margin) * i;
                    param.TopMargin = tempH - h - margin - margin / 2;
                    ImageView imgIv = new ImageView(this);
                    imgIv.LayoutParameters = param;
                    imgIv.Tag = i;
                    if (i == 0)
                        imgIv.SetBackgroundResource(Resource.Drawable.CreateVarBtn);
                    else
                        imgIv.SetBackgroundResource(Resource.Drawable.HideVarListBtn);
                    blockView.AddView(imgIv, param);
                    imgIv.Click += (t, e) =>
                    {
                        int tag = (int)((ImageView)t).Tag;
                        new SoundPlayer(this).PlayLocal(SoundPlayer.mouse_click);
                        switch (tag)
                        {
                            case 0:
                                {
                                    if (isPlay)
                                        break;

                                    VariableInitDialog dialog = new VariableInitDialog(this, (name, value) =>
                                    {
                                        Variable.variableMap[name] = value;
                                        mVariableAdapter.NotifyDataSetChanged();
                                    });
                                    dialog.Show();
                                    break;
                                }
                            case 1:
                                {
                                    ListView listView = FindViewById<ListView>(Resource.Id.variableListView);
                                    if (listView.Visibility == ViewStates.Visible)
                                    {
                                        listView.Visibility = ViewStates.Invisible;
                                        imgIv.SetBackgroundResource(Resource.Drawable.ShowVarListBtn);
                                    }
                                    else
                                    {
                                        listView.Visibility = ViewStates.Visible;
                                        imgIv.SetBackgroundResource(Resource.Drawable.HideVarListBtn);
                                    }
                                    break;
                                }
                            default: break;
                        }
                    };
                }
            }
        }

        // center main view
        public void InitMainView()
        {
            LinearLayout mainView = FindViewById<LinearLayout>(Resource.Id.mainView);
            FrameLayout centerView = FindViewById<FrameLayout>(Resource.Id.centerView);
            double width = ScreenUtil.ScreenWidth(this) * 890 / 1280.0;
            //double height = ScreenUtil.ScreenHeight(this) * 545 / 800.0;
            double height = ScreenUtil.ScreenHeight(this) * 545 / 800.0 - 15;
            int paddingL = (int)(18 / 913.0 * width);
            int paddingT = (int)(20 / 549.0 * height);
            mainView.SetPadding(paddingL, paddingT, paddingL, 0);
            ViewUtil.SetViewHeight(mainView, (int)height);
            ViewUtil.SetViewHeight(centerView, (int)(482 / 545.0 * height));

            LinearLayout mainViewHighlight = FindViewById<LinearLayout>(Resource.Id.mainView_highlight);
            ViewUtil.SetViewHeight(mainViewHighlight, (int)(ScreenUtil.ScreenHeight(this) * 545 / 800.0));

            int[] sbtsResIds = { Resource.Id.bt_center1, Resource.Id.bt_center2, Resource.Id.bt_center3, Resource.Id.bt_center4, Resource.Id.bt_center5 };
            int itemW = (int)(42 / 549.0 * height);

            LinearLayout rl = FindViewById<LinearLayout>(Resource.Id.rl_center);
            ViewUtil.SetViewSize(rl, itemW * 15 / 4, itemW * 3 / 4);
            rl.SetY(itemW / 8);

            // home、play、stop、full button, bgm mute
            for (int i = 0; i < sbtsResIds.Length; i++)
            {
                ImageView imgBt1 = FindViewById<ImageView>(sbtsResIds[i]);
                ViewUtil.SetViewSize(imgBt1, itemW, itemW);
                imgBt1.Tag = i;
                imgBt1.Click += (t, e) =>
                {
                    int tag = (int)((ImageView)t).Tag;
                    new SoundPlayer(this).PlayLocal(SoundPlayer.mouse_click);
                    switch (tag)
                    {
                        case 0:
                            {
                                if (isPlay)
                                {
                                    return;
                                }

                                // home button click
                                MessageBoxDialog dialog = new MessageBoxDialog(this, "确定要回到主界面吗?", () =>
                                {
                                    // initialize ActivatedSprite
                                    Project.mSprites.RemoveRange(0, Project.mSprites.Count);
                                    mSpriteIndex = -1;
                                    mSpriteAdapter.NotifyDataSetChanged();
                                    AddSpriteView();
                                    UpdateBlockView();
                                    // initialize LeftButtons
                                    ImageView imgBt;

                                    int[] btsResIds = {
                                        Resource.Id.bt_left_select1, Resource.Id.bt_left_select2, Resource.Id.bt_left_select3, Resource.Id.bt_left_select4, Resource.Id.bt_left_select5
                                    };
                                    imgBt = FindViewById<ImageView>(btsResIds[0]);
                                    imgBt.SetImageResource(Resource.Drawable.Button_event_Deactivation);
                                    imgBt = FindViewById<ImageView>(btsResIds[1]);
                                    imgBt.SetImageResource(Resource.Drawable.Button_coding1_activation);
                                    imgBt = FindViewById<ImageView>(btsResIds[2]);
                                    imgBt.SetImageResource(Resource.Drawable.Button_coding2_Deactivation);
                                    imgBt = FindViewById<ImageView>(btsResIds[3]);
                                    imgBt.SetImageResource(Resource.Drawable.Button_control_Deactivation);
                                    imgBt = FindViewById<ImageView>(btsResIds[4]);
                                    imgBt.SetImageResource(Resource.Drawable.Button_AI_Deactivation);

                                    ChangeLeftList(1);

                                    // initialize Background
                                    //mediaManager.ClickHomeButton();
                                    videoPlayer.SetPath(null, null, null);
                                    Project.currentBack = null;
                                    videoPlayer.hideVideo();
                                    videoPlayer.ClickHomeBt();
                                    FindViewById<RelativeLayout>(Resource.Id.p_wrapper_view).Visibility = ViewStates.Invisible;

                                    SoundPlayer.bgmPath = null;

                                    // initialize Variavles
                                    Variable.ClearVariable();
                                    mVariableAdapter.NotifyDataSetChanged();
                                });
                                dialog.Show();
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

                                if (!Project.RunSprite(this))
                                    return;

                                isPlay = true;
                                mainViewHighlight.Visibility = ViewStates.Visible;                                
                                videoPlayer.Play();
                                if (!isMute)
                                    bgmPlayer.Play(SoundPlayer.bgmPath);

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

                                mainViewHighlight.Visibility = ViewStates.Invisible;
                                isPlay = false;
                                Project.StopSprite();
                                videoPlayer.Stop();
                                SoundPlayer.StopAll();
                                if (Project.currentBack != null)
                                {
                                    if (!Project.currentBack.remoteVideoPath.Equals(videoPlayer.mPath))
                                    {
                                        videoPlayer.SetPath(Project.currentBack.remoteVideoPath, Project.currentBack.remotePreviewImgPath, null);
                                    }
                                }
                                break;
                            }
                        case 3:
                            {
                                if (isPlay || isPractice)
                                {
                                    return;
                                }
                                //full button click
                                Intent intent = new Intent(this, typeof(FullScreenActivity));
                                intent.AddFlags(ActivityFlags.ReorderToFront);
                                StartActivity(intent);
                                break;
                            }
                        case 4:
                            {
                                //bgm mute button click
                                if (isPlay)
                                {
                                    return;
                                }

                                isMute = !isMute;
                                if (isMute)
                                {
                                    ((ImageView)t).SetBackgroundResource(Resource.Drawable.BGM_mute);
                                }
                                else
                                {
                                    ((ImageView)t).SetBackgroundResource(Resource.Drawable.BGM);
                                }

                                break;
                            }
                        default:
                            break;
                    }
                };
            }

            ActivatedSprite.notFullSize = new Android.Util.Size((int)width - paddingL * 2, (int)(481 / 549.0 * height));
            ActivatedSprite.fullSize = new Android.Util.Size(ScreenUtil.ScreenWidth(this), ScreenUtil.ScreenHeight(this) - ScreenUtil.dip2px(this, 50));


            // video surfaceview
            VideoView videoView = FindViewById<VideoView>(Resource.Id.video_view);
            //videoView.SetBackgroundColor(Color.Red);
            //ViewUtil.SetViewSize(videoView, (int)width-paddingL-paddingL, (int)(481 / 549.0 * height));
            //SurfaceView surfaceView = FindViewById<SurfaceView>(Resource.Id.surfaceView);
            ImageView imgIv = FindViewById<ImageView>(Resource.Id.preimage);
            //mediaManager = new MediaManager(surfaceView, imgIv, this);

            videoPlayer = new VideoPlayer(videoView, imgIv, this);
            videoPlayer.PlayDefault();
            bgmPlayer = new SoundPlayer(this);
            bgmPlayer.PlayLocal(Resource.Raw.Stage_Default);

            //mediaManager = new MediaManager2(surfaceView, imgIv, this);

            //varible listView
            ListView listView = FindViewById<ListView>(Resource.Id.variableListView);
            mVariableAdapter = new VariableAdapter(this);
            mVariableAdapter.variableMap = Variable.variableMap;
            mVariableAdapter.mAction += (position, type) =>
            {
                if (type == 1)
                {
                    List<string> keys = Variable.variableMap.Keys.ToList();
                    Variable.RemoveVariable(keys[position]);
                    mVariableAdapter.NotifyDataSetChanged();
                }
                else
                {
                    VariableInitDialog dialog = new VariableInitDialog(this, (name, value) =>
                    {
                        List<string> keys = Variable.variableMap.Keys.ToList();
                        Variable.RemoveVariable(keys[position]);
                        Variable.AddVariable(name, value);
                        mVariableAdapter.NotifyDataSetChanged();
                    });
                    dialog.Show();
                }
            };
            listView.Adapter = mVariableAdapter;

            FindViewById<ImageView>(Resource.Id.p_start).Click += (t, e) =>
            {
                isPracticeStart = true;
                FindViewById<RelativeLayout>(Resource.Id.p_wrapper_view).Visibility = ViewStates.Invisible;
                Project.mSprites[0].isVisible = true;
                UpdateMainView();
                videoPlayer.Stop();
                ImageView imgIv = FindViewById<ImageView>(Resource.Id.preimage);
                Glide.With(this)
                    .Load(Project.currentBack.remotePreviewImgPath.Equals("") ? Project.currentBack.remoteVideoPath : Project.currentBack.remotePreviewImgPath)
                    .Apply(new RequestOptions().Placeholder(Resource.Drawable.home_bg))
                    .Into(imgIv);
                videoPlayer.SetPath(Project.currentBack.remoteVideoPath, Project.currentBack.remotePreviewImgPath, null);
                //videoPlayer.SetUri(practice.practiceId, true);
                FindViewById<RelativeLayout>(Resource.Id.p_wrapper_view).Visibility = ViewStates.Invisible;
                //new Java.Lang.Thread(new Java.Lang.Runnable(() =>
                //{
                //    Thread.Sleep(10);
                //    RunOnUiThread(() =>
                //    {
                //        videoPlayer.Play();
                //        new Java.Lang.Thread(new Java.Lang.Runnable(() =>
                //        {
                //            Thread.Sleep(2000);
                //            RunOnUiThread(() =>
                //            {
                //                if (!isPlay)
                //                {
                //                    videoPlayer.Stop();
                //                }
                //            });
                //        })).Start();
                //    });
                //})).Start();
            };

            FindViewById<ImageView>(Resource.Id.p_next_bt).Click += (t, e) =>
            {
                if (practice.level == "10")
                {
                    FindViewById<RelativeLayout>(Resource.Id.p_wrapper_view).Visibility = ViewStates.Invisible;
                    Project.mSprites[0].isVisible = true;
                    UpdateMainView();
                }
                else
                {
                    FindViewById<ImageView>(Resource.Id.bt_center3).PerformClick();
                    int level = int.Parse(practice.level);
                    List<List<Background>> backgrounds = Background._backgrounds;
                    Project.currentBack = backgrounds[0][level];
                    //mediaManager.SetPath(Project.currentBack.remoteVideoPath, Project.currentBack.remotePreviewImgPath, Project.currentBack.remoteSoundPath);
                    videoPlayer.SetPath(Project.currentBack.remoteVideoPath, Project.currentBack.remotePreviewImgPath, null);
                    practice = Practice.createPracticeMode((level + 1) + "");

                    Bundle bundle = new Bundle();
                    bundle.PutInt("row", 0);
                    bundle.PutInt("column", level);
                    LoadPracticeProject(bundle);
                }
            };

            FindViewById<ImageView>(Resource.Id.p_again_bt).Click += (t, e) =>
            {
                FindViewById<RelativeLayout>(Resource.Id.p_wrapper_view).Visibility = ViewStates.Invisible;
                Project.mSprites[0].isVisible = true;

                FindViewById<ImageView>(Resource.Id.bt_center3).PerformClick();
                UpdateMainView();
            };

            FindViewById<ImageView>(Resource.Id.p_stop_bt).Click += (t, e) =>
            {
                FindViewById<ImageView>(Resource.Id.bt_center3).PerformClick();
                Intent intent = new Intent(this, typeof(BackgroundActivity));
                intent.AddFlags(ActivityFlags.ReorderToFront);
                StartActivityForResult(intent, 2, null);
            };

            RelativeLayout activate_block_wrapperview = FindViewById<RelativeLayout>(Resource.Id.activate_block_wrapperview);
            FrameLayout blockView = FindViewById<FrameLayout>(Resource.Id.block_view);
            blockView.SetOnDragListener(this);
            ViewUtil.SetViewHeight(activate_block_wrapperview, (int)(ScreenUtil.ScreenHeight(this) * 175 / 800.0 - ScreenUtil.dip2px(this, 8)));

            // scale button
            ImageView scaleButton = FindViewById<ImageView>(Resource.Id.bt_scale);
            scaleButton.Click += (t, e) =>
            {
                if (isPlay)
                {
                    return;
                }
                new SoundPlayer(this).PlayLocal(SoundPlayer.mouse_click);
                activateBlockScale = !activateBlockScale;
                if (activateBlockScale)
                {
                    ViewUtil.SetViewHeight(activate_block_wrapperview, (int)(ScreenUtil.ScreenHeight(this) * 175 * 3 / 800.0)); 
                    activate_block_wrapperview.SetBackgroundResource(Resource.Drawable.BlockGlass_large);
                    scaleButton.SetImageResource(Resource.Drawable.zoom_in);
                }
                else
                {
                    ViewUtil.SetViewHeight(activate_block_wrapperview, (int)(ScreenUtil.ScreenHeight(this) * 175 / 800.0 - ScreenUtil.dip2px(this, 8)));
                    activate_block_wrapperview.SetBackgroundResource(Resource.Drawable.BlockGlass_small);
                    scaleButton.SetImageResource(Resource.Drawable.zoom_out);
                }
            };

            FindViewById<ImageView>(Resource.Id.bt_add).Visibility = ViewStates.Gone;
            // add variable button
            //FindViewById<ImageView>(Resource.Id.bt_add).Click += (t, e) =>
            //{
            //    if (isPlay)
            //    {
            //        return;
            //    }
            //    VariableInitDialog dialog = new VariableInitDialog(this, (name, value)=> {
            //        Project.variableMap[name] = value;
            //    });
            //    dialog.Show();
            //};

            // clear block button
            FindViewById<ImageView>(Resource.Id.bt_clear_block).Click += (t, e) =>
            {
                if (isPlay)
                {
                    return;
                }
                new SoundPlayer(this).PlayLocal(SoundPlayer.all_clear);
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
            FindViewById<ImageView>(Resource.Id.bt_delete).Click += (t, e) =>
            {
                if (!isPlay)
                {
                    new SoundPlayer(this).PlayLocal(SoundPlayer.all_clear);
                    Project.mSprites.RemoveRange(0, Project.mSprites.Count);
                    mSpriteIndex = -1;
                    mSpriteAdapter.NotifyDataSetChanged();
                    AddSpriteView();
                    UpdateBlockView();
                }
            };
        }

        // main screen add animate sprite view
        public void AddSpriteView()
        {
            FrameLayout containerView = FindViewById<FrameLayout>(Resource.Id.ContainerView);
            containerView.RemoveAllViews();
            imgList.RemoveRange(0, imgList.Count);
            speakViewList.RemoveRange(0, speakViewList.Count);

            TextView tvX = FindViewById<TextView>(Resource.Id.tv_coordX);
            TextView tvY = FindViewById<TextView>(Resource.Id.tv_coordY);

            for (int i = 0; i < spritesList.Count; i++)
            {
                ActivatedSprite activatedSprite = spritesList[i];
                DragImgView imgIv = new DragImgView(this);
                imgIv.SetImageBitmap(activatedSprite.GetSpriteBit());
                FrameLayout.LayoutParams layoutParams = new FrameLayout.LayoutParams(activatedSprite.curSize.Width, activatedSprite.curSize.Height);
                layoutParams.LeftMargin = activatedSprite.curPoint.X;
                layoutParams.TopMargin = activatedSprite.curPoint.Y;
                containerView.AddView(imgIv, layoutParams);
                imgIv.Visibility = activatedSprite.isVisible ? ViewStates.Visible : ViewStates.Invisible;
                imgList.Add(imgIv);
                imgIv.Tag = 100 + i;
                imgIv.MoveAction += (t, x, y) =>
                {
                    if (!isPlay && !isPractice)
                    {
                        activatedSprite.AddToOriginPoint((int)x, (int)y);
                        tvX.Text = "X : " + activatedSprite.originPoint.X;
                        tvY.Text = "Y : " + activatedSprite.originPoint.Y;
                    }
                };
                imgIv.ClickAction += (t) =>
                {
                    int tag = (int)((DragImgView)t).Tag;
                    if (!isPlay)
                    {
                        Intent intent = new Intent(this, typeof(EditActivity));
                        Bundle bundle = new Bundle();
                        bundle.PutInt("position", tag - 100);
                        intent.PutExtra("bundle", bundle);
                        intent.AddFlags(ActivityFlags.ReorderToFront);
                        StartActivityForResult(intent, 10, null);
                    }
                    else
                    {
                        if (tag - 100 < Project.mSprites.Count)
                        {
                            Project.mSprites[tag - 100].ReceiveClickSignal();
                        }
                    }
                };
                SpeakView speakView = new SpeakView(this);
                FrameLayout.LayoutParams p = new FrameLayout.LayoutParams(FrameLayout.LayoutParams.WrapContent, FrameLayout.LayoutParams.WrapContent);
                containerView.AddView(speakView, p);
                speakViewList.Add(speakView);
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
                imgIv.SetImageBitmap(activatedSprite.GetSpriteBit());
                FrameLayout.LayoutParams layoutParams = (FrameLayout.LayoutParams)imgIv.LayoutParameters;
                layoutParams.LeftMargin = activatedSprite.curPoint.X;
                layoutParams.TopMargin = activatedSprite.curPoint.Y;
                layoutParams.Width = activatedSprite.curSize.Width;
                layoutParams.Height = activatedSprite.curSize.Height;
                imgIv.Visibility = activatedSprite.isVisible ? ViewStates.Visible : ViewStates.Invisible;
                containerView.UpdateViewLayout(imgIv, layoutParams);

                SpeakView speakView = speakViewList[i];
                speakView.SetSpeakText(activatedSprite.speakText);
                if (speakView.Visibility == ViewStates.Visible)
                {
                    FrameLayout.LayoutParams p = (FrameLayout.LayoutParams)speakView.LayoutParameters;
                    p.LeftMargin = activatedSprite.curPoint.X;
                    p.TopMargin = activatedSprite.curPoint.Y - 100;
                    containerView.UpdateViewLayout(speakView, p);
                }
            }
            JudgeCollision();
        }

        // update bottom block views
        public void UpdateBlockView()
        {
            FrameLayout blockView = FindViewById<FrameLayout>(Resource.Id.block_view);
            int width = (int)(ScreenUtil.ScreenWidth(this) * 890 / 1280.0) - ScreenUtil.dip2px(this, 30);
            int GridViewH = (int)(ScreenUtil.ScreenHeight(this) * 175 / 800.0 - ScreenUtil.dip2px(this, 28));
            int margin = 3;
            int itemW = (int)((GridViewH - margin) / 2.0);
            //int padding = 5;
            int column = (width - itemW / 2 + margin) / (itemW + margin);
            //padding = (width - column * itemW - (column - 1) * margin)/2;
            //column = column - 1;
            blockView.RemoveAllViews();
            if (mSpriteIndex > -1)
            {
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
                        FrameLayout.LayoutParams layoutParams = new FrameLayout.LayoutParams(itemW / 2, itemW / 2);
                        //layoutParams.LeftMargin = padding;
                        layoutParams.TopMargin = originY + itemW / 4;
                        blockView.AddView(view, layoutParams);
                    }

                    for (int j = 0; j < list.Count; j++)
                    {
                        originY += (j > 0 && j % column == 0) ? itemW + margin : 0;
                        Block block = list[j];
                        FrameLayout view = new FrameLayout(this);
                        view.SetBackgroundResource(block.resourceId);
                        FrameLayout.LayoutParams layoutParams = new FrameLayout.LayoutParams(itemW, itemW);
                        //layoutParams.LeftMargin = padding + itemW /2 + (itemW + margin) * (j % column);
                        layoutParams.LeftMargin = itemW / 2 + (itemW + margin) * (j % column);
                        layoutParams.TopMargin = originY;
                        blockView.AddView(view, layoutParams);
                        view.SetOnLongClickListener(this);
                        view.Tag = i * 10000 + j;
                        List<Dictionary<string, string>> locationList = BlockResources.TextViewLocations(block);
                        if (locationList != null && locationList.Count > 0)
                        {
                            for (int k = 0; k < locationList.Count; k++)
                            {
                                Dictionary<string, string> dic = locationList[k];
                                float x = float.Parse(dic["x"]) * itemW;
                                float y = float.Parse(dic["y"]) * itemW;
                                float w = float.Parse(dic["w"]) * itemW;
                                float h = float.Parse(dic["h"]) * itemW;
                                TextView tv = new TextView(this);
                                FrameLayout.LayoutParams tvparams = new FrameLayout.LayoutParams((int)w, (int)h);
                                tvparams.LeftMargin = (int)x;
                                tvparams.TopMargin = (int)y;
                                tv.Text = dic["text"];
                                tv.TextSize =  h/2.8f; // h/1.5f; //
                                tv.SetPadding(0,0,0,0);
                                tv.TextAlignment = TextAlignment.Center;
                                view.AddView(tv, tvparams);
                            }
                        }
                        int clickType = BlockResources.GetClickType(block);
                        // activateblock click event
                        view.Click += (t, e) =>
                        {
                            if (isPlay)
                            {
                                return;
                            }
                            activatedSprite.curRow = block.row;
                            UpdateBlockView();

                            switch (clickType)
                            {
                                // input the value or select variable value
                                case 0:
                                    {
                                        List<string> varlist = new List<string>();
                                        foreach (string name in Variable.variableMap.Keys)
                                        {
                                            varlist.Add(name);
                                        }
                                        VariableSelectDialog dialog = new VariableSelectDialog(this, (selectIndex, text) =>
                                        {
                                            if (selectIndex != -1)
                                            {
                                                block.varName = varlist[selectIndex];
                                                block.text = null;
                                            }
                                            else
                                            {
                                                block.text = text;
                                                block.varName = null;
                                            }
                                            UpdateBlockView();
                                        });
                                        dialog.mList = varlist;
                                        dialog.Show();
                                        break;
                                    }
                                // set variable value
                                case 1:
                                // increase variable value
                                case 2:
                                    {
                                        // change variable dialog, example set variable value or increase variable value
                                        if (Variable.variableMap.Count <= 0)
                                        {
                                            ToastUtil.ShowToast(this, "你还没有添加变量");
                                            return;
                                        }
                                        List<string> varlist = new List<string>();
                                        foreach (string name in Variable.variableMap.Keys)
                                        {
                                            varlist.Add(name);
                                        }
                                        VariableChangeDialog dialog = new VariableChangeDialog(this, clickType > 1, (selectIndex, value) =>
                                          {
                                              block.varName = varlist[selectIndex];
                                              block.varValue = value;
                                              UpdateBlockView();
                                          });
                                        dialog.mList = varlist;
                                        dialog.Show();
                                        break;
                                    }
                                // receive signal dialog
                                case 3:
                                // send signal dialog
                                case 4:
                                    {
                                        SignalDialog dialog = new SignalDialog(this, (text) =>
                                        {
                                            block.text = text;
                                            UpdateBlockView();
                                        });
                                        dialog.Show();
                                        break;
                                    }
                                // select collision image dialog
                                case 5:
                                    {
                                        if (Project.mSprites.Count == 1)
                                        {
                                            ToastUtil.ShowToast(this, "至少选择两个精灵才会有碰撞");
                                            return;
                                        }
                                        List<string> titlelist = new List<string>();
                                        List<string> imglist = new List<string>();
                                        List<string> idlist = new List<string>();
                                        for (int q = 0; q < Project.mSprites.Count; q++)
                                        {
                                            if (q != mSpriteIndex)
                                            {
                                                ActivatedSprite sprite = Project.mSprites[q];
                                                titlelist.Add(sprite.sprite.name);
                                                imglist.Add(sprite.sprite.remotePath);
                                                idlist.Add(sprite.activateSpriteId);
                                            }
                                        }
                                        ImageSelectDialog dialog = new ImageSelectDialog(this, clickType > 4, (selectIndex) =>
                                        {
                                            block.text = titlelist[selectIndex];
                                            block.activateSpriteId = idlist[selectIndex];
                                            UpdateBlockView();
                                        });
                                        dialog.mList = titlelist;
                                        dialog.mImgList = imglist;
                                        dialog.Show();
                                        break;
                                    }
                                // input speck text dialog
                                case 6:
                                    {
                                        SpeakDialog dialog = new SpeakDialog(this, (text) =>
                                        {
                                            block.text = text;
                                            UpdateBlockView();
                                        });
                                        dialog.Show();
                                        break;
                                    }
                                // select sound 
                                case 7:
                                // select background
                                case 8:
                                    {
                                        int tag = (int)view.Tag;
                                        tag = tag - tag / 10000 * 10000;
                                        // click activate block to select background
                                        Intent intent = new Intent(this, clickType == 7 ? typeof(SoundActivity) : typeof(BackgroundActivity));
                                        Bundle bundle = new Bundle();
                                        bundle.PutInt("index", mSpriteIndex);
                                        bundle.PutInt("row", block.row);
                                        bundle.PutInt("column", tag);
                                        intent.PutExtra("bundle", bundle);
                                        intent.AddFlags(ActivityFlags.ReorderToFront);
                                        StartActivityForResult(intent, clickType, null);
                                        break;
                                    }
                                // ControlXY
                                case 10:
                                    {
                                        List<string> varlist = new List<string>();
                                        foreach (string name in Variable.variableMap.Keys)
                                        {
                                            varlist.Add(name);
                                        }
                                        ControlXYDialog dialog = new ControlXYDialog(this, (X, Y) =>
                                        {
                                            block.varName = X;
                                            block.varValue = Y;
                                            UpdateBlockView();
                                        });
                                        dialog.mList = varlist;
                                        dialog.Show();

                                        break;
                                    }
                                default:
                                    break;
                            }
                        };

                    }
                    originY += itemW + margin;
                }
            }
        }

        // check Collision
        public void JudgeCollision()
        {
            if (!isPlay || imgList.Count <= 1)
            {
                return;
            }
            new Java.Lang.Thread(new Java.Lang.Runnable(() =>
            {
                for (int i = 0; i < imgList.Count; i++)
                {
                    DragImgView firstIv = imgList[i];
                    for (int j = i + 1; j < imgList.Count; j++)
                    {
                        DragImgView twoIv = imgList[j];
                        FrameLayout.LayoutParams p1 = (FrameLayout.LayoutParams)firstIv.LayoutParameters;
                        FrameLayout.LayoutParams p2 = (FrameLayout.LayoutParams)twoIv.LayoutParameters;
                        Rect r1 = new Rect(p1.LeftMargin, p1.TopMargin, p1.LeftMargin + firstIv.Width, p1.TopMargin + firstIv.Height);
                        Rect r2 = new Rect(p2.LeftMargin, p2.TopMargin, p2.LeftMargin + twoIv.Width, p2.TopMargin + twoIv.Height);
                        if (RectUtil.intersect(r1, r2))
                        {
                            if (i < Project.mSprites.Count && j < Project.mSprites.Count)
                            {
                                // send collision signal
                                ActivatedSprite activatedSprite1 = Project.mSprites[i];
                                ActivatedSprite activatedSprite2 = Project.mSprites[j];
                                activatedSprite1.ReceiveCollisionSignal(activatedSprite2.activateSpriteId);
                                activatedSprite2.ReceiveCollisionSignal(activatedSprite1.activateSpriteId);
                            }
                        }
                    }
                }
            })).Start();
        }

        /*
         * ActivateSprite interface
         */
        public void UpdateView()
        {
            RunOnUiThread(() =>
            {
                UpdateMainView();
            });
        }

        public void UpdateBlockViewDelegate()
        {
            RunOnUiThread(() =>
            {
                UpdateBlockView();
            });
        }

        public void UpdateBackground(string name)
        {
            RunOnUiThread(() =>
            {
                Background background = Project.backgroundsList.ContainsKey(name) ? Project.backgroundsList[name] : null;
                if (background != null)
                {
                    videoPlayer.SetPath(background.remoteVideoPath, background.remotePreviewImgPath, null);
                }
            });
        }

        public void RowAnimateComplete()
        {
            RunOnUiThread(() =>
            {
                if (isPractice && Project.mSprites.Count > 0 && practice.solutionList.Count == Project.mSprites[0].mBlocks[0].Count)
                {
                    FindViewById<RelativeLayout>(Resource.Id.p_wrapper_view).Visibility = ViewStates.Visible;
                    FindViewById<LinearLayout>(Resource.Id.successView).Visibility = ViewStates.Visible;
                    FindViewById<ImageView>(Resource.Id.p_start).Visibility = ViewStates.Invisible;
                }
            });
        }

        private ISharedPreferences SharedPres
        {
            get {
                return GetSharedPreferences("_main_", 0);
            }
        }

        private ISharedPreferencesEditor Editor
        {
            get {
                return SharedPres.Edit();
            }
        }

        [System.Obsolete]
        bool View.IOnLongClickListener.OnLongClick(View v)
        {
            // 创建实现阴影的对象
            View.DragShadowBuilder builder = new View.DragShadowBuilder(v);
            //DragShadow Builder builder = new DragShadowBuilder(v);
            /*开始拖拽并把view对象传递给系统。作为响应startDrag()方法的一部分，
            *系统调用在View.DragShadowBuilder对象中定义的回调方法
            *来获取拖拽影子。
            */
            if (isPlay)
            {
                return false;
            }
            dragView = v;
            if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
            {
                v.StartDragAndDrop(null, builder, null, (int)DragFlags.Opaque);
            }
            else
            {
                v.StartDrag(null, builder, null, (int)DragFlags.Opaque);
            }
            v.Visibility = ViewStates.Invisible;
            return true;
        }

        /** OnDrag interface */
        bool View.IOnDragListener.OnDrag(View v, DragEvent e)
        {
            //获取拖拽的动作类型值
            DragAction action = e.Action;
            switch (action)
            {
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
                            new SoundPlayer(this).PlayLocal(SoundPlayer.all_clear);
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

                                int margin = 3;
                                int itemW = (int)((GridViewH - margin) / 2.0);
                                LogUtil.CustomLog("itemW=" + itemW + ", width=" + width);
                                //int padding = 10;
                                int columnNum = (width - itemW / 2 + margin) / (itemW + margin);
                                //padding = (width - columnNum * itemW - (columnNum - 1) * margin) / 2;
                                //columnNum = columnNum - 1;
                                float x = e.GetX();
                                float y = e.GetY();
                                //int firstX = itemW + padding + margin-margin/2;
                                int firstX = itemW / 2;
                                int firstY = -margin / 2;
                                int add = itemW + margin;
                                if (x > firstX)
                                {
                                    ActivatedSprite activatedSprite = spritesList[mSpriteIndex];
                                    List<List<Block>> blockList = activatedSprite.mBlocks;
                                    int r = Math.Round((y - firstY) / add);
                                    r = firstY + r * add < y ? r : r - 1;
                                    int c = Math.Round((x - firstX) / add);
                                    c = firstX + c * add < x ? c : c - 1;
                                    if (c < columnNum)
                                    {
                                        int cc = 0;
                                        bool isTrue = true;
                                        int i = 0;
                                        while (isTrue)
                                        {
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
                                            new SoundPlayer(this).PlayLocal(SoundPlayer.move_success);
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
                            new SoundPlayer(this).PlayLocal(SoundPlayer.move_fail);
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

        /* Delegate and DataSource interface */
        public int GetItemsCount(Java.Lang.Object adapter)
        {
            return spritesList.Count;
        }

        public View GetItemView(Java.Lang.Object adapter, ViewGroup parent)
        {
            View convertView = LayoutInflater.From(this).Inflate(Resource.Layout.item_list_sprite, parent, false);
            int itemW = (int)(ScreenUtil.ScreenWidth(this) * 146 / 1280.0 - ScreenUtil.dip2px(this, 24));
            ViewUtil.SetViewHeight(convertView, itemW);
            ViewHolder holder = new ViewHolder();
            holder.bgIv = convertView.FindViewById<ImageView>(Resource.Id.selected_material_bgIv);
            holder.imgIv = convertView.FindViewById<ImageView>(Resource.Id.selected_material_imgIv);
            holder.nameTv = convertView.FindViewById<TextView>(Resource.Id.sprite_tv);
            holder.deleteFl = convertView.FindViewById<FrameLayout>(Resource.Id.delete_fl);
            convertView.Tag = holder;
            convertView.Click += (t, e) =>
            {
                new SoundPlayer(this).PlayLocal(SoundPlayer.mouse_click);
                if (isPlay)
                {
                    return;
                }
                ViewHolder viewHolder = (ViewHolder)(((View)t).Tag);
                int position = (int)viewHolder.bgIv.Tag;
                ClickItem(position, (View)t);
            };
            convertView.LongClick += (t, e) =>
            {
                if (isPlay)
                {
                    return;
                }
                ViewHolder viewHolder = (ViewHolder)(((View)t).Tag);
                int position = (int)viewHolder.bgIv.Tag;
                LongClickItem(position, (View)t);
            };
            holder.deleteFl.Click += (t, e) =>
            {
                if (isPlay)
                {
                    return;
                }
                new SoundPlayer(this).PlayLocal(SoundPlayer.all_clear);
                int position = (int)((FrameLayout)t).Tag;
                ActivatedSprite sprite = Project.mSprites[position];
                Project.mSprites.RemoveAt(position);
                if (Project.mSprites.Count <= 0)
                {
                    mSpriteIndex = -1;
                    mSpriteAdapter.NotifyDataSetChanged();
                    AddSpriteView();
                    UpdateBlockView();
                }
                else
                {
                    mSpriteIndex = mSpriteIndex >= Project.mSprites.Count ? 0 : mSpriteIndex;
                    mSpriteAdapter.NotifyDataSetChanged();
                    AddSpriteView();
                    UpdateBlockView();
                }
                Project.ClearCollision(sprite.activateSpriteId);
            };
            return convertView;
        }

        public void UpdateItemView(Java.Lang.Object adapter, View contentView, int position)
        {
            ActivatedSprite sprite = spritesList[position];
            ViewHolder viewHolder = (ViewHolder)contentView.Tag;
            viewHolder.bgIv.Tag = position;
            viewHolder.deleteFl.Tag = position;
            viewHolder.deleteFl.Visibility = isPractice ? ViewStates.Gone : ViewStates.Visible;
            viewHolder.bgIv.SetBackgroundResource(position == mSpriteIndex ? Resource.Drawable.xml_asprite_item_bg_s : Resource.Drawable.xml_asprite_item_bg_n);
            if (sprite.originBitmapList.Count > 0)
            {
                viewHolder.imgIv.SetImageBitmap(sprite.originBitmapList[0]);
            }
            else
            {
                Glide.With(this).Load(GlideUtil.GetGlideUrl(sprite.sprite.remotePath)).Into(viewHolder.imgIv);
            }
            viewHolder.nameTv.Text = sprite.sprite.name;
        }

        public void ClickItem(int position, View view)
        {
            if (mSpriteIndex == position)
            {
                mLongPressSpriteIndex = position;
                ShowMenu(view);
            }
            else
            {
                mSpriteIndex = position;
                mSpriteAdapter.NotifyDataSetChanged();
                UpdateBlockView();
            }
        }

        public void LongClickItem(int position, View view)
        {
            mLongPressSpriteIndex = position;
            ShowMenu(view);
        }

        public void ShowMenu(View view)
        {
            PopupMenu popupMenu = new PopupMenu(view.Context, view);
            popupMenu.Menu.Add("克隆");
            popupMenu.Menu.Add("保存");
            popupMenu.Menu.Add("复制积木");
            popupMenu.Menu.Add("粘贴积木");
            popupMenu.Menu.Add("重新定位");
            popupMenu.Menu.Add("往后送");
            popupMenu.Menu.Add("编辑");
            popupMenu.SetOnMenuItemClickListener(this);
            popupMenu.Show();
        }

        public bool OnMenuItemClick(IMenuItem item)
        {
            string title = item.ToString();
            List<string> list = new List<string>() { "克隆", "保存", "复制积木", "粘贴积木", "重新定位", "往后送", "编辑" };
            int index = list.IndexOf(title);
            switch (index)
            {
                case 0:
                    {
                        ActivatedSprite activatedSprite = spritesList[mLongPressSpriteIndex];
                        Project.AddSprite(activatedSprite.sprite, true);
                        ListView listView = FindViewById<ListView>(Resource.Id.materailListView);
                        mSpriteAdapter.NotifyDataSetChanged();
                        AddSpriteView();
                        UpdateBlockView();
                        break;
                    }
                case 1:
                    {
                        SaveDialog dialog = new SaveDialog(this, (text) =>
                        {
                            new ArtcoObject(this).SaveObject(spritesList[mLongPressSpriteIndex], text);
                        });
                        dialog.Show();
                        break;
                    }
                case 2:
                    {
                        ActivatedSprite activatedSprite = spritesList[mLongPressSpriteIndex];
                        Project.CopyBlocks(activatedSprite.mBlocks);
                        ToastUtil.ShowToast(this, "复制积木成功");
                        break;
                    }
                case 3:
                    {
                        ActivatedSprite activatedSprite = spritesList[mLongPressSpriteIndex];
                        activatedSprite.AddBlocks(Project.PasteBlocks());
                        UpdateBlockView();
                        break;
                    }
                case 4:
                    {
                        ActivatedSprite activatedSprite = spritesList[mLongPressSpriteIndex];
                        activatedSprite.Location();
                        break;
                    }
                case 5:
                    {
                        ActivatedSprite activatedSprite = spritesList[mLongPressSpriteIndex];
                        Project.mSprites.Remove(activatedSprite);
                        Project.mSprites.Insert(0, activatedSprite);
                        mSpriteAdapter.NotifyDataSetChanged();
                        AddSpriteView();
                        UpdateBlockView();
                        break;
                    }
                case 6:
                    {
                        Intent intent = new Intent(this, typeof(EditActivity));
                        Bundle bundle = new Bundle();
                        bundle.PutInt("position", mLongPressSpriteIndex);
                        intent.PutExtra("bundle", bundle);
                        intent.AddFlags(ActivityFlags.ReorderToFront);
                        StartActivityForResult(intent, 10, null);
                        break;
                    }
                default: break;
            }
            return true;
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