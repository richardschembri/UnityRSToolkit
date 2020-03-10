using RSToolkit.Helpers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSToolkit.AI
{
    public class Bot : MonoBehaviour
    {
        public bool DebugMode = false;
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

        public HashSet<Transform> NoticedTransforms { get; private set; } = new HashSet<Transform>();
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

        public bool IsWithinInteractionDistance(Vector3 position)
        {
            return ProximityHelpers.IsWithinDistance(transform, position, SqrInteractionMagnitude);
        }
        public bool IsWithinPersonalSpace(Vector3 position)
        {
            return ProximityHelpers.IsWithinDistance(transform, position, SqrInteractionMagnitude * .75f);
        }

        public bool IsWithinInteractionDistance(Transform target)
        {
            return IsWithinInteractionDistance(transform.position);
        }

        public bool IsWithinPersonalSpace(Transform target)
        {
            return IsWithinPersonalSpace(transform.position);
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
            if(target.FocusedOnTransform != transform)
            {
                return target.AttractMyAttention_ToTransform(transform);
            }
            return true;
        }

        public bool AttractMyAttention_FromBot()
        {
            return AttractMyAttention_FromBot(FocusedOnTransform.GetComponent<Bot>());

        }

        public void FocusOnPosition(Vector3 target_position)
        {
            FocusedOnPosition = target_position;
        }

        public bool NoticeTransform(Transform target)
        {
            if (!NoticedTransforms.Contains(target))
            {
                NoticedTransforms.Add(target);
                return true;
            }
            return false;
        }

        public void StartForgetTransform(Transform target)
        {
            if (forgetTransformTimeout > 0)
            {
                StartCoroutine(DelayedForgetTransform(target));

            }
            /*
            else if (forgetTransformTimeout < 0)
            {
                NoticeTransform(target);
            }*/
        }

        IEnumerator DelayedForgetTransform(Transform target)
        {
            yield return new WaitForSeconds(forgetTransformTimeout);
            ForgetTransform(target);

        }


        public void ForgetTransform(Transform target)
        {
            if (DebugMode)
            {
                Debug.Log($"{transform.name}.ForgetTransform: {target.name}");
            }
            NoticedTransforms.Remove(target);

        }

        public void FocusOnTransform(Transform target)
        {
            if (DebugMode)
            {
                Debug.Log($"{transform.name}.FocusOnTransform: {target.name}");
            }
            NoticeTransform(target);
            FocusedOnTransform = target;

        }

        public bool UnFocus()
        {
            if(FocusedOnTransform == null)
            {
                return false;
            }
            if (DebugMode)
            {
                Debug.Log($"{transform.name}.UnFocus: {FocusedOnTransform.name}");
            }
            StartForgetTransform(FocusedOnTransform);
            FocusedOnTransform = null;
            return true;
        }


         public bool IsFacing(Transform target)
        {
            return transform.rotation == Quaternion.LookRotation(target.position - transform.position, Vector3.up);
            // return transform.rotation == target.rotation;
        }


        public bool IsFacing()
        {
            if (FocusedOnTransform != null)
            {
                return IsFacing(FocusedOnTransform);
            }
            return false;
        }

        public bool CanInteractWith(Bot target)
        {
            return target.FocusedOnTransform == transform || target.FocusedOnTransform == null && !target.NoticedTransforms.Contains(transform);
        }

        private void OnDrawGizmos()
        {
            ProximityHelpers.DrawGizmoProximity(transform, SqrInteractionMagnitude);

        }

    }
}