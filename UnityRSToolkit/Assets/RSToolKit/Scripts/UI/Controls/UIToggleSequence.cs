using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RSToolkit.Controls;

namespace RSToolkit.UI.Controls
{
    public class UIToggleSequence : Spawner<Toggle>
    {
        public ToggleGroup ToggleGroupComponent { get; private set; }
        public Button ButtonComponent { get; private set; }

        protected override void InitComponents()
        {
            base.InitComponents();

            ToggleGroupComponent = GetComponentInChildren<ToggleGroup>();
            SpawnParent = ToggleGroupComponent.transform;
            ToggleGroupComponent.allowSwitchOff = false;

            ButtonComponent = GetComponentInChildren<Button>();
            ButtonComponent.transform.SetAsLastSibling();
        }

        protected override void InitEvents()
        {
            base.InitEvents();
            OnSpawnEvent.AddListener(OnSpawnEvent_Listener);
            ButtonComponent.onClick.AddListener(OnClick_Listener); 
        }

        public override bool Init(bool force = false)
        {
            if (!base.Init(force))
            {
                return false;
            }
            for(int i = 0; i < SpawnedGameObjects.Count; i++)
            {
                InitToggle(SpawnedGameObjects[i]);
            }
            return true;
        }

        private void InitToggle(Toggle target)
        {
            target.group = ToggleGroupComponent;
            target.interactable = false;
        }

        public void ToggleNext()
        {
            for(int i = 0; i < SpawnedGameObjects.Count; i++)
            {
                if (SpawnedGameObjects[i].isOn)
                {
                    if (i + 1 < SpawnedGameObjects.Count)
                    {
                        SpawnedGameObjects[i + 1].isOn = true;
                    }
                    else
                    {
                        SpawnedGameObjects[0].isOn = true;
                    }
                    break;
                }
            }
        }

        private void OnSpawnEvent_Listener(Toggle spawn)
        {
            InitToggle(spawn);
        }

        private void OnClick_Listener()
        {
            ToggleNext();
        }
    }
}
