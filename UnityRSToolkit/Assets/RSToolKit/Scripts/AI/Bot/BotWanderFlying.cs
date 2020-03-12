using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RSToolkit.Helpers;

namespace RSToolkit.AI
{
    [RequireComponent(typeof(BotFlying))]
    public class BotWanderFlying : BotWander
    {
        public float DefaultY = 5f;
        private BotFlying m_botFlyingComponent;
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

        protected override void MoveTowardsWanderPosition()
        {
            BotFlyingComponent.FlyToPosition();
        }

        protected override Vector3 GetNewWanderPosition(float radius)
        {
            var newPos = transform.GetRandomPositionWithinCircle(radius, BotFlyingComponent.BotComponent.SqrPersonalSpaceMagnitude);
            newPos = new Vector3(newPos.x, DefaultY, newPos.z);
            return newPos;
        }

        protected override void Awake()
        {
            base.Awake();
            //DefaultY = transform.position.y;
            m_fsm.Changed += Fsm_Changed;
            
        }

        private void Fsm_Changed(WanderStates state)
        {
            /*
            try
            {
                if(m_fsm.LastState == WanderStates.NotWandering)
                {
                    default_y = transform.position.y;
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex.Message);
            }
            */

        }
    }

}