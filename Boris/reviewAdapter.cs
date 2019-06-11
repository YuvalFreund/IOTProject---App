using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Boris
{
    public class ReviewAdapter : BaseAdapter<Tuple<string, int>>
    {
        public Activity activity;
        public List<Tuple<string, int>> data;

        public ReviewAdapter(Activity activity, List<Tuple<string, int>> data)
        {
            this.activity = activity;
            this.data = data;
        }

        public override Tuple<string, int> this[int position]
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

            TextView reviewText = view.FindViewById<TextView>(Resource.Id.reviewText);
            RatingBar reviewScore = view.FindViewById<RatingBar>(Resource.Id.reviewScore);
            reviewText.Text = data[position].Item1;
            reviewScore.Rating = data[position].Item2;
            
            return view;
        }
    }
}