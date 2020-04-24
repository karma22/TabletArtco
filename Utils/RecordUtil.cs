
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Media.Projection;
using Android.OS;
using System.Collections.Generic;

namespace TabletArtco
{
    class RecordUtil
    {

        private static ScreenRecordService s_ScreenRecordService;

        private static List<RecordListener> s_RecordListener = new List<RecordListener>();

        private static List<OnPageRecordListener> s_PageRecordListener = new List<OnPageRecordListener>();

        //true,录制结束的提示语正在显示
        public static bool s_IsRecordingTipShowing = false;

        /**
         * 录屏功能 5.0+ 的手机才能使用
         * @return
         */
        public static bool isScreenRecordEnable()
        {
            return Build.VERSION.SdkInt >= Build.VERSION_CODES.Lollipop;
        }


        public static void setScreenService(ScreenRecordService screenService)
        {
            s_ScreenRecordService = screenService;
        }

        public static void clear()
        {
            if (isScreenRecordEnable() && s_ScreenRecordService != null)
            {
                s_ScreenRecordService.clearAll();
                s_ScreenRecordService = null;
            }

            if (s_RecordListener != null && s_RecordListener.Count > 0)
            {
                s_RecordListener.Clear();
            }

            if (s_PageRecordListener != null && s_PageRecordListener.Count > 0)
            {
                s_PageRecordListener.Clear();
            }
        }

        /**
         * 开始录制
         */
        public static void startScreenRecord(Activity activity, int requestCode)
        {
            if (isScreenRecordEnable())
            {
                if (s_ScreenRecordService != null && !s_ScreenRecordService.ismIsRunning())
                {
                    if (!s_ScreenRecordService.isReady())
                    {
                        MediaProjectionManager mediaProjectionManager = (MediaProjectionManager)activity.GetSystemService(Context.MediaProjectionService);
                        if (mediaProjectionManager != null)
                        {
                            Intent intent = mediaProjectionManager.CreateScreenCaptureIntent();
                            PackageManager packageManager = activity.PackageManager;
                            if (packageManager.ResolveActivity(intent, PackageInfoFlags.MatchDefaultOnly) != null)
                            {
                                //存在录屏授权的Activity
                                activity.StartActivityForResult(intent, requestCode);
                            }
                            else
                            {
                                ToastUtil.ShowToast(activity, "暂时无法录制");
                            }
                        }
                    }
                    else
                    {
                        s_ScreenRecordService.startRecord();
                    }
                }
            }
        }

        /**
         * 获取用户允许录屏后，设置必要的数据
         * @param resultCode
         * @param resultData
         */
        public static void setUpData(int resultCode, Intent resultData)
        {
            if (isScreenRecordEnable()){
                if (s_ScreenRecordService != null && !s_ScreenRecordService.ismIsRunning())
                {
                    s_ScreenRecordService.setResultData(resultCode, resultData);
                    s_ScreenRecordService.startRecord();
                }
            }
        }

        /**
        * 停止录制
        */
        public static void stopScreenRecord(Context context)
        {
            if (isScreenRecordEnable())
            {
                if (s_ScreenRecordService != null && s_ScreenRecordService.ismIsRunning())
                {
                    string str = "停止录制";
                    s_ScreenRecordService.stopRecord(str);
                }
            }
        }

        /**
        * 获取录制后的文件地址
        * @return
        */
        public static string getScreenRecordFilePath()
        {

            if (isScreenRecordEnable() && s_ScreenRecordService != null)
            {
                return s_ScreenRecordService.getRecordFilePath();
            }
            return null;
        }

        /**
        * 判断当前是否在录制
        * @return
        */
        public static bool isCurrentRecording()
        {
            if (isScreenRecordEnable() && s_ScreenRecordService != null)
            {
                return s_ScreenRecordService.ismIsRunning();
            }
            return false;
        }

