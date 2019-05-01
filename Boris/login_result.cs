using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;

namespace Boris
{
    class login_result
    {
        public int status;
        public string id;
        public string login_hash;
        public login_result () { }
        private static HttpClient client = new HttpClient();
        public login_result(int status, string id, string login_hash)
        {
            this.status = status;
            this.id = id;
            this.login_hash = login_hash;
            
        }
        public void get_from_cloud(String address)
        {
            var responseString = client.GetStringAsync(address);
            login_result response = JsonConvert.DeserializeObject<login_result>(responseString.Result);
            this.status = response.status;
            this.id = response.id;
            this.login_hash = response.login_hash;
        }
    }
}