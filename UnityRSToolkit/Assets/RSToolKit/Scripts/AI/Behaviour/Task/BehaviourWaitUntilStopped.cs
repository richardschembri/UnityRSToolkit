using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RSToolkit.AI.Behaviour.Task
{
    /// <summary>
    /// Wait until stopped by some other node. 
    /// It's often used to park at the end of a Selector, waiting for any beforehead sibling BlackboardCondition, BlackboardQuery or Condition to become active.
    /// </summary>
    public class BehaviourWaitUntilStopped : BehaviourNode
    {
        private bool m_waitResult;
        public BehaviourWaitUntilStopped(bool waitResult) : base("WaitUntilStopped", NodeType.TASK)
        {
            m_waitResult = waitResult;
            OnStopping.AddListener(OnStopping_Listener);
        }

        private void OnStopping_Listener()
        {
            OnStopped.Invoke(m_waitResult);
        }
    }
}