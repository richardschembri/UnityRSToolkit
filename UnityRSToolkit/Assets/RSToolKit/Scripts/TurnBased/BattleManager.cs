using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RSToolkit.AI.FSM;
using RSToolkit.Controls;
using RSToolkit.Character;

namespace RSToolkit.TurnBased{
    public class BattleManager : RSMonoBehaviour
    {
        public enum FStatesBattle { START, PLAYERTURN, ENEMYTURN, WON, LOST}

        public BTFiniteStateMachine<FStatesBattle> FSM { get; private set; } 
        #region Components
        public BTFiniteStateMachineManager BTFiniteStateMachineManagerComponent {get; private set;}
        #endregion Components

        public Spawner<BattleCharacterController> playerBattleStation; 
        public Spawner<BattleCharacterController> enemyBattleStation; 
        public BattleCharacterController BattlePlayer{get; private set;}
        public BattleCharacterController BattleEnemy{get; private set;}

        public BattleHUD HUDPlayer;
        public BattleHUD HUDEnemy;

        public Text BattleText;
        public override bool Init(bool force = false)
        {
            if(Init(force))
            {
                BTFiniteStateMachineManagerComponent = GetComponent<BTFiniteStateMachineManager>();           

                FSM = new BTFiniteStateMachine<FStatesBattle>(FStatesBattle.START);
                FSM.OnStarted_AddListener(FStatesBattle.START, Start_Enter);
                return true;
            }
            return false;
        }
        protected virtual void Start()
        {
            if (!Initialized)
            {
                return;
            }

            BTFiniteStateMachineManagerComponent.StartFSMs();
        }

        protected void Start_Enter(){
            playerBattleStation.DestroyAllSpawns();
            enemyBattleStation.DestroyAllSpawns();
            BattlePlayer = playerBattleStation.SpawnAndGetGameObject();
            BattleEnemy = enemyBattleStation.SpawnAndGetGameObject();
            HUDPlayer.SetValues(BattlePlayer);
            HUDEnemy.SetValues(BattleEnemy);
            BattleText.text = $"A wild {BattleEnemy.name} approaches...";
        }

    }

}