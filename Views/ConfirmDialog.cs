
using System;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Widget;

namespace TabletArtco
{
    public class ConfirmDialog
    {
        private AlertDialog dialog = null;
        public Action<bool> callbackAction;
        private View contentView;

        public ConfirmDialog(Context context)
        {
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
            contentView = LayoutInflater.From(context).Inflate(Resource.Layout.dialog_confirm, null, false);
            dialog = new AlertDialog.Builder(context).SetView(contentView).Create();

            contentView.FindViewById<TextView>(Resource.Id.cancelTv).Click += (t, e) =>
            {
                callbackAction?.Invoke(false);
                dialog.Dismiss();
            };
            contentView.FindViewById<TextView>(Resource.Id.confirmTv).Click += (t, e) =>
            {
                callbackAction?.Invoke(true);
                dialog.Dismiss();
            };
            dialog.Window.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));
        }

        public void SetMessage(string msg) {
            contentView.FindViewById<TextView>(Resource.Id.msgTv).Text = msg;
        }
    }
}
