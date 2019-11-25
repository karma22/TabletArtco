
using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using System;
using Android.Content;


namespace TabletArtco {
    [Activity(Theme = "@style/AppTheme", MainLauncher = true)]
    public class LoginActivity : AppCompatActivity {

        private LinearLayout loginLl;
        private EditText accountEt;
        private EditText pwdEt;
        private ImageView remIv;
        

        protected override void OnCreate(Bundle savedInstanceState) {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);            
            SetContentView(Resource.Layout.activity_login);
            RequestedOrientation = Android.Content.PM.ScreenOrientation.Landscape;
            initView();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults) {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public void initView() {
            Boolean isRem = sharedPres.GetBoolean("isremember", false);
            accountEt = FindViewById<EditText>(Resource.Id.accountEt);
            pwdEt = FindViewById<EditText>(Resource.Id.pwdEt);
            remIv = FindViewById<ImageView>(Resource.Id.remIv);
            remIv.SetImageResource(isRem ? Resource.Drawable.Login_checked : Resource.Drawable.Login_check);
            if (isRem) {
                accountEt.Text = sharedPres.GetString("username", "");
                pwdEt.Text = sharedPres.GetString("password", "");
            }

            FindViewById<ImageView>(Resource.Id.remIv).Click += (s, e) => {
                remember();
            };
            
            FindViewById<TextView>(Resource.Id.remTv).Click += (s, e) => {
                remember();
            };

            FindViewById<TextView>(Resource.Id.loginBt).Click += (s, e) => {
                signIn();
            };
        }

        public void remember() {
            Boolean isRem = !sharedPres.GetBoolean("isremember", false);
            editor.PutBoolean("isremember", isRem).Commit();
            remIv.SetImageResource(isRem ? Resource.Drawable.Login_checked : Resource.Drawable.Login_check);
            if (!isRem) {
                editor.Remove("username").Commit();
                editor.Remove("password").Commit();
            }
        }

        public void signIn() {
            String account = accountEt.Text;
            String pwd = pwdEt.Text;
            // if account is empty, then return;
            if (account.Length<=0) {
                Toast.MakeText(this, "账号不能为空", ToastLength.Short).Show();
                return;
            }
            // if password is empty, then return;
            if (pwd.Length <= 0) {
                Toast.MakeText(this, "密码不能为空", ToastLength.Short).Show();
                return;
            }
            // If verification is passed，to sign in;


            //if password is remembered, write userinfo to sharedPreference
            Boolean isRem = sharedPres.GetBoolean("isremember", false);
            if (isRem) {
                editor.PutString("username", account).Commit();
                editor.PutString("password", account).Commit();
            }

            // Enter main page
            Intent intent = new Intent(this, typeof(MainActivity));
            StartActivity(intent);
            Finish();
        }

        private ISharedPreferences sharedPres {
            get {
                return GetSharedPreferences("_User_", 0);
            }
        }

        private ISharedPreferencesEditor editor {
            get {
                return sharedPres.Edit();
            }
        }
    }
}
