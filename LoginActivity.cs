using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using System;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Media;

namespace TabletArtco
{
    [Activity(Theme = "@style/AppTheme", MainLauncher = true)]
    //[Activity(Theme = "@style/AppTheme")]
    public class LoginActivity : AppCompatActivity, MediaPlayer.IOnCompletionListener
    {
        private EditText accountEt { get; set; }
        private EditText pwdEt { get; set; }
        private ImageView remIv { get; set; }

        //private Android.App.AlertDialog dialog;
        private Dialog dialog;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            RequestedOrientation = Android.Content.PM.ScreenOrientation.Landscape;
            Window.SetFlags(Android.Views.WindowManagerFlags.Fullscreen, Android.Views.WindowManagerFlags.Fullscreen);
            SetContentView(Resource.Layout.activity_login);
            InitView();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            dialog?.Dismiss();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public void InitView()
        {
            accountEt = FindViewById<EditText>(Resource.Id.accountEt);
            pwdEt = FindViewById<EditText>(Resource.Id.pwdEt);
            remIv = FindViewById<ImageView>(Resource.Id.remIv);

            Boolean isRem = SharedPres.GetBoolean("isremember", false);
            remIv.SetImageResource(isRem ? Resource.Drawable.Login_checked : Resource.Drawable.Login_check);
            if (isRem)
            {
                accountEt.Text = SharedPres.GetString("username", "");
                pwdEt.Text = SharedPres.GetString("password", "");
            }

            FindViewById<ImageView>(Resource.Id.remIv).Click += (s, e) =>
            {
                Remember();
            };

            FindViewById<TextView>(Resource.Id.remTv).Click += (s, e) =>
            {
                Remember();
            };

            FindViewById<ImageView>(Resource.Id.loginBt).Click += (s, e) =>
            {
                SignIn();
            };
        }

        public void Remember()
        {
            Boolean isRem = !SharedPres.GetBoolean("isremember", false);
            Editor.PutBoolean("isremember", isRem).Commit();
            remIv.SetImageResource(isRem ? Resource.Drawable.Login_checked : Resource.Drawable.Login_check);

            if (!isRem)
            {
                Editor.Remove("username").Commit();
                Editor.Remove("password").Commit();
            }
        }

        public void SignIn()
        {
            String account = accountEt.Text;
            String pwd = pwdEt.Text;

            // if account is empty, then return;
            if (account.Length <= 0)
            {
                Toast.MakeText(this, "账号不能为空", ToastLength.Short).Show();
                return;
            }

            // if password is empty, then return;
            if (pwd.Length <= 0)
            {
                Toast.MakeText(this, "密码不能为空", ToastLength.Short).Show();
                return;
            }

            View contentView = LayoutInflater.From(this).Inflate(Resource.Layout.dialog_loading, null, false);
            VideoView videoView = contentView.FindViewById<VideoView>(Resource.Id.loading_video);
            videoView.SetOnCompletionListener(this);

            dialog = new Dialog(this, Resource.Style.Theme_Design_NoActionBar);
            dialog.SetContentView(contentView);
            dialog.Window.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));
            dialog.Window.SetLayout(ScreenUtil.ScreenWidth(this), ScreenUtil.ScreenHeight(this));
            dialog.SetCancelable(false);
            dialog.Show();

            Android.Net.Uri url = Android.Net.Uri.Parse("android.resource://" + PackageName + "/raw/" + Resource.Raw.ARTCO_Loding);
            videoView.SetVideoURI(url);
            videoView.Start();

            new Java.Lang.Thread(new Java.Lang.Runnable(() =>
            {
                // If verification is passed，to sign in;
                if (!DBManager.CheckLogin(account, pwd))
                {
                    dialog.Dismiss();
                    RunOnUiThread(() =>
                    {
                        Toast.MakeText(this, "登录失败", ToastLength.Short).Show();
                    }); 
                }
                else
                {
                    //if password is remembered, write userinfo to sharedPreference
                    Boolean isRem = SharedPres.GetBoolean("isremember", false);
                    if (isRem)
                    {
                        Editor.PutString("username", account).Commit();
                        Editor.PutString("password", pwd).Commit();
                    }

                    //new sun.misc.BASE64Encoder().encode(userPassword.getBytes());
                    GlideUtil.username = account;
                    GlideUtil.pwd = pwd;

                    try
                    {
                        DBManager.LoadSprites();
                        DBManager.LoadBackgrounds();
                        DBManager.LoadSounds();
                        DBManager.LoadMusic();

                        // Enter main page
                        Intent intent = new Intent(this, typeof(MainActivity));
                        StartActivity(intent);
                        Finish();
                    }
                    catch
                    {
                        dialog.Dismiss();
                    }
                }

            })).Start();
        }

        private ISharedPreferences SharedPres
        {
            get
            {
                return GetSharedPreferences("_User_", 0);
            }
        }

        private ISharedPreferencesEditor Editor
        {
            get
            {
                return SharedPres.Edit();
            }
        }

        public void OnCompletion(MediaPlayer mp)
        {
            mp.Start();
        }
    }
}
