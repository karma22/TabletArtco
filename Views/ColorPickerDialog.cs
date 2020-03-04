//package wang.relish.colorpicker;

//import android.annotation.SuppressLint;
//import android.app.Dialog;
//import android.content.Context;
//import android.content.res.ColorStateList;
//import android.graphics.Color;
//import android.graphics.drawable.ColorDrawable;
//import android.os.Bundle;
//import android.text.InputFilter;
//import android.text.InputType;
//import android.view.KeyEvent;
//import android.view.LayoutInflater;
//import android.view.View;
//import android.view.inputmethod.EditorInfo;
//import android.view.inputmethod.InputMethodManager;
//import android.widget.EditText;
//import android.widget.TextView;

//import androidx.annotation.NonNull;

//import java.util.Locale;



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

        public Context mCxt;
        public AlertDialog dialog = null;
        
        
        
        public ColorPickerDialog(Context context)
        {
            mCxt = context;
            initView(context);
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
        private void initView(Context context)
        {
            
            View contentView = LayoutInflater.From(context).Inflate(Resource.Layout.dialog_picker_color, null, false);
            //RelativeLayout view = contentView.FindViewById<RelativeLayout>(Resource.Id.SignalView);
            //view.Visibility = ViewStates.Visible;
            dialog = new AlertDialog.Builder(context).SetView(contentView).Create();
            //TextView closeBt = view.FindViewById<TextView>(Resource.Id.tv_close);
            //TextView cancelBt = view.FindViewById<TextView>(Resource.Id.tv_cancel);
            //TextView confirmBt = view.FindViewById<TextView>(Resource.Id.tv_confirm);
            //EditText textEt = view.FindViewById<EditText>(Resource.Id.et_signal);
            //closeBt.Click += (t, e) =>
            //{
            //    dialog.Dismiss();
            //};
            //cancelBt.Click += (t, e) =>
            //{
            //    dialog.Dismiss();
            //};
            //confirmBt.Click += (t, e) =>
            //{
            //    if (textEt.Text.Length <= 0)
            //    {
            //        Toast.MakeText(context, "输入内容不能为空", ToastLength.Long);
            //        return;
            //    }
            //    //mAction?.Invoke(textEt.Text);
            //    dialog.Dismiss();
            //};
            //dialog.Window.SetLayout(ScreenUtil.dip2px(context, 343), ScreenUtil.dip2px(context, 157));
            dialog.Window.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));
        }
    }

}