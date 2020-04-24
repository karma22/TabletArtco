using System;
using System.IO;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Media;
using Android.Views;
using Android.Widget;

namespace TabletArtco
{
    public class RecordDialog
    {
        private AlertDialog dialog = null;
        private View contentView;
        private Context context;
        private MediaRecorder recorder = new MediaRecorder();
        private bool isRecording = false;
        private string fileName = null;

        private Java.Lang.Thread timer;

        public RecordDialog(Context context, Action action)
        {
            this.context = context;
            Initialize(action);
        }

        public void Show()
        {
            dialog?.Show();
        }

        public void Dismiss()
        {
            dialog?.Dismiss();
        }

        private void Initialize(Action action)
        {
            contentView = LayoutInflater.From(context).Inflate(Resource.Layout.dialog_record, null, false);
            ImageView play = contentView.FindViewById<ImageView>(Resource.Id.record_btn1);
            ImageView record = contentView.FindViewById<ImageView>(Resource.Id.record_btn2);
            TextView tvTimer = contentView.FindViewById<TextView>(Resource.Id.record_timer);

            dialog = new AlertDialog.Builder(context).SetView(contentView).Create();
            dialog.Window.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));
            dialog.SetCancelable(false);

            timer = new Java.Lang.Thread(new Java.Lang.Runnable(() =>
            {
                SoundActivity activity = (SoundActivity)context;
                Java.Text.SimpleDateFormat format = new Java.Text.SimpleDateFormat("mm:ss");
                long time = Java.Lang.JavaSystem.CurrentTimeMillis();

                try
                {
                    while (true)
                    {
                        Java.Lang.Thread.Sleep(1000);
                        long delta = Java.Lang.JavaSystem.CurrentTimeMillis() - time;

                        activity.RunOnUiThread(() => {
                            tvTimer.Text = format.Format(delta);
                        });
                    }
                }
                catch(Exception e)
                {
                }
                
            }));

            play.Click += (t, e) =>
            {
            };

            record.Click += (t, e) =>
            {
                if (!Directory.Exists(UserDirectoryPath.userSoundPath))
                {
                    Directory.CreateDirectory(UserDirectoryPath.userSoundPath);
                }

                SaveDialog dialog = new SaveDialog(context, (text) => {
                    fileName = text;

                    record.Visibility = ViewStates.Invisible;
                    play.Visibility = ViewStates.Invisible;
                    isRecording = true;

                    recorder.SetAudioSource(AudioSource.Mic);
                    recorder.SetOutputFormat(OutputFormat.ThreeGpp);
                    recorder.SetAudioEncoder(AudioEncoder.Aac);
                    recorder.SetOutputFile(UserDirectoryPath.userSoundPath + "/" + fileName + ".wav");
                    recorder.Prepare();
                    recorder.Start();

                    timer.Start();
                });
                dialog.Show();
                
            };

            contentView.FindViewById<ImageView>(Resource.Id.record_btn3).Click += (t, e) =>
            {
                if(isRecording)
                {
                    recorder.Stop();
                    recorder.Reset();   // You can reuse the object by going back to setAudioSource() step
                    timer.Interrupt();
                }

                record.Visibility = ViewStates.Visible;
                isRecording = false;
            };

            contentView.FindViewById<ImageView>(Resource.Id.record_close).Click += (t, e) =>
            {
                if (isRecording)
                {
                    recorder.Stop();
                    recorder.Reset();
                    timer.Interrupt();
                }
                recorder.Release(); // Now the object cannot be reused
                action?.Invoke();
                dialog.Dismiss();
            };
        }

    }
}
