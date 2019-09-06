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

namespace StartedServicesDemo.MyPrinter
{
    public class Global
    {
        public const String PREFERENCES_FILENAME = "com.lvrenyang.drawer.PREFERENCES_FILENAME";

        public const String PREFERENCES_IPADDRESS = "com.lvrenyang.drawer.PREFERENCES_IPADDRESS";
        public const String PREFERENCES_PORTNUMBER = "com.lvrenyang.drawer.PREFERENCES_PORTNUMBER";
        public const String PREFERENCES_BTADDRESS = "com.lvrenyang.drawer.PREFERENCES_BTADDRESS";

        public const int MSG_WORKTHREAD_HANDLER_CONNECTNET = 100000;
        public const int MSG_WORKTHREAD_SEND_CONNECTNETRESULT = 100001;
        public const int MSG_WORKTHREAD_HANDLER_OPENDRAWERNET = 100002;
        public const int MSG_WORKTHREAD_SEND_OPENDRAWERNETRESULT = 100003;
        public const int MSG_WORKTHREAD_HANDLER_CONNECTBT = 100004;
        public const int MSG_WORKTHREAD_SEND_CONNECTBTRESULT = 100005;
        public const int MSG_WORKTHREAD_HANDLER_OPENDRAWERBT = 100006;
        public const int MSG_WORKTHREAD_SEND_OPENDRAWERBTRESULT = 100007;
        public const int MSG_WORKTHREAD_HANDLER_STRINGINFOBT = 100008;
        public const int MSG_WORKTHREAD_SEND_STRINGINFOBTRESULT = 100009;
        public const int MSG_WORKTHREAD_HANDLER_STRINGINFONET = 100010;
        public const int MSG_WORKTHREAD_SEND_STRINGINFONETRESULT = 100011;
        public const int MSG_WORKTHREAD_HANDLER_SETKEYBT = 100012;
        public const int MSG_WORKTHREAD_SEND_SETKEYBTRESULT = 100013;
        public const int MSG_WORKTHREAD_HANDLER_SETKEYNET = 100014;
        public const int MSG_WORKTHREAD_SEND_SETKEYNETRESULT = 100015;
        public const int MSG_WORKTHREAD_HANDLER_SETBTPARABT = 100016;
        public const int MSG_WORKTHREAD_SEND_SETBTPARABTRESULT = 100017;
        public const int MSG_WORKTHREAD_HANDLER_SETBTPARANET = 100018;
        public const int MSG_WORKTHREAD_SEND_SETBTPARANETRESULT = 100019;
        public const int MSG_WORKTHREAD_HANDLER_SETIPPARABT = 100020;
        public const int MSG_WORKTHREAD_SEND_SETIPPARABTRESULT = 100021;
        public const int MSG_WORKTHREAD_HANDLER_SETIPPARANET = 100022;
        public const int MSG_WORKTHREAD_SEND_SETIPPARANETRESULT = 100023;
        public const int MSG_WORKTHREAD_HANDLER_SETWIFIPARABT = 100024;
        public const int MSG_WORKTHREAD_SEND_SETWIFIPARABTRESULT = 100025;
        public const int MSG_WORKTHREAD_HANDLER_SETWIFIPARANET = 100026;
        public const int MSG_WORKTHREAD_SEND_SETWIFIPARANETRESULT = 100027;
        public const int MSG_WORKTHREAD_HANDLER_CONNECTUSB = 100028;
        public const int MSG_WORKTHREAD_SEND_CONNECTUSBRESULT = 100029;
        public const int MSG_WORKTHREAD_HANDLER_CONNECTBLE = 100030;
        public const int MSG_WORKTHREAD_SEND_CONNECTBLERESULT = 100031;

