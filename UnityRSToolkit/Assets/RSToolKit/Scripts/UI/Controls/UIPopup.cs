﻿using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Linq;
using System.Collections;

namespace RSToolkit.UI.Controls
{
    [DisallowMultipleComponent]
    [AddComponentMenu("RSToolKit/Controls/UIPopup")]
    public class UIPopup : RSMonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler {

        public bool Draggable = false;
        public bool ShowOnTop = true;

        public GameObject[] ControlsToHide;
        public UIPopup[] HigherPriorityPopups;

        [SerializeField]
        private float _popupTimeoutSeconds = -1f;
        public float PopupTimeoutSeconds { get => _popupTimeoutSeconds; set => _popupTimeoutSeconds = value; }
        private float _popupEndTime = 0f;

        [SerializeField]
        private bool _allowResetPopupTimeout = true;
        public bool AllowResetPopupTimeout { get => _allowResetPopupTimeout; set => _allowResetPopupTimeout = value; }

        public class OnOpenPopupEvent : UnityEvent<UIPopup, bool> { }
        public class OnClosePopupEvent : UnityEvent<UIPopup> { }
        [Header("Events")]
        public OnOpenPopupEvent OnOpenPopup = new OnOpenPopupEvent();
        public OnClosePopupEvent OnClosePopup = new OnClosePopupEvent();

        #region MonoBehaviour Functions

        protected virtual void Start(){
            PopupOnTop();
        }

        void Update()
        {

            if (Input.anyKeyDown)
            {
                ResetTimeoutTime();
            }
        }

        #endregion MonoBehaviour Functions

        protected virtual IEnumerator PopupTimeout(){
            // yield return new WaitForSeconds(PopupTimeoutSeconds);
            yield return new WaitUntil(() => Time.time > _popupEndTime);
            ClosePopup();
        }

        void PopupOnTop(){
            if(ShowOnTop){
                this.transform.SetAsLastSibling();
            }
        }

        public void SetPosition(Vector3 position)
        {
            var pt = this.transform.position;
            float xPos = position.x;
            float yPos = position.y;

            if (xPos < Screen.width * 0.15f)
            {
                xPos = Screen.width * 0.15f;
            }
            if (xPos > Screen.width * 0.85f)
            {
                xPos = Screen.width * 0.85f;
            }

            if (yPos < Screen.height * 0.325f)
            {
                yPos = Screen.height * 0.325f;
            }
            if (yPos > Screen.height * 0.70f)
            {
                yPos = Screen.height * 0.70f;
            }
            this.transform.position = new Vector3(xPos, yPos, pt.z);
        }

        private void ToggleControls(){
            for(int i = 0; i < ControlsToHide.Length; i++){
                ControlsToHide[i].SetActive(!IsOpen());                
            }
        }
        public virtual void OpenPopup(bool keepCache = false)
        {
            OpenPopup(keepCache, false);
        }
        public virtual void OpenPopup(bool keepCache, bool silent)
        {
            if(IsOpen() || HigherPriorityPopups.Any(p => p.IsOpen())){
                return; 
            }


            this.gameObject.SetActive(true);
            ToggleControls();
            PopupOnTop();
            if (!silent)
            {
                OnOpenPopup.Invoke(this, keepCache);
            }
            if(PopupTimeoutSeconds > 0){
                ResetTimeoutTime();
                StartCoroutine("PopupTimeout");
            }
        }

        public virtual void ClosePopup(bool showControls, bool silent)
        {
            this.gameObject.SetActive(false);
            if(showControls){
                ToggleControls();
            }

            if (!silent)
            {
                OnClosePopup.Invoke(this);
            }
            
            StopCoroutine("PopupTimeout");
        }
        public virtual void ClosePopup(bool showControls = true)
        {
            ClosePopup(showControls, false);
        }

        public virtual void DestroyPopup()
        {
            Destroy(this.gameObject);
        }

        void IEndDragHandler.OnEndDrag(PointerEventData eventData)
        {
            if (!Draggable)
            {
                return;
            }

        }
        public void ResetTimeoutTime()
        {
            if (PopupTimeoutSeconds > 0)
            {
                _popupEndTime = Time.time + PopupTimeoutSeconds;
            }
        }
        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            if (AllowResetPopupTimeout)
            {
                ResetTimeoutTime();
            }
            if (!Draggable)
            {
                return;
            }

            //transform.position += (Vector3)eventData.delta; //= Input.mousePosition;
            SetPosition(transform.position + (Vector3)eventData.delta);
        }

        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
        {
            this.transform.SetAsLastSibling();
            if (!Draggable)
            {
                return;
            }
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            if (AllowResetPopupTimeout)
            {
                ResetTimeoutTime();
            }
            this.transform.SetAsLastSibling();
        }

        public bool IsOpen(){
            return gameObject.activeSelf;
        }

    }
}
