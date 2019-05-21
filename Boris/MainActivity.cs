using System;
using System.Net;
using System.Threading;
using Android;
using Firebase.Messaging;

using Firebase.Iid;
using Android.Util;
using Android.App;
using Android.Content;
using Android.Gms.Common;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Provider;
using  supportFragment = Android.Support.V4.App.Fragment;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Boris.Resources;
using FCMClient;
using Xamarin.Android;
using Xamarin.Essentials;
namespace Boris
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener
    {
        internal static readonly string CHANNEL_ID = "my_notification_channel";
        internal static readonly int NOTIFICATION_ID = 100;
        static readonly string TAG = "MainActivity";

        private supportFragment mCurrrentFragment;
        private mainFragment mMainFragment;
        private myCars mMyCarsFragment;
        private History mHistoryFragment;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            IsPlayServicesAvailable();
            CreateNotificationChannel();

            Preferences.Set("isPending", false);
            Preferences.Set("isHandle", false);
            Preferences.Set("isWaiting", false);

            const string TAG = "MyFirebaseIIDService";
            var refreshedToken = FirebaseInstanceId.Instance.Token;
            Log.Debug(TAG, "Refreshed token: " + refreshedToken);

            string carID = "";
            
            //push handling

            if (Intent.Extras != null)
            {
                foreach (var key in Intent.Extras.KeySet())
                {
                    if (key == "vehicle_id")
                    {
                        carID = Intent.Extras.GetString(key);
                    }
                    if (key == "action")
                    {
                        var value = Intent.Extras.GetString(key);
                        int status=0;// 0 mean declined, 1 means approved
                        switch (Convert.ToInt32(value))
                        {
                            case 1:
                                Log.Debug(TAG, "Notification: Permit Request");
                                Preferences.Set("isHandle" ,true);
                                Preferences.Set("carId",carID);
                                break;
                            case 2:
                                Preferences.Set("isWaiting", true);
                                Preferences.Set("carId", carID);
                                Preferences.Set("responseStauts", status);
                                Log.Debug(TAG, "Notification: Permit Status Change");
                                break;
                            case 3:
                                Log.Debug(TAG, "Notification: Car Action");
                                break;
                            default:
                                Log.Debug(TAG, "Notification: Unknown");
                                break;
                        }
                    }
                }
            }

            //fragments init

            var trans = SupportFragmentManager.BeginTransaction();
            mMainFragment = new mainFragment();
            mMyCarsFragment = new myCars();
            mHistoryFragment = new History();
            mCurrrentFragment = mMainFragment; 
            trans.Add(Resource.Id.fragmentContainer, mHistoryFragment, "HistoryFragment");
            trans.Hide(mHistoryFragment);
            trans.Add(Resource.Id.fragmentContainer, mMyCarsFragment, "MyCarsFragment");
            trans.Hide(mMyCarsFragment);
            trans.Add(Resource.Id.fragmentContainer, mMainFragment, "Mainfragment");
            trans.Commit();

            //login try

            String login_hash = Preferences.Get("login_hash", "1");
            String user_id = Preferences.Get("user_id", "0");
            if (login_hash == "1")
            {
                Intent login_try = new Intent(this, typeof(loginActivity));
                StartActivity(login_try);
                Finish();
            }
            else
            {
                user user = new user();
                user.get_from_cloud(user_id, login_hash);
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

                //fab 

                FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
                fab.Click += FabOnClick;

                //navigation menu

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
        
        //handle push notification

        private void HandlePermitRequest(string carId)
        {
            TextView pending = FindViewById<TextView>(Resource.Id.pendingText);
            TextView waitingApproval = FindViewById<TextView>(Resource.Id.approvalText);
            Button details = FindViewById<Button>(Resource.Id.detailsButton);
            details.Click += handleClickAction;
            pending.Visibility = ViewStates.Invisible;
            details.Text = "details";
            details.Visibility = ViewStates.Visible;
            waitingApproval.Text = "Someone wants your car!";
            waitingApproval.Visibility = ViewStates.Visible;
        }

        private void handleClickAction(object sender, EventArgs e)
        {
            Intent handleTry = new Intent(this, typeof(handleReqActivity));
            handleTry.PutExtra("ID", "7029774");
            StartActivity(handleTry);
        }

        private void HandlePermitStatusChanged(string carId, int status)
        {
            if (status == 1){
                TextView pending = FindViewById<TextView>(Resource.Id.pendingText);
                TextView waitingApproval = FindViewById<TextView>(Resource.Id.approvalText);
                Button details = FindViewById<Button>(Resource.Id.detailsButton);
                pending.Visibility = ViewStates.Invisible;
                details.Text = "get the car!";
                details.Visibility = ViewStates.Visible;
                waitingApproval.Text = "Your Request was approved";
                waitingApproval.Visibility = ViewStates.Visible;
                Intent open_try = new Intent(this, typeof(openCar));
                open_try.PutExtra("ID", carId);
                StartActivity(open_try);
            }
            else{
                //declined
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

       //options menu

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
            Intent map_try = new Intent(this, typeof(mapActivity));
            StartActivity(map_try);
        }

        public bool OnNavigationItemSelected(IMenuItem item)
        {
            
            int id = item.ItemId;
            if (id == Resource.Id.nav_my_cars)
            {
              // showFragment(mMyCarsFragment);
               Intent login_try = new Intent(this, typeof(liveActivity));
                login_try.PutExtra("ID", "7029774");
                StartActivity(login_try);
            }
            if (id == Resource.Id.nav_home)
            {
                showFragment(mMainFragment);
            }
            else if (id == Resource.Id.nav_history)
            {
                showFragment(mHistoryFragment);
            }
            else if (id == Resource.Id.nav_logout)
            {
                Preferences.Clear();
                Intent login_try = new Intent(this, typeof(loginActivity));
                StartActivity(login_try);
                Finish();
            }
          
            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            drawer.CloseDrawer(GravityCompat.Start);
            return true;
        }
          
        //fragment mangament

        void showFragment(supportFragment fragment)
        {
            var trans = SupportFragmentManager.BeginTransaction();
            trans.Hide(mCurrrentFragment);
            trans.Show(fragment);
            trans.AddToBackStack(null);
            trans.Commit();
            mCurrrentFragment = fragment; 
        }

        // on resume main fragment
        protected override void OnResume()
        {
            base.OnResume();
            
            System.Console.WriteLine("resumed main activity");
            TextView pending = FindViewById<TextView>(Resource.Id.pendingText);
            TextView waitingApproval = FindViewById<TextView>(Resource.Id.approvalText);
            bool isPending = Preferences.Get("isPending", false);
            bool isHandle = Preferences.Get("isHandle", false);
            bool isWaiting = Preferences.Get("isWaiting", false);
            string vehicle = Preferences.Get("carId", "");
            string resStatus = Preferences.Get("responseStauts", "0");
            if (isHandle)
            {
                HandlePermitRequest(vehicle);
                return;
            }
            Log.Debug(TAG, "is pending create- " + isPending + ".");
            if (isWaiting)
            {
                HandlePermitStatusChanged(vehicle, resStatus);
                return;
            }
            if (isPending)
            {
                pending.Visibility = ViewStates.Invisible;
                waitingApproval.Visibility = ViewStates.Visible;
            }
            else
            {
                pending.Visibility = ViewStates.Visible;
                waitingApproval.Visibility = ViewStates.Invisible;
            }
        }

        //push notification setingns
        void CreateNotificationChannel()
        {
           
            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            {
                return;
            }

            var channel = new NotificationChannel(CHANNEL_ID,
                "FCM Notifications",
                NotificationImportance.Default)
            {
                Description = "Firebase Cloud Messages appear in this channel"
            };

            var notificationManager = (NotificationManager)GetSystemService(Android.Content.Context.NotificationService);
            notificationManager.CreateNotificationChannel(channel);
        }
        public bool IsPlayServicesAvailable()
        {
            int resultCode = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(this);
            return true;
        }
   
    }
}

