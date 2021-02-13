using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RSToolkit.Character;

namespace RSToolkit.TurnBased{
    public class BattleCharacterController : RSCharacterController
    {
        public uint level;
        public uint damage;
        private RSCharacterHealth _healthComponent;
        public RSCharacterHealth HealthComponent{
            get{
                if(_healthComponent == null){
                    _healthComponent = GetComponent<RSCharacterHealth>();
                }
                return _healthComponent;
            }
        }
    }
}
