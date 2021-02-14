using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RSToolkit.AI.FSM;
using RSToolkit.Controls;
using RSToolkit.Character;
using RSToolkit.AI.Behaviour;
using RSToolkit.AI.Behaviour.Composite;
using RSToolkit.AI.Behaviour.Task;
using RSToolkit.AI.Behaviour.Decorator;

namespace RSToolkit.TurnBased{
    public class BattleManager : RSMonoBehaviour
    {
        public enum FStatesBattle { START, PLAYERTURN, ENEMYTURN, WON, LOST}
        public struct EnemyBehaviours{
            public BehaviourRootNode Root;
            public BehaviourWaitForCondition WaitForTurn;
            public BehaviourSequence SequencePlayTurn;

            public BehaviourAction ActionDoAnnounceAttack;
            public BehaviourWait WaitBeforeAttack;
            public BehaviourAction ActionDoAttack;

        }
        private EnemyBehaviours EnemyBehaviourTree;

#region Behaviour Logic
#region DoAnnounceAttack
    private void DoAnnounceAttack(){
        BattleText.text = $"{BattleEnemy.DisplayName} attacks";
    }
        public BTFiniteStateMachine<FStatesBattle> FSM { get; private set; } 
        #region Components
        protected BehaviourManager _behaviourManagerComponent;
        private BTFiniteStateMachineManager _btFiniteStateMachineManagerComponent; 
        #endregion Components

        public BattleSpawner playerBattleStation; 
        public BattleSpawner enemyBattleStation; 
        public BattleCharacterController BattlePlayer{get; private set;}
        public BattleCharacterController BattleEnemy{get; private set;}

        public BattleHUD HUDPlayer;
        public BattleHUD HUDEnemy;

        public Text BattleText;
        public CanvasGroup BattleButtons;
        public override bool Init(bool force = false)
        {
            if(base.Init(force))
            {
                InitFSM();
                InitBehaviourTree();
                return true;
            }
            return false;
        }

        private void InitFSM(){
            _btFiniteStateMachineManagerComponent = GetComponent<BTFiniteStateMachineManager>();           
            FSM = new BTFiniteStateMachine<FStatesBattle>(FStatesBattle.START);
            FSM.OnStarted_AddListener(FStatesBattle.START, Start_Enter);
            FSM.OnStarted_AddListener(FStatesBattle.PLAYERTURN, PlayerTurn_Enter);
            FSM.OnStarted_AddListener(FStatesBattle.ENEMYTURN, EnemyTurn_Enter);
            FSM.OnStarted_AddListener(FStatesBattle.WON, Won_Enter);
            FSM.OnStarted_AddListener(FStatesBattle.LOST, Lose_Enter);
            _btFiniteStateMachineManagerComponent.AddFSM(FSM);
        }

        private void InitBehaviourTree(){
            _behaviourManagerComponent = GetComponent<BehaviourManager>();
            EnemyBehaviourTree.Root = new BehaviourRootNode();
            EnemyBehaviourTree.WaitForTurn = new BehaviourWaitForCondition(IsEnemyTurn, 1f, 0f);
            EnemyBehaviourTree.Root.AddChild(EnemyBehaviourTree.WaitForTurn);
            EnemyBehaviourTree.SequencePlayTurn = new BehaviourSequence(false);
            EnemyBehaviourTree.SequencePlayTurn.Name = "Play Turn";
            EnemyBehaviourTree.WaitForTurn.AddChild(EnemyBehaviourTree.SequencePlayTurn);

            EnemyBehaviourTree.ActionDoAnnounceAttack = new BehaviourAction(DoAnnounceAttack, "Do Announce Attack");
            EnemyBehaviourTree.SequencePlayTurn.AddChild(EnemyBehaviourTree.ActionDoAnnounceAttack);
            EnemyBehaviourTree.WaitBeforeAttack = new BehaviourWait(1f);
            EnemyBehaviourTree.SequencePlayTurn.AddChild(EnemyBehaviourTree.WaitBeforeAttack);
            EnemyBehaviourTree.ActionDoAttack = new BehaviourAction(DoAttack, "Do Attack");
            EnemyBehaviourTree.ActionDoAttack.OnStarted.AddListener(DoAttackOnStarted_Listener);

            EnemyBehaviourTree.SequencePlayTurn.AddChild(EnemyBehaviourTree.ActionDoAttack);
            _behaviourManagerComponent.SetCurrentTree(EnemyBehaviourTree.Root, true);
        }

