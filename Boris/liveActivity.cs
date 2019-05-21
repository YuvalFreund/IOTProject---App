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



namespace Boris
{
    [Activity(Label = "liveActivity")]
    public class liveActivity : AppCompatActivity, IOnMapReadyCallback 
    {
        private System.Timers.Timer timer;
        private int countSeconds;
        Location currentLocation;
        LocationManager locationManager;
        string locationProvider;
        private GoogleMap mMap;
        TextView totalTime;
        TextView totalCost;
        int totalTimeVal;
        double totalCostVal;
        FusedLocationProviderClient fusedLocationProviderClient;
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
           // Location location = locationManager.GetLastKnownLocation(locationProvider);
         //   Log.Debug(TAG, "new location: " + location.Latitude.ToString() + "," + location.Longitude.ToString());
            //sedLocationProviderClient = LocationServices.GetFusedLocationProviderClient(this);
            timer =  new Timer();
            timer.Interval = 1000;
            timer.Elapsed += OnTimedEvent;
            timer.Enabled = true;
            totalTime = FindViewById<TextView>(Resource.Id.totalTimeView);
            totalCost = FindViewById<TextView>(Resource.Id.totalCostView);
            totalTimeVal = 0;
            totalCostVal = 0;
            finishDrive = FindViewById<Button>(Resource.Id.finishDriveButton);
         }

        private void finishAction(object sender, EventArgs e)
        {
            Intent review_try= new Intent(this, typeof(review));
           // review_try.PutExtra("ID", "7029774");
            StartActivity(review_try);
            Finish();
        }

        private void OnTimedEvent(object sender, System.Timers.ElapsedEventArgs e)
        {
            totalTimeVal++;
            totalCostVal += 0.15;
            if (totalTimeVal < 600){
                string minuets = "0" + totalTimeVal / 60 + ":";
                string seconds = "0" + totalTimeVal % 60;
                totalTime.Text = "Total time:" + minuets+seconds;
           }
            else{
                totalTime.Text = totalTimeVal / 60  + ":" + totalTimeVal % 60;
            }
            totalCost.Text = "Total cost: " + totalCostVal;
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            mMap = googleMap;
            LatLng Israel = new LatLng(31.916312, 34.84811);
            CameraPosition.Builder builder = CameraPosition.InvokeBuilder();
            builder.Target(Israel);
            builder.Zoom(6);
            CameraPosition cameraPosition = builder.Build();
            CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);
            mMap.MoveCamera(cameraUpdate);
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
            Log.Debug(TAG, "new location: " + location.Latitude.ToString() + "," + location.Longitude.ToString());
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