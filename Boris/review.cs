using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
    [Activity(Label = "review")]
    public class review : Activity
    {
        Button submit;
        RatingBar stars;
        string carId;
        TextView content;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.reviewLayout);
            submit = FindViewById<Button>(Resource.Id.submitReview);
            stars = FindViewById<RatingBar>(Resource.Id.ratingBar);
            content = FindViewById<TextView>(Resource.Id.cont);
            submit.Click += submitAction;
            TextView price = FindViewById<TextView>(Resource.Id.reviewTotalCost);
            price.Text = "Total price: " + Intent.GetStringExtra("cost");
            carId = Intent.GetStringExtra("carId");
        }

        private void submitAction(object sender, EventArgs e)
        {
            string login_hash = Preferences.Get("login_hash", "");
            string reviewer = Preferences.Get("user_id", "");
            string reviewee= Preferences.Get("reviewee", "5");
            string rating = stars.Rating.ToString();
            string cont = content.Text;
            string Cost = Intent.GetStringExtra("cost");
            Cost = Cost.Substring(0, Cost.Length - 1);
            string Car = carId;
            string Date = DateTime.Now.ToString("dd/MM/yyyy");
            Console.WriteLine(Date);
            Preferences.Set("displaySetting", 0);
            string address = "https://carshareserver.azurewebsites.net/api/addReview?reviewer_id=" + reviewer + "&reviewee_id=" + reviewee + "&rate=" + rating + "&cont=" + cont + "&login_hash=" + login_hash + "&cost=" + Cost + "&car_id=" +Car+ "&date=" + Date;
            Console.WriteLine(address);
            HttpClient client = new HttpClient();
            var responseString = client.GetStringAsync(address);
            Finish();
        }
    }
}