using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSToolkit.AI
{

    public class BotWanderFlying : BotWander
    {

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
        public override bool IsWanderingToPosition()
        {
            return (WanderPosition != null && WanderPosition == BotComponent.FocusedOnPosition)
                && BotFlyingComponent.Flying3DObjectComponent.IsMovingHorizontally();
        }

        protected override void MoveToWanderPosition()
        {
            BotFlyingComponent.FlyToPosition();
        }

        void Update()
        {
            
        }
    }
}