using RSToolkit.Helpers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace RSToolkit.AI
{
    [DisallowMultipleComponent]
    public class Bot : MonoBehaviour
    {

        public enum InteractionStates
        {
            NotInteracting,
            Interactor,
            Interactee
        }

        public InteractionStates CurrentInteractionState = InteractionStates.NotInteracting;

        public bool DebugMode = false;

        protected BotWander[] m_botWanderComponents;
        protected BotWander[] m_BotWanderComponents
        {
            get
            {
                if(m_botWanderComponents == null)
                {
                    m_botWanderComponents = GetComponents<BotWander>();
                }
                return m_botWanderComponents;
            }
            private set
            {
                m_botWanderComponents = value;
            }
        }
        protected BotWander m_currentBotWanderComponent;

        protected BotLocomotion[] m_botMovementComponents;
        protected BotLocomotion[] m_BotMovementComponents
        {
            get
            {
                if (m_botMovementComponents == null)
                {
                    m_botMovementComponents = GetComponents<BotLocomotion>();
                }
                return m_botMovementComponents;
            }
            private set
            {
                m_botMovementComponents = value;
            }
        }
        protected BotLocomotion m_currenBotMovementComponent;

        protected void SetCurrentBotWander(BotWander b)
        {
            if (m_BotWanderComponents.Contains(b))
            {
                m_currentBotWanderComponent = b;
                for(int i = 0; i < m_BotWanderComponents.Length; i++)
                {
                    if(m_BotWanderComponents[i] != b)
                    {
                        m_BotWanderComponents[i].StopWandering(true);
                    }
                }
            }
            else
            {
                throw new System.Exception($"{name} does not contain component");
            }
        }

        protected void SetCurrentBotMovement(BotLocomotion b)
        {
            if (m_BotMovementComponents.Contains(b))
            {
                m_currenBotMovementComponent = b;
            }
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

        private Collider m_colliderComponent;
        public Collider ColliderComponent
        {
            get
            {
                if(m_colliderComponent == null)
                {
                    m_colliderComponent = GetComponent<Collider>();
                }
                return m_colliderComponent;
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

        public float SqrPersonalSpaceMagnitude
        {
            get
            {
                return SqrInteractionMagnitude * .75f;
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
            }else if( FocusedOnPosition != null)
            {
                return IsWithinPersonalSpace(FocusedOnPosition.Value);
            }
            return false;

        }

        public bool IsWithinInteractionDistance(Vector3 position)
        {
            return ProximityHelpers.IsWithinDistance(transform, position, SqrInteractionMagnitude);
        }
        public bool IsWithinPersonalSpace(Vector3 position)
        {
            return ProximityHelpers.IsWithinDistance(transform, position, SqrPersonalSpaceMagnitude);
        }

        public bool IsWithinInteractionDistance(Transform target)
        {
            return IsWithinInteractionDistance(transform.position);
        }

        public bool IsWithinPersonalSpace(Transform target)
        {
            return IsWithinPersonalSpace(transform.position);
        }


        public bool AttractMyAttention_ToTransform(Transform target, InteractionStates interactionState = InteractionStates.Interactor)
        {

            if (IsWithinInteractionDistance(target))
            {
                FocusOnTransform(target);
                CurrentInteractionState = interactionState;
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
                return target.AttractMyAttention_ToTransform(transform, InteractionStates.Interactee);
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
            CurrentInteractionState = InteractionStates.NotInteracting;
            return true;
        }


         public bool IsFacing(Transform target)
        {
            return Mathf.Round(transform.rotation.eulerAngles.y * 10) == Mathf.Round(Quaternion.LookRotation(target.position - transform.position, Vector3.up).eulerAngles.y * 10);
            // return transform.rotation == Quaternion.LookRotation(target.position - transform.position, Vector3.up);
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

        public bool Wander()
        {

           if (m_currentBotWanderComponent.Wander())
           {
               return true;
           }

           return false;
        }

        public bool StopWandering(bool stopMoving = false)
        {
            if (m_currentBotWanderComponent.StopWandering(stopMoving))
            {
                return true;
            }

            return false;
        }

        public bool IsWandering()
        {
            return m_currentBotWanderComponent.IsWandering();
        }

        public void MoveTowardsPosition(bool fullspeed = true)
        {
            m_currenBotMovementComponent.MoveTowardsPosition(fullspeed);
        }

        public void MoveTowardsTarget(bool fullspeed = true)
        {
            m_currenBotMovementComponent.MoveTowardsTarget(fullspeed);
        }

        public void MoveToPosition(BotLocomotion.StopMovementConditions stopMovementCondition, bool fullspeed = true)
        {
            m_currenBotMovementComponent.MoveToPosition(stopMovementCondition, fullspeed);
        }

        public void MoveToTarget(BotLocomotion.StopMovementConditions stopMovementCondition ,bool fullspeed = true)
        {
            m_currenBotMovementComponent.MoveToTarget(stopMovementCondition, fullspeed);
        }

        public void RotateTowardsPosition()
        {
            m_currenBotMovementComponent.RotateTowardsPosition();
        }

        public bool StopMoving()
        {
            return m_currenBotMovementComponent.StopMoving();
        }

        public BotLocomotion.LocomotionState GetMovementState()
        {
            return m_currenBotMovementComponent.CurrentState;
        }

        protected virtual void Awake()
        {
            if (m_BotMovementComponents.Length > 0)
            {
                SetCurrentBotMovement(m_BotMovementComponents[0]);
            }

            if (m_BotWanderComponents.Length > 0)
            {
                SetCurrentBotWander(m_BotWanderComponents[0]);
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!DebugMode)
            {
                return;
            }
            ProximityHelpers.DrawGizmoProximity(transform, SqrInteractionMagnitude, IsWithinInteractionDistance());
            if(FocusedOnPosition != null)
            {
                UnityEditor.Handles.color = new Color(1f, 1f, 0.008f, 0.55f);
                UnityEditor.Handles.DrawSolidDisc(FocusedOnPosition.Value, Vector3.up, 0.25f);
            }
            if(FocusedOnTransform != null)
            {
                var oldColor = UnityEditor.Handles.color;
                UnityEditor.Handles.color = IsWithinInteractionDistance() ? Color.red : Color.white;
                UnityEditor.Handles.DrawLine(transform.position, FocusedOnTransform.position);
                UnityEditor.Handles.color = oldColor;
            }
        }
#endif
    }
}