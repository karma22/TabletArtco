using System;
using System.IO;
using Android.Annotation;
using Android.App;
using Android.Content;
using Android.Hardware.Display;
using Android.Media;
using Android.Media.Projection;
using Android.OS;
using Java.IO;
using Java.Lang;
using Exception = Java.Lang.Exception;

namespace TabletArtco
{
    [Service]
    class ScreenRecordService: Service, Handler.ICallback
    {
        private MediaProjectionManager mProjectionManager;
        private MediaProjection mMediaProjection;
        private MediaRecorder mMediaRecorder;
        private VirtualDisplay mVirtualDisplay;

        private bool mIsRunning;
        private int mRecordWidth = 0; //CommonUtil.getScreenWidth();
        private int mRecordHeight = 0; //CommonUtil.getScreenHeight();
        private int mScreenDpi = 0; // CommonUtil.getScreenDpi();

        private int mResultCode;
        private Intent mResultData;

        private string mName;

        //录屏文件的保存地址
        private string mRecordFilePath;

        private Handler mHandler;
        //已经录制多少秒了
        private int mRecordSeconds = 0;

        private static int MSG_TYPE_COUNT_DOWN = 110;
        
        public override IBinder OnBind(Intent intent)
        {
            return (Android.OS.IBinder)(new RecordBinder(this));
        }
       
        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            return StartCommandResult.Sticky;
        }
        
        public override void OnCreate()
        {
            base.OnCreate();
            mIsRunning = false;
            mMediaRecorder = new MediaRecorder();
            mHandler = new Handler(Looper.MainLooper, this);
            mRecordWidth = ScreenUtil.ScreenWidth(this);
            mRecordHeight = ScreenUtil.ScreenHeight(this);
            mScreenDpi = ScreenUtil.Density(this);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }

        public bool isReady()
        {
            return mMediaProjection != null && mResultData != null;
        }

        [TargetApi(Value = 21)]
        public void clearRecordElement()
        {
            clearAll();
            if (mMediaRecorder != null)
            {
                mMediaRecorder.Reset();
                mMediaRecorder.Release();
                mMediaRecorder = null;
            }
            mResultData = null;
            mIsRunning = false;
        }

        public bool ismIsRunning()
        {
            return mIsRunning;
        }

        [TargetApi(Value = 21)]
        public void setResultData(int resultCode, Intent resultData)
        {
            mResultCode = resultCode;
            mResultData = resultData;

            mProjectionManager = (MediaProjectionManager)GetSystemService(Context.MediaProjectionService);
            if (mProjectionManager != null)
            {
                mMediaProjection = mProjectionManager.GetMediaProjection(mResultCode, mResultData);
            }
        }

        [TargetApi(Value = 21)]
        public bool startRecord(string name)
        {

            if (mIsRunning)
            {
                return false;
            }
            if (mMediaProjection == null)
            {
                mMediaProjection = mProjectionManager.GetMediaProjection(mResultCode, mResultData);
            }
            mName = name;

            setUpMediaRecorder();
            createVirtualDisplay();
            mMediaRecorder.Start();

            RecordUtil.startRecord();
            //最多录制三分钟
            mHandler.SendEmptyMessageDelayed(MSG_TYPE_COUNT_DOWN, 1000);

            mIsRunning = true;

            return true;
        }

        [TargetApi(Value = 21)]
        public bool stopRecord(string tip)
        {
            if (!mIsRunning)
            {
                return false;
            }
            mIsRunning = false;
            //        Log.w("lala","stopRecord  middle");

            try
            {
                mMediaRecorder.Stop();
                mMediaRecorder.Reset();
                mMediaRecorder = null;
                mVirtualDisplay.Release();
                mMediaProjection.Stop();
            }
            catch (Exception e)
            {
                mMediaRecorder.Release();
                mMediaRecorder = null;
            }


            mMediaProjection = null;

            mHandler.RemoveMessages(MSG_TYPE_COUNT_DOWN);
            RecordUtil.stopRecord(tip);

            if (mRecordSeconds <= 2)
            {
                FileUtil.deleteSDFile(mRecordFilePath);
            }
            else
            {
                //通知系统图库更新
                FileUtil.fileScanVideo(this, mRecordFilePath, mRecordWidth, mRecordHeight, mRecordSeconds);
            }
            mRecordSeconds = 0;

            return true;
        }

