using System;
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
namespace Boris
{
    [Activity(Label = "addCar")]
    public class addCar : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.addCarLayout);
  
            AutoCompleteTextView textCarModel = FindViewById<AutoCompleteTextView>(Resource.Id.inputCarModel);
            string[] models = Resources.GetStringArray(Resource.Array.carModels);
            var adapter = new ArrayAdapter<String>(this, Android.Resource.Layout.SimpleDropDownItem1Line, models);
            textCarModel.Adapter = adapter;

            AutoCompleteTextView textCarColor = FindViewById<AutoCompleteTextView>(Resource.Id.inputCarColor);
            string[] colors = Resources.GetStringArray(Resource.Array.carColors);
            var adapter2 = new ArrayAdapter<String>(this, Android.Resource.Layout.SimpleDropDownItem1Line, colors);
            textCarColor.Adapter = adapter2;
        }
    }
}