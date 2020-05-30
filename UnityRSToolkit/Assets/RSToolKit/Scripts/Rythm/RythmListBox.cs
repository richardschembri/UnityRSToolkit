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

        public bool HasStarted { get; private set; } = false;

        public RythmManager ParentRythmManager { get; private set; }

        public RythmHitArea HitArea { get { return ParentRythmManager.HitAreaComponent; } }

        protected override void Awake()
        {
            base.Awake();
            ParentRythmManager = GetComponent<RythmManager>();
        }

        protected override void Update()
        {
            base.Update();
            if (!HasStarted)
            {
                return;
            }
            ScrollRectComponent.content.anchoredPosition = new Vector2(0f, ScrollRectComponent.content.anchoredPosition.y + BeatsPerSecond);
        }

        public void StartScrolling()
        {
            HasStarted = true;
        }

        public void StopScrolling()
        {
            HasStarted = false;
        }

        public void SpawnPrompts(bool clearItems = true)
        {
            if (clearItems)
            {
                ClearSpawnedListItems();
            }
            
            for(int i = 0; i < 10; i++)
            {
                AddListItem();
            }
        }

        public bool HasPrompts()
        {
            return ListItemSpawner.SpawnedGameObjects.Count > 0;
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