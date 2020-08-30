
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
        private bool isYesOrNo = false;

        public ConfirmDialog(Context context, bool isYesOrNo = false)
        {
            Initialize(context, isYesOrNo);            
        }

        public void Show()
        {
            dialog?.Show();
        }

        public void Dismiss()
        {
            dialog?.Dismiss();
        }

        private void Initialize(Context context, bool isYesOrNo)
        {            
            contentView = LayoutInflater.From(context).Inflate(Resource.Layout.dialog_confirm, null, false);
            dialog = new AlertDialog.Builder(context).SetView(contentView).Create();

            if (isYesOrNo)
            {
                var cancelBtn = contentView.FindViewById<ImageView>(Resource.Id.cancelIv);
                var yesBtn = contentView.FindViewById<ImageView>(Resource.Id.yesIv);
                cancelBtn.Visibility = ViewStates.Visible;
                yesBtn.Visibility = ViewStates.Visible;

                cancelBtn.Click += (t, e) =>
                {
                    callbackAction?.Invoke(false);
                    dialog.Dismiss();
                };
                yesBtn.Click += (t, e) =>
                {
                    callbackAction?.Invoke(true);
                    dialog.Dismiss();
                };
            }
            else
            {
                var okBtn = contentView.FindViewById<ImageView>(Resource.Id.okIv);
                okBtn.Visibility = ViewStates.Visible;
                okBtn.Click += (t, e) =>
                {
                    dialog.Dismiss();
                };
            }

            dialog.Window.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));
        }

        public void SetMessage(string msg) {
            contentView.FindViewById<TextView>(Resource.Id.msgTv).Text = msg;
        }
    }
}
