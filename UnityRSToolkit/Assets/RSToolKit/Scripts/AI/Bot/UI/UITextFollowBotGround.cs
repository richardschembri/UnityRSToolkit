using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSToolkit.AI
{
    public class UITextFollowBotGround : UITextFollowBot<BotGround>
    {

        protected override void GenerateDebugText()
        {
            base.GenerateDebugText();
            
            _sbDebugText.AppendLine($"[{Target.CurrentStatesGround.ToString()}]");
        }

    }
}
