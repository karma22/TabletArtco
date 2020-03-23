using Android.Content;
using Android.Graphics.Drawables;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using System.Collections.Generic;
using System;
using Com.Bumptech.Glide;

namespace TabletArtco
{
    /***********************************************************************************
    ************************************************************************************
    * 
    * ParentDialog
    * 
    ************************************************************************************
    ************************************************************************************/
    class ParentDialog : Delegate, DataSource
    {
        public Context mCxt;
        public AlertDialog dialog = null;
        public int selectIndex = -1;
        public List<string> mImgList { get; set; } = new List<string>();
        public List<string> mList { get; set; } = new List<string>();
        public ListAdapter mAdapter;

        public ParentDialog(Context context)
        {
            mCxt = context;
        }

        public void Show()
        {
            dialog?.Show();
        }

        public void Dismiss()
        {
            dialog?.Dismiss();   
        }

        public void HandleInput(View view)
        {
            int[] resIds = {
                Resource.Id.tv_key1, Resource.Id.tv_key2, Resource.Id.tv_key3,
                Resource.Id.tv_key4, Resource.Id.tv_key5, Resource.Id.tv_key6,
                Resource.Id.tv_key7, Resource.Id.tv_key8, Resource.Id.tv_key9,
                Resource.Id.tv_key10, Resource.Id.tv_key11, Resource.Id.tv_key12,
            };
            for (int i = 0; i < resIds.Length; i++)
            {
                TextView tv = view.FindViewById<TextView>(resIds[i]);
                tv.Tag = i;
                tv.Click += (t, e) =>
                {
                    int tag = (int)((TextView)t).Tag;
                    string[] values = { "1", "2", "3", "4", "5", "6", "7", "8", "9", ".", "0", "" };
                    TextView valueTv = view.FindViewById<TextView>(Resource.Id.tv_value);
                    PreHander(valueTv);
                    if (tag < 9)
                    {
                        valueTv.Text = valueTv.Text + values[tag];
                    }
                    else if (tag < 11)
                    {
                        valueTv.Text = valueTv.Text.Length > 0 ? valueTv.Text + values[tag] : "";
                    }
                    else
                    {
                        valueTv.Text = valueTv.Text.Length > 0 ? valueTv.Text.Substring(0, valueTv.Text.Length - 1) : "";
                    }
                };
            }
        }

        public virtual void PreHander(TextView tv) {
            mAdapter?.NotifyDataSetChanged();
        }

        public virtual void ItemClickHander(int position) {

        }

        public int GetItemsCount(Java.Lang.Object adapter)
        {
            return mList.Count;
        }

        public View GetItemView(Java.Lang.Object adapter, ViewGroup parent)
        {
            View convertView = LayoutInflater.From(mCxt).Inflate(Resource.Layout.item_variable, parent, false);
            ViewUtil.SetViewHeight(convertView, ScreenUtil.dip2px(mCxt, 36));
            ViewHolder holder = new ViewHolder();
            holder.imgIv = convertView.FindViewById<ImageView>(Resource.Id.imgIv);
            holder.txtTv = convertView.FindViewById<TextView>(Resource.Id.varTv);
            convertView.Tag = holder;
            convertView.Click += (t, e) =>
            {
                ViewHolder viewHolder = (ViewHolder)(((View)t).Tag);
                int position = (int)viewHolder.txtTv.Tag;
                ClickItem(position);
            };
            return convertView;
        }

        public void UpdateItemView(Java.Lang.Object adapter, View contentView, int position)
        {
            string name = mList[position];
            ViewHolder viewHolder = (ViewHolder)contentView.Tag;
            viewHolder.txtTv.Tag = position;
            viewHolder.txtTv.Text = name;
            viewHolder.txtTv.SetTextSize(Android.Util.ComplexUnitType.Dip, 20);
            viewHolder.txtTv.SetTextColor(selectIndex == position ? Color.Red : Color.Black); 

            if (mImgList != null && mImgList.Count > 0)
            {
                string url = mImgList[position];
                Glide.With(mCxt).Load(GlideUtil.GetGlideUrl(url)).Into(viewHolder.imgIv);
                viewHolder.txtTv.SetPadding(ScreenUtil.dip2px(mCxt, 40), 0, 0, 0);
                viewHolder.imgIv.Visibility = ViewStates.Visible;
            }
            else
            {
                viewHolder.txtTv.SetPadding(0, 0, 0, 0);
                viewHolder.imgIv.Visibility = ViewStates.Gone;
            }
        }

