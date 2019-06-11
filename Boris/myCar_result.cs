using System;
using System.Collections.Generic;
using System.Net.Http;

using Newtonsoft.Json;

struct myCar
{
    public string prod_year;
    public string id;
    public string manufacturer;
    public string model;
    public bool mode;
}
namespace Boris
{

    class myCar_result
    {
        public List<myCar> allMyCars;
        private static HttpClient client = new HttpClient();
        public void get_from_cloud(String address)
        {
            var responseString = client.GetStringAsync(address);
            Console.WriteLine(address);
            allMyCars = JsonConvert.DeserializeObject<List<myCar>>(responseString.Result);
        }

        public List<myCar> getAllMyCars()
        {
            return allMyCars;
        }
    }
}