using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Content;
using Android.Support.V4.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Xamarin.Essentials;

namespace Boris
{
    public class mainFragment : Fragment
    {
        private View root;
        public string TAG
        {
            get;
            private set;
        }
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            
            // Create your fragment here
        }
        

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {

            root = inflater.Inflate(Resource.Layout.content_main, container, false);
            TextView pending = root.FindViewById<TextView>(Resource.Id.pendingText);
            TextView waitingApproval = root.FindViewById<TextView>(Resource.Id.approvalText);
            bool isPending = Preferences.Get("isPending", false);
            Log.Debug(TAG, "is pending create- " + isPending + ".");
            var addi = root.FindViewById<Button>(Resource.Id.detailsButton);
            // get my cars
            addi.Click += delegate
            {
                Intent try_open = new Intent(this.Context, typeof(openCar));
                StartActivity(try_open);
            };
            if (isPending)
            {
                pending.Visibility = ViewStates.Invisible;
                waitingApproval.Visibility = ViewStates.Visible;
            }
            else
            {
                pending.Visibility = ViewStates.Visible;
                waitingApproval.Visibility = ViewStates.Invisible;
            }
            return root;
        }
    }
}