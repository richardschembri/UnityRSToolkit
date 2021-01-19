using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSToolkit.AI
{
    public class UITextFollowBotFlyable : UITextFollowBot<BotFlyable>
    {
        protected override void GenerateDebugText()
        {
            base.GenerateDebugText();
            
            _sbDebugText.AppendLine($"[{Target.CurrentFlyableState.ToString()}]");
        }
    }
}
