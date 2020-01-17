using Android.Content;
using Android.Graphics.Drawables;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using System.Collections.Generic;
using Android.OS;

namespace TabletArtco
{
    enum DialogStyle { Signal, VariableInit, VariableAdd, VariableSet, VariableSelect, ImageCollilion, ImagePress };

    interface DialogViewDelegate
    {
        void DialogViewCallBack(DialogView dialogView, DialogStyle style, Bundle bundle);
    }

    class DialogView: Delegate, DataSource
    {
        private Context mCxt;
        private AlertDialog dialog = null;
        private ListView listview = null;
        private DialogStyle mDialogStyle;
        private int selectIndex = -1;
        private int VariableSelelctMode = 0;
        private ListAdapter mAdapter;

        public List<object> mList { get; set; } = new List<object>();
        public DialogViewDelegate dialogViewDelegate { get; set; }
        public string message { get; set; }


        public DialogView(Context context, DialogStyle style)
        {
            mDialogStyle = style;
            if (style == DialogStyle.Signal)
            {
                ShowSignalDialog(context);
            }
            else if (style == DialogStyle.VariableInit)
            {
                ShowVariableInitDialog(context);
            }
            else if (style == DialogStyle.VariableAdd)
            {
                ShowVariableAddOrSetDialog(context, style);
            }
            else if (style == DialogStyle.VariableSet)
            {
                ShowVariableAddOrSetDialog(context, style);
            }
            else if (style == DialogStyle.VariableSelect)
            {
                ShowVariableSelectDialog(context);
            }
            else if (style == DialogStyle.ImageCollilion)
            {
                ShowImageSelectDialog(context, style);
            }
            else
            {
                ShowImageSelectDialog(context, style);
            }
        }

