using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using RSToolkit.Helpers;
using RSToolkit.AI.Helpers;

namespace RSToolkit.AI
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class NavMeshNPC : MonoBehaviour
    {
        public float walkSpeed = 0.75f;
        public float walkRotationSpeed = 120f;

        public float runSpeed = 5f;
        public float runRotationSpeed = 120f;

        public Vector3? m_FocusedOnPosition = null;
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

        public bool IsWithinInteractionDistance(Transform target)
        {
            return ProximityHelpers.IsWithinDistance(transform, target, SqrInteractionMagnitude);
        }

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

        private NavMeshAgent m_navMeshAgentComponent;
        public NavMeshAgent NavMeshAgentComponent
        {
            get
            {
                if (m_navMeshAgentComponent == null)
                {
                    m_navMeshAgentComponent = GetComponent<NavMeshAgent>();
                }
                return m_navMeshAgentComponent;
            }

        }

        public List<Transform> NoticedTransforms { get; private set; } = new List<Transform>();
        public float forgetTransformTimeout = -1f;

        public Transform FocusedOnTransform { get; private set; } = null;

        public bool AttractMyAttention_ToTransform(Transform target) //, bool ignoredAlreadyNoticed = true)
        {
            /*
            if (ignoredAlreadyNoticed && NoticedTransforms.Contains(target))
            {
                return false;
            }
            */
            if (IsWithinInteractionDistance(target))
            {
                // StartForgetTransform(target);
                FocusOnTransform(target);

                return true;
            }
            return false;
            
        }

        public bool AttractMyAttention_ToTransform()
        {
            return AttractMyAttention_ToTransform(FocusedOnTransform);

        }

        public bool AttractMyAttention_FromNavMeshNPC(NavMeshNPC target)
        {
            return target.GetComponent<NavMeshNPC>().AttractMyAttention_ToTransform(transform);

        }

        public bool AttractMyAttention_FromNavMeshNPC()
        {
            return AttractMyAttention_FromNavMeshNPC(FocusedOnTransform.GetComponent<NavMeshNPC>());

        }


        /*

        public bool AttractAttention_FromTransform(Transform target, bool ignoredAlreadyNoticed = true)
        {
            if (ignoredAlreadyNoticed && NoticedTransforms.Contains(target))
            {
                return false;
            }
            if (IsWithinInteractionDistance(target))
            {
                StartForgetTransform(target);
                FocusOnPosition(target.position);
            }
            return true;
        }
        */

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
            FocusedOnTransform = null;
        }

        public float CurrentSpeed
        {
            get
            {
                return NavMeshHelpers.GetCurrentSpeed(NavMeshAgentComponent);
            }
        }

        public void WalkTo(Vector3 destination)
        {
            MoveTo(destination, walkSpeed, walkRotationSpeed);
        }

        public bool WalkToFocusedTarget()
        {
            if (FocusedOnTransform != null)
            {
                WalkTo(FocusedOnTransform.position);
                return true;
            }
            return false;
        }

        public void RunTo(Vector3 destination)
        {
            MoveTo(destination, runSpeed, runRotationSpeed);
        }

        public bool RunToFocusedTarget()
        {
            if (FocusedOnTransform != null)
            {
                RunTo(FocusedOnTransform.position);
                return true;
            }
            return false;
        }

        // To Refactor
        public void StepRotateTo(Transform target)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, target.rotation, NavMeshAgentComponent.angularSpeed * Time.deltaTime);
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


        public void MoveTo(Vector3 destination)
        {

            NavMeshAgentComponent.destination = destination;
            // Debug.Log(string.Format("Move to spot: [{0}]", destination.ToString()));
        }

        public void MoveTo()
        {
            if (FocusedOnPosition != null)
            {
                MoveTo(FocusedOnPosition.Value);
            }
        }

        public void MoveTo(Vector3 destination, float speed, float angularSpeed)
        {
            NavMeshAgentComponent.speed = speed;
            NavMeshAgentComponent.angularSpeed = angularSpeed;
            MoveTo(destination);
        }


        private void OnDrawGizmos()
        {
            ProximityHelpers.DrawGizmoProximity(transform, SqrInteractionMagnitude);
            NavMeshHelpers.DrawGizmoDestination(NavMeshAgentComponent);
        }



        private void Awake()
        {
            NavMeshAgentComponent.speed = walkSpeed;
            NavMeshAgentComponent.angularSpeed = walkRotationSpeed;
        }

    }
}

