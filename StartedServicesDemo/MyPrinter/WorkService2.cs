using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Util;

namespace StartedServicesDemo.MyPrinter
{
    [Service]
    public class WorkService2 : Service
    {
        static readonly string TAG = typeof(WorkService).FullName;

        // Service和workThread通信用mHandler
        public static WorkThread workThread = null;
        private static Handler mHandler = null;
        private static List<Handler> targetsHandler = new List<Handler>(5);

        public override IBinder OnBind(Intent arg0)
        {
            // TODO Auto-generated method stub
            return null;
        }

        public override void OnCreate()
        {
            Toast.MakeText(this, "Service Create", ToastLength.Long).Show();
            base.OnCreate();
            mHandler = new MHandler(this);
            workThread = new WorkThread(mHandler);
            workThread.Start();
            Log.Verbose("WorkService", "onCreate");
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            Toast.MakeText(this, $"{TAG} Start", ToastLength.Long).Show();
            Log.Verbose("WorkService", "onStartCommand");
            Message msg = Message.Obtain();
            msg.What = Global.MSG_ALLTHREAD_READY;
            NotifyHandlers(msg);
            return StartCommandResult.Sticky;
        }

        public override void OnDestroy()
        {
            workThread.DisconnectBt();
            workThread.DisconnectBle();
            workThread.DisconnectNet();
            workThread.DisconnectUsb();
            workThread.Quit();
            workThread = null;
            Log.Verbose("DrawerService", "onDestroy");
        }

        private class MHandler : Handler
        {

            WeakReference<WorkService2> mService;

            public MHandler(WorkService2 service)
            {
                mService = new WeakReference<WorkService2>(service);
            }

            public override void HandleMessage(Message msg)
            {
                NotifyHandlers(msg);
            }
        }

        /**
         * 
         * @param handler
         */
        public static void AddHandler(Handler handler)
        {
            if (!targetsHandler.Contains(handler))
            {
                targetsHandler.Add(handler);
            }
        }

        /**
         * 
         * @param handler
         */
        public static void DelHandler(Handler handler)
        {
            if (targetsHandler.Contains(handler))
            {
                targetsHandler.Remove(handler);
            }
        }

        /**
         * 
         * @param msg
         */
        public static void NotifyHandlers(Message msg)
        {
            for (int i = 0; i < targetsHandler.Count(); i++)
            {
                Message message = Message.Obtain(msg);
                targetsHandler[i].SendMessage(message);
            }
        }

    }

}