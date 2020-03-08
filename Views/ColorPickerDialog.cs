


using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Widget;
/**
* 取色器对话框
*
* @author Relish Wang
* @since 2017/08/02
*/
namespace TabletArtco
{
    public class ColorPickerDialog {

        private Context mCxt;
        private AlertDialog dialog = null;
        private ColorPickerView colorPickerView = null;
        private View contentView;
        private string curColor;

        public System.Action<string> colorAction { get; set; }

        public ColorPickerDialog(Context context, Color color)
        {
            mCxt = context;
            init(context);
            setCurColor(color);
        }

        public void Show()
        {
            dialog?.Show();
        }

        public void Dismiss()
        {
            dialog?.Dismiss();
        }

        //信号输入
        private void init(Context context)
        {
            contentView = LayoutInflater.From(context).Inflate(Resource.Layout.dialog_picker_color, null, false);
            FrameLayout conView = contentView.FindViewById<FrameLayout>(Resource.Id.colorPickerView_wrapper);
            FrameLayout.LayoutParams p = new FrameLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, FrameLayout.LayoutParams.MatchParent);
            colorPickerView = new ColorPickerView(context);
            conView.AddView(colorPickerView, p);
            colorPickerView.SetLayerType(LayerType.Software, new Paint());
            dialog = new AlertDialog.Builder(context).SetView(contentView).Create();


            //int[] ids = { Resource.Id.colorHEt, Resource.Id.colorSEt, Resource.Id.colorSEt, Resource.Id.rgbREt, Resource.Id.rgbGEt, Resource.Id.rgbBEt, Resource.Id.colorEt };
            //for (int i = 0; i < ids.Length; i++)
            //{
                //EditText editText = contentView.FindViewById<EditText>(ids[i]);
                //editText.Focusable = false;
                //editText.FocusableInTouchMode = false;
            //}

            colorPickerView.onColorChanged = (color) =>
            {
                SetNewColor(color);
            };
            
            contentView.FindViewById<TextView>(Resource.Id.cancelBt).Click += (t, e) =>
            {
                dialog.Dismiss();
            };
            contentView.FindViewById<TextView>(Resource.Id.confirmBt).Click += (t, e) =>
            {
                colorAction?.Invoke(curColor);
                dialog.Dismiss();
            };
            dialog.Window.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));
        }

        public void setCurColor(Color color) {
            ImageView curIv = contentView.FindViewById<ImageView>(Resource.Id.curColorIv);
            curIv?.SetBackgroundColor(color);
            colorPickerView?.setColor(ColorUtil.ColorToString(color));
            SetNewColor(color);
        }

        public void SetNewColor(Color color) {
            ImageView newIv = contentView.FindViewById<ImageView>(Resource.Id.newColorIv);
            newIv?.SetBackgroundColor(color);
            UpdateView(color);
        }

        public void UpdateView(Color color) {
            int mHue = (int)color.GetHue();
            int mSat = (int)(color.GetSaturation()*100);
            int mVal = (int)(color.GetBrightness()*100);
            //contentView.FindViewById<EditText>(Resource.Id.colorHEt).Text = "" + mHue;
            //contentView.FindViewById<EditText>(Resource.Id.colorSEt).Text = "" + mSat;
            //contentView.FindViewById<EditText>(Resource.Id.colorBEt).Text = "" + mVal;

            //contentView.FindViewById<EditText>(Resource.Id.rgbREt).Text = "" + color.R;
            //contentView.FindViewById<EditText>(Resource.Id.rgbGEt).Text = "" + color.G;
            //contentView.FindViewById<EditText>(Resource.Id.rgbBEt).Text = "" + color.B;

            //contentView.FindViewById<EditText>(Resource.Id.colorEt).Text = ColorUtil.ColorToString(color).Substring(1);

            contentView.FindViewById<TextView>(Resource.Id.colorHEt).Text = "" + mHue;
            contentView.FindViewById<TextView>(Resource.Id.colorSEt).Text = "" + mSat;
            contentView.FindViewById<TextView>(Resource.Id.colorBEt).Text = "" + mVal;

            contentView.FindViewById<TextView>(Resource.Id.rgbREt).Text = "" + color.R;
            contentView.FindViewById<TextView>(Resource.Id.rgbGEt).Text = "" + color.G;
            contentView.FindViewById<TextView>(Resource.Id.rgbBEt).Text = "" + color.B;

            contentView.FindViewById<TextView>(Resource.Id.colorEt).Text = ColorUtil.ColorToString(color).Substring(1);

            curColor = ColorUtil.ColorToString(color);
        }

    }

}