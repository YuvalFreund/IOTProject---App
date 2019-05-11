using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Gms.Maps;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;

namespace Boris
{
    [Activity(Label = "liveActivity")]
    public class liveActivitypublic : AppCompatActivity, IOnMapReadyCallback
    {
        public void OnMapReady(GoogleMap googleMap)
        {
            throw new NotImplementedException();
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.liveLayout);
            FragmentManager.FindFragmentById<MapFragment>(Resource.Id.liveMapContainer).GetMapAsync(this);



        }
    }
}