using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.OS;
using Android.Widget;
using Android.Util;
using Android.Runtime;
using StartedServicesDemo.MyPrinter;
using StartedServicesDemo.Utils;
using System;

namespace StartedServicesDemo
{
    [Activity(Label = "@string/app_name", MainLauncher = true, Icon = "@mipmap/icon")]
    public class MainActivity : Activity
    {
        static readonly string TAG = typeof(MainActivity).FullName;
        static readonly string SERVICE_STARTED_KEY = "has_service_been_started";

        Button stopServiceButton;
        Button startServiceButton;
        Intent serviceToStart;
        bool isStarted = false;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Main);

            if (savedInstanceState != null)
            {
                isStarted = savedInstanceState.GetBoolean(SERVICE_STARTED_KEY, false);
            }

            serviceToStart = new Intent(this, typeof(WorkService));

            stopServiceButton = FindViewById<Button>(Resource.Id.stop_timestamp_service_button);
            startServiceButton = FindViewById<Button>(Resource.Id.start_timestamp_service_button);

            if (isStarted)
            {
                stopServiceButton.Click += StopServiceButton_Click;
                stopServiceButton.Enabled = true;
                startServiceButton.Enabled = false;
            }
            else
            {
                startServiceButton.Click += StartServiceButton_Click;
                startServiceButton.Enabled = true;
                stopServiceButton.Enabled = false;
            }

            var listView1 = FindViewById<ListView>(Resource.Id.listView1);
            listView1.ItemClick += (object sender, AdapterView.ItemClickEventArgs e) =>
            {
                var dt = listView1.GetItemAtPosition(e.Position).ToString().Split("|");
                Printer.Name = dt[0].Trim();
                Printer.Mac = dt[1].Trim();

                if (null == WorkService.workThread)
                {
                    StartService(new Intent(this, typeof(WorkService)));
                }

                if (!WorkService.workThread.IsConnected())
                {
                    WorkService.workThread.ConnectBt(Printer.Mac);
                    //Sleep for 3 seconds
                    try
                    {
                        Java.Lang.Thread.Sleep(3000);
                    }
                    catch (Exception)
                    {
                    }
                }

                if (WorkService.workThread.IsConnected())
                {
                    int nTextAlign = 1;
                    String text = "Test message!\r\n\r\n\r\n";
                    String encoding = "UTF-8";
                    byte[] hdrBytes = { 0x1c, 0x26, 0x1b, 0x39, 0x01 };

                    Bundle dataAlign = new Bundle();
                    Bundle dataTextOut = new Bundle();
                    Bundle dataHdr = new Bundle();

                    dataHdr.PutByteArray(Global.BYTESPARA1, hdrBytes);
                    dataHdr.PutInt(Global.INTPARA1, 0);
                    dataHdr.PutInt(Global.INTPARA2, hdrBytes.Length);

                    dataAlign.PutInt(Global.INTPARA1, nTextAlign);

                    dataTextOut.PutString(Global.STRPARA1, text);
                    dataTextOut.PutString(Global.STRPARA2, encoding);

                    WorkService.workThread.HandleCmd(Global.CMD_POS_WRITE, dataHdr);
                    WorkService.workThread.HandleCmd(Global.CMD_POS_SALIGN, dataAlign);
                    WorkService.workThread.HandleCmd(Global.CMD_POS_STEXTOUT, dataTextOut);
                }
                else
                {
                    Toast.MakeText(this, Global.toast_notconnect, ToastLength.Short).Show();
                }
            };
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            outState.PutBoolean(SERVICE_STARTED_KEY, isStarted);
            base.OnSaveInstanceState(outState);
        }

        protected override void OnDestroy()
        {
            Log.Info(TAG, "Activity is being destroyed; stop the service.");

            StopService(serviceToStart);
            base.OnDestroy();
        }
        void StopServiceButton_Click(object sender, System.EventArgs e)
        {
            stopServiceButton.Click -= StopServiceButton_Click;
            stopServiceButton.Enabled = false;

            Log.Info(TAG, "User requested that the service be stopped.");
            StopService(serviceToStart);
            isStarted = false;

            startServiceButton.Click += StartServiceButton_Click;
            startServiceButton.Enabled = true;
        }

        void StartServiceButton_Click(object sender, System.EventArgs e)
        {
            startServiceButton.Enabled = false;
            startServiceButton.Click -= StartServiceButton_Click;

            StartService(serviceToStart);
            Log.Info(TAG, "User requested that the service be started.");

            isStarted = true;
            stopServiceButton.Click += StopServiceButton_Click;

            stopServiceButton.Enabled = true;

            InitListView();
        }

        private void InitListView()
        {
            //open list of paired BT devices
            var listView1 = FindViewById<ListView>(Resource.Id.listView1);
            var bondedDevices = BluetoothAdapter.DefaultAdapter.BondedDevices;
            var aa = new JavaList<string>();
            foreach (var item in bondedDevices)
            {
                aa.Add(item.Name + " | " + item.Address);
            }
            var adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, aa);
            listView1.Adapter = adapter;
        }
	}
}

