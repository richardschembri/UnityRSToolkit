using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using RSToolkit.Helpers;

namespace RSToolkit.Character
{
	// [RequireComponent(typeof(RSCharacterController))]
	public class RSCharacterHealth : RSMonoBehaviour
	{
		[SerializeField]
		public uint _maxHealth = 100;

		public uint MaxHealth{
			get{
				return _maxHealth;
			}
			set{
				_maxHealth = value;
				if(_maxHealth > CurrentHealth){
					CurrentHealth = (int)_maxHealth;
				}
			}
		}

		public bool Died{get; private set;} = false; 

		[SerializeField]
		private int _currentHealth = 100;
		public int CurrentHealth{
			get{
				return _currentHealth;
			}
			protected set{
				if(_currentHealth != value){
					_currentHealth = value;
                    OnHealthChanged.Invoke(_currentHealth);
                }
			}
		}
		public float CurrentHealthPercent{
			get{
				return MathHelpers.GetValuePercent(CurrentHealth, MaxHealth);
			}
		}
		public RSCharacterController.RSCharacterEvent OnDie = new RSCharacterController.RSCharacterEvent();
		public class OnValueChangedEvent : UnityEvent<int> {} 
		public OnValueChangedEvent OnHealthChanged = new OnValueChangedEvent();
		RSCharacterController _characterControllerComponent;
		RSCharacterController CharacterControllerComponent{
			get{
				if(_characterControllerComponent == null){
					_characterControllerComponent = GetComponent<RSCharacterController>();
				}
				return _characterControllerComponent;
			}
		}

		public bool IsAlive{get{ return CurrentHealth > 0;}}

		public virtual bool TakeDamage(uint damage)
		{
			if(IsAlive){
				CurrentHealth -= (int)damage;
				if (!IsAlive)
				{
					Die();
				}

				return true;
			}
			return false;
		}

		public virtual void Heal(int health){

			CurrentHealth += (int)health;
		}

		bool Die()
		{
			if(!Died){
				Died = true;
				OnDie.Invoke(CharacterControllerComponent);
				return true;
			}		
			return false;
		}

		public bool Revive(uint health){
			if(Died){
				CurrentHealth = (int)health;
				return true;
			}
			return false;
		}
		public bool Revive(){
            return Revive(MaxHealth);
		}
	}
}