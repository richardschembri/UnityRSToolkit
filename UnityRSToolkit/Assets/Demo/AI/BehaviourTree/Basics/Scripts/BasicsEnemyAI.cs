using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RSToolkit.AI.Behaviour;
using RSToolkit.Helpers;
using RSToolkit.AI.Behaviour.Decorator;
using RSToolkit.AI.Behaviour.Task;
using RSToolkit.AI.Behaviour.Composite;
using static RSToolkit.Helpers.ProximityHelpers;

namespace Demo.BehaviourTree.Basics{
    public class BasicsEnemyAI : MonoBehaviour
    {
        [SerializeField]
        BasicsPlayer _player;

        [SerializeField]
        private float _playerMinMagnitude = 7.5f;

        [SerializeField]
        private float _playerCheckInterval = 0.125f;

        BehaviourManager _BehaviourManager;

        private bool IsPlayerWithinDistance(){
            return ProximityHelpers.IsWithinDistance(this.transform, _player.transform.position, _playerMinMagnitude, DistanceDirection.ALL);
        }

        private BehaviourAction.ActionResult SeekPlayer(bool cancel){
            if(cancel){
                return BehaviourAction.ActionResult.FAILED;
            }
            Vector3 playerLocalPos = this.transform.InverseTransformPoint(_player.transform.position);
            transform.localPosition += playerLocalPos * 0.5f * Time.deltaTime;
            return BehaviourAction.ActionResult.PROGRESS;
        }

        private void InitBehaviours(){
            // We always need a root node
            var root = _BehaviourManager.AddBehaviourTree("Basic Enemy AI");
            var sequenceSeekPlayer = new BehaviourSequence(false); 
            sequenceSeekPlayer.Name = "Seek Player Sequence";
            sequenceSeekPlayer.AddChild(new BehaviourAction(() => SetColor(Color.red), "Set Seek Color"));
            var conditionIsPlayerWithinDistance = new BehaviourCondition(IsPlayerWithinDistance,  _playerCheckInterval, 0f, 
                                                                        new BehaviourAction(SeekPlayer, "Seek Player")
                                                                );
            conditionIsPlayerWithinDistance.Name = "Is Player Within Distance";
            sequenceSeekPlayer.AddChild(conditionIsPlayerWithinDistance);

            var sequenceWaitForPlayer = new BehaviourSequence(false); 
            sequenceWaitForPlayer.Name = "Wait For Player Sequence";
            sequenceWaitForPlayer.AddChild(new BehaviourAction(() => SetColor(Color.grey), "Set Wait Color"));
            sequenceWaitForPlayer.AddChild(new BehaviourWaitUntilStopped(false));

            var selectorEnemyAI = new BehaviourSelector(false);
            selectorEnemyAI.AddChild(sequenceSeekPlayer);
            selectorEnemyAI.AddChild(sequenceWaitForPlayer);
            root.AddChild(selectorEnemyAI);

        }

        private void SetColor(Color color){
            GetComponent<MeshRenderer>().material.SetColor("_Color", color);
        }


        void Awake(){
            _BehaviourManager = GetComponent<BehaviourManager>();
        }

        // Start is called before the first frame update
        void Start()
        {
            _BehaviourManager.StartTree();
        }

        // Update is called once per frame
        void Update()
        {
            
        }
    }
}
