using RSToolkit.Helpers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSToolkit.AI
{
    public class Bot : MonoBehaviour
    {

        private Animator m_animatorComponent;
        public Animator AnimatorComponent
        {
            get
            {
                if (m_animatorComponent == null)
                {
                    m_animatorComponent = GetComponent<Animator>();
                }
                return m_animatorComponent;
            }
        }

        public Transform FocusedOnTransform { get; private set; } = null;
        public Vector3? m_FocusedOnPosition = null;

        public List<Transform> NoticedTransforms { get; private set; } = new List<Transform>();
        public float forgetTransformTimeout = -1f;

        public Vector3? FocusedOnPosition
        {
            get
            {
                if (FocusedOnTransform != null)
                {
                    m_FocusedOnPosition = null;
                    return FocusedOnTransform.position;
                }
                return m_FocusedOnPosition;
            }
            private set
            {
                m_FocusedOnPosition = value;
            }
        }
        public bool IsFocused
        {
            get
            {
                return FocusedOnPosition != null;
            }
        }

        public float interactionMagnitude = 1.35f;
        public float SqrInteractionMagnitude
        {
            get
            {
                return interactionMagnitude * interactionMagnitude;
            }
        }

        public bool IsWithinInteractionDistance()
        {
            if (FocusedOnTransform != null)
            {
                return IsWithinInteractionDistance(FocusedOnTransform);
            }
            return false;

        }

        public bool IsWithinPersonalSpace()
        {
            if (FocusedOnTransform != null)
            {
                return IsWithinPersonalSpace(FocusedOnTransform);
            }
            return false;

        }

        public bool IsWithinInteractionDistance(Transform target)
        {
            return ProximityHelpers.IsWithinDistance(transform, target, SqrInteractionMagnitude);
        }
        public bool IsWithinPersonalSpace(Transform target)
        {
            return ProximityHelpers.IsWithinDistance(transform, target, SqrInteractionMagnitude * .75f);
        }


        public bool AttractMyAttention_ToTransform(Transform target)
        {

            if (IsWithinInteractionDistance(target))
            {
                FocusOnTransform(target);

                return true;
            }
            return false;

        }
        public bool AttractMyAttention_ToTransform()
        {
            return AttractMyAttention_ToTransform(FocusedOnTransform);

        }

        public bool AttractMyAttention_FromBot(Bot target)
        {
            return target.GetComponent<Bot>().AttractMyAttention_ToTransform(transform);

        }

        public bool AttractMyAttention_FromBot()
        {
            return AttractMyAttention_FromBot(FocusedOnTransform.GetComponent<Bot>());

        }

        public void FocusOnPosition(Vector3 target_position)
        {
            FocusedOnPosition = target_position;
        }

        public void NoticeTransform(Transform target)
        {
            if (!NoticedTransforms.Contains(target))
            {
                NoticedTransforms.Add(target);
            }

        }

        public void StartForgetTransform(Transform target)
        {
            if (forgetTransformTimeout > 0)
            {
                StartCoroutine(DelayedForgetTransform(target));

            }
            else if (forgetTransformTimeout < 0)
            {
                NoticeTransform(target);
            }
        }

        IEnumerator DelayedForgetTransform(Transform target)
        {
            yield return new WaitForSeconds(forgetTransformTimeout);
            ForgetTransform(target);

        }


        public void ForgetTransform(Transform target)
        {
            NoticedTransforms.Remove(target);
            if (FocusedOnTransform == target)
            {
                FocusedOnTransform = null;
            }
        }

        public void FocusOnTransform(Transform target)
        {
            NoticeTransform(target);
            FocusedOnTransform = target;
        }

        public void UnFocus()
        {
            StartForgetTransform(FocusedOnTransform);

        }


        public bool IsFacing(Transform target)
        {
            return transform.rotation == target.rotation;
        }

        public bool IsFacing()
        {
            if (FocusedOnTransform != null)
            {
                return IsFacing(FocusedOnTransform);
            }
            return false;
        }

        private void OnDrawGizmos()
        {
            ProximityHelpers.DrawGizmoProximity(transform, SqrInteractionMagnitude);

        }

    }
}