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
    class historyAdapter : BaseAdapter<Tuple<string, string, string>>
    {
        public Fragment activity;
        public List<Tuple<string, string,string>> data;

        public historyAdapter(Fragment activity, List<Tuple<string, string, string>> data)
        {
            this.activity = activity;
            this.data = data;
        }

        public override Tuple<string, string, string> this[int position]
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
                view = this.activity.LayoutInflater.Inflate(Resource.Layout.reviewRow, null);
            }

            TextView HistoryDate = view.FindViewById<TextView>(Resource.Id.historyDate);
            TextView HistoryCost = view.FindViewById<TextView>(Resource.Id.historyCost);
            TextView HistoryLisence = view.FindViewById<TextView>(Resource.Id.historyLisence);

            HistoryDate.Text = data[position].Item1;
            HistoryCost.Text = data[position].Item2;
            HistoryLisence.Text = data[position].Item3;
            return view;
        }
    }
}