using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RSToolkit.Character
{
	[RequireComponent(typeof(RSCharacterController))]
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
				if(_maxHealth > Health){
					Health = (int)_maxHealth;
				}
			}
		}

		[SerializeField]
		private int _health = 100;
		public int Health{
			get{
				return _health;
			}
			protected set{
				_health = value;
				if (!IsAlive)
				{
					Die();
				}
			}
		}
		public RSCharacterController.RSCharacterEvent OnDie = new RSCharacterController.RSCharacterEvent();
		RSCharacterController _characterControllerComponent;
		RSCharacterController CharacterControllerComponent{
			get{
				if(_characterControllerComponent == null){
					_characterControllerComponent = GetComponent<RSCharacterController>();
				}
				return _characterControllerComponent;
			}
		}

		public bool IsAlive{get{ return Health > 0;}}

		public virtual void TakeDamage(uint damage)
		{
			Health -= (int)damage;
		}

		public virtual void Heal(int health){

			Health += (int)health;
		}

		void Die()
		{
			OnDie.Invoke(CharacterControllerComponent);
		}

		public void ResetHealth(){
            Health = (int)MaxHealth;
		}
	}
}