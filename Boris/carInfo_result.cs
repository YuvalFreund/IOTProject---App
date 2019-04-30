using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;

struct carTotalInfo
{
    public string lat;
    public string lng;
    public string id;
    public string year;
    public string Manufacturer;
    public string model;
    public bool mode;
    public string img;
    public user User;
}
namespace Boris
{

    class carInfo_result
    {
        public carTotalInfo info;
        private static HttpClient client = new HttpClient();
        public void get_from_cloud(String address)
        {
            var responseString = client.GetStringAsync(address);
            info = JsonConvert.DeserializeObject<carTotalInfo>(responseString.Result);
        }

        public carTotalInfo getInfo()
        {
            return info;
        }
    }
}