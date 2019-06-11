using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Boris
{
    [Activity(Label = "myCarsInfo")]
    public class myCarsInfo : Activity
    {
        string imgAdress;
        ImageView imagen;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.myCarInfo);
            string carId = Intent.GetStringExtra("ID");
            TextView myCarModelView = FindViewById<TextView>(Resource.Id.myCarInfoModel);
            TextView myCarProductionView = FindViewById<TextView>(Resource.Id.myCarInfoProduction);
            TextView myCarLicenseView = FindViewById<TextView>(Resource.Id.myCarInfoLicense);
            TextView myCarModeView = FindViewById<TextView>(Resource.Id.myCarInfoMode);
            Button myCarsExit = FindViewById<Button>(Resource.Id.myCarExit);
            myCarsExit.Click += funcExit;


            carInfo_result info = new carInfo_result();
            info.get_from_cloud("https://carshareserver.azurewebsites.net/api/getCarDetails?vehicle_id=" + carId);
            carTotalInfo totalInfo = info.getInfo();
            myCarModelView.Text = totalInfo.Manufacturer + " " + totalInfo.model;
            myCarProductionView.Text = totalInfo.year;
            myCarLicenseView.Text = carId;
            if (totalInfo.mode)
            {
                myCarModeView.Text = "Automatic";
            }
            else
            {
                myCarModeView.Text = "Manual";
            }

            if (totalInfo.img != "")
            {
                imgAdress = totalInfo.img;
                GetImageBitmapFromUrl(imgAdress, loadCarImage);

                void loadCarImage(object sender, DownloadDataCompletedEventArgs e)
                {
                    byte[] raw = e.Result;
                    Bitmap img1 = BitmapFactory.DecodeByteArray(raw, 0, raw.Length);
                    if (img1 != null)
                    {
                        imagen = FindViewById<ImageView>(Resource.Id.myCarInfoImg);
                        imagen.SetImageBitmap(img1);

                        //Calculate image size
                        double ratio = (double)img1.Height / (double)img1.Width;
                        FindViewById<RelativeLayout>(Resource.Id.myCarloadingPanel).Visibility = ViewStates.Gone;
                        imagen.LayoutParameters.Height = (int)((double)Resources.DisplayMetrics.WidthPixels * ratio);
                    }
                }
            }
            else
            {
                FindViewById<RelativeLayout>(Resource.Id.myCarloadingPanel).Visibility = ViewStates.Gone;
            }
        }
        async void GetImageBitmapFromUrl(string url, DownloadDataCompletedEventHandler eventFunc)
        {
            Bitmap img1 = null;
            using (var webClient = new System.Net.WebClient())
            {
                webClient.DownloadDataCompleted += eventFunc;
                webClient.DownloadDataAsync(new Uri(url));
            }
        }
        private void funcExit(object sender, EventArgs e)
        {
            Finish();
        }
    }
}