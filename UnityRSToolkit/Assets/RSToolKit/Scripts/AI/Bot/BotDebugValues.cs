using RSToolkit.AI.Locomotion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSToolkit.AI
{
    public struct BotDebugValues
    {
        public Bot TargetBot;
        public BotLocomotive TargetBotLocomotive;
        public Object Waypoint;
        public bool Fullspeed; // = false;
        public float InteractionCooldown; // = 0f;
        public float CurrentSpeed; // = 0;
        public string DebugDistanceType; // = "";

        public BotDebugValues(Bot targetBot, BotLocomotive targetBotLocomotive = null,
                                Object waypoint = null, bool fullspeed = false,
                                float interactionCooldown = 0f, float currentSpeed = 0,
                                string debugDistanceType = "")
        {
            TargetBot = targetBot;
            TargetBotLocomotive = targetBotLocomotive;
            Waypoint = waypoint;
            Fullspeed = fullspeed;
            InteractionCooldown = interactionCooldown;
            CurrentSpeed = currentSpeed;
            DebugDistanceType = debugDistanceType;
        }
    }
}
