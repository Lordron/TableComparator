using System;
using System.IO;
using System.Globalization;
namespace TableComparator
{
    public static class Generic
    {
        public static uint ToUint32(this object val)
        {
            uint num;
            uint.TryParse(val.ToString(), out num);
            return num;
        }

        public static ushort ToUInt16(this object val)
        {
            ushort num;
            ushort.TryParse(val.ToString(), out num);
            return num;
        }

        public static ulong ToUInt64(this object val)
        {
            ulong num;
            ulong.TryParse(val.ToString(), out num);
            return num;
        }

        public static int ToInt32(this object val)
        {
            int num;
            int.TryParse(val.ToString(), out num);
            return num;
        }

        public static short ToInt16(this object val)
        {
            short num;
            short.TryParse(val.ToString(), out num);
            return num;
        }

        public static long ToInt64(this object val)
        {
            long num;
            long.TryParse(val.ToString(), out num);
            return num;
        }

        public static float ToFloat(this object val)
        {
            float num;
            float.TryParse(val.ToString(), out num);
            return num;
        }

        public static sbyte ToSByte(this object val)
        {
            sbyte num;
            sbyte.TryParse(val.ToString(), out num);
            return num;
        }

        public static byte ToByte(this object val)
        {
            byte num;
            byte.TryParse(val.ToString(), out num);
            return num;
        }

        public static void CrashReport(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        public static void Qu(this StreamWriter wri, object val, uint type)
        {
        }
    }
}
