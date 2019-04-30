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
    [Activity(Label = "getCar")]
    public class getCar : Activity
    {
        public TextView timerView;
        private System.Timers.Timer timer;
        private int countSeconds;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.getCarLayout);
            timerView = FindViewById<TextView>(Resource.Id.timerText);
            timer = new System.Timers.Timer();
            timer.Interval = 1000;
            timer.Elapsed += OnTimedEvent;
            countSeconds = 1800;
            timer.Enabled = true;
            timerView.Text = "30:00";
        }

        private void OnTimedEvent(object sender, System.Timers.ElapsedEventArgs e)
        {
            countSeconds--;
            timerView.Text = countSeconds / 60 + ":" + countSeconds % 60;
        }
    }
}