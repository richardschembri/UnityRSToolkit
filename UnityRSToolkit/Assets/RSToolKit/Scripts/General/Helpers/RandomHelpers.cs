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

        private static System.Random rnd = new System.Random();  

        /// <summary>
        /// EN: Shuffle the specified list.
        /// JA: 渡したリストの内容をシャッフルします。
        /// </summary>
        public static void Shuffle<T>(this IList<T> list)  
        {  
            int n = list.Count;  
            while (n > 1) {  
                n--;  
                int k = rnd.Next(n + 1);  
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
            var randomVal = rnd.Next(99) + 1;
            if (percentage >= randomVal){
                return true;
            }
                return false;
        }

        public static int RandomInt(int MaxVal)
        {
            return rnd.Next(MaxVal);
        }

        public static int RandomIntWithinRange(int MinVal, int MaxVal){
            return rnd.Next(MinVal, MaxVal);
        }
        
        public static double RandomDoubleWithinRange(double MinVal, double MaxVal, int digits = 2){
            return System.Math.Round(rnd.NextDouble() * (MaxVal - MinVal) + MinVal, digits);
        }

        public static float RandomFloatWithinRange(float MinVal, float MaxVal, int digits = 2){
            return (float)RandomDoubleWithinRange(MinVal ,MinVal, digits);
        }

        public static bool RandomBool(){
            return rnd.Next(2) == 1;
        }

        public static string GetRandomHexNumber(int digits)
        {
            byte[] buffer = new byte[digits / 2];
            rnd.NextBytes(buffer);
            string result = String.Concat(buffer.Select(x => x.ToString("X2")).ToArray());
            if (digits % 2 == 0)
                return result;
            return result + rnd.Next(16).ToString("X");
        }

        public static Vector3 GetRandomPositionWithinCircle(this Transform self, float radius, float offset = 0f)
        {
            Vector2 randPos = UnityEngine.Random.insideUnitCircle * (radius - offset);
            float new_x = randPos.x + (randPos.x > 0 ? offset : -offset);
            float new_z = randPos.y + (randPos.y > 0 ? offset : -offset);
            return self.position + new Vector3(new_x, self.position.y, new_z);
        }
    }
}