        // Bundle data使用
        public const String BYTESPARA1 = "bytespara1";
        public const String BYTESPARA2 = "bytespara2";
        public const String BYTESPARA3 = "bytespara3";
        public const String BYTESPARA4 = "bytespara4";
        public const String INTPARA1 = "intpara1";
        public const String INTPARA2 = "intpara2";
        public const String INTPARA3 = "intpara3";
        public const String INTPARA4 = "intpara4";
        public const String INTPARA5 = "intpara5";
        public const String INTPARA6 = "intpara6";
        public const String STRPARA1 = "strpara1";
        public const String STRPARA2 = "strpara2";
        public const String STRPARA3 = "strpara3";
        public const String STRPARA4 = "strpara4";
        public const String OBJECT1 = "object1";
        public const String OBJECT2 = "object2";
        public const String OBJECT3 = "object3";
        public const String OBJECT4 = "object4";
        public const String PARCE1 = "parce1";
        public const String PARCE2 = "parce2";
        public const String SERIAL1 = "serial1";
        public const String SERIAL2 = "serial2";

        public const int CMD_POS_WRITE = 100100;
        public const int CMD_POS_WRITERESULT = 100101;
        public const int CMD_POS_READ = 100102;
        public const int CMD_POS_READRESULT = 100103;
        public const int CMD_POS_SETKEY = 100104;
        public const int CMD_POS_SETKEYRESULT = 100105;
        public const int CMD_POS_CHECKKEY = 100106;
        public const int CMD_POS_CHECKKEYRESULT = 100107;
        public const int CMD_POS_PRINTPICTURE = 100108;
        public const int CMD_POS_PRINTPICTURERESULT = 100109;
        public const int CMD_POS_STEXTOUT = 100110;
        public const int CMD_POS_STEXTOUTRESULT = 100111;
        public const int CMD_POS_SALIGN = 100112;
        public const int CMD_POS_SALIGNRESULT = 100113;
        public const int CMD_POS_SETLINEHEIGHT = 100114;
        public const int CMD_POS_SETLINEHEIGHTRESULT = 100115;
        public const int CMD_POS_SETRIGHTSPACE = 100116;
        public const int CMD_POS_SETRIGHTSPACERESULT = 100117;
        public const int CMD_POS_SETCHARSETANDCODEPAGE = 100118;
        public const int CMD_POS_SETCHARSETANDCODEPAGERESULT = 100119;
        public const int CMD_POS_SETBARCODE = 100120;
        public const int CMD_POS_SETBARCODERESULT = 100121;
        public const int CMD_POS_SETQRCODE = 100122;
        public const int CMD_POS_SETQRCODERESULT = 100123;
        public const int CMD_EPSON_SETQRCODE = 100123;
        public const int CMD_EPSON_SETQRCODERESULT = 100124;
        //public const  int CMD_POS_SETQRCODEV2 = 100125;
        //public const  int CMD_POS_SETQRCODEV2RESULT = 100126;
        //public const  int CMD_EPSON_SETQRCODEV2 = 100127;
        //public const  int CMD_EPSON_SETQRCODEV2RESULT = 100128;
        public const int MSG_ALLTHREAD_READY = 100300;
        public const int MSG_PAUSE_HEARTBEAT = 100301;
        public const int MSG_RESUME_HEARTBEAT = 100302;
        public const int MSG_ON_RECIVE = 100303;
        public const int CMD_WRITE = 100304;
        public const int CMD_WRITERESULT = 100305;
        public const int CMD_POS_PRINTBWPICTURE = 100306;
        public const int CMD_POS_WRITE_BT_FLOWCONTROL = 100307; // 使用蓝牙流控
        public const int CMD_POS_WRITE_BT_FLOWCONTROL_RESULT = 100308;
        public const int CMD_UPDATE_PROGRAM = 100309;
        public const int CMD_UPDATE_PROGRAM_RESULT = 100310;
        public const int CMD_UPDATE_PROGRAM_PROGRESS = 100311;
        public const int CMD_EMBEDDED_SEND_TO_UART = 100312;
        public const int CMD_EMBEDDED_SEND_TO_UART_RESULT = 100313;
        public const int CMD_PORTABLE_SETBTPARA = 100314;
        public const int CMD_PORTABLE_SETBTPARA_RESULT = 100315;

        public static String toast_success = "Done";
        public static String toast_fail = "Fail";
        public static String toast_notconnect = "Please connect printer";
        public static String toast_usbpermit = "Please permit app use usb and reclick this button";
        public static String toast_connecting = "Connecting";
    }
}
