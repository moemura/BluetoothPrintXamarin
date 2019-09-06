using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Hardware.Usb;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Util;
using Java.Lang;
using Com.Lvrenyang.IO;

namespace StartedServicesDemo.MyPrinter
{
    public class WorkThread : Thread
    {

        private static readonly System.String TAG = "WorkThread";
        public static Handler workHandler = null;
        private static Looper mLooper = null;
        public static Handler targetHandler = null;
        private static bool threadInitOK = false;
        private static bool isConnecting = false;
        private static BTPrinting bt = null;
        private static BLEPrinting ble = null;
        private static NETPrinting net = null;
        private static USBPrinting usb = null;
        private static Pos pos = new Pos();

        public WorkThread(Handler handler)
        {
            threadInitOK = false;
            targetHandler = handler;
            if (Build.VERSION.SdkInt >= BuildVersionCodes.HoneycombMr2)
            {
                if (usb == null)
                    usb = new USBPrinting();
            }
            if (Build.VERSION.SdkInt >= BuildVersionCodes.JellyBeanMr2)
            {
                if (ble == null)
                    ble = new BLEPrinting();
            }
            if (bt == null)
                bt = new BTPrinting();
            if (net == null)
                net = new NETPrinting();
        }

        public override void Start()
        {
            base.Start();
            while (!threadInitOK)
                ;
        }

        public override void Run()
        {
            Looper.Prepare();
            mLooper = Looper.MyLooper();
            if (null == mLooper)
                Log.Verbose(TAG, "mLooper is null pointer");
            else
                Log.Verbose(TAG, "mLooper is valid");
            workHandler = new WorkHandler();
            threadInitOK = true;
            Looper.Loop();
        }

