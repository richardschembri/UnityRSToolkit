using RSToolkit.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Demo
{
    public class TankBot : MonoBehaviour
    {
        private BotGround m_botGroundComponent;
        public BotGround BotGroundComponent
        {
            get
            {
                if (m_botGroundComponent == null)
                {
                    m_botGroundComponent = GetComponent<BotGround>();
                }

                return m_botGroundComponent;
            }

        }
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyUp(KeyCode.Alpha4))
            {
                if (BotGroundComponent.BotWanderManagerComponent.IsWandering())
                {
                    BotGroundComponent.BotWanderManagerComponent.StopWandering();
                }
                else
                {
                    BotGroundComponent.BotWanderManagerComponent.Wander();
                }

            }
        }
    }
}