using System;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Support;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using System.Net.Http;
using System.Json;
using Newtonsoft.Json;
using Xamarin.Essentials;


public class user
{
    public int id;
    public string first_name;
    public string last_name;
    public string email;
    public string licence_number;
    public string profile_image;
    public user() { }
    private static HttpClient client = new HttpClient();
    public void get_from_cloud(int id, string login_hash)
    {
        var responseString = client.GetStringAsync("https://carshareserver.azurewebsites.net/api/getUserDetails?user_id=" + id.ToString() + "&login_hash=" + login_hash);
        user response = JsonConvert.DeserializeObject<user>(responseString.Result);
        this.id = response.id;

        this.first_name = response.first_name;
        System.Diagnostics.Debug.WriteLine(response.first_name);
        this.last_name = response.last_name;
        this.email = response.email;
        this.licence_number = response.licence_number;
        this.profile_image = response.profile_image;

    }
    public user(int id, string first_name, string last_name, string email, string licence_number, string profile_image)
    {
        this.id = id;
        this.first_name = first_name;
        this.last_name = last_name;
        this.email = email;
        this.licence_number = licence_number;
        this.profile_image = profile_image;
    }
}