        protected virtual void Start()
        {
            if (!Initialized)
            {
                return;
            }

            _btFiniteStateMachineManagerComponent.StartFSMs();
            _behaviourManagerComponent.StartTree();
        }

        protected void Start_Enter(){
            // BattleButtons.interactable = false; 
            BattleButtons.gameObject.SetActive(false); 
            playerBattleStation.DestroyAllSpawns();
            enemyBattleStation.DestroyAllSpawns();
            BattlePlayer = playerBattleStation.SpawnAndGetGameObject();
            BattlePlayer.HealthComponent.HealFull();
            BattleEnemy = enemyBattleStation.SpawnAndGetGameObject();
            BattleEnemy.HealthComponent.HealFull();
            HUDPlayer.SetValues(BattlePlayer);
            HUDEnemy.SetValues(BattleEnemy);
            BattleText.text = $"A wild {BattleEnemy.name} approaches...";
            
            FSM.ChangeStateIn(2f, FStatesBattle.PLAYERTURN);
        }

        protected void PlayerTurn_Enter(){
            BattleText.text = "Choose an action:";
            BattleButtons.gameObject.SetActive(true); 
        }

        protected void EnemyTurn_Enter(){
            //BattleButtons.interactable = false; 
            BattleButtons.gameObject.SetActive(false); 
        }

        protected void Won_Enter(){
            BattleText.text = "You won the battle!";
        }

        protected void Lose_Enter(){
            BattleText.text = "You were defeated";
        }

        public void OnClickHealButton_Listener(){
            if(FSM.CurrentState != FStatesBattle.PLAYERTURN){
                return;
            }
            PlayerHeal();
        }

        public void OnClickAttackButton_Listener(){
            if(FSM.CurrentState != FStatesBattle.PLAYERTURN){
                return;
            }
            PlayerAttack();
        }

        private void PlayerHeal(){
            BattlePlayer.HealthComponent.Heal(5);
            BattleText.text = "You feel renewed strength!";
            //BattleButtons.interactable = false; 
            BattleButtons.gameObject.SetActive(false); 
            FSM.ChangeStateIn(2f, FStatesBattle.ENEMYTURN);
        }

        private void PlayerAttack(){
            BattleEnemy.HealthComponent.TakeDamage(BattlePlayer.damage);

            BattleText.text = "The attack is successful!";
            if(BattleEnemy.HealthComponent.IsAlive){
                FSM.ChangeStateIn(2f, FStatesBattle.ENEMYTURN);
            }else{
                FSM.ChangeStateIn(2f, FStatesBattle.WON);
            }

        }

        private bool IsEnemyTurn(){
            return FSM.CurrentState == FStatesBattle.ENEMYTURN;
        }

#endregion DoAnnounceAttack
#region DoAttack
        private void DoAttackOnStarted_Listener(){
            BattlePlayer.HealthComponent.TakeDamage(BattleEnemy.damage);
            if(BattlePlayer.HealthComponent.IsAlive){
                FSM.ChangeStateIn(1f, FStatesBattle.PLAYERTURN);
            }else{
                FSM.ChangeStateIn(1f, FStatesBattle.LOST);
            }
        }
        private BehaviourAction.ActionResult DoAttack(bool cancel){
            if(cancel){
                return BehaviourAction.ActionResult.FAILED;
            }
            if(!IsEnemyTurn()){
                return BehaviourAction.ActionResult.SUCCESS;
            }

            return BehaviourAction.ActionResult.PROGRESS;
        }
#endregion DoAttack
#endregion Behaviour Logic
    }

}