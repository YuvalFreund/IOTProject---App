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

struct hisStruct
{
    public string hisDate;
    public string hisCost;
    public string hisLisence;
}
struct allHistory
{
    public int status;
    public List<hisStruct> events;
}
namespace Boris
{
    class historyResult
    {
        allHistory total_history;
        private static HttpClient client = new HttpClient();
        public void get_from_cloud(string address)
        {
            var responseString = client.GetStringAsync(address);
            total_history = JsonConvert.DeserializeObject<allHistory>(responseString.Result);
        }
        public allHistory getHistory()
        {
            return total_history;
        }
    }
}