        public void pauseRecord()
        {
            if (mMediaRecorder != null)
            {
                if (Build.VERSION.SdkInt >= Build.VERSION_CODES.N)
                {
                    mMediaRecorder.Pause();
                }
            }
        }

        public void resumeRecord()
        {
            if (mMediaRecorder != null)
            {
                if (Build.VERSION.SdkInt >= Build.VERSION_CODES.N)
                {
                    mMediaRecorder.Resume();
                }
            }
        }

        [TargetApi(Value = 21)]
        [Obsolete]
        private void createVirtualDisplay()
        {
            mVirtualDisplay = mMediaProjection.CreateVirtualDisplay("MainScreen", mRecordWidth, mRecordHeight, mScreenDpi, Android.Views.DisplayFlags.Round, mMediaRecorder.Surface, null, null);
        }

        [TargetApi(Value = 21)]
        private void setUpMediaRecorder()
        {
            mRecordFilePath = getSaveDirectory() + Java.IO.File.Separator + mName + ".mp4";
            if (mMediaRecorder == null)
            {
                mMediaRecorder = new MediaRecorder();
            }
            mMediaRecorder.SetAudioSource(AudioSource.Default);
            mMediaRecorder.SetVideoSource(VideoSource.Surface);
            mMediaRecorder.SetOutputFormat(OutputFormat.Mpeg4);
            mMediaRecorder.SetOutputFile(mRecordFilePath);
            mMediaRecorder.SetVideoSize(mRecordWidth, mRecordHeight);
            mMediaRecorder.SetVideoEncoder(VideoEncoder.H264);
            mMediaRecorder.SetAudioEncoder(AudioEncoder.AmrNb);
            mMediaRecorder.SetVideoEncodingBitRate((int)(mRecordWidth * mRecordHeight * 3.6));
            mMediaRecorder.SetVideoFrameRate(20);

            try
            {
                mMediaRecorder.Prepare();
            }
            catch (Java.IO.IOException e)
            {
                e.PrintStackTrace();
            }
        }

        [TargetApi(Value = 21)]
        public void clearAll()
        {
            if (mMediaProjection != null)
            {
                mMediaProjection.Stop();
                mMediaProjection = null;
            }
        }

        public string getRecordFilePath()
        {
            return mRecordFilePath;
        }

        public string getSaveDirectory()
        {
            if (Android.OS.Environment.ExternalStorageState.Equals(Android.OS.Environment.MediaMounted))
            {
                string galleryPath = Android.OS.Environment.ExternalStorageDirectory.Path + Java.IO.File.Separator + Android.OS.Environment.DirectoryDcim + Java.IO.File.Separator + "Camera";
                //if (!Directory.Exists(UserDirectoryPath.userVideoPath))
                //{
                //    Directory.CreateDirectory(UserDirectoryPath.userVideoPath);
                //}
                return galleryPath;//UserDirectoryPath.userVideoPath;
            }
            else
            {
                return null;
            }
        }

        public bool HandleMessage(Message msg)
        {
            switch (msg.What)
            {
                case 110:
                    {
                        string str = null;
                        bool enough = FileUtil.getSDFreeMemory() / (1024 * 1024) < 4;
                        if (enough)
                        {
                            //空间不足，停止录屏
                            str = "存储空间不足";
                            stopRecord(str);
                            mRecordSeconds = 0;
                            break;
                        }

                        mRecordSeconds++;
                        int minute = 0, second = 0;
                        if (mRecordSeconds >= 60)
                        {
                            minute = mRecordSeconds / 60;
                            second = mRecordSeconds % 60;
                        }
                        else
                        {
                            second = mRecordSeconds;
                        }
                        RecordUtil.onRecording("0" + minute + ":" + (second < 10 ? "0" + second : second + ""));

                        if (mRecordSeconds < 3 * 60)
                        {
                            mHandler.SendEmptyMessageDelayed(MSG_TYPE_COUNT_DOWN, 1000);
                        }
                        else if (mRecordSeconds == 3 * 60)
                        {
                            str = "录制已到限定时长";
                            stopRecord(str);
                            mRecordSeconds = 0;
                        }

                        break;
                    }
            }
            return true;
       
        }

        public class RecordBinder : Binder
        {
            ScreenRecordService mainService;
            public RecordBinder(ScreenRecordService ms)
            {
               mainService = ms;
            }
  
            public ScreenRecordService getRecordService()
            {
                return mainService;
            }
        }
    }
}