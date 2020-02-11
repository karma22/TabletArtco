using System;
using Android.Views;
namespace TabletArtco
{
    public interface Delegate
    {
        View GetItemView(Java.Lang.Object adapter, ViewGroup parent);

        void UpdateItemView(Java.Lang.Object adapter, View contentView, int position);
    }
}
