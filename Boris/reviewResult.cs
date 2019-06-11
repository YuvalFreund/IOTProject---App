using System;
using System.Collections.Generic;
using System.Net.Http;


using Newtonsoft.Json;
 struct revData
{
    public double rate;
    public string content;
    public string reg_time;
}
struct all_res 
{
    public int status;
    public List<revData> reviews;
    public double avg_rate;
}
namespace Boris
{

    class review_result
    {
        all_res total_res;
        private static HttpClient client = new HttpClient();
        public void get_from_cloud(String address)
        {
            var responseString = client.GetStringAsync(address);
            total_res = JsonConvert.DeserializeObject<all_res>(responseString.Result);
        }
        public all_res getReviews()
        {
            return total_res;
        }
        public List<revData> GetRevs()
        {
            return total_res.reviews;
        }
    }
}