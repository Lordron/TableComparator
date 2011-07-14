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

        public static void WriteQuery(this StreamWriter writer, string field, object fieldValue, uint creatureId)
        {
            writer.WriteLine(string.Format(NumberFormatInfo.InvariantInfo, "UPDATE `{0}` SET `{1}` = '{2}' WHERE {3} = {4};", TableName, field, fieldValue, FieldName, creatureId)); writer.Flush();
            ++BadQuery;
        }

        public static void WriteQuery(this StreamWriter writer, string fieldA, string fieldB, object fieldsValue, uint creatureId)
        {
            writer.WriteLine(string.Format(NumberFormatInfo.InvariantInfo, "UPDATE `{0}` SET `{1}` = '{2}', `{3}` = '{2}' WHERE {4} = {5};", TableName, fieldA, fieldsValue, fieldB, FieldName, creatureId)); writer.Flush();
            ++BadQuery;
        }

        public static uint BadQuery { get; set; }

        public static string TableName { get; set; }

        public static string FieldName { get; set; }
    }
}
