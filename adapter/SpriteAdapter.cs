using System;
using Android.Widget;
using Android.Content;
using Android.Views;
using Android.Graphics;

namespace TabletArtco {
    public class SpriteAdapter : BaseAdapter {

        private DataSource mDataCount;
        private Delegate mViewHandler;

        public SpriteAdapter(DataSource dataCount, Delegate viewHandler)
        {
            mDataCount = dataCount;
            mViewHandler = viewHandler;
        }

        public override int Count {
            get {
                return mDataCount.GetItemsCount(this);
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
                convertView = mViewHandler.GetItemView(this, parent);
                //convertView = LayoutInflater.From(mcxt).Inflate(Resource.Layout.selected_material_item, parent, false);
                //ViewUtil.SetViewHeight(convertView, itemH);
                //ViewHolder holder = new ViewHolder();
                //holder.bgIv = convertView.FindViewById<ImageView>(Resource.Id.selected_material_bgIv);
                //holder.imgIv = convertView.FindViewById<ImageView>(Resource.Id.selected_material_imgIv);
                //convertView.Tag = holder;
            }
            mViewHandler.UpdateItemView(this, convertView, position);
            //ViewHolder viewHolder = (ViewHolder)convertView.Tag;
            //viewHolder.bgIv.SetBackgroundColor(Color.Red);

            return convertView;
        }

        //定义ViewHolder内部类，用于对控件实例进行缓存
        class ViewHolder : Java.Lang.Object {
            public ImageView bgIv;
            public ImageView imgIv;
        }
    }

    
}