        private class WorkHandler : Handler
        {
            public override void HandleMessage(Message msg)
            {

                switch (msg.What)
                {
                    case Global.MSG_WORKTHREAD_HANDLER_CONNECTBT:
                        {
                            isConnecting = true;

                            pos.Set(bt);

                            var BTAddress = (System.String)msg.Obj;
                            bool result = bt.Open(BTAddress);

                            Message smsg = targetHandler
                                    .ObtainMessage(Global.MSG_WORKTHREAD_SEND_CONNECTBTRESULT);
                            smsg.Arg1 = result ? 1 : 0;
                            targetHandler.SendMessage(smsg);

                            isConnecting = false;
                            break;
                        }

                    case Global.MSG_WORKTHREAD_HANDLER_CONNECTBLE:
                        {
                            isConnecting = true;

                            pos.Set(ble);

                            var BTAddress = (System.String)msg.Obj;
                            bool result = ble.Open(BTAddress);

                            Message smsg = targetHandler
                                    .ObtainMessage(Global.MSG_WORKTHREAD_SEND_CONNECTBLERESULT);
                            smsg.Arg1 = result ? 1 : 0;
                            targetHandler.SendMessage(smsg);

                            isConnecting = false;
                            break;
                        }

                    case Global.MSG_WORKTHREAD_HANDLER_CONNECTNET:
                        {
                            isConnecting = true;

                            pos.Set(net);

                            int PortNumber = msg.Arg1;
                            var IPAddress = (System.String)msg.Obj;
                            bool result = net.Open(IPAddress, PortNumber);

                            Message smsg = targetHandler
                                    .ObtainMessage(Global.MSG_WORKTHREAD_SEND_CONNECTNETRESULT);
                            smsg.Arg1 = result ? 1 : 0;
                            targetHandler.SendMessage(smsg);

                            isConnecting = false;
                            break;
                        }

                    case Global.MSG_WORKTHREAD_HANDLER_CONNECTUSB:
                        {
                            isConnecting = true;

                            pos.Set(usb);

                            UsbManager manager = (UsbManager)msg.Obj;
                            Bundle data = msg.Data;
                            UsbDevice device = (UsbDevice)data.GetParcelable(Global.PARCE1);

                            bool result = usb.Open(manager, device);
                            Message smsg = targetHandler
                                    .ObtainMessage(Global.MSG_WORKTHREAD_SEND_CONNECTUSBRESULT);
                            smsg.Arg1 = result ? 1 : 0;
                            targetHandler.SendMessage(smsg);

                            isConnecting = false;
                            break;
                        }

                    case Global.CMD_WRITE:
                        {
                            Bundle data = msg.Data;
                            var buffer = data.GetByteArray(Global.BYTESPARA1);
                            int offset = data.GetInt(Global.INTPARA1);
                            int count = data.GetInt(Global.INTPARA2);

                            Message smsg = targetHandler
                                    .ObtainMessage(Global.CMD_WRITERESULT);
                            if (pos.Io.Write(buffer, offset, count) == count)
                            {
                                smsg.Arg1 = 1;
                            }
                            else
                            {
                                smsg.Arg1 = 0;
                            }
                            targetHandler.SendMessage(smsg);

                            break;
                        }

                    // USB写入自带流控制，不需要读取状态。
                    case Global.CMD_POS_WRITE:
                        {
                            Bundle data = msg.Data;
                            byte[] buffer = data.GetByteArray(Global.BYTESPARA1);
                            int offset = data.GetInt(Global.INTPARA1);
                            int count = data.GetInt(Global.INTPARA2);

                            Message smsg = targetHandler
                                    .ObtainMessage(Global.CMD_POS_WRITERESULT);

                            bool result = false;

                            if (typeof(USBPrinting).Name.Equals(pos.Io.GetType().Name))
                                result = true;
                            else
                                result = pos.POS_QueryOnline(1000);

                            if (result)
                            {
                                if (pos.Io.Write(buffer, offset, count) == count)
                                    smsg.Arg1 = 1;
                                else
                                    smsg.Arg1 = 0;
                            }
                            else
                            {
                                smsg.Arg1 = 0;
                            }
                            targetHandler.SendMessage(smsg);

                            break;
                        }

                    case Global.CMD_POS_SETKEY:
                        {
                            Bundle data = msg.Data;
                            byte[] key = data.GetByteArray(Global.BYTESPARA1);

                            bool result = false;

                            if (typeof(USBPrinting).Name.Equals(pos.Io.GetType().Name))
                                result = true;
                            else
                                result = pos.POS_QueryOnline(1000);

                            Message smsg = targetHandler
                                    .ObtainMessage(Global.CMD_POS_SETKEYRESULT);

                            if (result)
                            {
                                pos.POS_SetKey(key);
                                smsg.Arg1 = 1;
                            }
                            else
                            {
                                smsg.Arg1 = 0;
                            }
                            targetHandler.SendMessage(smsg);

                            break;
                        }

                    case Global.CMD_POS_CHECKKEY:
                        {
                            Bundle data = msg.Data;
                            byte[] key = data.GetByteArray(Global.BYTESPARA1);

                            bool result = false;

                            if (typeof(USBPrinting).Name.Equals(pos.Io.GetType().Name))
                                result = true;
                            else
                                result = pos.POS_QueryOnline(1000);

                            Message smsg = targetHandler
                                    .ObtainMessage(Global.CMD_POS_CHECKKEYRESULT);
                            if (result)
                            {
                                if (pos.POS_CheckKey(key))
                                    smsg.Arg1 = 1;
                                else
                                    smsg.Arg1 = 0;
                            }
                            else
                            {
                                smsg.Arg1 = 0;
                            }
                            targetHandler.SendMessage(smsg);

                            break;
                        }
                    case Global.CMD_POS_PRINTPICTURE:
                        {
                            Bundle data = msg.Data;
                            Bitmap mBitmap = (Bitmap)data.GetParcelable(Global.PARCE1);
                            int nWidth = data.GetInt(Global.INTPARA1);
                            int nMode = data.GetInt(Global.INTPARA2);

                            bool result = false;

                            if (typeof(USBPrinting).Name.Equals(pos.Io.GetType().Name))
                                result = true;
                            else
                                result = pos.POS_QueryOnline(1000);

                            Message smsg = targetHandler
                                    .ObtainMessage(Global.CMD_POS_PRINTPICTURERESULT);
                            if (result)
                            {
                                pos.POS_PrintPicture(mBitmap, nWidth, nMode);

                                if (typeof(USBPrinting).Name.Equals(pos.Io.GetType().Name))
                                    result = true;
                                else
                                    result = pos.POS_QueryOnline(1000);

                                if (result)
                                    smsg.Arg1 = 1;
                                else
                                    smsg.Arg1 = 0;
                            }
                            else
                            {
                                smsg.Arg1 = 0;
                            }
                            targetHandler.SendMessage(smsg);

                            break;
                        }

                    case Global.CMD_POS_PRINTBWPICTURE:
                        {
                            Bundle data = msg.Data;
                            // Bitmap mBitmap = (Bitmap) data.get(Global.OBJECT1);
                            Bitmap mBitmap = (Bitmap)data.GetParcelable(Global.PARCE1);
                            int nWidth = data.GetInt(Global.INTPARA1);
                            int nMode = data.GetInt(Global.INTPARA2);

                            bool result = false;

                            if (typeof(USBPrinting).Name.Equals(pos.Io.GetType().Name))
                                result = true;
                            else
                                result = pos.POS_QueryOnline(1000);

                            Message smsg = targetHandler
                                    .ObtainMessage(Global.CMD_POS_PRINTPICTURERESULT);
                            if (result)
                            {
                                pos.POS_PrintBWPic(mBitmap, nWidth, nMode);

                                if (typeof(USBPrinting).Name.Equals(pos.Io.GetType().Name))
                                    result = true;
                                else
                                    result = pos.POS_QueryOnline(1000);

                                if (result)
                                    smsg.Arg1 = 1;
                                else
                                    smsg.Arg1 = 0;
                            }
                            else
                            {
                                smsg.Arg1 = 0;
                            }
                            targetHandler.SendMessage(smsg);

                            break;
                        }

                    case Global.CMD_POS_SALIGN:
                        {
                            Bundle data = msg.Data;
                            int align = data.GetInt(Global.INTPARA1);

                            bool result = false;

                            if (typeof(USBPrinting).Name.Equals(pos.Io.GetType().Name))
                                result = true;
                            else
                                result = pos.POS_QueryOnline(1000);

                            Message smsg = targetHandler
                                    .ObtainMessage(Global.CMD_POS_SALIGNRESULT);
                            if (result)
                            {
                                pos.POS_S_Align(align);
                                smsg.Arg1 = 1;
                            }
                            else
                            {
                                smsg.Arg1 = 0;
                            }
                            targetHandler.SendMessage(smsg);

                            break;
                        }

                    case Global.CMD_POS_SETLINEHEIGHT:
                        {
                            Bundle data = msg.Data;
                            int nHeight = data.GetInt(Global.INTPARA1);

                            bool result = false;

                            if (typeof(USBPrinting).Name.Equals(pos.Io.GetType().Name))
                                result = true;
                            else
                                result = pos.POS_QueryOnline(1000);

                            Message smsg = targetHandler
                                    .ObtainMessage(Global.CMD_POS_SETLINEHEIGHTRESULT);
                            if (result)
                            {
                                pos.POS_SetLineHeight(nHeight);
                                smsg.Arg1 = 1;
                            }
                            else
                            {
                                smsg.Arg1 = 0;
                            }
                            targetHandler.SendMessage(smsg);

                            break;
                        }

                    case Global.CMD_POS_SETRIGHTSPACE:
                        {
                            Bundle data = msg.Data;
                            int nDistance = data.GetInt(Global.INTPARA1);

                            bool result = false;

                            if (typeof(USBPrinting).Name.Equals(pos.Io.GetType().Name))
                                result = true;
                            else
                                result = pos.POS_QueryOnline(1000);

                            Message smsg = targetHandler
                                    .ObtainMessage(Global.CMD_POS_SETRIGHTSPACERESULT);
                            if (result)
                            {
                                pos.POS_SetRightSpacing(nDistance);
                                smsg.Arg1 = 1;
                            }
                            else
                            {
                                smsg.Arg1 = 0;
                            }
                            targetHandler.SendMessage(smsg);

                            break;
                        }

                    case Global.CMD_POS_STEXTOUT:
                        {
                            Bundle data = msg.Data;
                            var pszString = data.GetString(Global.STRPARA1);
                            var encoding = data.GetString(Global.STRPARA2);
                            int nOrgx = data.GetInt(Global.INTPARA1);
                            int nWidthTimes = data.GetInt(Global.INTPARA2);
                            int nHeightTimes = data.GetInt(Global.INTPARA3);
                            int nFontType = data.GetInt(Global.INTPARA4);
                            int nFontStyle = data.GetInt(Global.INTPARA5);

                            bool result = false;

                            if (typeof(USBPrinting).Name.Equals(pos.Io.GetType().Name))
                                result = true;
                            else
                                result = pos.POS_QueryOnline(1000);

                            Message smsg = targetHandler
                                    .ObtainMessage(Global.CMD_POS_STEXTOUTRESULT);
                            if (result)
                            {
                                pos.POS_S_TextOut(pszString, encoding, nOrgx, nWidthTimes,
                                        nHeightTimes, nFontType, nFontStyle);
                                smsg.Arg1 = 1;
                            }
                            else
                            {
                                smsg.Arg1 = 0;
                            }
                            targetHandler.SendMessage(smsg);

                            break;
                        }

                    case Global.CMD_POS_SETCHARSETANDCODEPAGE:
                        {
                            Bundle data = msg.Data;
                            int nCharSet = data.GetInt(Global.INTPARA1);
                            int nCodePage = data.GetInt(Global.INTPARA2);

                            bool result = false;

                            if (typeof(USBPrinting).Name.Equals(pos.Io.GetType().Name))
                                result = true;
                            else
                                result = pos.POS_QueryOnline(1000);

                            Message smsg = targetHandler
                                    .ObtainMessage(Global.CMD_POS_SETCHARSETANDCODEPAGERESULT);
                            if (result)
                            {
                                pos.POS_SetCharSetAndCodePage(nCharSet, nCodePage);
                                smsg.Arg1 = 1;
                            }
                            else
                            {
                                smsg.Arg1 = 0;
                            }
                            targetHandler.SendMessage(smsg);

                            break;
                        }

                    case Global.CMD_POS_SETBARCODE:
                        {
                            Bundle data = msg.Data;
                            var strBarcode = data.GetString(Global.STRPARA1);
                            int nOrgx = data.GetInt(Global.INTPARA1);
                            int nType = data.GetInt(Global.INTPARA2);
                            int nWidthX = data.GetInt(Global.INTPARA3);
                            int nHeight = data.GetInt(Global.INTPARA4);
                            int nHriFontType = data.GetInt(Global.INTPARA5);
                            int nHriFontPosition = data.GetInt(Global.INTPARA6);

                            bool result = false;

                            if (typeof(USBPrinting).Name.Equals(pos.Io.GetType().Name))
                                result = true;
                            else
                                result = pos.POS_QueryOnline(1000);

                            Message smsg = targetHandler
                                    .ObtainMessage(Global.CMD_POS_SETBARCODERESULT);
                            if (result)
                            {
                                pos.POS_S_SetBarcode(strBarcode, nOrgx, nType, nWidthX,
                                        nHeight, nHriFontType, nHriFontPosition);
                                smsg.Arg1 = 1;
                            }
                            else
                            {
                                smsg.Arg1 = 0;
                            }
                            targetHandler.SendMessage(smsg);

                            break;
                        }

                    case Global.CMD_POS_SETQRCODE:
                        {
                            Bundle data = msg.Data;
                            var strQrcode = data.GetString(Global.STRPARA1);
                            int nWidthX = data.GetInt(Global.INTPARA1);
                            int nVersion = data.GetInt(Global.INTPARA2);
                            int necl = data.GetInt(Global.INTPARA3);

                            bool result = false;

                            if (typeof(USBPrinting).Name.Equals(pos.Io.GetType().Name))
                                result = true;
                            else
                                result = pos.POS_QueryOnline(1000);

                            Message smsg = targetHandler
                                    .ObtainMessage(Global.CMD_POS_SETQRCODERESULT);
                            if (result)
                            {
                                pos.POS_S_SetQRcode(strQrcode, nWidthX, nVersion, necl);
                                smsg.Arg1 = 1;
                            }
                            else
                            {
                                smsg.Arg1 = 0;
                            }
                            targetHandler.SendMessage(smsg);

                            break;
                        }

                    case Global.CMD_EPSON_SETQRCODE:
                        {
                            Bundle data = msg.Data;
                            var strQrcode = data.GetString(Global.STRPARA1);
                            int nWidthX = data.GetInt(Global.INTPARA1);
                            int nVersion = data.GetInt(Global.INTPARA2);
                            int necl = data.GetInt(Global.INTPARA3);

                            bool result = false;

                            if (typeof(USBPrinting).Name.Equals(pos.Io.GetType().Name))
                                result = true;
                            else
                                result = pos.POS_QueryOnline(1000);

                            Message smsg = targetHandler
                                    .ObtainMessage(Global.CMD_EPSON_SETQRCODERESULT);
                            if (result)
                            {
                                pos.POS_EPSON_SetQRCode(strQrcode, nWidthX, nVersion, necl);
                                smsg.Arg1 = 1;
                            }
                            else
                            {
                                smsg.Arg1 = 0;
                            }
                            targetHandler.SendMessage(smsg);

                            break;
                        }

                    case Global.CMD_EMBEDDED_SEND_TO_UART:
                        {
                            Bundle data = msg.Data;
                            byte[] buffer = data.GetByteArray(Global.BYTESPARA1);
                            int offset = data.GetInt(Global.INTPARA1);
                            int count = data.GetInt(Global.INTPARA2);

                            Message smsg = targetHandler
                                .ObtainMessage(Global.CMD_EMBEDDED_SEND_TO_UART_RESULT);
                            if (pos.EMBEDDED_WriteToUart(buffer, offset, count))
                            {
                                smsg.Arg1 = 1;
                            }
                            else
                            {
                                smsg.Arg1 = 0;
                            }
                            targetHandler.SendMessage(smsg);

                            break;
                        }
                }

            }

        }

