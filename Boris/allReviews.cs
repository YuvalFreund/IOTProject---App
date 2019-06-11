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
using Xamarin.Essentials;

namespace Boris
{
    [Activity(Label = "allReviews")]
    public class allReviews : Activity
    {
        review_result reviewInfo = new review_result();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.allReviews);
            ListView listi = FindViewById<ListView>(Resource.Id.allReviewsList);
            TextView noReview = FindViewById<TextView>(Resource.Id.noReviewsText);
            Button exit = FindViewById<Button>(Resource.Id.allReviewsExit);
            exit.Click += exitFunk;
            string user_id = Preferences.Get("user_id", "0");
            string login_hash = Preferences.Get("login_hash", "0");
            string reviewee = Intent.GetStringExtra("reviewee");
            string address = "https://carshareserver.azurewebsites.net/api/getReviews?reviewee_id=" + reviewee + "&login_hash=" + login_hash + "&user_id=" + user_id;
            reviewInfo.get_from_cloud(address);
            List<Tuple<string, int>> cleanReviews = new List<Tuple<string, int>>();  
            int stauts = reviewInfo.getReviews().status;
            if (stauts != -1)
            {
                noReview.Visibility = ViewStates.Gone;
                List<revData> all_reviews ;
                all_reviews = reviewInfo.GetRevs();
                foreach (var review in all_reviews)
                {
                    Tuple<string, int> temp = Tuple.Create("Review: " + review.content.ToString(),  Convert.ToInt32(review.rate));
                    Console.WriteLine(temp.Item1 + temp.Item2);

                    cleanReviews.Add(temp);
                }
            }
            ReviewAdapter adapter = new ReviewAdapter(this, cleanReviews);
            listi.Adapter = adapter;
        }

        private void exitFunk(object sender, EventArgs e)
        {
            Finish();
        }
    }
}