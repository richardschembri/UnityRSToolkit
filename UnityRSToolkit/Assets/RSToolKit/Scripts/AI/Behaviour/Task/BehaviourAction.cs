using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSToolkit.AI.Behaviour.Task
{

    public class BehaviourAction : BehaviourNode
    {
        private const string NODE_NAME = "Action";

        public enum ActionResult
        {
            SUCCESS,
            FAILED,
            BLOCKED,
            PROGRESS
        }

        public enum ActionRequest
        {
            START,
            UPDATE,            
            CANCEL,
            SKIP
        }

        private System.Func<bool> m_singleFrameFunc = null;
        private System.Func<bool, ActionResult> m_multiFrameFunc = null;
        private System.Func<ActionRequest, ActionResult> m_multiFrameRequestFunc = null;
        private System.Action m_singleFrameAction = null;
        private bool m_bWasBlocked = false;
        private bool m_skipping = false;

        private ActionResult m_actionResult = ActionResult.PROGRESS;
        private ActionRequest m_actionRequest = ActionRequest.START;
        private void Init()
        {
            OnStarted.AddListener(OnStarted_Listener);
            OnStartedSilent.AddListener(OnStartedSilent_Listener);
            OnStopping.AddListener(OnStopping_Listener);
            m_actionResult = ActionResult.PROGRESS;
        }
        #region Constructors

        /// <summary>
        /// Single frame action (Always finishes successfully)
        /// </summary>
        /// <param name="action"></param>
        /// <param name="name"></param>
        public BehaviourAction(System.Action action, string name = NODE_NAME) : base(name, NodeType.TASK)
        {
            m_singleFrameAction = action;
            Init();
        }

        /// <summary>
        /// A single frame action which can succeed or fail
        /// </summary>
        /// <param name="singleFrameFunc"></param>
        /// <param name="name"></param>
        public BehaviourAction(System.Func<bool> singleFrameFunc, string name = NODE_NAME) : base(name, NodeType.TASK)
        {
            m_singleFrameFunc = singleFrameFunc;
            Init();
        }

        /// <summary>
        /// Multiple frame action which returns it's state:
        /// - BLOCKED: Action is not yet ready to run.
        /// - PROGRESS: Action is in progress.
        /// - SUCCESS: Action finished successfully.
        /// - FAILED: Action finished unsuccessfully.
        /// 
        /// The bool parameter that is passed to the delegate turns true when the task has to be aborted. The action returns SUCCESS or FAILED. 
        /// When considering using this type of Action, you should also think about creating a custom subclass of the Task instead.
        /// </summary>
        /// <param name="multiFrameFunc">The function that will run every frame</param>
        /// <param name="name">The Action`s name</param>
        public BehaviourAction(System.Func<bool, ActionResult> multiFrameFunc, string name = NODE_NAME) : base(name, NodeType.TASK)
        {
            m_multiFrameFunc = multiFrameFunc;
            Init();
        }

        /// <summary>
        /// Multiple frame action which returns it's state:
        /// - BLOCKED: Your action is not yet ready.
        /// - PROGRESS: Action is in progress.
        /// - SUCCESS: Action finished successfully.
        /// - FAILED: Action finished unsuccessfully.
        /// 
        /// The ActionRequest will give you a state information: 
        /// - START: First tick the action or BLOCKED was returned in the last tick.
        /// - UPDATE: PROGRESS returned in the last tick.
        /// - CANCEL: the action should be canceled and return SUCCESS or FAILED.
        /// - SKIP: invoked by RequestSkipAction. The action should be skipped and return SUCCESS or FAILED. 
        /// </summary>
        /// <param name="multiFrameRequestFunc"></param>
        /// <param name="name"></param>
        public BehaviourAction(System.Func<ActionRequest, ActionResult> multiFrameRequestFunc, string name = NODE_NAME) : base(name, NodeType.TASK)
        {
            m_multiFrameRequestFunc = multiFrameRequestFunc;
            Init();
        }

        
        #endregion Constructors
 
        public override void Update()
        {
            base.Update();
            if (m_singleFrameAction != null)
            {
                m_singleFrameAction.Invoke();
                // OnStopped.Invoke(true);
                StopNode(true);
            }
            else if (m_multiFrameFunc != null || m_multiFrameRequestFunc != null)
            {
                if (m_multiFrameFunc != null)
                {
                    m_actionResult = m_multiFrameFunc.Invoke(false);
                }
                else if (m_multiFrameRequestFunc != null)
                {
                    m_actionResult = m_multiFrameRequestFunc.Invoke(m_actionRequest);
                    m_actionRequest = m_bWasBlocked ? ActionRequest.START : ActionRequest.UPDATE;
                }

                if (m_actionResult == ActionResult.BLOCKED)
                {
                    m_bWasBlocked = true;
                }
                else if (m_actionResult != ActionResult.PROGRESS)
                {
                    // OnStopped.Invoke(m_actionResult == ActionResult.SUCCESS);
                    StopNode(m_actionResult == ActionResult.SUCCESS);
                }
            }
            else if (m_singleFrameFunc != null)
            {
                // OnStopped.Invoke(m_singleFrameFunc.Invoke());
                StopNode(m_singleFrameFunc.Invoke());
            }
        }

        public void RequestSkipAction()
        {
            m_skipping = true;
            RequestStopNode();
        }

        #region Events
        private void OnStarted_Common()
        {
            m_bWasBlocked = false;
            m_skipping = false;

            if (m_multiFrameRequestFunc != null)
            {
                m_actionRequest = ActionRequest.START;
            }
        }

        private void OnStarted_Listener()
        {
            OnStarted_Common();
        }

        private void OnStartedSilent_Listener()
        {
            OnStarted_Common();
        }

        private void OnStopping_Listener()
        {
            if (m_multiFrameFunc != null)
            {
                m_actionResult = m_multiFrameFunc.Invoke(true);
            }
            else if (m_multiFrameRequestFunc != null)
            {
                m_actionResult = m_multiFrameRequestFunc.Invoke(m_skipping ? ActionRequest.SKIP : ActionRequest.CANCEL);
            }
            // OnStopped.Invoke(m_actionResult == ActionResult.SUCCESS);

            // StopNode(m_actionResult == ActionResult.SUCCESS);
            StopNodeOnNextTick(m_actionResult == ActionResult.SUCCESS);            
        }

        #endregion Events
    }

}
