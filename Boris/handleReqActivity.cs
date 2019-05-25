using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Essentials;

namespace Boris
{
    [Activity(Label = "handleReqActivity")]
    public class handleReqActivity : Activity
    {

        string carId;
        string renter_id;
        string IMG;
        Button approve;
        Button decline;
        ImageView carImage;
        TextView lisence;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.handleReqLayout);
            carId = Intent.GetStringExtra("ID");
            renter_id = Intent.GetStringExtra("renter_id");
            approve = FindViewById<Button>(Resource.Id.approveReqButton);
            decline = FindViewById<Button>(Resource.Id.declineReqButton);
            carImage = FindViewById<ImageView>(Resource.Id.handleRequstImg);
            lisence = FindViewById<TextView>(Resource.Id.handelReqLicense);
            approve.Click += approveAction;
            decline.Click += declineAction;
            Console.WriteLine("car num is:" + carId);
            carInfo_result info = new carInfo_result();
            info.get_from_cloud("https://carshareserver.azurewebsites.net/api/getCarDetails?vehicle_id=" + carId);
            carTotalInfo totalInfo = info.getInfo();
            IMG = totalInfo.img;
            lisence.Text = totalInfo.id;
            async void GetImageBitmapFromUrl(string url, DownloadDataCompletedEventHandler eventFunc)
            {
                Bitmap img1 = null;
                using (var webClient = new System.Net.WebClient())
                {
                    webClient.DownloadDataCompleted += eventFunc;
                    webClient.DownloadDataAsync(new Uri(url));
                }
            }
            if (IMG != "")
            {
                GetImageBitmapFromUrl(IMG, loadCarImage);

                void loadCarImage(object sender, DownloadDataCompletedEventArgs e)
                {
                    byte[] raw = e.Result;
                    Bitmap img1 = BitmapFactory.DecodeByteArray(raw, 0, raw.Length);
                    if (img1 != null)
                    {
                        ImageView imagen = FindViewById<ImageView>(Resource.Id.handleRequstImg);
                        imagen.SetImageBitmap(img1);

                        //Calculate image size
                        double ratio = (double)img1.Height / (double)img1.Width;
                        FindViewById<RelativeLayout>(Resource.Id.handelReqLoadingPanel).Visibility = ViewStates.Gone;
                        imagen.LayoutParameters.Height = (int)((double)Resources.DisplayMetrics.WidthPixels * ratio);
                    }
                }
            }
            else
            {
                FindViewById<RelativeLayout>(Resource.Id.loadingPanel).Visibility = ViewStates.Gone;
            }

        }

        void approveAction(object sender, EventArgs eventArgs)
        {
            string login_hash = Preferences.Get("login_hash", "1");
            string user_name = Preferences.Get("user_id", "");
            String address = "https://carshareserver.azurewebsites.net/api/permitAction?action=" + "1" + "&login_hash=" + login_hash + "&vehicle_id=" + carId + "&user_id=" + user_name +"&renter_id=" + renter_id;
            HttpClient client = new HttpClient();
            Console.WriteLine(address);

            var responseString = client.GetStringAsync(address);
            Console.WriteLine("this is the respone:" +responseString);
            Context context = Application.Context;
            string text = "You accepted the request.";
            ToastLength duration = ToastLength.Long;
            var toast = Toast.MakeText(context, text, duration);
            toast.Show();
            Preferences.Set("isPending", false);
            Preferences.Set("isHandle", false);
            Preferences.Set("displaySettings", 0);
            Finish();
        }
        void declineAction(object sender, EventArgs eventArgs)
        {
            string login_hash = Preferences.Get("login_hash", "1");
            string user_name = Preferences.Get("user_id", "");
            String address = "https://carshareserver.azurewebsites.net/api/permitAction?action=" + "0" + "&login_hash=" + login_hash + "&vehicle_id=" + carId + "&user_id=" + user_name + "&renter_id=" + renter_id;
            HttpClient client = new HttpClient();
            var responseString = client.GetStringAsync(address);
            //decline action to server
            Context context = Application.Context;
            string text = "You declined the request.";
            ToastLength duration = ToastLength.Long;
            var toast = Toast.MakeText(context, text, duration);
            Preferences.Set("isPending", false);
            Preferences.Set("isHandle", false);
            Preferences.Set("displaySettings", 0);
            toast.Show();
            Finish();

        }
    }
}