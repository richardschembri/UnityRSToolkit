using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RSToolkit.Controls;
using RSToolkit.Character;
using RSToolkit;
using RSToolkit.AI.FSM;
using RSToolkit.AI.Behaviour;
using RSToolkit.AI.Behaviour.Composite;
using RSToolkit.AI.Behaviour.Task;
using RSToolkit.AI.Behaviour.Decorator;
using RSToolkit.UI.Controls;
using RSToolkit.Helpers;

namespace Demo.BehaviourTree.TurnBased{
    public class BattleManager : RSMonoBehaviour
    {
        public enum FStatesBattle { MENU, START, PLAYERTURN, ENEMYTURN, PLAYERWIN, ENEMYWIN}
        public struct EnemyBehaviours{
            public BehaviourRootNode Root;
            public BehaviourWaitForCondition WaitForTurn;
            public BehaviourSequence SequencePlayTurn;

            public BehaviourAction ActionDoAnnounceAction;
            public BehaviourWait WaitBeforeAction;

            public BehaviourSelector SelectorDecideAction;
            public BehaviourCondition ConditionShouldHeal;
            public BehaviourAction ActionDoHeal;
            public BehaviourAction ActionDoAttack;

        }
        private EnemyBehaviours EnemyBehaviourTree;

#region Behaviour Logic
#region DoAnnounceAttack
    private void DoAnnounceAction(){
        BattleText.text = $"It's {BattleEnemy.DisplayName} turn!";
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

        public UIPopup PopupMenu;
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
            FSM = new BTFiniteStateMachine<FStatesBattle>(FStatesBattle.MENU);
            FSM.OnStarted_AddListener(FStatesBattle.MENU, Menu_Enter);
            FSM.OnStarted_AddListener(FStatesBattle.START, Start_Enter);
            FSM.OnStarted_AddListener(FStatesBattle.PLAYERTURN, PlayerTurn_Enter);
            FSM.OnStarted_AddListener(FStatesBattle.ENEMYTURN, EnemyTurn_Enter);
            FSM.OnStarted_AddListener(FStatesBattle.PLAYERWIN, PlayerWin_Enter);
            FSM.OnStarted_AddListener(FStatesBattle.ENEMYWIN, EnemyWin_Enter);
            FSM.OnStateChanged_AddListener(FSMOnStateChanged_Listener);
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

            EnemyBehaviourTree.ActionDoAnnounceAction = new BehaviourAction(DoAnnounceAction, "Do Announce Attack");
            EnemyBehaviourTree.SequencePlayTurn.AddChild(EnemyBehaviourTree.ActionDoAnnounceAction);

            EnemyBehaviourTree.WaitBeforeAction = new BehaviourWait(1f);
            EnemyBehaviourTree.SequencePlayTurn.AddChild(EnemyBehaviourTree.WaitBeforeAction);

            EnemyBehaviourTree.SelectorDecideAction = new BehaviourSelector(false);
            EnemyBehaviourTree.ActionDoHeal = new BehaviourAction(DoHeal, "Do Heal");
            EnemyBehaviourTree.ActionDoHeal.OnStarted.AddListener(DoHealOnStarted_Listener);
            EnemyBehaviourTree.ConditionShouldHeal = new BehaviourCondition(ShouldEnemyHeal, EnemyBehaviourTree.ActionDoHeal);
            EnemyBehaviourTree.ConditionShouldHeal.Name = "Should Heal";
            EnemyBehaviourTree.SelectorDecideAction.AddChild(EnemyBehaviourTree.ConditionShouldHeal);

            EnemyBehaviourTree.ActionDoAttack = new BehaviourAction(DoAttack, "Do Attack");
            EnemyBehaviourTree.ActionDoAttack.OnStarted.AddListener(DoAttackOnStarted_Listener);

            EnemyBehaviourTree.SelectorDecideAction.AddChild(EnemyBehaviourTree.ActionDoAttack);
            EnemyBehaviourTree.SequencePlayTurn.AddChild(EnemyBehaviourTree.SelectorDecideAction);
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

        protected void Menu_Enter(){
            PopupMenu.OpenPopup();
        }

        protected void Start_Enter(){
            playerBattleStation.DestroyAllSpawns();
            enemyBattleStation.DestroyAllSpawns();
            BattlePlayer = playerBattleStation.SpawnAndGetGameObject();
            BattlePlayer.HealthComponent.HealFull();
            BattleEnemy = enemyBattleStation.SpawnAndGetGameObject();
            BattleEnemy.HealthComponent.HealFull();
            HUDPlayer.SetValues(BattlePlayer);
            HUDEnemy.SetValues(BattleEnemy);
            BattleText.text = $"A wild {BattleEnemy.DisplayName} approaches...";
            
            FSM.ChangeStateIn(2f, FStatesBattle.PLAYERTURN);
        }

        protected void PlayerTurn_Enter(){
            BattleText.text = "Choose an action:";
        }

        protected void EnemyTurn_Enter(){
        }

        protected void PlayerWin_Enter(){
            BattleText.text = "You won the battle!";
            FSM.ChangeStateIn(2f, FStatesBattle.MENU);
        }

        protected void EnemyWin_Enter(){
            BattleText.text = "You were defeated";
            FSM.ChangeStateIn(2f, FStatesBattle.MENU);
        }

        public void HealButtonOnClick_Listener(){
            if(FSM.CurrentState != FStatesBattle.PLAYERTURN){
                return;
            }
            PlayerHeal();
        }

        public void AttackButtonOnClick_Listener(){
            if(FSM.CurrentState != FStatesBattle.PLAYERTURN){
                return;
            }
            PlayerAttack();
        }

        public void NewGameButtonOnClick_Listener(){
            PopupMenu.ClosePopup();
            FSM.ChangeState(FStatesBattle.START);
        }

        public void FSMOnStateChanged_Listener(FStatesBattle state){
            BattleButtons.gameObject.SetActive(state == FStatesBattle.PLAYERTURN); 
        }

        private void PlayerHeal(){
            BattleButtons.gameObject.SetActive(false); 
            BattlePlayer.HealthComponent.Heal(5);
            BattleText.text = "You feel renewed strength!";
            FSM.ChangeStateIn(2f, FStatesBattle.ENEMYTURN);
        }

        private void PlayerAttack(){
            BattleButtons.gameObject.SetActive(false); 
            BattleEnemy.HealthComponent.TakeDamage(BattlePlayer.damage);
            BattleText.text = "The attack is successful!";
            if(BattleEnemy.HealthComponent.IsAlive){
                FSM.ChangeStateIn(2f, FStatesBattle.ENEMYTURN);
            }else{
                BattleEnemy.gameObject.SetActive(false);
                FSM.ChangeStateIn(2f, FStatesBattle.PLAYERWIN);
            }

        }

        private bool IsEnemyTurn(){
            return FSM.CurrentState == FStatesBattle.ENEMYTURN;
        }

        private bool ShouldEnemyHeal(){
            return BattleEnemy.HealthComponent.CurrentHealthPercent < 40 && RandomHelpers.PercentTrue(30);
        }

#endregion DoAnnounceAttack
#region DoAttack
        private void DoAttackOnStarted_Listener(){
            BattlePlayer.HealthComponent.TakeDamage(BattleEnemy.damage);
            if(BattlePlayer.HealthComponent.IsAlive){
                FSM.ChangeStateIn(1f, FStatesBattle.PLAYERTURN);
            }else{
                BattlePlayer.gameObject.SetActive(false);
                FSM.ChangeStateIn(1f, FStatesBattle.ENEMYWIN);
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
#region DoHeal
        private void DoHealOnStarted_Listener(){
            BattleEnemy.HealthComponent.Heal(7);
            FSM.ChangeStateIn(1f, FStatesBattle.PLAYERTURN);
        }
        private BehaviourAction.ActionResult DoHeal(bool cancel){
            if(cancel){
                return BehaviourAction.ActionResult.FAILED;
            }
            if(!IsEnemyTurn()){
                return BehaviourAction.ActionResult.SUCCESS;
            }

            return BehaviourAction.ActionResult.PROGRESS;
        }
#endregion DoHeal
#endregion Behaviour Logic
    }

}