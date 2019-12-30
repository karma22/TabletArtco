using System;
using Android.Widget;
using Android.Content;
using Android.Views;
using Android.Graphics;

namespace TabletArtco {

    public class GridAdapter : BaseAdapter {

        public bool isMove = false;
        public int movePosition = -1;

        private DataSource mDataCount;
        private Delegate mViewHandler;

        public GridAdapter(DataSource dataCount, Delegate viewHandler) {
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
            }
            //if (position == movePosition && isMove)
            //{
            //    convertView.Visibility = ViewStates.Invisible;
            //}
            //else
            //{
            //    convertView.Visibility = ViewStates.Visible;
            //}
            mViewHandler.UpdateItemView(this, convertView, position);
            return convertView;
        }
    }
}