        //信号输入
        private void ShowSignalDialog(Context context) {
            LogUtil.CustomLog("ShowSignalDialog");
            View contentView = LayoutInflater.From(context).Inflate(Resource.Layout.activity_dialog, null, false);
            RelativeLayout view = contentView.FindViewById<RelativeLayout>(Resource.Id.SignalView);
            view.Visibility = ViewStates.Visible; 
            dialog = new AlertDialog.Builder(context).SetView(contentView).Create();
            TextView closeBt = view.FindViewById<TextView>(Resource.Id.tv_close);
            TextView cancelBt = view.FindViewById<TextView>(Resource.Id.tv_cancel);
            TextView confirmBt = view.FindViewById<TextView>(Resource.Id.tv_confirm);
            EditText textEt = view.FindViewById<EditText>(Resource.Id.et_signal);
            closeBt.Click += (t, e) =>
            {
                dialog.Dismiss();
            };
            cancelBt.Click += (t, e) =>
            {
                dialog.Dismiss();
            };
            confirmBt.Click += (t, e) =>
            {
                if (textEt.Text.Length<=0)
                {
                    Toast.MakeText(context, "输入内容不能为空", ToastLength.Long);
                    return;
                }
                dialog.Dismiss();
                Callback(textEt.Text);
            };
            dialog.Window.SetLayout(ScreenUtil.dip2px(context, 343), ScreenUtil.dip2px(context, 157));
            dialog.Window.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));
        }

        //新增变量
        private void ShowVariableInitDialog(Context context)
        {
            LogUtil.CustomLog("ShowVariableInitDialog");
            View contentView = LayoutInflater.From(context).Inflate(Resource.Layout.activity_dialog, null, false);
            RelativeLayout view = contentView.FindViewById<RelativeLayout>(Resource.Id.VariableIintView);
            view.Visibility = ViewStates.Visible;
            dialog = new AlertDialog.Builder(context).SetView(contentView).Create();
            TextView closeBt = view.FindViewById<TextView>(Resource.Id.tv_close);
            TextView cancelBt = view.FindViewById<TextView>(Resource.Id.tv_cancel);
            TextView confirmBt = view.FindViewById<TextView>(Resource.Id.tv_confirm);

            EditText nameEt = view.FindViewById<EditText>(Resource.Id.et_name);
            EditText valueEt = view.FindViewById<EditText>(Resource.Id.et_value);
            closeBt.Click += (t, e) =>
            {
                dialog.Dismiss();
            };
            cancelBt.Click += (t, e) =>
            {
                dialog.Dismiss();
            };
            confirmBt.Click += (t, e) =>
            {
                if (nameEt.Text.Length <= 0)
                {
                    ToastUtil.ShowToast(context, "变量名不能为空");
                    return;
                }
                if (valueEt.Text.Length <= 0)
                {
                    ToastUtil.ShowToast(context, "变量值不能为空");
                    return;
                }
                Callback(valueEt.Text, nameEt.Text);
                dialog.Dismiss();
            };
            dialog.Window.SetLayout(ScreenUtil.dip2px(context, 333), ScreenUtil.dip2px(context, 199));
            dialog.Window.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));
        }

        //增加或设置变量值
        private void ShowVariableAddOrSetDialog(Context context, DialogStyle style)
        {
            LogUtil.CustomLog("ShowVariableAddOrSetDialog");
            View contentView = LayoutInflater.From(context).Inflate(Resource.Layout.activity_dialog, null, false);
            RelativeLayout view = contentView.FindViewById<RelativeLayout>(Resource.Id.VariableAddOrSetView);
            view.Visibility = ViewStates.Visible;
            dialog = new AlertDialog.Builder(context).SetView(contentView).Create();
            view.SetBackgroundResource(style==DialogStyle.VariableAdd ? Resource.Drawable.dialog_variable_add : Resource.Drawable.dialog_variable_set);
            TextView cancelBt = view.FindViewById<TextView>(Resource.Id.tv_cancel);
            TextView confirmBt = view.FindViewById<TextView>(Resource.Id.tv_confirm);
            TextView valueTv = view.FindViewById<TextView>(Resource.Id.tv_value);
            cancelBt.Click += (t, e) =>
            {
                dialog.Dismiss();
            };
            confirmBt.Click += (t, e) =>
            {
                if (selectIndex == -1)
                {
                    ToastUtil.ShowToast(context, "请在列表中选择变量名称");
                    return;
                }
                if (valueTv.Text.Length == 0)
                {
                    ToastUtil.ShowToast(context, "请输入变量值");
                    return;
                }
                Callback(valueTv.Text, null, selectIndex);
                dialog.Dismiss();
            };
            mAdapter = new ListAdapter(this, this);
            ListView listView = view.FindViewById<ListView>(Resource.Id.listview);
            listview.Adapter = mAdapter;
            HandleInput(view);
            dialog.Window.SetLayout(ScreenUtil.dip2px(context, 380), ScreenUtil.dip2px(context, 360));
            dialog.Window.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));
        }

        //变量选择
        private void ShowVariableSelectDialog(Context context)
        {
            LogUtil.CustomLog("ShowVariableSelectDialog");
            View contentView = LayoutInflater.From(context).Inflate(Resource.Layout.activity_dialog, null, false);
            RelativeLayout view = contentView.FindViewById<RelativeLayout>(Resource.Id.VariableSelectView);
            view.Visibility = ViewStates.Visible;
            dialog = new AlertDialog.Builder(context).SetView(contentView).Create();
            TextView cancelBt = view.FindViewById<TextView>(Resource.Id.tv_cancel);
            TextView confirmBt = view.FindViewById<TextView>(Resource.Id.tv_confirm);
            TextView valueTv = view.FindViewById<TextView>(Resource.Id.tv_value);
            cancelBt.Click += (t, e) =>
            {
                dialog.Dismiss();
            };
            confirmBt.Click += (t, e) =>
            {
                if (selectIndex == -1 && valueTv.Text.Length==0)
                {
                    ToastUtil.ShowToast(context, "请选择变量或输入值");
                    return;
                }
                if (selectIndex != -1)
                {
                    Callback(null, valueTv.Text, selectIndex);
                }
                else
                {
                    Callback(valueTv.Text);
                }
                HandleInput(view);
                dialog.Dismiss();
            };
            mAdapter = new ListAdapter(this, this);
            ListView listView = view.FindViewById<ListView>(Resource.Id.listview);
            listview.Adapter = mAdapter;
            dialog.Window.SetLayout(ScreenUtil.dip2px(context, 347), ScreenUtil.dip2px(context, 339));
            dialog.Window.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));
        }

        //图片选择
        private void ShowImageSelectDialog(Context context, DialogStyle style)
        {
            LogUtil.CustomLog("ShowImageSelectDialog");
            View contentView = LayoutInflater.From(context).Inflate(Resource.Layout.activity_dialog, null, false);
            RelativeLayout view = contentView.FindViewById<RelativeLayout>(Resource.Id.ImageSelectView);
            view.Visibility = ViewStates.Visible;
            dialog = new AlertDialog.Builder(context).SetView(contentView).Create();
            TextView cancelBt = view.FindViewById<TextView>(Resource.Id.tv_cancel);
            TextView confirmBt = view.FindViewById<TextView>(Resource.Id.tv_confirm);
            cancelBt.Click += (t, e) =>
            {
                dialog.Dismiss();
            };
            confirmBt.Click += (t, e) =>
            {
                if (selectIndex == -1)
                {
                    ToastUtil.ShowToast(context, "请选择图片");
                    return;
                }
                Callback(null, null, selectIndex);
                dialog.Dismiss();
            };
            mAdapter = new ListAdapter(this, this);
            ListView listView = view.FindViewById<ListView>(Resource.Id.listview);
            listview.Adapter = mAdapter;
            dialog.Window.SetLayout(ScreenUtil.dip2px(context, 270), ScreenUtil.dip2px(context, 339));
            dialog.Window.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));
        }


        private void HandleInput(View view) {
            int[] resIds = {
                Resource.Id.tv_key1, Resource.Id.tv_key2, Resource.Id.tv_key3,
                Resource.Id.tv_key4, Resource.Id.tv_key5, Resource.Id.tv_key6,
                Resource.Id.tv_key7, Resource.Id.tv_key8, Resource.Id.tv_key9,
                Resource.Id.tv_key10, Resource.Id.tv_key11, Resource.Id.tv_key12
            };
            for (int i = 0; i < resIds.Length; i++)
            {
                TextView tv = view.FindViewById<TextView>(resIds[i]);
                tv.Tag = i;
                tv.Click += (t, e) =>
                {
                    int tag = (int) ((TextView)t).Tag;
                    string[] values = {"1","2","3","4","5", "6", "7", "8", "9", ".", "0", ""};
                    TextView valueTv = view.FindViewById<TextView>(Resource.Id.tv_value);
                    if (mDialogStyle == DialogStyle.VariableSelect && VariableSelelctMode == 1) {
                        selectIndex = -1;
                        VariableSelelctMode = 0;
                        valueTv.Text = "";
                    }
                    if (i < 9)
                    {
                        valueTv.Text = valueTv.Text + values[tag];
                    }
                    else if (i > 10)
                    {
                        valueTv.Text = valueTv.Text.Length > 0 ? valueTv.Text.Substring(0, valueTv.Text.Length-1) : "";
                    }
                    else
                    {
                        valueTv.Text = valueTv.Text.Length > 0 ? valueTv.Text + values[tag] : "";
                    }
                };
            }
        }

        public void Show() {
            if (dialog != null)
            {
                dialog.Show();
            }
        }

        public void Dismiss() {
            if (dialog != null)
            {
                dialog.Dismiss();
            }
        }

        public void Callback(string value, string name=null, int index = -1) {

        }


        /*
         * 
         * Delegate and DataSource interface 
         * 
        */
        public int GetItemsCount(Java.Lang.Object adapter)
        {
            return mList.Count;
        }

        public View GetItemView(Java.Lang.Object adapter, ViewGroup parent)
        {
            View convertView = LayoutInflater.From(mCxt).Inflate(Resource.Layout.item_variable, parent, false);
            ViewUtil.SetViewHeight(convertView, ScreenUtil.dip2px(mCxt, 44));
            ViewHolder holder = new ViewHolder();
            holder.imgIv = convertView.FindViewById<ImageView>(Resource.Id.imgIv);
            holder.txtTv = convertView.FindViewById<TextView>(Resource.Id.varTv);
            convertView.Tag = holder;
            convertView.Click += (t, e) =>
            {
                ViewHolder viewHolder = (ViewHolder)(((View)t).Tag);
                int position = (int)viewHolder.imgIv.Tag;
                ClickItem(position);
            };
            return convertView;
        }

        public void UpdateItemView(Java.Lang.Object adapter, View contentView, int position)
        {
            object o = mList[position];
            
        }

        public void ClickItem(int position)
        {
            selectIndex = position;
            if (mDialogStyle == DialogStyle.VariableSelect)
            {
                VariableSelelctMode = 1;
                TextView tv = dialog.FindViewById<RelativeLayout>(Resource.Id.VariableSelectView).FindViewById<TextView>(Resource.Id.tv_value);
                tv.Text = "";
            }
            mAdapter.NotifyDataSetChanged();
        }

        //定义ViewHolder内部类，用于对控件实例进行缓存
        class ViewHolder : Java.Lang.Object
        {
            public ImageView imgIv;
            public TextView txtTv;
        }
    }
}
