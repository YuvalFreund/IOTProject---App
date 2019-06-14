using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Support.V4.App;

using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Net.Http;
using Xamarin.Essentials;

namespace Boris
{
    public class ButtonAdapter : BaseAdapter<string>
    {
        string carid;
        public Fragment activity;
        public List<string> data;

        public ButtonAdapter(Fragment activity, List<string> data)
        {
            this.activity = activity;
            this.data = data;
        }

        public override string this[int position]
        {
            get { return this.data[position]; }
        }

        public override int Count
        {
            get { return this.data.Count; }
        }

        public override long GetItemId(int position)
        {
            return 0;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView;

            if (view == null)
            {
                view = this.activity.LayoutInflater.Inflate(Resource.Layout.listCarRow, null);
            }

            TextView lisence = view.FindViewById<TextView>(Resource.Id.myCarsCarNumber);
            lisence.Text = this.data[position];
            carid = data[position];
            Button details = view.FindViewById<Button>(Resource.Id.myCarsDetails);
            details.Tag = lisence.Text ;
            details.SetOnClickListener(new ButtonClickListener(this.activity));
            Switch toggle = view.FindViewById<Switch>(Resource.Id.myCarsSwitch);
            toggle.CheckedChange += Toggle_CheckedChange;

            return view;
        }

        private void Toggle_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            string login_hash = Preferences.Get("login_hash", "");
            string user_id = Preferences.Get("user_id", "");
            Console.WriteLine("pppppppppppppp");
            if (e.IsChecked)
            {
                String address = "https://carshareserver.azurewebsites.net/api/setVehicleStatus?user_id=" + user_id + "&vehicle_id=" + carid + "&status="+ "ACTIVATED" + "&login_hash=" + login_hash;
                HttpClient client = new HttpClient();
                var responseString = client.GetStringAsync(address);
                Console.WriteLine(address);
                Console.WriteLine(responseString);
            }
            else
            {
                String address = "https://carshareserver.azurewebsites.net/api/setVehicleStatus?user_id=" + user_id + "&vehicle_id=" + carid + "&status=" + "DEACTIVATED" + "&login_hash=" + login_hash;
                HttpClient client = new HttpClient();
                var responseString = client.GetStringAsync(address);
                Console.WriteLine(address);
                Console.WriteLine(responseString);
            }
        }

        private class ButtonClickListener : Java.Lang.Object, View.IOnClickListener
        {
            private Fragment activity;

            public ButtonClickListener(Fragment activity)
            {
                this.activity = activity;
            }

            public void OnClick(View v)
            {
                
                string name = (string)v.Tag;
                Intent car_try = new Intent(activity.Context, typeof(myCarsInfo));
                car_try.PutExtra("ID", name);
                activity.StartActivity(car_try);
            }
        }
    }
}