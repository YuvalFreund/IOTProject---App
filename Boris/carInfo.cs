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
    [Activity(Label = "carInfo")]
    public class carInfo : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            string carId = Intent.GetStringExtra("ID");
            SetContentView(Resource.Layout.carInfoLayout);
            TextView carModelView = FindViewById<TextView>(Resource.Id.carInfoModel);
            TextView carProductionView = FindViewById<TextView>(Resource.Id.carInfoProduction);
            TextView carLicenseView = FindViewById<TextView>(Resource.Id.carInfoLicense);
            TextView carModeView = FindViewById<TextView>(Resource.Id.carInfoMode);
            TextView carUserView = FindViewById<TextView>(Resource.Id.carInfoUser);

            carInfo_result info = new carInfo_result();
            info.get_from_cloud("https://carshareserver.azurewebsites.net/api/getCarDetails?vehicle_id=" + carId);
            carTotalInfo totalInfo = info.getInfo();
            carModelView.Text = "Car model:" + " " + totalInfo.Manufacturer + " " + totalInfo.model;
            carProductionView.Text = "Production year:" + " " + totalInfo.year;
            carLicenseView.Text = "License plate:" + " " + carId;
            if (totalInfo.mode)
            {
                carModeView.Text = "Gear:" + " " + "Automatic";
            }
            else
            {
                carModeView.Text = "Gear:" + " " + "Manual";
            }

            user carOwner = totalInfo.User;
            carUserView.Text = "Owner's name:" + " " + carOwner.first_name + " " + carOwner.last_name;
            if (totalInfo.img != "")
            {
                string imgAdress = totalInfo.img;
                GetImageBitmapFromUrl(imgAdress);

                async void GetImageBitmapFromUrl(string url)
                {
                    Bitmap img1 = null;
                    using (var webClient = new System.Net.WebClient())
                    {
                        webClient.DownloadDataCompleted += DownloadDataCompleted;
                        webClient.DownloadDataAsync(new Uri(url));
                    }
                }

                void DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
                {
                    byte[] raw = e.Result;
                    Bitmap img1 = BitmapFactory.DecodeByteArray(raw, 0, raw.Length);
                    if (img1 != null)
                    {
                        ImageView imagen = FindViewById<ImageView>(Resource.Id.carInfoImg);
                        imagen.SetImageBitmap(img1);
                    }
                }
            }
        }
    }
}