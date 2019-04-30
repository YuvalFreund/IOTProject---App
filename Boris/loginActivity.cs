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

using Firebase.Iid;
using Android.Util;
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
            Button signi = FindViewById<Button>(Resource.Id.link_signup);
            signi.Click += openSignIn;
        }
        void loginSend(object sender, EventArgs eventArgs)
        {
            int status=0;
            var refreshedToken = FirebaseInstanceId.Instance.Token;
            login_result result = new login_result();
            String email = FindViewById<EditText>(Resource.Id.input_email).Text;
            String password = FindViewById<EditText>(Resource.Id.input_password).Text;
            String address = "https://carshareserver.azurewebsites.net/api/Login?email=" + email + "&password=" +password + "&token=" + refreshedToken;
            result.get_from_cloud(address);
            status = result.status;
            if (status != 1)
            {
                Context context = Application.Context;
                string text = "Wrong Email or Password";
                ToastLength duration = ToastLength.Long;
                var toast = Toast.MakeText(context, text, duration);
                toast.Show();
            }
            else
            {
                Preferences.Set("login_hash", result.login_hash);
                Intent main = new Intent(this, typeof(MainActivity));
                StartActivity(main);
                Finish();
            }
        }

        void openSignIn(object sender, EventArgs eventArgs)
        {
            Intent signIntry = new Intent(this, typeof(signinActivity));
            StartActivity(signIntry);
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