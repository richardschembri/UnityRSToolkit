
namespace RSToolkit.Helpers
{
    public static class PrimitiveHelpers 
    {
        public static bool isWithinRange(this int value, int min, int max){
            return value >= min && value <= max;
        }
        public static bool isWithinRange(this float value, float min, float max){
            return value >= min && value <= max;
        }
    }
}
