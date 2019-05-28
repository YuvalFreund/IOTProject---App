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
namespace Boris
{

    class review_result
    {
        public List<revData> all_reviews;
        private static HttpClient client = new HttpClient();
        public void get_from_cloud(String address)
        {
            var responseString = client.GetStringAsync(address);
            all_reviews = JsonConvert.DeserializeObject<List<revData>>(responseString.Result);
        }
        public List<revData> getReviews()
        {
            return all_reviews;
        }
    }
}