
using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using System;
using Android.Content;


namespace TabletArtco
{
    [Activity(Theme = "@style/AppTheme", MainLauncher = true)]
    public class LoginActivity : AppCompatActivity
    {

        private LinearLayout loginLl;
        private EditText accountEt;
        private EditText pwdEt;
        private ImageView remIv;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            Window.SetFlags(Android.Views.WindowManagerFlags.Fullscreen, Android.Views.WindowManagerFlags.Fullscreen);
            SetContentView(Resource.Layout.activity_login);
            RequestedOrientation = Android.Content.PM.ScreenOrientation.Landscape;
            InitView();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public void InitView()
        {
            Boolean isRem = SharedPres.GetBoolean("isremember", false);
            accountEt = FindViewById<EditText>(Resource.Id.accountEt);
            pwdEt = FindViewById<EditText>(Resource.Id.pwdEt);
            remIv = FindViewById<ImageView>(Resource.Id.remIv);
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

            FindViewById<TextView>(Resource.Id.loginBt).Click += (s, e) =>
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
            // If verification is passed，to sign in;


            //if password is remembered, write userinfo to sharedPreference
            Boolean isRem = SharedPres.GetBoolean("isremember", false);
            if (isRem)
            {
                Editor.PutString("username", account).Commit();
                Editor.PutString("password", account).Commit();
            }

            // Enter main page
            Intent intent = new Intent(this, typeof(MainActivity));
            StartActivity(intent);
            Finish();
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
    }
}
