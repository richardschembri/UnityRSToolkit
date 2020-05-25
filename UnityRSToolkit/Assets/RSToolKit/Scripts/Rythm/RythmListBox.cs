using RSToolkit.Helpers;
using RSToolkit.UI.Controls;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RSToolkit.Rythm
{
    public class RythmListBox : UIListBox
    {
        public float tempo = 5f;

        public float BeatsPerSecond
        {
            get
            {
                return (tempo / 60f); //* Time.deltaTime;
            }
        }

        public bool AutoStart = false;
        public bool HasStarted { get; private set; }

        public void StartScrolling()
        {
            HasStarted = true;
        }

        public void StopScrolling()
        {
            HasStarted = false;
        }

        protected override void Awake()
        {
            base.Awake();
            if (AutoStart)
            {
                StartScrolling();
            }
            //OnShiftMostVertical.AddListener()
        }

        protected override void Update()
        {
            base.Update();
            ScrollRectComponent.content.anchoredPosition = new Vector2(0f, ScrollRectComponent.content.anchoredPosition.y + BeatsPerSecond);
        }

        public void SpawnPrompts()
        {
            ClearSpawnedListItems();
            for(int i = 0; i < 10; i++)
            {
                AddListItem();
            }
        }

        private void OnShiftMostVertical_Listener(RectTransform toPlace, RectTransformHelpers.VerticalPosition verticalPosition)
        {
            SetPrompt(toPlace.GetComponent<RythmPrompt>());
        }

        private void SetPrompt(RythmPrompt rythmPrompt)
        {

        }
    }
}