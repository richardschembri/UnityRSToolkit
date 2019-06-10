
namespace RSToolkit.Helpers
{
    using System.Collections.Generic;
    public static class PrimitiveHelpers 
    {
        public static bool isWithinRange(this int value, int min, int max){
            return value >= min && value <= max;
        }
        public static bool isWithinRange(this float value, float min, float max){
            return value >= min && value <= max;
        }

        public static int[] ToIntArray(this int value)
        {
            var result = new List<int>();
            while(value > 0)
            {
                result.Add(value % 10);
                value = value / 10;
            }
            result.Reverse();
            return result.ToArray();
        }

        public static uint[] ToUIntArray(this uint value)
        {
            var result = new List<uint>();
            while(value > 0)
            {
                result.Add(value % 10);
                value = value / 10;
            }
            result.Reverse();
            return result.ToArray();
        }
    }
}
