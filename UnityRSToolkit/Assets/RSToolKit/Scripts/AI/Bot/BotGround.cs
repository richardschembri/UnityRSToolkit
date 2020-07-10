using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RSToolkit.AI.Locomotion;

namespace RSToolkit.AI
{
    public class BotGround : Bot
    {
        bool m_freefall = false;

        #region Components

        private BotNavMesh m_botNavMeshComponent;
        public BotNavMesh BotNavMeshComponent
        {
            get
            {
                if (m_botNavMeshComponent == null)
                {
                    m_botNavMeshComponent = GetComponent<BotNavMesh>();
                }
                return m_botNavMeshComponent;
            }
        }

        private Rigidbody m_rigidBodyComponent;
        public Rigidbody RigidBodyComponent
        {
            get
            {
                if (m_rigidBodyComponent == null)
                {
                    m_rigidBodyComponent = GetComponent<Rigidbody>();
                }

                return m_rigidBodyComponent;
            }

        }

        #endregion Components

        private void HandleFailling()
        {
            if (m_currentBotMovementComponent.IsFarFromGround() && !m_freefall)
            {
                m_freefall = true;
                BotNavMeshComponent.NavMeshAgentComponent.enabled = false;
            }
        }

        public override void ToggleComponentsForNetwork(bool owner)
        {
            base.ToggleComponentsForNetwork(owner);
            if (m_freefall && !owner)
            {
                m_freefall = false;
            }
            else if (owner)
            {
                HandleFailling();
            }
        }

        #region MonoBehaviour Functions

        protected override void Awake()
        {
            base.Awake();
            HandleFailling();
        }

        protected override void Update()
        {
            base.Update();
            // There must be a better way to do this
            if (m_freefall && m_currentBotMovementComponent.IsAlmostGrounded())
            {
                m_freefall = false;
                RigidBodyComponent.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
                RigidBodyComponent.velocity = Vector3.zero;
                transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
                BotNavMeshComponent.NavMeshAgentComponent.enabled = true;
            }
        }

        #endregion MonoBehaviour Functions
    }
}