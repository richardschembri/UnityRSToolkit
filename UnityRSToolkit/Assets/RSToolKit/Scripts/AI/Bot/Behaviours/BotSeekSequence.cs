using RSToolkit.AI.Behaviour.Task;
using RSToolkit.AI.Behaviour.Decorator;
using RSToolkit.AI.Locomotion;

namespace RSToolkit.AI.Behaviour.Composite
{
using StopMovementConditions = BotLocomotive.StopMovementConditions;
	public class BotSeekWithinSightSequence : BehaviourSequence
	{
		public BotIsWithinSight IsWithinSight {get; private set;}
		public BotDoSeek DoSeek {get; private set;}

		private void Init(BotLocomotive botLocomotiveComponent, BotPartVision botVisionComponent,
				StopMovementConditions stopMovementCondition , string targetTag, BehaviourNode[] nodes = null){
			DoSeek = new BotDoSeek(botLocomotiveComponent, stopMovementCondition);
			IsWithinSight = new BotIsWithinSight(botVisionComponent, targetTag, DoSeek);
			AddChild(IsWithinSight);
			if(nodes != null){
				for(int i = 0; i < nodes.Length; i++){
					AddChild(nodes[i]);
				}
			}
		}

		public BotSeekWithinSightSequence(BotLocomotive botLocomotiveComponent,
								BotPartVision botVisionComponent, string targetTag,
								StopMovementConditions stopMovementCondition = StopMovementConditions.WITHIN_INTERACTION_DISTANCE) : base(false){
			Init(botLocomotiveComponent, botVisionComponent,
								stopMovementCondition, targetTag);
		}

		public BotSeekWithinSightSequence(BehaviourNode[] nodes, BotLocomotive botLocomotiveComponent,
								BotPartVision botVisionComponent, string targetTag,
								StopMovementConditions stopMovementCondition = StopMovementConditions.WITHIN_INTERACTION_DISTANCE)
								: base(false){
			Init(botLocomotiveComponent, botVisionComponent,
								stopMovementCondition, targetTag, nodes);
		}
	}
}
