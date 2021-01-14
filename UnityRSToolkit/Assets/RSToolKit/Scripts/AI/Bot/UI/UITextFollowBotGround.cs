using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSToolkit.AI
{
    public class UITextFollowBotGround : UITextFollowBot
    {
        BotGround _targetBotGround;
        protected override void GenerateDebugText()
        {
            base.GenerateDebugText();
            // _sbDebugText.AppendLine($"Ground State: [{_targetBotGround .CurrentStatesGround.ToString()}]");
            _sbDebugText.AppendLine($"[{_targetBotGround .CurrentStatesGround.ToString()}]");
        }

        protected override void Awake()
        {
            base.Awake();
            _targetBotGround = _botDebugValues.TargetBot.GetComponent<BotGround>();
        }
    }
}
