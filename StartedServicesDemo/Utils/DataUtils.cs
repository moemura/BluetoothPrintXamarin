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

using System.Globalization;

namespace StartedServicesDemo.Utils
{
    public class DataUtils
    {
        public static bool BytesEquals(byte[] d1, byte[] d2)
        {
            if (d1 == null && d2 == null)
                return true;
            else if (d1 == null || d2 == null)
                return false;

            if (d1.Length != d2.Length)
                return false;

            for (int i = 0; i < d1.Length; i++)
                if (d1[i] != d2[i])
                    return false;

            return true;
        }

        public static bool BytesEquals(byte[] d1, int offset1, byte[] d2,
                int offset2, int Length)
        {
            if (d1 == null || d2 == null)
                return false;

            if ((offset1 + Length > d1.Length) || (offset2 + Length > d2.Length))
                return false;

            for (int i = 0; i < Length; i++)
                if (d1[i + offset1] != d2[i + offset2])
                    return false;

            return true;
        }

        public static char[] BytesToChars(byte[] data)
        {
            char[] cdata = new char[data.Length];
            for (int i = 0; i < cdata.Length; i++)
                cdata[i] = (char)(data[i] & 0xff);
            return cdata;
        }

        public static byte[] GetRandomByteArray(int nLength)
        {
            byte[] data = new byte[nLength];
            var rmByte = new Java.Util.Random(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
            for (int i = 0; i < nLength; i++)
            {
                // 该方法的作用是生成一个随机的int值，该值介于[0,n)的区间，也就是0到n之间的随机int值，包含0而不包含n
                data[i] = (byte)rmByte.NextInt(256);
            }
            return data;
        }

        public static void BlackWhiteReverse(byte[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = (byte)~(data[i] & 0xff);
            }
        }

        public static byte[] GetSubBytes(byte[] org, int start, int Length)
        {
            byte[] ret = new byte[Length];
            for (int i = 0; i < Length; i++)
            {
                ret[i] = org[i + start];
            }
            return ret;
        }

        public static String BytesToStr(byte[] rcs)
        {
            if (null == rcs)
                return "";
            StringBuilder stringBuilder = new StringBuilder();
            String tmp;
            for (int i = 0; i < rcs.Length; i++)
            {
                tmp = (rcs[i] & 0xff).ToString("X");
                tmp = tmp.ToUpper(CultureInfo.CurrentCulture);
                if (tmp.Length == 1)
                {
                    stringBuilder.Append("0x0" + tmp);
                }
                else
                {
                    stringBuilder.Append("0x" + tmp);
                }

                if ((i % 16) != 15)
                {
                    stringBuilder.Append(" ");
                }
                else
                {
                    stringBuilder.Append("\n");
                }
            }
            return stringBuilder.ToString();
        }

        public static byte[] CloneBytes(byte[] data)
        {
            byte[] ret = new byte[data.Length];
            for (int i = 0; i < data.Length; i++)
                ret[i] = data[i];
            return ret;
        }

        public static byte BytesToXor(byte[] data, int start, int Length)
        {
            if (Length == 0)
                return 0;
            else if (Length == 1)
                return data[start];
            else
            {
                int result = data[start] ^ data[start + 1];
                for (int i = start + 2; i < start + Length; i++)
                    result ^= data[i];
                return (byte)result;
            }
        }

        /**
         * 将多个字节数组按顺序合并
         * 
         * @param data
         * @return
         */
        public static byte[] ByteArraysToBytes(byte[][] data)
        {

            int Length = 0;
            for (int i = 0; i < data.Length; i++)
                Length += data[i].Length;
            byte[] send = new byte[Length];
            int k = 0;
            for (int i = 0; i < data.Length; i++)
                for (int j = 0; j < data[i].Length; j++)
                    send[k++] = data[i][j];
            return send;
        }

        /**
         * 
         * @param orgdata
         * @param orgstart
         * @param desdata
         * @param desstart
         * @param copylen
         */
        public static void CopyBytes(byte[] orgdata, int orgstart, byte[] desdata,
                int desstart, int copylen)
        {
            for (int i = 0; i < copylen; i++)
            {
                desdata[desstart + i] = orgdata[orgstart + i];
            }
        }

        public static String BytesToStr(byte[] rcs, int offset, int count)
        {
            StringBuilder stringBuilder = new StringBuilder();
            String tmp;
            for (int i = 0; i < count; i++)
            {
                tmp = (rcs[i + offset] & 0xff).ToString("X");
                tmp = tmp.ToUpper(CultureInfo.CurrentCulture);
                if (tmp.Length == 1)
                {
                    stringBuilder.Append("0x0" + tmp);
                }
                else
                {
                    stringBuilder.Append("0x" + tmp);
                }

                if ((i % 16) != 15)
                {
                    stringBuilder.Append(" ");
                }
                else
                {
                    stringBuilder.Append("\r\n");
                }
            }
            return stringBuilder.ToString();
        }

        private static readonly byte[] chartobyte = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0,
            0, 0, 0, 0, 0, 0, 0xA, 0xB, 0xC, 0xD, 0xE, 0xF };
        private static readonly char[] bytetochar = { '0', '1', '2', '3', '4', '5',
            '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };

        /**
         * 必须保证传进来的ch1和ch2都是数字或者大写ABCDEF
         * 
         * @param ch
         * @param cl
         * @return
         */
        public static byte HexCharsToByte(char ch, char cl)
        {

            byte b = (byte)(((chartobyte[ch - '0'] << 4) & 0xF0) | ((chartobyte[cl - '0']) & 0xF));

            return b;
        }

        public static char[] ByteToHexChars(byte b)
        {
            char[] chs = { '0', '0' };
            chs[0] = bytetochar[(b >> 4) & 0xF];
            chs[1] = bytetochar[(b) & 0xF];
            return chs;
        }

        public static bool IsHexChar(char c)
        {
            if ((c >= '0' && c <= '9') || (c >= 'a' && c <= 'f')
                    || (c >= 'A' && c <= 'F'))
                return true;
            else
                return false;
        }

        public static byte[] HexStringToBytes(String str)
        {
            int count = str.Length;
            byte[] data = null;
            if (count % 2 == 0)
            {
                data = new byte[count / 2];

                for (int i = 0; i < count; i += 2)
                {
                    char ch = str[i];
                    char cl = str[i + 1];

                    if (IsHexChar(ch) && IsHexChar(cl))
                    {
                        if (ch >= 'a')
                            ch -= (char)0x20;
                        if (cl >= 'a')
                            cl -= (char)0x20;
                        data[i / 2] = HexCharsToByte(ch, cl);
                    }
                    else
                    {
                        data = null;
                        break;
                    }
                }
            }
            return data;
        }

        public static StringBuilder BytesToHexStr(byte[] data, int offset, int count)
        {
            StringBuilder str = new StringBuilder();
            for (int i = offset; i < offset + count; i++)
            {
                str.Append(ByteToHexChars(data[i]));
            }
            return str;
        }

        public static StringBuilder RemoveChar(String str, char c)
        {
            StringBuilder sb = new StringBuilder();
            int Length = str.Length;
            char tmp;
            for (int i = 0; i < Length; i++)
            {
                tmp = str[i];
                if (tmp != c)
                    sb.Append(tmp);
            }
            return sb;
        }

        public static string ByteToStr(byte rc)
        {
            String rec;
            String tmp = (rc & 0xff).ToString("X");
            tmp = tmp.ToUpper(CultureInfo.CurrentCulture);

            if (tmp.Length == 1)
            {
                rec = "0x0" + tmp;
            }
            else
            {
                rec = "0x" + tmp;
            }

            return rec;
        }

        /**
         * 获取byte的idx处的位 位和索引对应关系为 10001000 76543210
         * 
         * @param ch
         * @param idx
         * @return
         */
        public static int GetBit(byte ch, int idx)
        {
            return 0x1 & (ch >> idx);
        }

        public static void BytesArrayFill(byte[] data, int offset, int count,
                byte value)
        {
            for (int i = offset; i < offset + count; ++i)
                data[i] = value;

        }
    }
}