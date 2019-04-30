using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Content;
using Android.Support.V7.Widget;
using Android.Support.V4.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace Boris
{
    public class myCars : Fragment
    {
        public override Android.Views.View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.myCarsLayout, container, false);

            var addi = view.FindViewById<ImageButton>(Resource.Id.addCarButton);
            // get my cars
            addi.Click += delegate
            {
                Intent try_add = new Intent(this.Context, typeof(addCar));
                StartActivity(try_add);
            };

            return view;
        }
    }
}