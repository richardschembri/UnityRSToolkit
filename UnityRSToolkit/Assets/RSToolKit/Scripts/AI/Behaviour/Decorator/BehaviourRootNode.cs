using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RSToolkit.AI.Behaviour
{

    /// <summary>
    /// The root node of a tree
    /// </summary>
    public class BehaviourRootNode : BehaviourParentNode
    {
        private NodeTimer m_rootTimer;
        public bool IsSilent { get; private set; } = false;

        /// <summary>
        /// The root node of a tree
        /// </summary>
        /// <param name="name"></param>
        public BehaviourRootNode(string name = "Root") : base(name, NodeType.DECORATOR)
        {
            OnChildNodeStopped.AddListener(OnChildNodeStopped_Listener);
            OnStopping.AddListener(OnStopping_Listener);
            OnStoppingSilent.AddListener(OnStoppingSilent_Listener);
            OnStarted.AddListener(OnStarted_Listener);
            OnStartedSilent.AddListener(OnStartedSilent_Listener);
        }

        #region Events

        private void OnStarted_Listener()
        {
            IsSilent = false;
            StartFirstChildNodeOnNextTick();
        }

        private void OnStartedSilent_Listener()
        {
            IsSilent = true;
        }

        private void OnChildNodeStopped_Listener(BehaviourNode child, bool success)
        {
            if (State != NodeState.STOPPING)
            {
                // wait one tick, to prevent endless recursions
                m_rootTimer = StartFirstChildNodeOnNextTick();
            }
            else
            {
                StopNodeOnNextTick(success);
            }
        }

        private void OnStopping_Listener()
        {
            if (this.Children[0].State == NodeState.ACTIVE)
            {
                StopChildren();
                
            }
            else
            {
                RemoveTimer(m_rootTimer);
                StopNodeOnNextTick(true);
            }
        }

        private void OnStoppingSilent_Listener()
        {

            if (this.Children[0].State != NodeState.ACTIVE)
            {
                RemoveTimer(m_rootTimer);
            }
        }

        #endregion Events

        // To Refactor
        public override void SetParent(BehaviourParentNode parent)
        {
            throw new System.Exception("Root nodes cannot have parents");
        }

        public override bool UpdateRecursively(UpdateType updateType = UpdateType.DEFAULT)
        {
            if (IsSilent)
            {
                return false;
            }
            return base.UpdateRecursively(updateType);
        }

        public void Wake()
        {
            IsSilent = false;
        }

        public void Sleep()
        {
            IsSilent = true;
        }

        #region SyncLeaves
        // This is used to sync behaviour trees (for example when it comes to Network play)

        public bool SyncActiveLeaves(List<BehaviourNode> activeLeaves, bool silent = true)
        {
            var myLeaves = GetLeaves(NodeState.ACTIVE).ToList();

            // Recursively stop all leaves that should not be running
            while (myLeaves.Count() > 0)
            {
                var ml = myLeaves[0];

                if (!activeLeaves.Contains(ml))
                {
                    if (!ml.StopNode(silent))
                    {
                        return false;
                    }                    
                }
                myLeaves.Remove(ml);

                if (!ml.Parent.HasChildren(activeLeaves))
                {
                    myLeaves.Add(ml.Parent);
                }
            }
            
            // Recursivly start all nodes that should be running
            for (int i = 0; i < activeLeaves.Count; i++)
            {
                if (activeLeaves[i].State != NodeState.ACTIVE)
                {
                    if (!activeLeaves[i].StartNodePath(silent))
                    {
                        return false;
                    }
                }

            }

            return true;
        }

        public bool SyncActiveLeaves(string[] nodeIDs, bool silent = true)
        {
            var myLeaves = GetLeaves();
            return SyncActiveLeaves(myLeaves.Where(l => nodeIDs.Contains(l.GetUniqueID())).ToList(), silent);

        }

        public bool SyncActiveLeaves(string nodeIDs, char seperator = '|', bool silent = true)
        {
            var nodeIDArray = nodeIDs.Split(seperator);
            return SyncActiveLeaves(nodeIDArray, silent);           
        }

        #endregion
    }
}
