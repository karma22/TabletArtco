using System;
using Android.Widget;
using Android.Content;
using Android.Views;
using Android.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace TabletArtco {
    public class VariableAdapter : BaseAdapter {


        private Context mCxt;
        public Dictionary<string, string> variableMap { get; set; } = new Dictionary<string, string>();
        public Action<int, int> mAction { get; set; }

        public VariableAdapter(Context context)
        {
            mCxt = context;
        }

        public override int Count {
            get {
                return variableMap.Count;
            }
        }

        public override Java.Lang.Object GetItem(int position) {
            return position;
        }

        public override long GetItemId(int position) {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent) {
            //如果convertView为空，则使用LayoutInflater()去加载布局
            if (convertView == null) {
                
                convertView = LayoutInflater.From(mCxt).Inflate(Resource.Layout.item_center_variable, parent, false);
                ViewHolder holder = new ViewHolder();
                holder.nameTv = convertView.FindViewById<TextView>(Resource.Id.nameTv);
                holder.valueTv = convertView.FindViewById<TextView>(Resource.Id.valueTv);
                holder.deleteIv = convertView.FindViewById<ImageView>(Resource.Id.deleteIv);
                convertView.Tag = holder;
                convertView.Click += (t, e) =>
                {
                    ViewHolder h = (ViewHolder)((View)t).Tag;
                    int p =(int)h.deleteIv.Tag;
                    mAction?.Invoke(p, 0);
                };
                holder.deleteIv.Click += (t, e) =>
                {
                    int p = (int)((ImageView)t).Tag;
                    mAction?.Invoke(p, 1);
                };
            }
            ViewHolder viewHolder = (ViewHolder)convertView.Tag;
            List<string> keys = variableMap.Keys.ToList();
            string name = keys[position];
            viewHolder.nameTv.Text = name;
            viewHolder.valueTv.Text = variableMap[name];
            viewHolder.deleteIv.Tag = position;
            return convertView;
        }

        //定义ViewHolder内部类，用于对控件实例进行缓存
        class ViewHolder : Java.Lang.Object {
            public TextView nameTv;
            public TextView valueTv;
            public ImageView deleteIv;
        }
    }

    
}

