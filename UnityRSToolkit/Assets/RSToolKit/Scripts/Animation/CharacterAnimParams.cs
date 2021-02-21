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

            public void SetTrigger(Animator animatorComponent)
            {
                animatorComponent.SetTrigger(TriggerHash);
            }

            public void ResetTrigger(Animator animatorComponent)
            {
                animatorComponent.ResetTrigger(TriggerHash);
            }

            public bool IsInTransition(Animator animatorComponent)
            {
                return animatorComponent.GetAnimatorTransitionInfo(0).IsName(TriggerName);
            }

            public bool IsPlayingAnimation(Animator animatorComponent, string prefix = "")
            {
                return animatorComponent.GetCurrentAnimatorStateInfo(0).IsName(GetAnimName(prefix));
            }

            public bool IsPlayingAnimationOrIsInTransition(Animator animatorComponent, string prefix = "")
            {
                return IsPlayingAnimation(animatorComponent, prefix) || IsInTransition(animatorComponent);
            }

            public TriggerAnim(string triggerName)
            {
                TriggerName = triggerName;
                TriggerHash = Animator.StringToHash(TriggerName);
            }
        }

        public static readonly int Speed = Animator.StringToHash("Speed");
        public static readonly int SpeedPercent = Animator.StringToHash("SpeedPercent");
        public static readonly int IsGrounded = Animator.StringToHash("IsGrounded");
        public static readonly int HorizontalSpeed = Animator.StringToHash("HorizontalSpeed");
        public static readonly int VerticalSpeed = Animator.StringToHash("VerticalSpeed");
        public static readonly int FStateFlyable = Animator.StringToHash("FStateFlyable");

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

        public static void TrySetSpeedPercent(Animator target, float value)
        {
            if (target != null)
            {
                target.SetFloat(SpeedPercent, value);
            }

        }
        public static void TrySetIsGrounded(Animator target, bool value)
        {
            if (target != null)
            {
                target?.SetBool(IsGrounded, value);
            }
        }

        public static void TrySetFStateFlyable(Animator target, int value)
        {
            if (target != null)
            {
                target?.SetInteger(FStateFlyable, value);
            }
        }

    }
}

