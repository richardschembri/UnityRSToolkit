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
        private List<BehaviourRootNode> _behaviourtrees = new List<BehaviourRootNode>();
        private List<BehaviourBlackboard> _blackboards = new List<BehaviourBlackboard>();

        #region Properties

        public BehaviourRootNode CurrentTree { get; private set; } = null;
        public BehaviourRootNode NextTree { get; private set; } = null;

        public BehaviourBlackboard CurrentBlackboard { get; private set; } = null;

        public bool StoppingTree { get; private set; } = false;
        public bool StartingTree { get; private set; } = false;
        public bool StartingTreeSilent { get; private set; } = false;

        public bool Paused { get; private set; } = false;
        
        #endregion Properties

        public void PauseTree()
        {
            Paused = true;
        }

        public void UnPauseTree()
        {
            Paused = false;
        }

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
            _behaviourtrees.Add(behaviourtree);
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
            _behaviourtrees.Remove(behaviourtree);
            if (behaviourtree == CurrentTree)
            {
                CurrentTree = _behaviourtrees.FirstOrDefault();
            }
        }

        public BehaviourBlackboard AddBlackboard()
        {
            var blackboard = new BehaviourBlackboard();
            _blackboards.Add(blackboard);
            if (CurrentBlackboard == null)
            {
                CurrentBlackboard = blackboard;
            }

            return blackboard;
        }

        public void RemoveBlackboard(BehaviourBlackboard blackboard)
        {
            _blackboards.Remove(blackboard);
            if (blackboard == CurrentBlackboard)
            {
                CurrentBlackboard = _blackboards.FirstOrDefault();
            }
        }

        public bool SetCurrentTree(BehaviourRootNode behaviourtree, bool stopCurrentTree, bool addIfNotPresent = true)
        {
            if (!_behaviourtrees.Contains(behaviourtree))
            {
                if (addIfNotPresent)
                {
                    _behaviourtrees.Add(behaviourtree);
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

        #region SyncLeaves

        public bool SyncActiveLeaves(BehaviourNode[] activeLeaves, bool silent = true)
        {
            return CurrentTree.SyncActiveLeaves(activeLeaves, silent);
        }

        public bool SyncActiveLeaves(string[] nodeIDs, bool silent = true)
        {
            return CurrentTree.SyncActiveLeaves(nodeIDs, silent); ;
        }

        public bool SyncActiveLeaves(string nodeIDs, char seperator = '|', bool silent = true)
        {
            return CurrentTree.SyncActiveLeaves(nodeIDs, seperator, silent);
        }

        #endregion SyncLeaves

        #region Events
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
        #endregion Events

        private void UpdateCommon(BehaviourNode.UpdateType updateType)
        {
            if (Paused)
            {
                return;
            }
            // BehaviourNode.UpdateTime(Time.deltaTime);
            BehaviourNode.OverrideElapsedTime(Time.time);
            CurrentTree?.UpdateRecursively(updateType);            
        }

        #region MonoBehaviour Functions

        void Update()
        {
            UpdateCommon(BehaviourNode.UpdateType.DEFAULT);
            CurrentBlackboard?.Update();
        }

        void FixedUpdate()
        {
            UpdateCommon(BehaviourNode.UpdateType.FIXED);
        }

        void LateUpdate()
        {
            UpdateCommon(BehaviourNode.UpdateType.LATE);
        }

        void OnDestroy()
        {
            for(int i = 0; i < _behaviourtrees.Count; i++)
            {
                _behaviourtrees[i].RemoveAllListenersRecursively();
            }
        }

        #endregion MonoBehaviour Functions
    }
}