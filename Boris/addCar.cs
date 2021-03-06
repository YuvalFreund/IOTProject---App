﻿using System;
using System.Net;
using System.Threading;
using Android;
using Android.App;
using Android.Content;

using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Provider;
using supportFragment = Android.Support.V4.App.Fragment;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Boris.Resources;
using Xamarin.Android;
using Xamarin.Essentials;
using System.Net.Http;

namespace Boris
{
    [Activity(Label = "addCar")]
    public class addCar : Activity
    {
        Button submit;
        Spinner manufacturer;
        string make; 
        AutoCompleteTextView model;
        AutoCompleteTextView color;
        AutoCompleteTextView year;
        AutoCompleteTextView lisence;
        RadioGroup mode;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.addCarLayout);
            

            submit = FindViewById<Button>(Resource.Id.submitNewCar);
            lisence = FindViewById<AutoCompleteTextView>(Resource.Id.inputCarLP);
            manufacturer = FindViewById<Spinner>(Resource.Id.inputCarManufacturer);
            model = FindViewById<AutoCompleteTextView>(Resource.Id.inputCarModel);
            color = FindViewById<AutoCompleteTextView>(Resource.Id.inputCarColor);
            year = FindViewById<AutoCompleteTextView>(Resource.Id.inputCarProductionYear);
            mode= FindViewById<RadioGroup>(Resource.Id.radioMode);
            manufacturer.ItemSelected += setString;
            string[] models = Resources.GetStringArray(Resource.Array.carModels);
            var adapter = new ArrayAdapter<String>(this, Android.Resource.Layout.SimpleSpinnerItem, models);
            manufacturer.Adapter = adapter;

            submit.Click += submitAction;
            AutoCompleteTextView textCarColor = FindViewById<AutoCompleteTextView>(Resource.Id.inputCarColor);
            string[] colors = Resources.GetStringArray(Resource.Array.carColors);
            var adapter2 = new ArrayAdapter<String>(this, Resource.Layout.spinner_item, colors);
            textCarColor.Adapter = adapter2;
        }

        private void setString(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            make = manufacturer.GetItemAtPosition(e.Position).ToString();
        }

        private void submitAction(object sender, EventArgs e)
        {
            string mmode = "1";
            string mmodel = model.Text;
            string mmanufacturer = make;
            string mcolor = color.Text;
            RadioButton radioButton = FindViewById<RadioButton>(mode.CheckedRadioButtonId);
       
            string myear = year.Text;
            string mlisence = lisence.Text;
            string login_hash = Preferences.Get("login_hash", "");
            string user_id = Preferences.Get("user_id", "");
            String address = "https://carshareserver.azurewebsites.net/api/addCar?manufacturer=" + mmanufacturer + "&Mode=" + "1" + "&Lisence=" + mlisence + "&color=" + mcolor + "&model=" + mmodel + "&year=" + myear + "&login_hash=" + login_hash + "&user_id=" + user_id;
            Console.WriteLine(address);
            HttpClient client = new HttpClient();
            var responseString = client.GetStringAsync(address);
            Finish();
        }
    }
}