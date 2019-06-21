using System;
using System.Collections.Generic;

using Android.Support.V4.App;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Xamarin.Essentials;

namespace Boris
{
    public class History : Fragment
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            
            View view = inflater.Inflate(Resource.Layout.historyLayout, container, false);
            ListView listi = view.FindViewById<ListView>(Resource.Id.historyList);
            string user_id = Preferences.Get("user_id", "0");
            string login_hash = Preferences.Get("login_hash", "0");
            historyResult History = new historyResult();
            List<hisStruct> all = new List<hisStruct>();
            History.get_from_cloud("https://carshareserver.azurewebsites.net/api/getUserAllHistory?login_hash=" + login_hash + "&user_id=" + user_id);
            Console.WriteLine("https://carshareserver.azurewebsites.net/api/getUserAllHistory?login_hash=" + login_hash + "&user_id=" + user_id);
            all = History.getHistory().events;
            List<Tuple<string, string, string>> cleanHistory =new List<Tuple<string, string, string>>();
            if (all != null)
            {
                foreach (var row in all)
                {
                    cleanHistory.Add(Tuple.Create(row.hisDate, row.hisCost + "₪", row.hisLisence));
                    Console.WriteLine(row.hisDate + row.hisCost + "₪" + row.hisLisence);
                }
               // historyAdapter adapter = new historyAdapter(this, cleanHistory);
                //listi.Adapter = adapter;
            }
            return view;
        }
    }
}