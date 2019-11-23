using System;
using Android.Content;
using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using System.Collections.Generic;


namespace TabletArtco {
    public class TopBarAdapter : RecyclerView.Adapter {
        private List<int> data = new List<int>();
        private Context _context;
        public TopBarAdapter(Context context) {
            int[] res = {
                Resource.Drawable.Button_choice_picture,
                Resource.Drawable.Button_choice_educationBG,
                Resource.Drawable.Button_choice_BG,
                Resource.Drawable.Button_choice_sound,
                Resource.Drawable.Button_choice_projectsave,
                Resource.Drawable.Button_choice_projectcalling,
                Resource.Drawable.Button_choice_update,
                Resource.Drawable.Button_choice_setting
            };
            for (int i = 0; i < res.Length; i++) {
                data.Add(res[i]);
            }
            
            _context = context;
        }
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position) {
            TopBarHolder item = holder as TopBarHolder;
            item.imgIv.SetImageResource(data[position]); 
        }
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewItem) {
            View view = LayoutInflater.From(_context).Inflate(Resource.Layout.topbar_item, parent, false);
            TopBarHolder holder = new TopBarHolder(view);
            return holder;
        }
        public override int ItemCount {
            get {
                return data.Count;
            }
        }
        public override void OnViewRecycled(Java.Lang.Object holder) {
            base.OnViewRecycled(holder);
            TopBarHolder myViewHolder = holder as TopBarHolder;
        }
    }

    public class TopBarHolder : RecyclerView.ViewHolder {
        public RelativeLayout itemRl;
        public ImageView imgIv;
        public TopBarHolder(View itemView) : base(itemView) {
            itemRl = itemView.FindViewById<RelativeLayout>(Resource.Id.toolbarItem);
            imgIv = itemView.FindViewById<ImageView>(Resource.Id.tbi_bt);
            int height = (int)(ScreenUtil.ScreenHeight(itemView.Context) * 82 / 800.0);
            Console.Write(height);
            ViewUtil.setViewHeight(itemRl, height);
        }
    }
}
