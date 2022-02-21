namespace ToastForm.Extensions
{
    public static class FloatingPointExtensions
    {
        public static bool EqualsWithin(this float f, float compare, float epsilon = float.Epsilon)
        {
            return (f - compare <= epsilon);
        }
        public static bool EqualsWithin(this float f, double compare, double epsilon = double.Epsilon)
        {
            return (((double)f) - compare <= epsilon);
        }
        public static bool EqualsWithin(this double d, double compare, double epsilon = double.Epsilon)
        {
            return (d - compare <= epsilon);
        }
        public static bool EqualsWithin(this double d, float compare, double epsilon = double.Epsilon)
        {
            return (d - compare <= epsilon);
        }
    }
}
