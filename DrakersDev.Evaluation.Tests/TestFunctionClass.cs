using System;
using System.Linq;

namespace DrakersDev.Evaluation.Tests
{
    internal static class TestFunctionClass
    {
        public static Double[] ReturnDoubleArray()
        {
            return Array.Empty<Double>();
        }

        public static void ReturnVoid()
        {
        }

        public static Object? ReturnObject()
        {
            return null;
        }

        public static Double[] ReturnDoubleArray(Double arg1, Double arg2)
        {
            return new Double[] { arg1, arg2 };
        }

        public static Double[] ArrayParamMethod(Double[] arg1, Int32 arg2)
        {
            return arg1.Select(a => a * arg2).ToArray();
        }

        public static Double[] SameName(Int32 arg1, Int32 arg2)
        {
            return new Double[] { arg1 + arg2 };
        }

        public static Double[] SameName(Double arg1, Int32 arg2)
        {
            return new Double[] { arg1 * arg2 };
        }
    }
}
