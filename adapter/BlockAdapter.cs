using System;
using Android.Content;
using Android.Views;
using Android.Widget;
using Android.Support.V4.View;
using System.Collections.Generic;

namespace TabletArtco {
    public class BlockAdapter : PagerAdapter {
        //上下文
        private Context mContext;
        //数据
        private List<String> mData;

        
        public BlockAdapter(Context context, List<String> list) {
            mContext = context;
            mData = list;
        }
        
        public override int Count {
            get {
                return mData.Count;
            }
        }

        public override Java.Lang.Object InstantiateItem(ViewGroup container, int position) {
            View view = View.Inflate(mContext, Resource.Layout.block_item, null);
            //view.SetTag(1, position);
            initBlock(view);
            container.AddView(view);
            return view;
        }

        public override void DestroyItem(ViewGroup container, int position, Java.Lang.Object @object) {
            base.DestroyItem(container, position, @object);
            container.RemoveView((View)@object);
        }

        public override bool IsViewFromObject(View view, Java.Lang.Object @object) {
            return view == @object;
        }

        public void initBlock(View view) {
            //int page = (int)view.GetTag(1);
            int width = (int)(ScreenUtil.ScreenWidth(view.Context)*890/1280.0);
            int height = (int)(ScreenUtil.ScreenHeight(view.Context) * 175 / 720.0 - 10 - ScreenUtil.dip2px(view.Context, 4));
            int w = (int) (width - ((height - 40) / 3 * 168 / 50.0));
            int margin = 40;
            int itemH = (int)((height - margin) / 2.0);
            int column = w / itemH;
            int start = (w - column * itemH)/2;
            FrameLayout containView = view.FindViewById<FrameLayout>(Resource.Id.containView);
            containView.RemoveAllViews();
            
            for (int i = 0; i < column*2; i++) {
                FrameLayout.LayoutParams param = new FrameLayout.LayoutParams(itemH, itemH);
                param.LeftMargin = start + i%column*itemH;
                param.TopMargin = (int) (i / column * (margin/2.0 + itemH));

                FrameLayout item = new FrameLayout(view.Context);
                item.LayoutParameters = param;
                item.SetBackgroundResource(Resource.Drawable.block_bg);
                containView.AddView(item);

                if (i < 5) {
                    FrameLayout.LayoutParams param1 = new FrameLayout.LayoutParams(itemH - 16, itemH - 16);
                    param1.SetMargins(8, 8, 8, 8);
                    ImageView imgIv = new ImageView(view.Context);
                    imgIv.LayoutParameters = param1;
                    imgIv.SetScaleType(ImageView.ScaleType.FitXy);
                    imgIv.SetImageResource(Resource.Drawable.Contblock_Time1);
                    item.AddView(imgIv);
                }
            }

        }
    }
}
