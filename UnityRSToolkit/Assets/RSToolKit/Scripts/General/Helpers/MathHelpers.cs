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
    }
}