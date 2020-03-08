
using System;
using System.Collections.Generic;
using Android.Content;
using Android.Media;

namespace TabletArtco
{
    public class SoundPlayer: Java.Lang.Object, MediaPlayer.IOnCompletionListener
    {

        public static int mouse_click = Resource.Raw.mouse_click;
        public static int block_link = Resource.Raw.block_link;
        public static int recycle = Resource.Raw.recycle;
        public static int all_clear = Resource.Raw.all_clear;
        public static int move_fail = Resource.Raw.move_fail;
        public static int move_success = Resource.Raw.move_success;
        public static int arrive_dest = Resource.Raw.arrive_dest;

        public static List<MediaPlayer> playerList = new List<MediaPlayer>();
        private Context mCxt;

        public SoundPlayer(Context context) {
            mCxt = context;
        }

        public static void StopAll() {
            for (int i = playerList.Count-1; i >=0 ; i--)
            {
                MediaPlayer player = playerList[i];
                playerList.Remove(player);
                if (player != null)
                {
                    player.Stop();
                    player.Release();
                    player = null;
                }
            }
        }

        public void Play(string url) {
            MediaPlayer player = new MediaPlayer();
            playerList.Add(player);
            player.SetOnCompletionListener(this);
            try
            {
                player.SetDataSource(url);
                player.Prepare();
                player.Start();
            }
            catch (System.Exception ex)
            {
                LogUtil.CustomLog(ex.ToString());
            }
        }

        public void PlayLocal(int raw)
        {
            Android.Net.Uri url = Android.Net.Uri.Parse("android.resource://" + mCxt.PackageName + "/raw/" + raw);
            MediaPlayer player = new MediaPlayer();
            playerList.Add(player);
            player.SetOnCompletionListener(this);
            try
            {
                player.SetDataSource(mCxt, url);
                player.Prepare();
                player.Start();
            }
            catch (System.Exception ex)
            {
                LogUtil.CustomLog(ex.ToString());
            }
        }

        public void OnCompletion(MediaPlayer mp)
        {
            playerList.Remove(mp);
            mp.Release();
            mp = null;
        }
    }

}
