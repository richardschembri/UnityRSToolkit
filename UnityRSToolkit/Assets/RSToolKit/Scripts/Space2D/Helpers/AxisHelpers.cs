using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSToolkit.Space2D.Helpers
{
    public static class AxisHelpers
    {
        public enum Axis
        {
            UP,
            DOWN,
            LEFT,
            RIGHT,
        }

        public static Vector2 ToVector2(this Axis target)
        {
            switch (target)
            {
                case Axis.DOWN:
                    return Vector2.down;
                case Axis.LEFT:
                    return Vector2.left;
                case Axis.RIGHT:
                    return Vector2.right;
            }

            // default is Vector2.up
            return Vector2.up;
        }
    }
}
