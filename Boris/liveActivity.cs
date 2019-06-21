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
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Bluetooth;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Essentials;

using ILocationListener = Android.Locations.ILocationListener;

namespace Boris
{
    [Activity(Label = "Active Drive")]
    public class liveActivity : AppCompatActivity, IOnMapReadyCallback 
    {
        private System.Timers.Timer timer;
        private System.Timers.Timer timer2;
        string MAC_ADDRESS = "98:D3:71:F9:7A:48";
        private BluetoothDeviceReceiver bluetoothDeviceReceiver;
        private BluetoothAdapter mBluetoothAdapter = null;
        private BluetoothSocket btSocket;
        private Stream outStream = null;
        private Stream inStream = null;
        private static Java.Util.UUID MY_UUID = Java.Util.UUID.FromString("00001101-0000-1000-8000-00805F9B34FB");
        private Java.Lang.String dataToSend;

        Location currentLocation;
        //LocationManager locationManager;
        string locationProvider;
        private GoogleMap mMap;
        string carId;
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
            
            mBluetoothAdapter = BluetoothAdapter.DefaultAdapter;
            carId = Intent.GetStringExtra("carId");
            //InitializeLocationManager();
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
            BluetoothDevice device = mBluetoothAdapter.GetRemoteDevice(MAC_ADDRESS);
            btSocket = device.CreateRfcommSocketToServiceRecord(MY_UUID);
            dataToSend = new Java.Lang.String("1");
            writeData(dataToSend);
            beginListenForData();
        }
        protected override void OnResume()
        {
            base.OnResume();
            //locationManager.RequestLocationUpdates(locationProvider, 0, 0, this);
        }
        protected override void OnPause()
        {
            base.OnPause();
           // locationManager.RemoveUpdates(this);
        }
        private void OnTimedEvent2(object sender, System.Timers.ElapsedEventArgs e)
        {
            //locationManager.RequestLocationUpdates(locationProvider, 0, 0, this);
        }

        private void finishAction(object sender, EventArgs e)
        {
            Console.WriteLine("button clicked");
            dataToSend = new Java.Lang.String("1");
            writeData(dataToSend); Intent review_try= new Intent(this, typeof(review));
            review_try.PutExtra("cost", totalCost.Text);

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
        /*
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
        }*/

        public void beginListenForData()
        {
            //Extraemos el stream de entrada
            try
            {
                inStream = btSocket.InputStream;
            }
            catch (System.IO.IOException ex)
            {
                Console.WriteLine("message not recieved yet");
                Console.WriteLine(ex.Message);
            }
            // We create a thread that will be running in background which will check if there is any data
            // by the arduino
            Task.Factory.StartNew(() => {
                // declare the buffer where we will keep the reading
                byte[] buffer = new byte[1024];
                // declare the number of bytes received
                int bytes;
                while (true)
                {
                    try
                    {
                        System.Console.WriteLine("Begin listen for data");

                        bytes = inStream.Read(buffer, 0, buffer.Length);
                        if (bytes > 0)
                        {
                            System.Console.WriteLine("Got something");
                            RunOnUiThread(() => {
                                System.Console.WriteLine(buffer[0]);
                                System.Console.WriteLine(buffer[1]);
                                System.Console.WriteLine(buffer[2]);
                                if (buffer[0] == 50)
                                {
                                    Intent live_try = new Intent(this, typeof(liveActivity));
                                    live_try.PutExtra("carId", carId);
                                    Preferences.Set("isPending", false);
                                    Preferences.Set("isHandle", false);
                                    Preferences.Set("displaySettings", 0);
                                    StartActivity(live_try);
                                    Finish();
                                }
                                else
                                {
                                    Context context = Application.Context;
                                    string text = "Something went wrong.";
                                    ToastLength duration = ToastLength.Long;
                                    var toast = Toast.MakeText(context, text, duration);
                                    toast.Show();
                                    Preferences.Set("isPending", false);
                                    Preferences.Set("isHandle", false);
                                    Preferences.Set("displaySettings", 0);
                                    Finish();
                                }
                            });
                        }
                    }
                    catch (Java.IO.IOException)
                    {
                        System.Console.WriteLine("catched something");
                        RunOnUiThread(() => {
                        });
                        break;
                    }
                }
            });
        }
        private void writeData(Java.Lang.String data)
        {
            try
            {
                outStream = btSocket.OutputStream;
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine("Error al enviar" + e.Message);
            }

            Java.Lang.String message = data;
            byte[] msgBuffer = message.GetBytes();
            try
            {
                outStream.Write(msgBuffer, 0, msgBuffer.Length);
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine("Error al enviar" + e.Message);
            }
        }
        void tryOpenCar(object sender, EventArgs eventArgs)
        {
            //openCarButton.Enabled = false;
            Console.WriteLine("button clicked");
            dataToSend = new Java.Lang.String("0");
            writeData(dataToSend);
            beginListenForData();
        }
    }
}