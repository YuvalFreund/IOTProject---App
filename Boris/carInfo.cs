﻿using System;
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
using Boris;

namespace Boris
{
    [Activity(Label = "carInfo")]
    public class carInfo : Activity
    {
        Button requestCarButton;
        string carId;
        string imgAdress;
        private ListView nListView;
        //private List<list_item> nItems;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            carId = Intent.GetStringExtra("ID");
            SetContentView(Resource.Layout.carInfoLayout);
            GridLayout carModeGridLayout = FindViewById<GridLayout>(Resource.Id.gridLayout1);

            //carModeGridLayout.SetBackgroundColor(Color.Aqua);
            TextView carModelView = FindViewById<TextView>(Resource.Id.carInfoModel);
            TextView carProductionView = FindViewById<TextView>(Resource.Id.carInfoProduction);
            TextView carLicenseView = FindViewById<TextView>(Resource.Id.carInfoLicense);
            TextView carModeView = FindViewById<TextView>(Resource.Id.carInfoMode);
            TextView carUserView = FindViewById<TextView>(Resource.Id.carInfoUser);
            TextView carUserEmailView = FindViewById<TextView>(Resource.Id.carInfoUserEmail);

            requestCarButton = FindViewById<Button>(Resource.Id.requestCarButton);
            requestCarButton.Click += requestCar;


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
                imgAdress = totalInfo.img;
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
                        FindViewById<RelativeLayout>(Resource.Id.loadingPanel).Visibility = ViewStates.Gone;
                        imagen.LayoutParameters.Height = (int)((double)Resources.DisplayMetrics.WidthPixels * ratio);
                    }
                }
            }
            else
            {
                FindViewById<RelativeLayout>(Resource.Id.loadingPanel).Visibility = ViewStates.Gone;
            }

        }
        void requestCar(object sender, EventArgs eventArgs)
        {
            
            Intent openCar_try = new Intent(this, typeof(openCar));
            openCar_try.PutExtra("ID", carId);
            openCar_try.PutExtra("IMG", imgAdress);
            StartActivity(openCar_try);
            Finish();

          
        }
    }
}