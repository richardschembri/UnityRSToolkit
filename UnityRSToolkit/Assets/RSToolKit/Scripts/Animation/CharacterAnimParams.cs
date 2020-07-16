using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSToolkit.Animation
{
    public static class CharacterAnimParams
    {
        public class TriggerAnim
        {
            public string TriggerName { get; private set; }
            public int TriggerHash { get; private set; }

            public string GetAnimName(string prefix = "")
            {
                return $"{prefix}{TriggerName}";
            }

            public void SetTrigger(Animator animatorComponent, string prefix = "")
            {
                animatorComponent.SetTrigger(TriggerHash);
            }

            public bool IsPlayingAnimation(Animator animatorComponent, string prefix = "")
            {
                return animatorComponent.GetCurrentAnimatorStateInfo(0).IsName(GetAnimName());
            }

            public TriggerAnim(string triggerName)
            {
                TriggerName = triggerName;
                TriggerHash = Animator.StringToHash(TriggerName);
            }
        }

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
            if (target != null)
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

