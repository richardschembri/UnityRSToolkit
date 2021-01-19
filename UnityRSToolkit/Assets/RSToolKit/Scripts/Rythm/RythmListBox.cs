using RSToolkit.Helpers;
using RSToolkit.UI.Controls;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RSToolkit.Rythm
{
    public class RythmListBox : UIListBox<RythmPrompt>
    {
        public KeyCode promptKey;

        public float tempo = 5f;

        public float BeatsPerSecond
        {
            get
            {
                return (tempo / 60f); //* Time.deltaTime;
            }
        }

        public bool HasStarted { get; private set; } = false;

        public RythmManager m_parentRythmManager;
        public RythmManager ParentRythmManager
        {
            get
            {
                if(m_parentRythmManager == null)
                {
                    m_parentRythmManager = GetComponentInParent<RythmManager>();
                }
                return m_parentRythmManager;
            }
        }

        public RythmHitArea HitArea { get { return ParentRythmManager.HitAreaComponent; } }

        protected override void Awake()
        {
            base.Awake();
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

        public RythmPrompt[] SpawnPrompts(bool clearItems = true)
        {
            if (clearItems)
            {
                ClearSpawnedListItems();
            }

            var rythmPrompts = new RythmPrompt[10];
            for(int i = 0; i < rythmPrompts.Length; i++)
            {
                rythmPrompts[i] = AddListItem().GetComponent<RythmPrompt>();
                rythmPrompts[i].name = $"Rythm Prompt {i}";
                rythmPrompts[i].SetPrompt(promptKey);
                rythmPrompts[i].SetPromptText($"{promptKey.ToString()} {i}");
            }

            return rythmPrompts;
        }

        public bool HasPrompts()
        {
            return SpawnedGameObjects.Count > 0;
        }

        private void OnShiftMostVertical_Listener(RectTransform toPlace, RectTransformHelpers.VerticalPosition verticalPosition)
        {
            toPlace.GetComponent<RythmPrompt>().SetPrompt(promptKey);
        }

    }
}