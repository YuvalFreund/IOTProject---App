using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using Xamarin.Essentials;
namespace Boris.Resources
{
    [Activity(Label = "loginActivity", MainLauncher = true)]
    public class loginActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.loginLayout);
            Button logi = FindViewById<Button>(Resource.Id.btn_login1);
            logi.Click += loginSend;
        }
        void loginSend(object sender, EventArgs eventArgs)
        {
            int status=0;
            login_result result = new login_result();
            String email = FindViewById<EditText>(Resource.Id.input_email).Text;
            String password = FindViewById<EditText>(Resource.Id.input_password).Text;
            String address = "https://carshareserver.azurewebsites.net/api/Login?email=" + email + "&password=" + password;
            result.get_from_cloud(address);
            status = result.status;
            if (status != 1)
            {
                Context context = Application.Context;
                string text = "Wrong Email or Password";
                ToastLength duration = ToastLength.Long;
                var toast = Toast.MakeText(context, text, duration);
                toast.Show();
                Intent login = new Intent(this, typeof(loginActivity));
            }
            else
            {
                Preferences.Set("login_hash", result.login_hash);
                Intent main = new Intent(this, typeof(MainActivity));
                StartActivity(main);
                Finish();
            }
        }
        string Get(string uri){ 
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}