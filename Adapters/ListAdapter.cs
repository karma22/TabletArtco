using Android.Widget;
using Android.Views;

namespace TabletArtco
{

	public class ListAdapter : BaseAdapter
	{

		public bool isMove = false;
		public int movePosition = -1;

		private DataSource mDataCount;
		private Delegate mDelegate;

		public ListAdapter(DataSource dataCount, Delegate ldelegate)
		{
			mDataCount = dataCount;
            mDelegate = ldelegate;
		}

		public override int Count
		{
			get
			{
				return mDataCount.GetItemsCount(this);
			}
		}

		public override Java.Lang.Object GetItem(int position)
		{
			return position;
		}

		public override long GetItemId(int position)
		{
			return position;
		}

		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			//如果convertView为空，则使用LayoutInflater()去加载布局
			if (convertView == null)
			{
				convertView = mDelegate.GetItemView(this, parent);
			}
            mDelegate.UpdateItemView(this, convertView, position);
			return convertView;
		}
	}
}
