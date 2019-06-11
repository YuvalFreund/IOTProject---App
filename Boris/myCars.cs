using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Content;
using Android.Support.V7.Widget;
using Android.Support.V4.App;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Xamarin.Essentials;

namespace Boris
{
    public class myCars : Fragment
    {

        public override Android.Views.View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.myCarsLayout, container, false);

            var addi = view.FindViewById<ImageButton>(Resource.Id.addCarButton);

            addi.Click += delegate
            {
                Intent try_add = new Intent(this.Context, typeof(addCar));
                StartActivity(try_add);
            };
            ListView listi = view.FindViewById<ListView>(Resource.Id.myCarsList);
            string user_id = Preferences.Get("user_id", "0");
            string login_hash = Preferences.Get("login_hash", "0");
            myCar_result getCars = new myCar_result();
            Console.WriteLine("started my cars");

            getCars.get_from_cloud("https://carshareserver.azurewebsites.net/api/myCars?login_hash=" + login_hash + "&user_id=" + user_id);

            List<myCar> allMyCars = getCars.getAllMyCars();
            List<string> cleanCars = new List<string>();
            foreach (var car in allMyCars)
            {
                cleanCars.Add(car.id);
            }
            ButtonAdapter adapter = new ButtonAdapter(this, cleanCars);
            listi.Adapter = adapter;
            return view;
        }
    }
}