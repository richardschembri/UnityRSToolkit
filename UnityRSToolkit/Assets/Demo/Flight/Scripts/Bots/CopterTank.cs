using RSToolkit.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Demo{
    public class CopterTank : MonoBehaviour
    {
        public GameObject HelicopterParts;
        public GameObject TankParts;

        public BotFlyable BotFlyableComponent {get; private set;}
        
        void Awake()
        {
            BotFlyableComponent = GetComponent<BotFlyable>();
            BotFlyableComponent.AddStateChangedListener(FlyableStateChanged_Listener);    
        }

        private void FlyableStateChanged_Listener(BotFlyable.FlyableStates state)
        {
            TankParts.SetActive(state == BotFlyable.FlyableStates.NotFlying);
            HelicopterParts.SetActive(!TankParts.activeSelf);
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.Alpha1))
            {
                BotFlyableComponent.TakeOff();
            }

            if (Input.GetKeyUp(KeyCode.Alpha2))
            {
                BotFlyableComponent.Land(true);
            }

            if (Input.GetKeyUp(KeyCode.Alpha3))
            {
                BotFlyableComponent.Land(false);
            }

            if (Input.GetKeyUp(KeyCode.Alpha4))
            {
                if (BotFlyableComponent.BotWanderManagerComponent.IsWandering())
                {
                    BotFlyableComponent.BotWanderManagerComponent.StopWandering();
                }
                else
                {
                    BotFlyableComponent.BotWanderManagerComponent.Wander();
                }
                
            }

            if (Input.GetKeyUp(KeyCode.Alpha5))
            {
                BotFlyableComponent.BotNavMeshRef.MoveToClosestEdge();
            }
            if (Input.GetKeyUp(KeyCode.Alpha6))
            {
                BotFlyableComponent.BotNavMeshRef.JumpOffLedge();
            }
        }
    }
}