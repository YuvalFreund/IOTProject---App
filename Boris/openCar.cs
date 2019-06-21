using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Bluetooth;
using Java.Util;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Boris
{
    [Activity(Label = "openCar")]
    public class openCar : Activity
    {
        private System.Timers.Timer timer;
        
        string carId;
        string IMG;
        string MAC_ADDRESS = "98:D3:71:F9:7A:48"; // arduino address
                                                  // string GUY_ADDRESS= "AC:5F:3E:B5:85:57"; // guy's address
                                                  // string JBL_ADDRESS= "78:44:05:e0:df:87"; // arduino address

        Button openCarButton;
        string valor;

        private BluetoothDeviceReceiver bluetoothDeviceReceiver;
        private BluetoothAdapter mBluetoothAdapter = null;
        private BluetoothSocket btSocket;
        private Stream outStream = null;
        private Stream inStream = null;
        private static UUID MY_UUID = UUID.FromString("00001101-0000-1000-8000-00805F9B34FB");
        private Java.Lang.String dataToSend;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            SetContentView(Resource.Layout.openCarLayout);
            base.OnCreate(savedInstanceState);
            carId = Intent.GetStringExtra("ID");

            carInfo_result info = new carInfo_result();
            info.get_from_cloud("https://carshareserver.azurewebsites.net/api/getCarDetails?vehicle_id=" + carId);
            carTotalInfo totalInfo = info.getInfo();
            IMG = totalInfo.img;

            CheckBt();
            bluetoothDeviceReceiver = new BluetoothDeviceReceiver(MAC_ADDRESS,this);
            RegisterReceiver(bluetoothDeviceReceiver, new IntentFilter(BluetoothDevice.ActionFound));

            openCarButton = FindViewById<Button>(Resource.Id.openCarButton);
            var gradientDrawable = openCarButton.Background.Current as GradientDrawable;
            gradientDrawable.SetColor(Color.Gray);
            openCarButton.Click += tryOpenCar;
            openCarButton.Enabled = false;

            TextView lisence = FindViewById<TextView>(Resource.Id.openCarLicense);
            lisence.Text = carId;
            async void GetImageBitmapFromUrl(string url, DownloadDataCompletedEventHandler eventFunc)
            {
                Bitmap img1 = null;
                using (var webClient = new System.Net.WebClient())
                {
                    webClient.DownloadDataCompleted += eventFunc;
                    webClient.DownloadDataAsync(new Uri(url));
                }
            }

            if (IMG != "")
            {

                GetImageBitmapFromUrl(IMG, loadCarImage);
                void loadCarImage(object sender, DownloadDataCompletedEventArgs e)
                {
                    byte[] raw = e.Result;
                    Bitmap img1 = BitmapFactory.DecodeByteArray(raw, 0, raw.Length);
                    if (img1 != null)
                    {
                        ImageView imagen = FindViewById<ImageView>(Resource.Id.openCarImg);
                        imagen.SetImageBitmap(img1);

                        //Calculate image size
                        double ratio = (double)img1.Height / (double)img1.Width;
                        FindViewById<RelativeLayout>(Resource.Id.openCarloadingPanel).Visibility = ViewStates.Gone;
                        imagen.LayoutParameters.Height = (int)((double)Resources.DisplayMetrics.WidthPixels * ratio);
                    }
                }
            }
            else
            {
                FindViewById<RelativeLayout>(Resource.Id.openCarloadingPanel).Visibility = ViewStates.Gone;
            }


            bool started = mBluetoothAdapter.StartDiscovery();
            System.Console.WriteLine("is scan startd: " + started);
            //Connect();
        }
   


        public void Connect()
        {
            BluetoothDevice device = mBluetoothAdapter.GetRemoteDevice(MAC_ADDRESS);
            System.Console.WriteLine("trying to connect to: " + device);
            mBluetoothAdapter.CancelDiscovery();
            try
            {
                btSocket = device.CreateRfcommSocketToServiceRecord(MY_UUID);
                btSocket.Connect();

                bool ivs = false;
                ivs = btSocket.IsConnected;
                System.Console.WriteLine("ivs is: " + ivs);
                if (ivs) {
                    System.Console.WriteLine("Connection success1");
                    var gradientDrawable = openCarButton.Background.Current as GradientDrawable;
                    gradientDrawable.SetColor(Color.ParseColor("#009688"));
                    System.Console.WriteLine("Connection success2");
                    openCarButton.Enabled = true;
                    System.Console.WriteLine("Connection success3");
                    openCarButton.Text = "unlock car";
                    System.Console.WriteLine("Connection success4");
                    FindViewById<RelativeLayout>(Resource.Id.openCarWaitPanel).Visibility = ViewStates.Gone;
                    // timer.Stop();
                }
            }
            catch (System.Exception e)
            {
                Console.WriteLine("exepction: " + e.Message);
                Console.WriteLine("didn't manage to connect");
                try
                {
                     btSocket.Close();
                }
                catch (System.Exception)
                {
                    System.Console.WriteLine("couldn't close socket");
                }
                System.Console.WriteLine("Socket close");
            }

        }
        public void beginListenForData()
        {
            //Extraemos el stream de entrada
            try
            {
                inStream = btSocket.InputStream;
            }
            catch (System.IO.IOException ex)
            {
                Console.WriteLine("message not recieved yet");
                Console.WriteLine(ex.Message);
            }
            // We create a thread that will be running in background which will check if there is any data
            // by the arduino
            Task.Factory.StartNew(() => {
                // declare the buffer where we will keep the reading
                byte[] buffer = new byte[1024];
                // declare the number of bytes received
                int bytes;
                while (true)
                {
                    try
                    {
                        System.Console.WriteLine("Begin listen for data");

                        bytes = inStream.Read(buffer, 0, buffer.Length);
                        if (bytes > 0)
                        {
                            System.Console.WriteLine("Got something");
                            RunOnUiThread(() => {
                                System.Console.WriteLine(buffer[0]);
                                System.Console.WriteLine(buffer[1]);
                                System.Console.WriteLine(buffer[2]);
                                if (buffer[0] == 50)
                                {
                                    Intent live_try = new Intent(this, typeof(liveActivity));
                                    live_try.PutExtra("carId", carId);
                                    Preferences.Set("isPending", false);
                                    Preferences.Set("isHandle", false);
                                    Preferences.Set("displaySettings", 0);
                                    StartActivity(live_try);
                                    Finish();
                                }
                                else
                                {
                                    Context context = Application.Context;
                                    string text = "Something went wrong.";
                                    ToastLength duration = ToastLength.Long;
                                    var toast = Toast.MakeText(context, text, duration);
                                    toast.Show();
                                    Preferences.Set("isPending", false);
                                    Preferences.Set("isHandle", false);
                                    Preferences.Set("displaySettings", 0);
                                    Finish();
                                }
                            });
                        }
                    }
                    catch (Java.IO.IOException)
                    {
                        System.Console.WriteLine("catched something");
                        RunOnUiThread(() => {
                        });
                        break;
                    }
                }
            });
        }
        private void writeData(Java.Lang.String data)
        {
            try
            {
                outStream = btSocket.OutputStream;
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine("Error al enviar" + e.Message);
            }

            Java.Lang.String message = data;
            byte[] msgBuffer = message.GetBytes();
            try
            {
                outStream.Write(msgBuffer, 0, msgBuffer.Length);
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine("Error al enviar" + e.Message);
            }
        }
        void tryOpenCar(object sender, EventArgs eventArgs)
        {
            string OTK = Preferences.Get("login_hash", "");
            string id = Preferences.Get("user_id", "");
            Console.WriteLine("button clicked");
            dataToSend = new Java.Lang.String("0"+id+OTK);
            writeData(dataToSend);
            beginListenForData();
        }
        private void CheckBt() 
        {
            mBluetoothAdapter = BluetoothAdapter.DefaultAdapter;
            if (!mBluetoothAdapter.Enable())
            {
                Toast.MakeText(this, "Bluetooth is not active",
                    ToastLength.Short).Show();
            }
            if (mBluetoothAdapter == null)
            {
                Toast.MakeText(this,
                    "Bluetooth No Existe o esta Ocupado", ToastLength.Short)
                    .Show();
            }
        }
    }
    class BluetoothDeviceReceiver : BroadcastReceiver
    {
        private string address;
        openCar activity;

        public BluetoothDeviceReceiver(string address, Activity activity) : base()
        {
            Console.WriteLine("Receiver created");

            this.activity = (openCar)activity;
            this.address = address;
        }
        public override void OnReceive(Context context, Intent intent)
        {
            string action = intent.Action;
            Console.WriteLine("Something happend");
            
            if (action == BluetoothDevice.ActionFound)
            {
                BluetoothDevice found = (BluetoothDevice)intent.GetParcelableExtra(BluetoothDevice.ExtraDevice);
                Console.WriteLine("Some device was found");

                if (found.Address == this.address)
                {
                    activity.Connect();
                }
            }
        }
    }
}

        
    

    
