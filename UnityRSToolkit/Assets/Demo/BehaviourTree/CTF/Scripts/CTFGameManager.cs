using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RSToolkit;
using System.Linq;

namespace Demo.BehaviourTree.CTF{
    public class CTFGameManager : SingletonMonoBehaviour<CTFGameManager>
    {
        public const string TAG_OFFENCE = "Offence";
        public const string TAG_DEFENCE = "Defence";
        public const string TAG_FLAG = "Flag";
        public Transform BotsContainer;
        public CTFBot[] Bots{get; private set;} 

        public Vector3 FlagStartPosition {get; private set;}
        public Quaternion FlagStartRotation {get; private set;}

        public Transform Flag {get; private set;}

        [SerializeField]
        private Transform _level = null;
        public Transform Level {get{return _level;}}

        // private bool m_flagTakeCache = false;

#region Reset
        private void ResetFlag(){
           Flag.parent = Level; 
           Flag.position = FlagStartPosition;
           Flag.rotation = FlagStartRotation;
        }

        public void ResetGame(){
            ResetFlag();
        }
#endregion Reset

        public bool IsFlagTaken(){
            //return Bots.Any(b => b.HasFlag());
            return Flag.parent == Level; 
        }

        public bool GiveFlagToBot(CTFBot bot){
           if(IsFlagTaken()){
               return false;
           }
            Flag.parent = bot.FlagHolder.transform;
            Flag.localPosition = Vector3.zero;
           return true;
        }

        public void StartGame(){

        }

        #region Mono Functions
        protected override void Awake(){
            base.Awake();
            Bots = BotsContainer.GetComponentsInChildren<CTFBot>();
            Flag = GameObject.FindGameObjectWithTag(TAG_FLAG).transform;
            FlagStartPosition = Flag.position;
            FlagStartRotation = Flag.rotation;
        }

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            
        }
        #endregion Mono Functions
    }
}