        /**
        * true,录制结束的提示语正在显示
        * @return
        */
        public static bool isRecodingTipShow()
        {
            return s_IsRecordingTipShowing;
        }

        public static void setRecordingStatus(bool isShow)
        {
            s_IsRecordingTipShowing = isShow;
        }

        /**
        * 系统正在录屏，app 录屏会有冲突，清理掉一些数据
        */
        public static void clearRecordElement()
        {
            if (isScreenRecordEnable())
            {
                if (s_ScreenRecordService != null)
                {
                    s_ScreenRecordService.clearRecordElement();
                }
            }
        }

        public static void addRecordListener(RecordListener listener)
        {
            if (listener != null && !s_RecordListener.Contains(listener))
            {
                s_RecordListener.Add(listener);
            }
        }

        public static void removeRecordListener(RecordListener listener)
        {
            if (listener != null && s_RecordListener.Contains(listener))
            {
                s_RecordListener.Remove(listener);
            }
        }

        public static void addPageRecordListener(OnPageRecordListener listener)
        {

            if (listener != null && !s_PageRecordListener.Contains(listener))
            {
                s_PageRecordListener.Add(listener);
            }
        }

        public static void removePageRecordListener(OnPageRecordListener listener)
        {

            if (listener != null && s_PageRecordListener.Contains(listener))
            {
                s_PageRecordListener.Remove(listener);
            }
        }

        public static void onPageRecordStart()
        {
            if (s_PageRecordListener != null && s_PageRecordListener.Count > 0)
            {
                foreach (OnPageRecordListener listener in s_PageRecordListener) {
                    listener.onStartRecord();
                }
            }
        }


        public static void onPageRecordStop()
        {
            if (s_PageRecordListener != null && s_PageRecordListener.Count > 0)
            {
                foreach (OnPageRecordListener listener in s_PageRecordListener)
                {
                    listener.onStopRecord();
                }
            }
        }

        public static void onPageBeforeShowAnim()
        {
            if (s_PageRecordListener != null && s_PageRecordListener.Count > 0)
            {
                foreach (OnPageRecordListener listener in s_PageRecordListener)
                {
                    listener.onBeforeShowAnim();
                }
            }
        }

        public static void onPageAfterHideAnim()
        {
            if (s_PageRecordListener != null && s_PageRecordListener.Count > 0)
            {
                foreach (OnPageRecordListener listener in s_PageRecordListener)
                {
                    listener.onAfterHideAnim();
                }
            }
        }

        public static void startRecord()
        {
            if (s_RecordListener.Count > 0)
            {
                foreach (OnPageRecordListener listener in s_PageRecordListener)
                {
                    listener.onStartRecord();
                }
            }
        }

        public static void pauseRecord()
        {
            if (s_RecordListener.Count > 0)
            {
                foreach (RecordListener listener in s_RecordListener)
                {
                    listener.onPauseRecord();
                }
            }
        }

        public static void resumeRecord()
        {
            if (s_RecordListener.Count > 0)
            {
                foreach (RecordListener listener in s_RecordListener)
                {
                    listener.onResumeRecord();
                }
            }
        }

        public static void onRecording(string timeTip)
        {
            if (s_RecordListener.Count > 0)
            {
                foreach (RecordListener listener in s_RecordListener)
                {
                    listener.onRecording(timeTip);
                }
            }
        }

        public static void stopRecord(string stopTip)
        {
            if (s_RecordListener.Count > 0)
            {
                foreach (RecordListener listener in s_RecordListener)
                {
                    listener.onStopRecord(stopTip);
                }
            }
        }

        public interface RecordListener
        {
            void onStartRecord();
            void onPauseRecord();
            void onResumeRecord();
            void onStopRecord(string stopTip);
            void onRecording(string timeTip);
        }


        public interface OnPageRecordListener
        {
            void onStartRecord();
            void onStopRecord();
            void onBeforeShowAnim();
            void onAfterHideAnim();
        }
    }
}