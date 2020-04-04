using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RSToolkit.Helpers;
using System.Linq;

namespace RSToolkit.AI
{
    [RequireComponent(typeof(BotFlying))]
    public class BotWanderFlying : BotWander
    {
        public float DefaultY = 5f;
        private BotFlying m_botFlyingComponent;
        private int m_findNewPositionAttempts = 0;
        private const int MAX_FINDNEWPOSITIONATTEMPTS = 20;
        public bool AboveSurface = true;

        public BotFlying BotFlyingComponent
        {
            get
            {
                if (m_botFlyingComponent == null)
                {
                    m_botFlyingComponent = GetComponent<BotFlying>();
                }
                return m_botFlyingComponent;
            }
        }
        public override bool CanWander()
        {
            return Mathf.Abs(BotFlyingComponent.Flying3DObjectComponent.MovementFlightThrust.x) > 0f
                && Mathf.Abs(BotFlyingComponent.Flying3DObjectComponent.MovementFlightThrust.z) > 0f;
        }
        Ray m_wanderray = new Ray();
        RaycastHit m_wanderhit;

        protected override Vector3 GetNewWanderPosition(float radius)
        {
            if (m_findNewPositionAttempts >= MAX_FINDNEWPOSITIONATTEMPTS)
            {
                m_FSM.ChangeState(WanderStates.CannotWander);
                return transform.position;
            }
            m_findNewPositionAttempts++;
            var newPos = transform.GetRandomPositionWithinCircle(radius, BotFlyingComponent.BotComponent.SqrPersonalSpaceMagnitude);
            newPos = new Vector3(newPos.x, DefaultY, newPos.z);

            if (AboveSurface && !Physics.Raycast(newPos, Vector3.down, Mathf.Infinity))
            {
                Debug.Log("Not above surface");
                return GetNewWanderPosition(radius);
            }

            if (BotFlyingComponent.BotComponent.ColliderComponent.LinecastFromOutsideBounds(out m_wanderhit, newPos))
            {
               
                if (debugMode)
                {
                    Debug.Log($"Wander position is behind {m_wanderhit.transform.name}");
                }

            
                if (Vector3.Distance(transform.position, m_wanderhit.point) * 0.75f >= BotFlyingComponent.BotComponent.SqrPersonalSpaceMagnitude)
                {
                    newPos = Vector3.Lerp(transform.position, m_wanderhit.point, 0.75f);
                }
                else
                {
                    newPos = GetNewWanderPosition(radius);
                }
            }

            m_findNewPositionAttempts = 0;
            return newPos;    
        }


    }

}
 