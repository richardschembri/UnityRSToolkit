using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSToolkit.AI.Behaviour.Decorator{
	public class BotIsWithinSight : BehaviourCondition
	{
        private const string NODE_NAME = "IsWithinSight";
		private BotPartVision _botVisionComponent;
		public string TargetTag {get; set;}
		private bool _inverseCondition;

        public BotIsWithinSight(BotPartVision botVisionComponent, string targetTag,
									BehaviourNode decoratee, bool inverseCondition = false,
									string nodeName = NODE_NAME,
									AbortRule abortRule = AbortRule.NONE) : base(decoratee, nodeName, abortRule)
		{
			_botVisionComponent = botVisionComponent;
			TargetTag = targetTag;
			_inverseCondition = inverseCondition;
			Init(IsWithinSight);
		}

        private bool IsWithinSight(){
			bool  result = _botVisionComponent.IsWithinSight(TargetTag);
			if(_inverseCondition){
				return !result;
			}
			return result;
        }
	}
}
