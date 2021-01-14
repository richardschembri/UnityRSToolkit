using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSToolkit.AI
{
    public class UITextFollowBotFlyable : UITextFollowBot
    {
        BotFlyable _targetBotFlyable;

        protected override void GenerateDebugText()
        {
            base.GenerateDebugText();
            // _sbDebugText.AppendLine($"Flyable State: [{_targetBotFlyable.CurrentFlyableState.ToString()}]");
            _sbDebugText.AppendLine($"[{_targetBotFlyable.CurrentFlyableState.ToString()}]");
        }
        // Update is called once per frame
        protected override void Awake()
        {
            base.Awake();
            _targetBotFlyable = _botDebugValues.TargetBot.GetComponent<BotFlyable>();
        }
    }
}
