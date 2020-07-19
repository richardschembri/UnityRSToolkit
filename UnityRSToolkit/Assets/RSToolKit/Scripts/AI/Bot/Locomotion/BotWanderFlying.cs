using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RSToolkit.Helpers;
using RSToolkit.Space3D;
using System.Linq;

namespace RSToolkit.AI.Locomotion
{
    public class BotWanderFlying : BotWander
    {
        public float DefaultY = 5f;
        private int m_findNewPositionAttempts = 0;
        private const int MAX_FINDNEWPOSITIONATTEMPTS = 20;
        public bool AboveSurface = true;
        [Tooltip("If 0 or less a linecast will be used")]
        public float SpherecastRadius = 1f;

        private Flying3DObject _flying3DObjectComponent;
        public Flying3DObject Flying3DObjectComponent
        {
            get
            {
                if (_flying3DObjectComponent == null)
                {
                    _flying3DObjectComponent = GetComponent<Flying3DObject>();
                }
                return _flying3DObjectComponent;
            }

        }
        public override bool CanWander()
        {
            return BotLocomotiveComponent.CurrentLocomotionType is BotLogicFlight 
                && Mathf.Abs(Flying3DObjectComponent.MovementFlightThrust.x) > 0f
                && Mathf.Abs(Flying3DObjectComponent.MovementFlightThrust.z) > 0f;
        }

        RaycastHit m_wanderhit;

        protected override Vector3? GetNewWanderPosition(Transform wanderCenter, float radius)
        {
            if (m_findNewPositionAttempts >= MAX_FINDNEWPOSITIONATTEMPTS)
            {
                // FSM.ChangeState(WanderStates.CannotWander);
                return null;//WanderCenter.position;
            }
            m_findNewPositionAttempts++;
            //var newPos = transform.GetRandomPositionWithinCircle(radius, BotFlyingComponent.BotComponent.SqrPersonalSpaceMagnitude);
            Vector3? newPos = wanderCenter.GetRandomPositionWithinCircle(radius, BotLocomotiveComponent.SqrPersonalSpaceMagnitude);
            newPos = new Vector3(newPos.Value.x, DefaultY, newPos.Value.z);

            if (AboveSurface && !Physics.Raycast(newPos.Value, Vector3.down, Mathf.Infinity))
            {
                Debug.Log($"{newPos} Not above surface");
                return GetNewWanderPosition(wanderCenter, radius);
            }

            if ((SpherecastRadius <= 0 && BotLocomotiveComponent.ColliderComponent.RaycastFromOutsideBounds(out m_wanderhit, newPos.Value, radius))
                || (SpherecastRadius > 0 && BotLocomotiveComponent.ColliderComponent.SpherecastFromOutsideBounds(out m_wanderhit, newPos.Value, SpherecastRadius, radius)))
            {
               
                if (DebugMode)
                {
                    Debug.Log($"Wander position is behind {m_wanderhit.transform.name}");
                }

            
                if (Vector3.Distance(transform.position, m_wanderhit.point) * 0.75f >= BotLocomotiveComponent.SqrPersonalSpaceMagnitude)
                {
                    newPos = Vector3.Lerp(transform.position, m_wanderhit.point, 0.75f);
                }
                else
                {
                    newPos = GetNewWanderPosition(wanderCenter, radius);
                }
            }

            m_findNewPositionAttempts = 0;
            return newPos;    
        }


    }

}
 