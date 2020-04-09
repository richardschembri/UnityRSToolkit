using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RSToolkit.Space3D;

namespace RSToolkit.AI
{
    [RequireComponent(typeof(Bot))]
    [RequireComponent(typeof(Flying3DObject))]
    public class BotFlying : BotLocomotion
    {
        private Flying3DObject m_flying3DObjectComponent;
        public Flying3DObject Flying3DObjectComponent
        {
            get
            {
                if (m_flying3DObjectComponent == null)
                {
                    m_flying3DObjectComponent = GetComponent<Flying3DObject>();
                }
                return m_flying3DObjectComponent;
            }

        }

        public override float CurrentSpeed
        {
            get
            {
                return Flying3DObjectComponent.CurrentSpeed;
            }
        }

        public override void RotateTowardsPosition()
        {
            var rotation = Quaternion.LookRotation(BotComponent.FocusedOnPosition.Value - transform.position, Vector3.up);
            Flying3DObjectComponent.YawTo(rotation.eulerAngles.y);
        }

        public override void MoveTowardsPosition(bool fullspeed = true)
        {

            RotateTowardsPosition();

            if (Mathf.RoundToInt(BotComponent.FocusedOnPosition.Value.y) > Mathf.RoundToInt(transform.position.y))
            {
                Flying3DObjectComponent.ApplyVerticalThrust(true);
            }
            if (Mathf.RoundToInt(BotComponent.FocusedOnPosition.Value.y) < Mathf.RoundToInt(transform.position.y))
            {
                Flying3DObjectComponent.ApplyVerticalThrust(false);
            }
            if (!BotComponent.IsWithinInteractionDistance())
            {
                Flying3DObjectComponent.ApplyForwardThrust(fullspeed ? 1f : 0.2f);
            }
            else if (BotComponent.IsWithinPersonalSpace())
            {
                Flying3DObjectComponent.ApplyForwardThrust(fullspeed ? -0.5f : 0.1f);
            }
        }

    }
}