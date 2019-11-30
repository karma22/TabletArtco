using System;
using Android.Widget;
using Android.Content;
using Android.Views;
using Android.Graphics;

namespace TabletArtco {
    public class MaterailAdapter : BaseAdapter {

        Context mcxt;
        int itemH;
        public MaterailAdapter(Context cxt, int itemW) {
            mcxt = cxt;
            itemH = (int)(itemW*31/27.0);
        }

        public override int Count {
            get {
                return 10;
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
                convertView = LayoutInflater.From(mcxt).Inflate(Resource.Layout.selected_material_item, parent, false);
                ViewUtil.SetViewHeight(convertView, itemH);
                ViewHolder holder = new ViewHolder();
                holder.bgIv = convertView.FindViewById<ImageView>(Resource.Id.selected_material_bgIv);
                holder.imgIv = convertView.FindViewById<ImageView>(Resource.Id.selected_material_imgIv);
                convertView.Tag = holder;
            } 
            ViewHolder viewHolder = (ViewHolder)convertView.Tag;
            viewHolder.bgIv.SetBackgroundColor(Color.Red);
            return convertView;
        }

        //定义ViewHolder内部类，用于对控件实例进行缓存
        class ViewHolder : Java.Lang.Object {
            public ImageView bgIv;
            public ImageView imgIv;
        }
    }

    
}