        public void Quit()
        {
            try
            {
                if (null != mLooper)
                {
                    mLooper.Quit();
                    mLooper = null;
                }
            }
            catch (Java.Lang.Exception e)
            {
                e.PrintStackTrace();
            }
        }

        public void DisconnectBt()
        {
            try
            {
                bt.Close();
            }
            catch (Java.Lang.Exception e)
            {
                e.PrintStackTrace();
            }
        }

        public void DisconnectBle()
        {
            try
            {
                ble.Close();
            }
            catch (Java.Lang.Exception e)
            {
                e.PrintStackTrace();
            }
        }

        public void DisconnectNet()
        {
            try
            {
                net.Close();
            }
            catch (Java.Lang.Exception e)
            {
                e.PrintStackTrace();
            }
        }

        public void DisconnectUsb()
        {
            try
            {
                usb.Close();
            }
            catch (Java.Lang.Exception e)
            {
                e.PrintStackTrace();
            }
        }

        public void ConnectBle(System.String BTAddress)
        {
            if ((null != workHandler) && (null != mLooper))
            {
                Message msg = workHandler
                        .ObtainMessage(Global.MSG_WORKTHREAD_HANDLER_CONNECTBLE);
                msg.Obj = BTAddress;
                workHandler.SendMessage(msg);
            }
            else
            {
                if (null == workHandler)
                    Log.Verbose(TAG, "workHandler is null pointer");

                if (null == mLooper)
                    Log.Verbose(TAG, "mLooper is null pointer");
            }
        }

