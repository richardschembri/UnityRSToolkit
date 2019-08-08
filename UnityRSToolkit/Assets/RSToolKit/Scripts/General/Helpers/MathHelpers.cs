// Reference: https://wiki.unity3d.com/index.php?title=Mathfx

namespace RSToolkit.Helpers
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public sealed class MathHelpers 
    {

        /// <summary>
        /// Berp - Short for 'boing-like interpolation', this method will first overshoot,
        /// then wav er back and forth around the end value before coming to a rest. 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float Berp(float start, float end, float value)
        {
            value = Mathf.Clamp01(value);
            value = (Mathf.Sin(value * Mathf.PI * (0.2f + 2.5f * value * value * value)) * Mathf.Pow(1f - value, 2.2f) + value) * (1f + (1.2f * (1f - value)));
            return start + (end - start) * value;
        }

        /// <summary>
        /// EN: Returns the fraction of the value specified depending on the percentage. 
        /// Ex: Given the following parameters -> percent: 10% val: 100; The result will be will 10% of 100 which is 10. (100x0.1)
        /// JA: 与えた数に対して、パーセント計算を行い計算結果を返します。
        /// 例： percent: 10% val: 100を渡した場合は、結果は１００の１０％、それは１０です。(100x0.1)
        /// </summary>
        public static float GetPercentValue(float percent, float val)
        {
            return  val * (percent / 100);
        }

        /// <summary>
        /// EN: Returns the percentage of val in relation to maxVal.
        /// Ex: Given the following parameters -> val: 10 maxVal: 100; val is 10% of maxVal therefore the result will be 10%. 
        /// JA: 与えた二値のうち、片方を分子、もう片方を分母とし％に変換した結果を返します。
        /// 例： val: 10 maxVal: 100を渡した場合は、valはmaxValの１０％ですから、結果は１０％です。
        /// </summary>
        public static float GetValuePercent(float val, float maxVal)
        {
            return  (100 * val) / maxVal;
        }

        /// <summary>
        /// EN: Gets the Odd number from the sequence of odd numbers depending on the specified index.
        /// Xn = 2n+1
        /// JA: （与えた数＋１）番目の奇数を返します。
        /// Xn = 2n+1
        /// </summary>
        /// <param name="seqNumber">n</param>
        public static int GetOddNumber(int seqIndex)
        {
            if (seqIndex == 0)
            {
                return 0;
            }
            int oddNo = -1;
            for (int oddIndex = 0; oddIndex < seqIndex; oddIndex++)
            {
                oddNo += 2;
            }

            return oddNo;
        }
    }
}