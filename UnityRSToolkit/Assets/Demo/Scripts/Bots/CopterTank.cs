using RSToolkit.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopterTank : MonoBehaviour
{
    public GameObject HelicopterParts;
    public GameObject TankParts;

    private BotFlyable m_botFlyableComponent;
    public BotFlyable BotFlyableComponent
    {
        get
        {
            if (m_botFlyableComponent == null)
            {
                m_botFlyableComponent = GetComponent<BotFlyable>();
            }

            return m_botFlyableComponent;
        }

    }

    void Awake()
    {
        BotFlyableComponent.AddStateChangedListener(FlyableStateChanged_Listener);    
    }

    private void FlyableStateChanged_Listener(BotFlyable.FlyableStates state)
    {
        TankParts.SetActive(state == BotFlyable.FlyableStates.Grounded);
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
            if (BotFlyableComponent.IsWandering())
            {
                BotFlyableComponent.StopWandering();
            }
            else
            {
                BotFlyableComponent.Wander();
            }
            
        }

        if (Input.GetKeyUp(KeyCode.Alpha5))
        {
            BotFlyableComponent.BotNavMeshComponent.MoveToClosestEdge();
        }
        if (Input.GetKeyUp(KeyCode.Alpha6))
        {
            BotFlyableComponent.BotNavMeshComponent.JumpOffLedge();
        }
    }
}
