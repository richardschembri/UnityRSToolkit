using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSToolkit.Space3D.Helpers
{
    public static class AxisHelpers
    {
        public enum Axis
        {
            UP,
            DOWN,
            LEFT,
            RIGHT,
            FORWARD,
            BACK
        }

        public static Vector3 ToVector3(this Axis target)
        {
            switch (target)
            {
                case Axis.DOWN:
                    return Vector3.down;
                case Axis.FORWARD:
                    return Vector3.forward;
                case Axis.BACK:
                    return Vector3.back;
                case Axis.LEFT:
                    return Vector3.left;
                case Axis.RIGHT:
                    return Vector3.right;
            }

            // default is Vector3.up
            return Vector3.up;
        }
    }
}
