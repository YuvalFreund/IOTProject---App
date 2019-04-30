using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Boris
{
    [Activity(Label = "signinActivity")]
    public class signinActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.signInLayout);
            Button signi = FindViewById<Button>(Resource.Id.signInButton);
            signi.Click += signInSend;
        }
        void signInSend(object sender, EventArgs eventArgs)
        {
            Finish();
        }
    }
}