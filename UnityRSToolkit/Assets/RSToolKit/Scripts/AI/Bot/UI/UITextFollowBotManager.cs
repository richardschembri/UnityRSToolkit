using RSToolkit.UI.Controls;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSToolkit.AI
{
    public class UITextFollowBotManager<T,J> : UITextFollow3DManager<T,J>
        where T : UITextFollowBot<J>
        where J : Bot
    {
    }
}
