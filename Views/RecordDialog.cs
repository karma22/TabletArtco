﻿using System;
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
        private MediaRecorder recorder = new MediaRecorder();
        private bool isRecording = false;
        private string filePath = null;

        public RecordDialog(Context context, Action action)
        {
            Initialize(context, action);
        }

        public void Show()
        {
            dialog?.Show();
        }

        public void Dismiss()
        {
            dialog?.Dismiss();
        }

        private void Initialize(Context context, Action action)
        {
            contentView = LayoutInflater.From(context).Inflate(Resource.Layout.dialog_record, null, false);
            ImageView play = contentView.FindViewById<ImageView>(Resource.Id.record_btn1);
            ImageView record = contentView.FindViewById<ImageView>(Resource.Id.record_btn2);

            dialog = new AlertDialog.Builder(context).SetView(contentView).Create();
            dialog.Window.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));
            dialog.SetCancelable(false);

            play.Click += (t, e) =>
            {
            };

            record.Click += (t, e) =>
            {
                if (!Directory.Exists(UserDirectoryPath.userSoundPath))
                {
                    Directory.CreateDirectory(UserDirectoryPath.userSoundPath);
                }

                record.Visibility = ViewStates.Invisible;
                play.Visibility = ViewStates.Invisible;
                isRecording = true;

                recorder.SetAudioSource(AudioSource.Mic);
                recorder.SetOutputFormat(OutputFormat.ThreeGpp);
                recorder.SetAudioEncoder(AudioEncoder.Aac);
                recorder.SetOutputFile(UserDirectoryPath.userSoundPath + "/test.wav");
                recorder.Prepare();
                recorder.Start();
            };

            contentView.FindViewById<ImageView>(Resource.Id.record_btn3).Click += (t, e) =>
            {
                if(isRecording)
                {
                    recorder.Stop();
                    recorder.Reset();   // You can reuse the object by going back to setAudioSource() step
                }

                if(filePath != null)
                {
                    play.Visibility = ViewStates.Visible;
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
                }
                recorder.Release(); // Now the object cannot be reused
                action?.Invoke();
                dialog.Dismiss();
            };
        }

    }
}
