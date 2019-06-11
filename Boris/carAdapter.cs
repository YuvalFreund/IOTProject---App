using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Support.V4.App;

using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Boris
{
    public class ButtonAdapter : BaseAdapter<string>
    {
        public Fragment activity;
        public List<string> data;

        public ButtonAdapter(Fragment activity, List<string> data)
        {
            this.activity = activity;
            this.data = data;
        }

        public override string this[int position]
        {
            get { return this.data[position]; }
        }

        public override int Count
        {
            get { return this.data.Count; }
        }

        public override long GetItemId(int position)
        {
            return 0;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView;

            if (view == null)
            {
                view = this.activity.LayoutInflater.Inflate(Resource.Layout.listCarRow, null);
            }

            TextView lisence = view.FindViewById<TextView>(Resource.Id.myCarsCarNumber);
            lisence.Text = this.data[position];

            Button details = view.FindViewById<Button>(Resource.Id.myCarsDetails);
            details.Tag = lisence.Text ;
            details.SetOnClickListener(new ButtonClickListener(this.activity));

            return view;
        }

        private class ButtonClickListener : Java.Lang.Object, View.IOnClickListener
        {
            private Fragment activity;

            public ButtonClickListener(Fragment activity)
            {
                this.activity = activity;
            }

            public void OnClick(View v)
            {
                
                string name = (string)v.Tag;
                Intent car_try = new Intent(activity.Context, typeof(myCarsInfo));
                car_try.PutExtra("ID", name);
                activity.StartActivity(car_try);
            }
        }
    }
}