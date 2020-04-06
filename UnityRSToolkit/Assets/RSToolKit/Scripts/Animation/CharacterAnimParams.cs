using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSToolkit.Animation
{
    public class CharacterAnimParams
    {
        public static readonly int Speed = Animator.StringToHash("Speed");
        public static readonly int IsGrounded = Animator.StringToHash("IsGrounded");      
      
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

