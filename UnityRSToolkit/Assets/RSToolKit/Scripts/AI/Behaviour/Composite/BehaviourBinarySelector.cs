using System;
using RSToolkit.AI.Behaviour.Task;

namespace RSToolkit.AI.Behaviour.Composite
{
    public class BehaviourBinarySelector : BehaviourParentNode
    {

        public BehaviourNode TrueNode { get { return Children[0]; } }
        public BehaviourNode FalseNode { get { return Children[1]; } }
        private Func<bool> _isConditionMetFunc;


        public BehaviourBinarySelector(Func<bool> isConditionMetFunc, BehaviourNode trueNode, BehaviourNode falseNode) : base("Binary Selector", NodeType.COMPOSITE)
        {
            AddChild(trueNode);
            AddChild(falseNode);
            TrueNode.OnStopped.AddListener(TrueActionOnStopped_Listener);
            FalseNode.OnStopped.AddListener(FalseActionOnStopped_Listener);

            _isConditionMetFunc = isConditionMetFunc;

            OnStarted.AddListener(OnStarted_Listener);
            OnStopping.AddListener(OnStopping_Listener);
        }

        private void OnStarted_Listener()
        {
            if (_isConditionMetFunc())
            {
                TrueNode.StartNodeOnNextTick();
            }
            else
            {
                FalseNode.StartNodeOnNextTick();
            }
        }

        private void OnStopping_Listener()
        {
            if (TrueNode.State == NodeState.ACTIVE)
            {
                TrueNode.RequestStopNodeOnNextTick();
            }
            else if (FalseNode.State == NodeState.ACTIVE)
            {
                FalseNode.RequestStopNodeOnNextTick();
            }
            else
            {
                StopNodeOnNextTick(false);
            }
        }
        private void TrueActionOnStopped_Listener(bool success)
        {
            if (!_isConditionMetFunc() && State == NodeState.ACTIVE)
            {
                FalseNode.StartNodeOnNextTick();
            }
            else
            {
                StopNodeOnNextTick(success);
            }
        }

        private void FalseActionOnStopped_Listener(bool success)
        {
            if (_isConditionMetFunc() && State == NodeState.ACTIVE)
            {
                TrueNode.StartNodeOnNextTick();
            }
            else
            {
                StopNodeOnNextTick(success);
            }
        }

        public bool IsTrueNodeMode()
        {
            return TrueNode.State == NodeState.ACTIVE && FalseNode.State == NodeState.INACTIVE;
        }

        public bool IsFalseNodeMode()
        {
            return FalseNode.State == NodeState.ACTIVE && TrueNode.State == NodeState.INACTIVE;
        }



        public override void Update(UpdateType updateType = UpdateType.DEFAULT)
        {
            base.Update(updateType);

            if (_isConditionMetFunc() && IsFalseNodeMode())
            {
                FalseNode.RequestStopNode();               
            }
            else if (!_isConditionMetFunc() && IsTrueNodeMode())
            {
                TrueNode.RequestStopNode();
            }
        }

    }
}