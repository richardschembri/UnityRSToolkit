using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RSToolkit.AI.Behaviour.Task
{
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