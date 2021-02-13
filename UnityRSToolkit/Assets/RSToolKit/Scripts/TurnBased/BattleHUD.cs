using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RSToolkit.Character;

namespace RSToolkit.TurnBased{
    public class BattleHUD : RSMonoBehaviour
    {
        public Text nameText;
        public Text levelText;

        public Slider healthSlider;

        public BattleCharacterController Target {get; private set;}

        public override void ResetValues(){
            Target = null;
        }

        public bool SetValues(BattleCharacterController target){
            if(Target == target){
                return false;
            }
            nameText.text = target.DisplayName;
            levelText.text = $"Lvl {target.level}";
            healthSlider.maxValue = target.HealthComponent.MaxHealth;
            healthSlider.value = target.HealthComponent.CurrentHealth;
            target.HealthComponent.OnHealthChanged.AddListener(OnHealthChanged_Listener);
            return true;
        }

        private void OnHealthChanged_Listener(int value){
            healthSlider.value = value;
        }
    }
}
