using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RSToolkit.AI
{
    [RequireComponent(typeof(Bot))]
    public abstract class BotWander : MonoBehaviour
    {
        public Vector3? WanderPosition { get; private set; } = null;
        public float wanderRadius = 20f;

        private Bot m_botComponent;
        public Bot BotComponent
        {
            get
            {
                if (m_botComponent == null)
                {
                    m_botComponent = GetComponent<Bot>();
                }
                return m_botComponent;
            }

        }

        public abstract bool IsWanderingToPosition();

        public void Wander(float distance)
        {
            BotComponent.UnFocus();
            if (!IsWanderingToPosition())
            {
                
                WanderPosition = GetNewWanderPosition(distance);
                BotComponent.FocusOnPosition(WanderPosition.Value);
                MoveToWanderPosition();
            }
        }

        public void Wander()
        {
            Wander(wanderRadius);
        }

        protected abstract void MoveToWanderPosition();

        protected abstract Vector3 GetNewWanderPosition(float distance);

        public void OnDrawGizmos()
        {
#if UNITY_EDITOR
            var oldColor = UnityEditor.Handles.color;
            UnityEditor.Handles.color = new Color(0f, 0f, 0.75f, .075f);

            UnityEditor.Handles.DrawSolidDisc(transform.position, Vector3.up, wanderRadius);

            if(WanderPosition != null)
            {
                UnityEditor.Handles.color = new Color(1f, 1f, 0.008f, 0.55f);
                UnityEditor.Handles.DrawSolidDisc(WanderPosition.Value, Vector3.up, 0.25f);
            }
            
            UnityEditor.Handles.color = oldColor;
#endif
        }
    }
}