namespace RSToolkit.Helpers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    /// <summary>
    /// EN: Helper class used for random value functions
    /// JA: 乱数系ヘルパークラス
    /// </summary>
    public static class RandomHelpers{

        private static System.Random _globalRandom = new System.Random();  
        private static readonly object Lock = new object();

        /// <summary>
        /// EN: Shuffle the specified list.
        /// JA: 渡したリストの内容をシャッフルします。
        /// </summary>
        public static void Shuffle<T>(this IList<T> list)  
        {  
            int n = list.Count;  
            while (n > 1) {  
                n--;  
                int k = _globalRandom.Next(n + 1);  
                T value = list[k];  
                list[k] = list[n];  
                list[n] = value;  
            }  
        }

        public static List<int> GetShuffledRange(int start, int count){
           var result = Enumerable.Range(start, count).ToList(); 
           result.Shuffle();
           return result;
        }

        /// <summary>
        /// EN:
        /// Supply a % value and it will effect the likelyhood of the function to return a true value(otherwise it will return false).
        /// For example: If [5] is passed there is a 5% chance the function will return true.
        /// JA:
        /// 当たる確率を渡して、確率に応じた結果を返します。
        /// 例：「５」を渡したら、５％の確率でTRUEを返します。
        /// </summary>
        public static bool PercentTrue(int percentage){
            var randomVal = _globalRandom.Next(99) + 1;
            if (percentage >= randomVal){
                return true;
            }
                return false;
        }

        public static int RandomInt(System.Random random, int MaxVal)
        {
            return random.Next(MaxVal);
        }

        public static int RandomInt(int MaxVal)
        {
            return RandomInt(_globalRandom,MaxVal);
        }


        public static int RandomIntWithinRange(int MinVal, int MaxVal){
            return _globalRandom.Next(MinVal, MaxVal);
        }
        
        public static double RandomDoubleWithinRange(double MinVal, double MaxVal, int digits = 2){
            return System.Math.Round(_globalRandom.NextDouble() * (MaxVal - MinVal) + MinVal, digits);
        }

        public static float RandomFloatWithinRange(float MinVal, float MaxVal, int digits = 2){
            return (float)RandomDoubleWithinRange(MinVal ,MinVal, digits);
        }

        public static bool RandomBool(){
            return _globalRandom.Next(2) == 1;
        }

        public static string GetRandomHexNumber(int digits)
        {
            byte[] buffer = new byte[digits / 2];
            _globalRandom.NextBytes(buffer);
            string result = String.Concat(buffer.Select(x => x.ToString("X2")).ToArray());
            if (digits % 2 == 0)
                return result;
            return result + _globalRandom.Next(16).ToString("X");
        }

        public static Vector3 GetRandomPositionWithinCircle(this Vector3 self, float radius, float offset = 0f, float y = 0f)
        {
            Vector2 direction = UnityEngine.Random.insideUnitCircle.normalized;
            var distance = UnityEngine.Random.Range(offset, radius);
            var result = self + (new Vector3(direction.x, 0f, direction.y) * distance);
            if(y != 0f)
            {
                result = new Vector3(result.x, y, result.z);
            }
            return result;
        }

        public static T GetRandomEnumValue<T>()
        {
            var v = Enum.GetValues(typeof(T));
            return (T)v.GetValue(_globalRandom.Next(v.Length));
        }

        /// <summary>
        /// Create a new <c cref="Random">Random</c> instance with a random seed that is safe to
        /// use alongside others created with this same method as thread-local instances. The
        /// naive process of generating <c cref="Random">Random</c> instances back-to-back (often
        /// in a loop) without this safety can generate instances with a sufficiently similar or
        /// even the equal seeds such that querying them in parallel produces the same values.
        /// </summary>
        public static System.Random CreateForThread()
        {
            lock (Lock)
            {
                return new System.Random(_globalRandom.Next());
            }
        }

        /// <summary>
        /// Return a random element from the List.
        /// </summary>
        /// <typeparam name="T">The type of elements in the List.</typeparam>
        /// <param name="self">The calling List.</param>
        public static T RandomListItem<T>(this List<T> self, System.Random random)
        {
            return self[RandomInt(random, self.Count)];
        }

    }
}