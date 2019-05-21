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
    [Activity(Label = "review")]
    public class review : Activity
    {
        Button submit;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.reviewLayout);
            submit = FindViewById<Button>(Resource.Id.submitReview);
            submit.Click += submitAction;
        }

        private void submitAction(object sender, EventArgs e)
        {
            Finish();
        }
    }
}