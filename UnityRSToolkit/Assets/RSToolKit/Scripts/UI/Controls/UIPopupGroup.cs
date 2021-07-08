using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace RSToolkit.UI.Controls
{
    public class UIPopupGroup : RSMonoBehaviour
    {
        [SerializeField] private UIPopup[] _uiPopups;

        public UIPopup[] UIPopups { get { return _uiPopups; } }

        [SerializeField] private bool _AllowCloseAll = true;

        [SerializeField] private bool _OpenOnStart = false;

        public bool allowCloseAll { get { return _AllowCloseAll; } set { _AllowCloseAll = value; } }

        public UnityEvent OnCancelAll { get; private set; } = new UnityEvent();
        private void ValidateUIPopupIsInGroup(UIPopup uiPopup)
        {
            if (uiPopup == null || !_uiPopups.Contains(uiPopup ))
                throw new ArgumentException($"UIPopup {uiPopup.name} is not part of UIPopupGroup {this.name}");
        }

        public void NotifyUIPopupOpen(UIPopup uiPopup, bool showControls = true, bool silent = false)
        {
            ValidateUIPopupIsInGroup(uiPopup);

            // disable all toggles in the group
            for (var i = 0; i < _uiPopups.Length; i++)
            {
                if (_uiPopups[i] == uiPopup)
                    continue;

                _uiPopups[i].ClosePopup(showControls, silent);
            }
        }

        /*
        public void UnregisterUIPopup(UIPopup uiPopup, bool showControls = true, bool silentOpen = false, bool silentClose = false)
        {
            if (_uiPopups.Contains(uiPopup))
                _uiPopups.Remove(uiPopup);

            if (!allowCloseAll && !AnyUIPopupsOpen() && _uiPopups.Length != 0)
            {
                _uiPopups[0].OpenPopup(false, silentOpen);
                NotifyUIPopupOpen(_uiPopups[0], showControls, silentClose);
            }
        }

        public void RegisterUIPopup(UIPopup uiPopup, bool showControls = true, bool silentOpen = false, bool silentClose = false)
        {
            if (!_uiPopups.Contains(uiPopup))
                _uiPopups.Add(uiPopup);

            if (!allowCloseAll && !AnyUIPopupsOpen())
            {
                uiPopup.OpenPopup(false, silentOpen);
                NotifyUIPopupOpen(uiPopup, showControls, silentClose);
            }
        }
        */

        public bool AnyUIPopupsOpen()
        {
            return _uiPopups.Any(x => x.IsOpen());
        }

        public IEnumerable<UIPopup> GetOpenUIPopups()
        {
            return _uiPopups.Where(x => x.IsOpen());
        }

        public void CloseAllUIPopups(bool showControls = true, bool silent = false)
        {
            bool oldAllowCloseAll = _AllowCloseAll;
            _AllowCloseAll = true;
            for (var i = 0; i < _uiPopups.Length; i++)
                _uiPopups[i].ClosePopup(showControls, silent);

            _AllowCloseAll = oldAllowCloseAll ;
        }

        public void OpenFirstUIPopup(bool keepCache = false)
        {
            _uiPopups[0].OpenPopup(keepCache);
        }

        /// <summary>
        /// Returns false if cannot open next UIPopup
        /// </summary>
        /// <param name="keepCache"></param>
        /// <returns></returns>
        public bool OpenNextUIPopup(bool keepCache = false)
        {
            var uiPopup = GetOpenedPopup();
            if(uiPopup == null)
            {
                OpenFirstUIPopup(keepCache);
                return true;
            }
            int currentIndex = Array.IndexOf(_uiPopups, GetOpenedPopup());
            currentIndex++;
            if(currentIndex >= _uiPopups.Length)
            {
                LogInDebugMode($"Already in last of UIPopupGroup {gameObject.name} list");
                return false;
            }

            _uiPopups[currentIndex].OpenPopup(keepCache);

            return true;
        }

        /// <summary>
        /// Returns false if cannot open previous UIPopup
        /// </summary>
        /// <param name="keepCache"></param>
        /// <returns></returns>
        public bool OpenPreviousUIPopup(bool keepCache = false)
        {
            var uiPopup = GetOpenedPopup();
            if(uiPopup == null)
            {
                return false;
            }
            int currentIndex = Array.IndexOf(_uiPopups, GetOpenedPopup());
            currentIndex--;
            if(currentIndex <= 0)
            {
                LogInDebugMode($"Already in first of UIPopupGroup {gameObject.name} list");
                return false;
            }

            _uiPopups[currentIndex].OpenPopup(keepCache);

            return true;
        }

        public UIPopup GetOpenedPopup()
        {
            return _uiPopups.FirstOrDefault(p => p.IsOpen());
        }

        private void OnOpenPopup_Listener(UIPopup uiPopup, bool keepCache)
        {
            NotifyUIPopupOpen(uiPopup);
        }
        private void OnClosePopup_Listener(UIPopup uiPopup)
        {
            if (!allowCloseAll && !AnyUIPopupsOpen() && _uiPopups.Length > 1)
            {
                int toOpenIndex = uiPopup != _uiPopups[0] ? 0 : 1;
                _uiPopups[toOpenIndex].OpenPopup();
                NotifyUIPopupOpen(_uiPopups[toOpenIndex]);
            }
        }

        public void CancelAll()
        {
            CloseAllUIPopups(true, true);
            OnCancelAll.Invoke();
        }

        #region RSMonoBehavior Functions
        public override bool Init(bool force = false)
        {
            if(!base.Init(force))
            {
                return false;
            }

            for(int i = 0; i < _uiPopups.Length; i++)
            {
                _uiPopups[i].OnOpenPopup.AddListener(OnOpenPopup_Listener);
                _uiPopups[i].OnClosePopup.AddListener(OnClosePopup_Listener);
            }

            return true;
        }
        #endregion RSMonoBehavior Functions
        #region MonoBehavior Functions
        void Start()
        {
            bool opened = false;
            for(int i = 0; i < _uiPopups.Length; i++)
            {
                if (_uiPopups[i].IsOpen())
                {
                    if (!opened)
                    {
                        opened = true;
                    }
                    else
                    {
                        _uiPopups[i].ClosePopup(true, true);
                    }
                }
            }

            if (_OpenOnStart && !opened)
            {
                _uiPopups[0].OpenPopup();
            } 
        }
        #endregion MonoBehavior Functions
    }
}