        public void ConnectBt(System.String BTAddress)
        {
            if ((null != workHandler) && (null != mLooper))
            {
                Message msg = workHandler
                        .ObtainMessage(Global.MSG_WORKTHREAD_HANDLER_CONNECTBT);
                msg.Obj = BTAddress;
                workHandler.SendMessage(msg);
            }
            else
            {
                if (null == workHandler)
                    Log.Verbose(TAG, "workHandler is null pointer");

                if (null == mLooper)
                    Log.Verbose(TAG, "mLooper is null pointer");
            }
        }

        public void ConnectNet(System.String IPAddress, int PortNumber)
        {
            if ((null != workHandler) && (null != mLooper))
            {
                Message msg = workHandler
                        .ObtainMessage(Global.MSG_WORKTHREAD_HANDLER_CONNECTNET);
                msg.Arg1 = PortNumber;
                msg.Obj = IPAddress;
                workHandler.SendMessage(msg);
            }
            else
            {
                if (null == workHandler)
                    Log.Verbose("WorkThread connectNet", "workHandler is null pointer");

                if (null == mLooper)
                    Log.Verbose("WorkThread connectNet", "mLooper is null pointer");
            }
        }

        public void ConnectUsb(UsbManager manager, UsbDevice device)
        {
            if ((null != workHandler) && (null != mLooper))
            {
                Message msg = workHandler
                        .ObtainMessage(Global.MSG_WORKTHREAD_HANDLER_CONNECTUSB);
                msg.Obj = manager;
                Bundle data = new Bundle();
                data.PutParcelable(Global.PARCE1, device);
                msg.Data = data;
                workHandler.SendMessage(msg);
            }
            else
            {
                if (null == workHandler)
                    Log.Verbose("WorkThread connectUsb", "workHandler is null pointer");

                if (null == mLooper)
                    Log.Verbose("WorkThread connectUsb", "mLooper is null pointer");

            }
        }

        public bool IsConnecting()
        {
            return isConnecting;
        }

        public bool IsConnected()
        {
            if (bt.IsOpened || net.IsOpened || usb.IsOpened || ble.IsOpened)
                return true;
            else
                return false;
        }

        /**
         * 
         * @param cmd
         */
        public void HandleCmd(int cmd, Bundle data)
        {
            if ((null != workHandler) && (null != mLooper))
            {
                Message msg = workHandler.ObtainMessage(cmd);
                msg.Data = data;
                workHandler.SendMessage(msg);
            }
            else
            {
                if (null == workHandler)
                    Log.Verbose(TAG, "workHandler is null pointer");

                if (null == mLooper)
                    Log.Verbose(TAG, "mLooper is null pointer");
            }
        }
    }

}