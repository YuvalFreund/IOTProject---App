using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using Android.App;
using Android.Content;
using Android.Gms.Common;
using Android.Gms.Location;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using ILocationListener = Android.Locations.ILocationListener;

namespace Boris
{
    [Activity(Label = "Active Drive")]
    public class liveActivity : AppCompatActivity, IOnMapReadyCallback , ILocationListener
    {
        private System.Timers.Timer timer;
        private System.Timers.Timer timer2;

        Location currentLocation;
        LocationManager locationManager;
        string locationProvider;
        private GoogleMap mMap;
        TextView totalTime;
        TextView totalCost;
        int totalTimeVal;
        double totalCostVal;
        Button finishDrive;
        public string TAG
        {
            get;
            private set;
        }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.liveLayout);
            finishDrive = FindViewById<Button>(Resource.Id.finishDriveButton);
            finishDrive.Click += finishAction;
            FragmentManager.FindFragmentById<MapFragment>(Resource.Id.liveMapContainer).GetMapAsync(this);
            InitializeLocationManager();
          //  currentLocation = locationManager.GetLastKnownLocation(locationProvider);
          //  Log.Debug(TAG, "new location: " + currentLocation.Latitude.ToString() + "," + currentLocation.Longitude.ToString());
            //sedLocationProviderClient = LocationServices.GetFusedLocationProviderClient(this);
            timer =  new Timer();
            timer2 = new Timer();
            timer.Interval = 1000;
            timer2.Interval = 5000;
            timer.Elapsed += OnTimedEvent;
            timer.Start();
            timer.Enabled = true;
            timer2.Elapsed += OnTimedEvent2;
           // timer2.Start();
           // timer2.Enabled = true;
            totalTime = FindViewById<TextView>(Resource.Id.totalTimeView);
            totalCost = FindViewById<TextView>(Resource.Id.totalCostView);
            totalTimeVal = 0;
            totalCostVal = 0;
            finishDrive = FindViewById<Button>(Resource.Id.finishDriveButton);
         }
        protected override void OnResume()
        {
            base.OnResume();
            locationManager.RequestLocationUpdates(locationProvider, 0, 0, this);
        }
        protected override void OnPause()
        {
            base.OnPause();
            locationManager.RemoveUpdates(this);
        }
        private void OnTimedEvent2(object sender, System.Timers.ElapsedEventArgs e)
        {
            //locationManager.RequestLocationUpdates(locationProvider, 0, 0, this);
        }

        private void finishAction(object sender, EventArgs e)
        {
            Intent review_try= new Intent(this, typeof(review));
            StartActivity(review_try);
            Finish();
        }

        private void OnTimedEvent(object sender, System.Timers.ElapsedEventArgs e)
        {
            totalTimeVal++;
            totalCostVal += 0.05;
            if (totalTimeVal < 600){
                string minuets = "0" + totalTimeVal / 60 + ":";
                string seconds = (totalTimeVal % 60).ToString();
                if (totalTimeVal % 60 < 10) {
                    seconds = "0" + totalTimeVal % 60;
                }
                totalTime.Text = minuets+seconds;
           }
            else{
                totalTime.Text = totalTimeVal / 60 + ":" + totalTimeVal % 60;
            }
            totalCost.Text = totalCostVal.ToString("0.00") + "₪";
       
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            mMap = googleMap;
            LatLng Israel = new LatLng(31.916312, 34.84811);
            CameraPosition.Builder builder = CameraPosition.InvokeBuilder();
            builder.Target(Israel);
            builder.Zoom(7);
            CameraPosition cameraPosition = builder.Build();
            CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);
            mMap.MoveCamera(cameraUpdate);
          //  LatLng latlng = new LatLng(currentLocation.Latitude, currentLocation.Longitude);
            //Log.Debug(TAG, "new location: " + currentLocation.Latitude.ToString() + "," + currentLocation.Longitude.ToString());
           // MarkerOptions mo = new MarkerOptions();
         //   mo.SetPosition(latlng);
         //   mMap.AddMarker(mo);

        }

        public void OnLocationChanged(Location location)
        {
            currentLocation = location;
            if (currentLocation == null)
            {
                Log.Debug(TAG, "location is null");
                return;
            }
            LatLng latlng = new LatLng(location.Latitude, location.Longitude);
            Log.Debug(TAG, "here location: " + location.Latitude.ToString() + "," + location.Longitude.ToString());
            MarkerOptions mo = new MarkerOptions();
            mo.SetPosition(latlng);
            mMap.AddMarker(mo);
            CameraPosition.Builder builder = CameraPosition.InvokeBuilder();
            builder.Target(latlng);
            CameraPosition camPos = builder.Build();
            CameraUpdate camUpdate = CameraUpdateFactory.NewCameraPosition(camPos);
            mMap.MoveCamera(camUpdate);
        }

        public void OnProviderDisabled(string provider)
        {
            Log.Debug(TAG, "prvider disabled");
            return;

        }

        public void OnProviderEnabled(string provider)
        {
            Log.Debug(TAG, "provider status enabled");

            return;
        }

        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
        {
            Log.Debug(TAG, "provider status changed");

            return;
        }
        private void InitializeLocationManager()
        {
            locationManager = (LocationManager)GetSystemService(LocationService);

            Criteria criteriaForLocationService = new Criteria
            {
                Accuracy = Accuracy.Fine
            };
            IList<string> acceptableLocationProviders = locationManager.GetProviders(criteriaForLocationService, true);
            if (acceptableLocationProviders.Count >= 1)
            {
                Log.Debug(TAG,"somethign found");

                locationProvider = acceptableLocationProviders[0];

            }
            else
            {
                locationProvider = string.Empty;
            }
            Log.Debug(TAG, "Using " + locationProvider + ".");
        }
    }
}