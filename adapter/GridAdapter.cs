﻿using System;
using Android.Widget;
using Android.Content;
using Android.Views;
using Android.Graphics;

namespace TabletArtco {

    public class GridAdapter : BaseAdapter {
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
            mViewHandler.UpdateItemView(this, convertView, position);
            return convertView;
        }
    }
}
