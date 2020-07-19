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

        private System.Func<bool> _singleFrameFunc = null;
        private System.Func<bool, ActionResult> _multiFrameFunc = null;
        private System.Func<ActionRequest, ActionResult> _multiFrameRequestFunc = null;
        private System.Action _singleFrameAction = null;

        private System.Action _actionFixedUpdate = null;
        private System.Action _actionLateUpdate = null;

        private bool _bWasBlocked = false;
        private bool _skipping = false;

        private ActionResult _actionResult = ActionResult.PROGRESS;
        private ActionRequest _actionRequest = ActionRequest.START;
        private void Init()
        {
            OnStarted.AddListener(OnStarted_Listener);
            OnStartedSilent.AddListener(OnStartedSilent_Listener);
            OnStopping.AddListener(OnStopping_Listener);
            _actionResult = ActionResult.PROGRESS;
        }

        public void SetFixedUpdate(System.Action actionFixedUpdate)
        {
            _actionFixedUpdate = actionFixedUpdate;
        }

        public void SetLateUpdate(System.Action actionLateUpdate)
        {
            _actionLateUpdate = actionLateUpdate;
        }

        #region Constructors

        /// <summary>
        /// Single frame action (Always finishes successfully)
        /// </summary>
        /// <param name="action"></param>
        /// <param name="name"></param>
        public BehaviourAction(System.Action action, string name = NODE_NAME) : base(name, NodeType.TASK)
        {
            _singleFrameAction = action;
            Init();
        }

        /// <summary>
        /// A single frame action which can succeed or fail
        /// </summary>
        /// <param name="singleFrameFunc"></param>
        /// <param name="name"></param>
        public BehaviourAction(System.Func<bool> singleFrameFunc, string name = NODE_NAME) : base(name, NodeType.TASK)
        {
            _singleFrameFunc = singleFrameFunc;
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
            _multiFrameFunc = multiFrameFunc;
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
            _multiFrameRequestFunc = multiFrameRequestFunc;
            Init();
        }

        
        #endregion Constructors
 
        private bool OtherUpdate(UpdateType updateType)
        {
            switch (updateType)
            {
                case UpdateType.DEFAULT:
                    return false;
                case UpdateType.FIXED when _actionFixedUpdate != null:
                    _actionFixedUpdate.Invoke();
                    break;
                case UpdateType.LATE when _actionLateUpdate != null:
                    _actionFixedUpdate.Invoke();
                    break;
            }
            return true;
        }

        public override void Update(UpdateType updateType)
        {
            base.Update();

            if (OtherUpdate(updateType))
            {
                return;
            }

            if (_singleFrameAction != null)
            {
                _singleFrameAction.Invoke();
                StopNode(true);
            }
            else if (_multiFrameFunc != null || _multiFrameRequestFunc != null)
            {
                if (_multiFrameFunc != null)
                {
                    _actionResult = _multiFrameFunc.Invoke(false);
                }
                else if (_multiFrameRequestFunc != null)
                {
                    _actionResult = _multiFrameRequestFunc.Invoke(_actionRequest);
                    _actionRequest = _bWasBlocked ? ActionRequest.START : ActionRequest.UPDATE;
                }

                if (_actionResult == ActionResult.BLOCKED)
                {
                    _bWasBlocked = true;
                }
                else if (_actionResult != ActionResult.PROGRESS)
                {
                    // OnStopped.Invoke(m_actionResult == ActionResult.SUCCESS);
                    StopNode(_actionResult == ActionResult.SUCCESS);
                }
            }
            else if (_singleFrameFunc != null)
            {
                // OnStopped.Invoke(m_singleFrameFunc.Invoke());
                StopNode(_singleFrameFunc.Invoke());
            }
        }

        public void RequestSkipAction()
        {
            _skipping = true;
            RequestStopNode();
        }

        #region Events
        private void OnStarted_Common()
        {
            _bWasBlocked = false;
            _skipping = false;

            if (_multiFrameRequestFunc != null)
            {
                _actionRequest = ActionRequest.START;
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
            if (_multiFrameFunc != null)
            {
                _actionResult = _multiFrameFunc.Invoke(true);
            }
            else if (_multiFrameRequestFunc != null)
            {
                _actionResult = _multiFrameRequestFunc.Invoke(_skipping ? ActionRequest.SKIP : ActionRequest.CANCEL);
            }
            // OnStopped.Invoke(m_actionResult == ActionResult.SUCCESS);

            // StopNode(m_actionResult == ActionResult.SUCCESS);
            StopNodeOnNextTick(_actionResult == ActionResult.SUCCESS);            
        }

        #endregion Events
    }

}
