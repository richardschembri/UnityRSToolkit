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
                BotNavMeshComponent.NavMeshAgentComponent.enabled = true;
            }
        }

        #endregion MonoBehaviour Functions
    }
}