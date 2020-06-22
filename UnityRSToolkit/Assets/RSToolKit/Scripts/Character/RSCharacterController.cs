using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSToolkit.Character
{
    public abstract class RSCharacterController : ScriptableObject
    {
        
        public CharacterBase CharacterComponent {get; set;}
        public abstract void Init();
		public abstract void OnCharacterUpdate();
		public abstract void OnCharacterFixedUpdate();
    }
}
