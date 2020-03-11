using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RSToolkit.Space3D;

namespace RSToolkit.AI
{
    [RequireComponent(typeof(Bot))]
    [RequireComponent(typeof(Flying3DObject))]
    public class BotFlying : MonoBehaviour
    {
        public Transform target;
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

        public void FlyTowardsPosition(Vector3 position)
        {
            var rotation = Quaternion.LookRotation(position - transform.position, Vector3.up);
            Flying3DObjectComponent.YawTo(rotation.eulerAngles.y);

            if (Mathf.RoundToInt(position.y) > Mathf.RoundToInt(transform.position.y))
            {
                Flying3DObjectComponent.ApplyVerticalThrust(true);
            }
            if (Mathf.RoundToInt(position.y) < Mathf.RoundToInt(transform.position.y))
            {
                Flying3DObjectComponent.ApplyVerticalThrust(false);
            }
            if (!BotComponent.IsWithinInteractionDistance())
            {
                Flying3DObjectComponent.ApplyForwardThrust(0.2f);

            }
            else if (BotComponent.IsWithinPersonalSpace())
            {
                Flying3DObjectComponent.ApplyForwardThrust(-0.1f);
            }
        }

        public void FlyToPosition()
        {
            if(BotComponent.FocusedOnPosition != null)
            {
                FlyTowardsPosition(BotComponent.FocusedOnPosition.Value);
            }
        }

        public void FlyToTarget()
        {
            if (BotComponent.FocusedOnTransform != null)
            {
                FlyTowardsPosition(BotComponent.FocusedOnTransform.position);
            }           
        }

        private void Awake()
        {
            if(target != null)
            {
                BotComponent.FocusOnTransform(target);
            }
        }

        // Update is called once per frame
        void Update()
        {
            
        }


    }
}