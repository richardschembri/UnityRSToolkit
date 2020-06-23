using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSToolkit.Animation
{
    public class CharacterAnimParams
    {
        public static readonly int Speed = Animator.StringToHash("Speed");
        public static readonly int IsGrounded = Animator.StringToHash("IsGrounded");
        public static readonly int HorizontalSpeed = Animator.StringToHash("HorizontalSpeed");
        public static readonly int VerticalSpeed = Animator.StringToHash("VerticalSpeed");

        public static void TrySetAxisSpeed(Animator target, float value, bool isHorizontal = true)
        {
            if (target != null)
            {
                target.SetFloat(isHorizontal ? HorizontalSpeed : VerticalSpeed, value);
            }
        }

        public static void TrySetSpeed(Animator target, float value)
        {
            if(target != null)
            {
                target.SetFloat(Speed, value);
            }
            
        }

        public static void TrySetIsGrounded(Animator target, bool value)
        {
            if (target != null)
            {
                target?.SetBool(IsGrounded, value);
            }
        }

    }
}

