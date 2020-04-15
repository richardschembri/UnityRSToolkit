using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSToolkit.AI.Behaviour
{
    public class BehaviourManager : SingletonMonoBehaviour<BehaviourManager>
    {
        private List<BehaviourNode> m_behaviourtrees = new List<BehaviourNode>();
        private List<BehaviourBlackboard> m_blackboards = new List<BehaviourBlackboard>();

        public void AddBehaviourTree(BehaviourNode behaviourtree)
        {
            m_behaviourtrees.Add(behaviourtree);
        }

        public void RemoveBehaviourTree(BehaviourNode behaviourtree)
        {
            m_behaviourtrees.Remove(behaviourtree);
        }

        public void AddBlackboard(BehaviourBlackboard blackboard)
        {
            m_blackboards.Add(blackboard);
        }
        public void RemoveBlackboard(BehaviourBlackboard blackboard)
        {
            m_blackboards.Remove(blackboard);
        }

        private void Update()
        {
            for(int i = 0; i < m_behaviourtrees.Count; i++)
            {
                m_behaviourtrees[i].UpdateRecursively();
            }

            for(int i = 0; i < m_blackboards.Count; i++)
            {
                m_blackboards[i].Update();
            }
        }
    }
}