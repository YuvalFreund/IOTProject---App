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

struct carNZ
{
    public string lat;
    public string lng;
    public string id;
    public string Manufacturer;
    public string model;
    public bool mode;
}
namespace Boris
{

    class NZ_result
    {
        public List<carNZ> coordiantes;
        private static HttpClient client = new HttpClient();
        public void get_from_cloud(String address)
        {
            var responseString = client.GetStringAsync(address);
            coordiantes = JsonConvert.DeserializeObject<List<carNZ>>(responseString.Result);
        }

        public List<carNZ> getCoords()
        {
            return coordiantes;
        }
    }
}