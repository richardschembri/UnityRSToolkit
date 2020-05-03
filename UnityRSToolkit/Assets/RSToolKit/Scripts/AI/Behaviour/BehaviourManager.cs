using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RSToolkit.AI.Behaviour
{
    [DisallowMultipleComponent]
    public class BehaviourManager : MonoBehaviour
    {
        private List<BehaviourParentNode> m_behaviourtrees = new List<BehaviourParentNode>();
        private List<BehaviourBlackboard> m_blackboards = new List<BehaviourBlackboard>();

        public BehaviourParentNode CurrentTree { get; private set; } = null;
        public BehaviourBlackboard CurrentBlackboard { get; private set; } = null;

        /*
        public void AddBehaviourTree(BehaviourNode behaviourtree)
        {
            m_behaviourtrees.Add(behaviourtree);
        }*/

        public BehaviourParentNode AddBehaviourTree(string name)
        {
            var behaviourtree = new BehaviourParentNode(name, BehaviourNode.NodeType.ROOT);
            m_behaviourtrees.Add(behaviourtree);

            if(CurrentTree == null)
            {
                CurrentTree = behaviourtree;
            }

            return behaviourtree;
        }

        public void RemoveBehaviourTree(BehaviourParentNode behaviourtree)
        {
            m_behaviourtrees.Remove(behaviourtree);
            if(behaviourtree == CurrentTree)
            {
                CurrentTree = m_behaviourtrees.FirstOrDefault();
            }
        }

        public BehaviourBlackboard AddBlackboard()
        {
            var blackboard = new BehaviourBlackboard();
            m_blackboards.Add(blackboard);
            if (CurrentBlackboard == null)
            {
                CurrentBlackboard = blackboard;
            }

            return blackboard;
        }

        public void RemoveBlackboard(BehaviourBlackboard blackboard)
        {
            m_blackboards.Remove(blackboard);
            if (blackboard == CurrentBlackboard)
            {
                CurrentBlackboard = m_blackboards.FirstOrDefault();
            }
        }

        public bool SetCurrentTree(BehaviourParentNode behaviourtree, bool addIfNotPresent = true)
        {
            if (!m_behaviourtrees.Contains(behaviourtree))
            {
                if (addIfNotPresent)
                {
                    m_behaviourtrees.Add(behaviourtree);
                }
                else
                {
                    return false;
                }
            }
            CurrentTree = behaviourtree;
            return true;
        }

        // To Refactor
        public bool StartTree()
        {
            if (CurrentTree != null && CurrentTree.Children.Any())
            {
                CurrentTree.StartNode();                
                CurrentTree.Children[0].StartNode();                
                return true;
            }
            return false;
        }

        private void Update()
        {
            CurrentTree?.UpdateRecursively();
            CurrentBlackboard?.Update();
        }
    }
}