
using System;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Widget;

namespace TabletArtco
{
    public class RecordDialog
    {
        private AlertDialog dialog = null;
        private Action callbackAction;
        private View contentView;

        public RecordDialog(Context context, Action action)
        {
            callbackAction = action;
            Initialize(context);
        }

        public void Show()
        {
            dialog?.Show();
        }

        public void Dismiss()
        {
            dialog?.Dismiss();
        }

        private void Initialize(Context context)
        {
            contentView = LayoutInflater.From(context).Inflate(Resource.Layout.dialog_record, null, false);
            dialog = new AlertDialog.Builder(context).SetView(contentView).Create();
            dialog.Window.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));

            contentView.FindViewById<TextView>(Resource.Id.record_btn1).Click += (t, e) =>
            {
            };
            contentView.FindViewById<TextView>(Resource.Id.record_btn2).Click += (t, e) =>
            {
            };
            contentView.FindViewById<TextView>(Resource.Id.record_btn3).Click += (t, e) =>
            {
            };
            contentView.FindViewById<TextView>(Resource.Id.record_close).Click += (t, e) =>
            {
                dialog.Dismiss();
            };
        }

    }
}
