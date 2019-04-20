using System;
using System.Net;
using System.Threading;
using Android;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Boris.Resources;
using Xamarin.Android;
using Xamarin.Essentials;
namespace Boris
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            String login_hash = Preferences.Get("login_hash", "1");
            if (login_hash == "1")
            {
                Intent login_try = new Intent(this, typeof(loginActivity));
                StartActivity(login_try);
                Finish();
            }
            else
            {
                user user = new user();
                user.get_from_cloud(2, login_hash);
                GetImageBitmapFromUrl("https://carshareserver.azurewebsites.net/api/getUserImage?user_id=" + user.id);
                void DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
                {
                    byte[] raw = e.Result;
                    Bitmap img1 = BitmapFactory.DecodeByteArray(raw, 0, raw.Length);
                    if (img1 != null)
                    {
                        ImageView imagen = FindViewById<ImageView>(Resource.Id.imageView1);
                        imagen.SetImageBitmap(img1);
                        TextView username = FindViewById<TextView>(Resource.Id.userName1);
                        TextView email = FindViewById<TextView>(Resource.Id.email1);
                        username.Text = user.first_name + " " + user.last_name;
                        email.Text = user.email;
                    }

                }


                FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
                fab.Click += FabOnClick;

                DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
                ActionBarDrawerToggle toggle = new ActionBarDrawerToggle(this, drawer, toolbar, Resource.String.navigation_drawer_open, Resource.String.navigation_drawer_close);
                drawer.AddDrawerListener(toggle);
                toggle.SyncState();

                NavigationView navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
                navigationView.SetNavigationItemSelectedListener(this);

                async void GetImageBitmapFromUrl(string url)
                {
                    Bitmap img1 = null;
                    using (var webClient = new WebClient())
                    {
                        webClient.DownloadDataCompleted += DownloadDataCompleted;
                        webClient.DownloadDataAsync(new Uri(url));
                    }
                }
            }
        }
        

        public override void OnBackPressed()
        {
            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            if(drawer.IsDrawerOpen(GravityCompat.Start))
            {
                drawer.CloseDrawer(GravityCompat.Start);
            }
            else
            {
                base.OnBackPressed();
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        private void FabOnClick(object sender, EventArgs eventArgs)
        {

        }


        public bool OnNavigationItemSelected(IMenuItem item)
        {
            int id = item.ItemId;

            if (id == Resource.Id.nav_my_cars)
            {

            }
            else if (id == Resource.Id.nav_history)
            {

            }
            else if (id == Resource.Id.nav_logout)
            {
                Preferences.Clear();
                Intent login_try = new Intent(this, typeof(loginActivity));
                StartActivity(login_try);
                Finish();
            }
            else if (id == Resource.Id.nav_else)
            {

            }

            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            drawer.CloseDrawer(GravityCompat.Start);
            return true;
        }
    }
}