        public void ClickItem(int position)
        {
            selectIndex = position;
            mAdapter.NotifyDataSetChanged();
            ItemClickHander(position);
        }

        //定义ViewHolder内部类，用于对控件实例进行缓存
        class ViewHolder : Java.Lang.Object
        {
            public ImageView imgIv;
            public TextView txtTv;
        }
    }

    /***********************************************************************************
    ************************************************************************************
    * 
    * SignalDialog
    * 
    ************************************************************************************
    ************************************************************************************/
    class SignalDialog : ParentDialog
    {
        Action<string> mAction;

        public SignalDialog(Context context, Action<string> action): base(context) {
            mAction = action;
            initView(context);
        }

        //信号输入
        private void initView(Context context)
        {
            View contentView = LayoutInflater.From(context).Inflate(Resource.Layout.dialog_signal, null, false);
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
                if (textEt.Text.Length <= 0)
                {
                    Toast.MakeText(context, "输入内容不能为空", ToastLength.Long);
                    return;
                }
                mAction?.Invoke(textEt.Text);
                dialog.Dismiss();
            };
            dialog.Window.SetLayout(ScreenUtil.dip2px(context, 343), ScreenUtil.dip2px(context, 157));
            dialog.Window.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));
        }
    }

    /***********************************************************************************
    ************************************************************************************
    * 
    * ImageSelectDialog
    * 
    ************************************************************************************
    ************************************************************************************/
    class ImageSelectDialog : ParentDialog
    {
        Action<int> mAction;
        public ImageSelectDialog(Context context, bool isClick, Action<int> action) : base(context)
        {
            mAction = action;
            initView(context, isClick);
        }

        private void initView(Context context, bool isClick)
        {
            LogUtil.CustomLog("ShowImageSelectDialog");
            View contentView = LayoutInflater.From(context).Inflate(Resource.Layout.dialog_img_select, null, false);
            RelativeLayout view = contentView.FindViewById<RelativeLayout>(Resource.Id.ImageSelectView);
            view.Visibility = ViewStates.Visible;
            view.SetBackgroundResource(!isClick ? Resource.Drawable.dialog_collision_bg : Resource.Drawable.dialog_press_bg);
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
                mAction?.Invoke(selectIndex);
                dialog.Dismiss();
            };
            mAdapter = new ListAdapter(this, this);
            ListView listView = view.FindViewById<ListView>(Resource.Id.listview);
            listView.Adapter = mAdapter;
            dialog.Window.SetLayout(ScreenUtil.dip2px(context, 270), ScreenUtil.dip2px(context, 339));
            dialog.Window.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));
        }
    }


    /***********************************************************************************
    ************************************************************************************
    * 
    * VariableSelectDialog
    * 
    ************************************************************************************
    ************************************************************************************/
    class VariableSelectDialog : ParentDialog
    {
        Action<int, string> mAction;
        int VariableSelelctMode = 0;

        public VariableSelectDialog(Context context, Action<int, string> action) : base(context)
        {
            mAction = action;
            initView(context);
        }

        private void initView(Context context)
        {
            LogUtil.CustomLog("ShowVariableSelectDialog");
            View contentView = LayoutInflater.From(context).Inflate(Resource.Layout.dialog_var_select, null, false);
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
                if (selectIndex == -1 && valueTv.Text.Length == 0)
                {
                    ToastUtil.ShowToast(context, "请选择变量或输入值");
                    return;
                }
                if (selectIndex != -1)
                {
                    mAction?.Invoke(selectIndex, null);
                }
                else
                {
                    mAction?.Invoke(-1, valueTv.Text);
                }
                HandleInput(view);
                dialog.Dismiss();
            };
            mAdapter = new ListAdapter(this, this);
            ListView listView = view.FindViewById<ListView>(Resource.Id.listview);
            listView.Adapter = mAdapter;
            HandleInput(view);
            dialog.Window.SetLayout(ScreenUtil.dip2px(context, 347), ScreenUtil.dip2px(context, 339));
            dialog.Window.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));
        }

        public override void PreHander(TextView tv)
        {
            base.PreHander(tv);
            if (VariableSelelctMode == 1)
            {
                selectIndex = -1;
                VariableSelelctMode = 0;
                tv.Text = "";
            }
        }

        public override void ItemClickHander(int position) {
            VariableSelelctMode = 1;
            TextView valueTv = dialog.FindViewById<TextView>(Resource.Id.tv_value);
            valueTv.Text = mList[position];
        }
    }

    /***********************************************************************************
    ************************************************************************************
    * 
    * VariableChangeDialog
    * 
    ************************************************************************************
    ************************************************************************************/
    class VariableChangeDialog : ParentDialog
    {
        Action<int, string> mAction;
        public VariableChangeDialog(Context context, bool isSet, Action<int, string> action) : base(context)
        {
            mAction = action;
            initView(context, isSet);
        }

        private void initView(Context context, bool isSet)
        {
            LogUtil.CustomLog("ShowVariableAddOrSetDialog");
            View contentView = LayoutInflater.From(context).Inflate(Resource.Layout.dialog_var_change, null, false);
            RelativeLayout view = contentView.FindViewById<RelativeLayout>(Resource.Id.VariableAddOrSetView);
            view.Visibility = ViewStates.Visible;
            dialog = new AlertDialog.Builder(context).SetView(contentView).Create();
            view.SetBackgroundResource(!isSet ? Resource.Drawable.dialog_variable_add : Resource.Drawable.dialog_variable_set);
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
                mAction?.Invoke(selectIndex, valueTv.Text);
                dialog.Dismiss();
            };
            mAdapter = new ListAdapter(this, this);
            ListView listView = view.FindViewById<ListView>(Resource.Id.listview);
            listView.Adapter = mAdapter;
            HandleInput(view);
            dialog.Window.SetLayout(ScreenUtil.dip2px(context, 380), ScreenUtil.dip2px(context, 360));
            dialog.Window.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));
        }
    }

    /***********************************************************************************
    ************************************************************************************
    * 
    * VariableInitDialog
    * 
    ************************************************************************************
    ************************************************************************************/
    class VariableInitDialog : ParentDialog
    {
        Action<string, string> mAction;
        public VariableInitDialog(Context context, Action<string, string> action) : base(context)
        {
            mAction = action;
            initView(context);
        }

        private void initView(Context context)
        {
            LogUtil.CustomLog("ShowVariableInitDialog");
            View contentView = LayoutInflater.From(context).Inflate(Resource.Layout.dialog_var_init, null, false);
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
                mAction?.Invoke(nameEt.Text, valueEt.Text);
                dialog.Dismiss();
            };
            dialog.Window.SetLayout(ScreenUtil.dip2px(context, 333), ScreenUtil.dip2px(context, 199));
            dialog.Window.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));
        }
    }

    /***********************************************************************************
    ************************************************************************************
    * 
    * SpeakDialog
    * 
    ************************************************************************************
    ************************************************************************************/
    class SpeakDialog : ParentDialog
    {
        Action<string> mAction;

        public SpeakDialog(Context context, Action<string> action) : base(context)
        {
            mAction = action;
            initView(context);
        }

        //信号输入
        private void initView(Context context)
        {
            View contentView = LayoutInflater.From(context).Inflate(Resource.Layout.dialog_speak, null, false);
            RelativeLayout view = contentView.FindViewById<RelativeLayout>(Resource.Id.SignalView);
            view.Visibility = ViewStates.Visible;
            dialog = new AlertDialog.Builder(context).SetView(contentView).Create();
            TextView cancelBt = view.FindViewById<TextView>(Resource.Id.tv_cancel);
            TextView confirmBt = view.FindViewById<TextView>(Resource.Id.tv_confirm);
            EditText textEt = view.FindViewById<EditText>(Resource.Id.et_speak);
            
            cancelBt.Click += (t, e) =>
            {
                dialog.Dismiss();
            };
            confirmBt.Click += (t, e) =>
            {
                if (textEt.Text.Length <= 0)
                {
                    Toast.MakeText(context, "输入内容不能为空", ToastLength.Long);
                    return;
                }
                mAction?.Invoke(textEt.Text);
                dialog.Dismiss();
            };
            dialog.Window.SetLayout(ScreenUtil.dip2px(context, 406), ScreenUtil.dip2px(context, 240));
            dialog.Window.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));
        }
    }

    /***********************************************************************************
    ************************************************************************************
    * 
    * ControlXYDialog
    * 
    ************************************************************************************
    ************************************************************************************/
    class ControlXYDialog : Delegate, DataSource
    {
        Action<string, string> mAction;

        public Context mCxt;
        public AlertDialog dialog = null;
        public List<string> mList { get; set; } = new List<string>();

        public int selectIndexX = -1;
        //public int selectIndexY = -1;

        public ListAdapter mAdapterX;
        //public ListAdapter mAdapterY;

        public int selectTextBox = 0;
        public int VariableSelelctMode = 0;

        public ControlXYDialog(Context context, Action<string, string> action)
        {
            mCxt = context;
            mAction = action;
            initView(context);
        }

        public void Show()
        {
            dialog?.Show();
        }

        public void Dismiss()
        {
            dialog?.Dismiss();
        }

        private void initView(Context context)
        {
            View contentView = LayoutInflater.From(context).Inflate(Resource.Layout.dialog_xy_select, null, false);
            RelativeLayout view = contentView.FindViewById<RelativeLayout>(Resource.Id.VariableSelectView);

            view.Visibility = ViewStates.Visible;
            dialog = new AlertDialog.Builder(context).SetView(contentView).Create();
            TextView cancelBt = view.FindViewById<TextView>(Resource.Id.tv_cancel);
            TextView confirmBt = view.FindViewById<TextView>(Resource.Id.tv_confirm);
            TextView valueTvX = view.FindViewById<TextView>(Resource.Id.tv_valueX);
            TextView valueTvY = view.FindViewById<TextView>(Resource.Id.tv_valueY);

            cancelBt.Click += (t, e) =>
            {
                dialog.Dismiss();
            };
            confirmBt.Click += (t, e) =>
            {
                mAction?.Invoke(valueTvX.Text, valueTvY.Text);
                dialog.Dismiss();
            };
            valueTvX.Click += (t, e) =>
            {
                selectTextBox = 0;
            };
            valueTvY.Click += (t, e) =>
            {
                selectTextBox = 1;
            };

            mAdapterX = new ListAdapter(this, this);
            ListView listViewX = view.FindViewById<ListView>(Resource.Id.listviewX);
            listViewX.Adapter = mAdapterX;

            HandleInput(view);
            dialog.Window.SetLayout(ScreenUtil.dip2px(context, 347), ScreenUtil.dip2px(context, 339));
            dialog.Window.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));
        }

        public void HandleInput(View view)
        {
            int[] resIds = {
                Resource.Id.tv_key1, Resource.Id.tv_key2, Resource.Id.tv_key3,
                Resource.Id.tv_key4, Resource.Id.tv_key5, Resource.Id.tv_key6,
                Resource.Id.tv_key7, Resource.Id.tv_key8, Resource.Id.tv_key9,
                Resource.Id.tv_key10, Resource.Id.tv_key11, Resource.Id.tv_key12,
            };
            for (int i = 0; i < resIds.Length; i++)
            {
                TextView tv = view.FindViewById<TextView>(resIds[i]);
                tv.Tag = i;
                tv.Click += (t, e) =>
                {
                    int tag = (int)((TextView)t).Tag;
                    string[] values = { "1", "2", "3", "4", "5", "6", "7", "8", "9", ".", "0", "" };
                    TextView valueTv;
                    if (selectTextBox==0)
                        valueTv = view.FindViewById<TextView>(Resource.Id.tv_valueX);
                    else
                        valueTv = view.FindViewById<TextView>(Resource.Id.tv_valueY);

                    PreHander(valueTv);
                    if (tag < 9)
                    {
                        valueTv.Text = valueTv.Text + values[tag];
                    }
                    else if (tag < 11)
                    {
                        valueTv.Text = valueTv.Text.Length > 0 ? valueTv.Text + values[tag] : "";
                    }
                    else
                    {
                        valueTv.Text = valueTv.Text.Length > 0 ? valueTv.Text.Substring(0, valueTv.Text.Length - 1) : "";
                    }
                };
            }
        }

        public void PreHander(TextView tv)
        {
            if (VariableSelelctMode == 1)
            {
                selectIndexX = -1;
                VariableSelelctMode = 0;
                tv.Text = "";
            }
        }

        public void ItemClickHander(int position)
        {
            VariableSelelctMode = 1;
            TextView valueTv;
            if (selectTextBox == 0)
                valueTv = dialog.FindViewById<TextView>(Resource.Id.tv_valueX);
            else
                valueTv = dialog.FindViewById<TextView>(Resource.Id.tv_valueY);
            valueTv.Text = mList[position];
        }

        public int GetItemsCount(Java.Lang.Object adapter)
        {
            return mList.Count;
        }

        public View GetItemView(Java.Lang.Object adapter, ViewGroup parent)
        {
            View convertView = LayoutInflater.From(mCxt).Inflate(Resource.Layout.item_variable, parent, false);
            ViewUtil.SetViewHeight(convertView, ScreenUtil.dip2px(mCxt, 36));
            ViewHolder holder = new ViewHolder();
            holder.imgIv = convertView.FindViewById<ImageView>(Resource.Id.imgIv);
            holder.txtTv = convertView.FindViewById<TextView>(Resource.Id.varTv);
            convertView.Tag = holder;
            convertView.Click += (t, e) =>
            {
                ViewHolder viewHolder = (ViewHolder)(((View)t).Tag);
                int position = (int)viewHolder.txtTv.Tag;
                ClickItem(position);
            };
            return convertView;
        }

        public void UpdateItemView(Java.Lang.Object adapter, View contentView, int position)
        {
            string name = mList[position];
            ViewHolder viewHolder = (ViewHolder)contentView.Tag;
            viewHolder.txtTv.Tag = position;
            viewHolder.txtTv.Text = name;
            viewHolder.txtTv.SetTextSize(Android.Util.ComplexUnitType.Dip, 20);
            viewHolder.txtTv.SetTextColor(selectIndexX == position ? Color.Red : Color.Black);
            viewHolder.txtTv.SetPadding(0, 0, 0, 0);
            viewHolder.imgIv.Visibility = ViewStates.Gone;
        }

        public void ClickItem(int position)
        {
            selectIndexX = position;
            mAdapterX.NotifyDataSetChanged();
            ItemClickHander(position);
        }

        //定义ViewHolder内部类，用于对控件实例进行缓存
        class ViewHolder : Java.Lang.Object
        {
            public ImageView imgIv;
            public TextView txtTv;
        }
    }




    /***********************************************************************************
    ************************************************************************************
    * 
    * MessageBoxDialog
    * 
    ************************************************************************************
    ************************************************************************************/
    class MessageBoxDialog : ParentDialog
    {
        Action mAction;

        public MessageBoxDialog(Context context, string msg, Action action) : base(context)
        {
            mAction = action;
            initView(context, msg);
        }

        private void initView(Context context, string msg)
        {
            View contentView = LayoutInflater.From(context).Inflate(Resource.Layout.dialog_msgbox, null, false);
            RelativeLayout view = contentView.FindViewById<RelativeLayout>(Resource.Id.SignalView);
            view.Visibility = ViewStates.Visible;
            dialog = new AlertDialog.Builder(context).SetView(contentView).Create();
            TextView cancelBt = view.FindViewById<TextView>(Resource.Id.tv_cancel);
            TextView confirmBt = view.FindViewById<TextView>(Resource.Id.tv_confirm);
            TextView msgTv = view.FindViewById<TextView>(Resource.Id.tv_message);
            msgTv.Text = msg;

            cancelBt.Click += (t, e) =>
            {
                dialog.Dismiss();
            };
            confirmBt.Click += (t, e) =>
            {
                mAction?.Invoke();
                dialog.Dismiss();
            };
            dialog.Window.SetLayout(ScreenUtil.dip2px(context, 343), ScreenUtil.dip2px(context, 157));
            dialog.Window.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));
        }
    }
}
