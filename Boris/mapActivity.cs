using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Android.Util;

using Android.App;
using Android.Content;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Boris.Resources;
using Java.Util;

namespace Boris
{

    [Activity(Label = "mapActivity")]
    public class mapActivity : AppCompatActivity, IOnMapReadyCallback,  GoogleMap.IOnInfoWindowClickListener
    {

        public Dictionary<string,string> markers = new Dictionary<string, string>();
        public double lat;
        public double lng;
        public string id;
        public string manufacturer;
        public string model;
        public bool mode;
        NZ_result points = new NZ_result();
        public string TAG
        {
            get;
            private set;
        }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.mapLayout);
            FragmentManager.FindFragmentById<MapFragment>(Resource.Id.mapContainer).GetMapAsync(this);

        }


        public void OnMapReady(GoogleMap map)
        {
            LatLng Israel = new LatLng(31.916312,34.84811);
            CameraPosition.Builder builder = CameraPosition.InvokeBuilder();
            builder.Target(Israel);
            builder.Zoom(10);
            CameraPosition cameraPosition = builder.Build();
            CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);
            map.MoveCamera(cameraUpdate);
            points.get_from_cloud("https://carshareserver.azurewebsites.net/api/getAvailableCars");
            List<carNZ> coords = points.getCoords();
            int i = 0;
            foreach (var coord in coords)
            { 
                lat = System.Convert.ToDouble(coord.lat);
                lng = System.Convert.ToDouble(coord.lng);
                id = coord.id;
                model = coord.model;
                manufacturer = coord.Manufacturer;
                mode = coord.mode;
                MarkerOptions markerOpt1 = new MarkerOptions();
                markerOpt1.SetPosition(new LatLng(lat, lng));
                markerOpt1.SetTitle(manufacturer + " " + model);
                if (mode) { 
                    markerOpt1.SetSnippet("Automatic");
                }else{
                    markerOpt1.SetSnippet("Manual");
                }
                markers.Add(i.ToString(), id);
                map.AddMarker(markerOpt1);
                map.SetOnInfoWindowClickListener(this);
                i++;
            }
        }
        void MapOnMarkerClick(object sender, GoogleMap.MarkerClickEventArgs markerClickEventArgs)
        {
            markerClickEventArgs.Handled = true;

            var marker = markerClickEventArgs.Marker;
            
        }

        
        public void OnInfoWindowClick(Marker marker)
        {
            string carIdString = marker.Id;
            carIdString = carIdString.Substring(1);
            carIdString = markers[carIdString];
            Intent login_try = new Intent(this, typeof(carInfo));
            login_try.PutExtra("ID",carIdString);
            StartActivity(login_try);
        }
    }
}