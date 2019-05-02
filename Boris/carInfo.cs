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
            GridLayout carModeGridLayout = FindViewById<GridLayout>(Resource.Id.gridLayout1);

            //carModeGridLayout.SetBackgroundColor(Color.Aqua);
            TextView carModelView = FindViewById<TextView>(Resource.Id.carInfoModel);
            TextView carProductionView = FindViewById<TextView>(Resource.Id.carInfoProduction);
            TextView carLicenseView = FindViewById<TextView>(Resource.Id.carInfoLicense);
            TextView carModeView = FindViewById<TextView>(Resource.Id.carInfoMode);
            TextView carUserView = FindViewById<TextView>(Resource.Id.carInfoUser);
            TextView carUserEmailView = FindViewById<TextView>(Resource.Id.carInfoUserEmail);



            carInfo_result info = new carInfo_result();
            info.get_from_cloud("https://carshareserver.azurewebsites.net/api/getCarDetails?vehicle_id=" + carId);
            carTotalInfo totalInfo = info.getInfo();
            carModelView.Text = totalInfo.Manufacturer + " " + totalInfo.model;
            carProductionView.Text = totalInfo.year;
            carLicenseView.Text = carId;
            if (totalInfo.mode)
            {
                carModeView.Text = "Automatic";
            }
            else
            {
                carModeView.Text = "Manual";
            }

            user carOwner = totalInfo.User;
            carUserView.Text = carOwner.first_name + " " + carOwner.last_name;
            carUserEmailView.Text = carOwner.email;
            async void GetImageBitmapFromUrl(string url, DownloadDataCompletedEventHandler eventFunc)
            {
                Bitmap img1 = null;
                using (var webClient = new System.Net.WebClient())
                {
                    webClient.DownloadDataCompleted += eventFunc;
                    webClient.DownloadDataAsync(new Uri(url));
                }
            }
            if (carOwner.img != "")
            {
                GetImageBitmapFromUrl("https://carshareserver.azurewebsites.net/api/getUserImage?user_id=" + carOwner.id, loadUserImage);

                void loadUserImage(object sender, DownloadDataCompletedEventArgs e)
                {
                    byte[] raw = e.Result;
                    Bitmap img1 = BitmapFactory.DecodeByteArray(raw, 0, raw.Length);
                    if (img1 != null)
                    {
                        ImageView userImage = FindViewById<ImageView>(Resource.Id.userImg);
                        userImage.SetImageBitmap(img1);

                    }
                }
            }
            if (totalInfo.img != "")
            {
                string imgAdress = totalInfo.img;
                GetImageBitmapFromUrl(imgAdress, loadCarImage);

                void loadCarImage(object sender, DownloadDataCompletedEventArgs e)
                {
                    byte[] raw = e.Result;
                    Bitmap img1 = BitmapFactory.DecodeByteArray(raw, 0, raw.Length);
                    if (img1 != null)
                    {
                        ImageView imagen = FindViewById<ImageView>(Resource.Id.carInfoImg);
                        imagen.SetImageBitmap(img1);

                        //Calculate image size
                        double ratio = (double)img1.Height / (double)img1.Width;
                        imagen.LayoutParameters.Height = (int)((double)Resources.DisplayMetrics.WidthPixels * ratio);
                    }
                }
            }
        }
    }
}