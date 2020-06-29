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
        private List<BehaviourRootNode> m_behaviourtrees = new List<BehaviourRootNode>();
        private List<BehaviourBlackboard> m_blackboards = new List<BehaviourBlackboard>();

        public BehaviourRootNode CurrentTree { get; private set; } = null;
        public BehaviourRootNode NextTree { get; private set; } = null;

        public BehaviourBlackboard CurrentBlackboard { get; private set; } = null;

        public bool StoppingTree { get; private set; } = false;
        public bool StartingTree { get; private set; } = false;
        public bool StartingTreeSilent { get; private set; } = false;
        /// <summary>
        /// Used for when the Behaviour Tree is mimicking another tree (ex: network peer)
        /// </summary>

        /*
        public void AddBehaviourTree(BehaviourNode behaviourtree)
        {
            m_behaviourtrees.Add(behaviourtree);
        }*/

        private void AddBehaviourTree(BehaviourRootNode behaviourtree)
        {
            m_behaviourtrees.Add(behaviourtree);
            behaviourtree.OnStopped.AddListener(BehaviourtreeOnStopped_Listener);
        }

        public BehaviourRootNode AddBehaviourTree(string name)
        {
            var behaviourtree = new BehaviourRootNode(name);
            AddBehaviourTree(behaviourtree);

            if (CurrentTree == null)
            {
                CurrentTree = behaviourtree;
            }

            return behaviourtree;
        }

        public void RemoveBehaviourTree(BehaviourRootNode behaviourtree)
        {
            m_behaviourtrees.Remove(behaviourtree);
            if (behaviourtree == CurrentTree)
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

        public bool SetCurrentTree(BehaviourRootNode behaviourtree, bool stopCurrentTree, bool addIfNotPresent = true)
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
            NextTree = null;
            StoppingTree = false;
            if (stopCurrentTree)
            {

                if (RequestStopTree())
                {
                    StoppingTree = true;
                    NextTree = behaviourtree;
                }
            }
            if (NextTree == null)
            {
                CurrentTree = behaviourtree;
            }
            return true;
        }

        /// <summary>
        /// Starts the current behaviour tree
        /// </summary>
        /// <param name="silent">If true will not invoke the OnStarted event</param>
        /// <returns>If node successfully started</returns>
        public bool StartTree(bool silent = false)
        {
            var toStartTree = NextTree != null ? NextTree : CurrentTree;
            if (toStartTree != null && toStartTree.Children.Any())
            {
                if (!StoppingTree)
                {
                    toStartTree.StartNode(silent);
                }
                else
                {
                    StoppingTree = false;
                    if (silent)
                    {
                        StartingTreeSilent = true;
                    }
                    else
                    {
                        StartingTree = true;
                    }
                }
                //CurrentTree.Children[0].StartNode();                
                return true;
            }
            return false;
        }

        public bool RequestStopTree()
        {
            if (CurrentTree != null)
            {
                return CurrentTree.RequestStopNode();
            }
            return false;
        }

        private void BehaviourtreeOnStopped_Listener(bool success)
        {
            if (NextTree != null)
            {
                CurrentTree = NextTree;
                NextTree = null;
            }

            if (StartingTree)
            {
                StartingTree = false;
                CurrentTree.StartNode(false);
            }
            else if (StartingTreeSilent)
            {
                StartingTreeSilent = false;
                CurrentTree.StartNode(true);
            }
        }

        private void Update()
        {
            BehaviourNode.UpdateTime(Time.deltaTime);
            CurrentTree?.UpdateRecursively();
            CurrentBlackboard?.Update();
        }
    }
}