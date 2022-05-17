namespace DrakersDev
{
    public static class TypeChecker
    {
        public static Boolean IsNumber(Type type)
        {
            return type == typeof(Byte) || type == typeof(Int16) ||
                type == typeof(Int32) || type == typeof(Int64) ||
                type == typeof(Single) || type == typeof(Double) ||
                type == typeof(Decimal);
        }
    }
}
