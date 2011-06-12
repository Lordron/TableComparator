using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TableComparator
{
    public static class Generic
    {
        public static uint ToUint(this object val)
        {
            uint num;
            uint.TryParse(val.ToString(), out num);
            return num;
        }

        public static UInt16 ToUint16(this object val)
        {
            UInt16 num;
            UInt16.TryParse(val.ToString(), out num);
            return num;
        }

        public static int ToInt(this object val)
        {
            int num;
            int.TryParse(val.ToString(), out num);
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
    }

}
