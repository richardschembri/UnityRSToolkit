using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RSToolkit.UI.Controls;
using System.Text;
using RSToolkit.AI.Locomotion;

namespace RSToolkit.AI
{

    public class UITextFollowBot : UITextFollow3D 
    {
        protected BotDebugValues _botDebugValues;

        protected override void GenerateDebugText()
        {
            base.GenerateDebugText();
            // _sbDebugText.AppendLine($"Interaction State: [{_botDebugValues.TargetBot.CurrentInteractionState.ToString()}]");
            _sbDebugText.AppendLine($"[{_botDebugValues.TargetBot.CurrentInteractionState.ToString()}]");
            if (_botDebugValues.TargetBotLocomotive != null)
            {
                // _sbDebugText.AppendLine($"Movement State: [{_botDebugValues.TargetBotLocomotive.CurrentFState.ToString()}]");
                _sbDebugText.AppendLine($"[{_botDebugValues.TargetBotLocomotive.CurrentFState.ToString()}]");

                if(_botDebugValues.TargetBotLocomotive.BotWanderManagerComponent != null){

                    _sbDebugText.AppendLine($"[{_botDebugValues.TargetBotLocomotive.BotWanderManagerComponent.FSM.CurrentState.ToString()}]");
                }
            }
        }

        #region MonoBehaviour Functions

        protected override void Awake()
        {
            base.Awake();
            _botDebugValues = new BotDebugValues(Target.GetComponent<Bot>(), Target.GetComponent<BotLocomotive>());
        }

        #endregion MonoBehaviour Functions
    }
}
