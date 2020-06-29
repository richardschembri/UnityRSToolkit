using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RSToolkit.AI.Behaviour;
using RSToolkit.AI.Behaviour.Composite;
using RSToolkit.AI.Behaviour.Task;
using RSToolkit.AI.Behaviour.Decorator;
using RSToolkit.AI;

namespace Demo.CTF{
    public class CTFOffenceBot : CTFBot 
    {

        public struct DefendFlagNotTakenBehaviours{
            public BehaviourSequence MainSequence;
            public BehaviourInverter IsFlagCapturedInverter;
            public BehaviourCondition IsFlagCaptured;
            public BehaviourAction DoSeekFlag;
        }
        protected override void InitFlagNotTakenBehaviours(){
        }
        protected override void InitFlagTakenBehaviours(){

        }
        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            
        }
    }
}
