using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSToolkit.Space3D
{
    public class LightNavSpawnMarker : NavSpawnMarker
    {
        public Color CanSpawnColor = new Color(1f, 1f, 1f);
        public Color CannotSpawnColor = new Color(1f, 0f, 0f);

        private Light m_lightcomponent;
        public Light LightComponent
        {
            get
            {
                if(m_lightcomponent == null)
                {
                    m_lightcomponent = GetComponentInChildren<Light>();
                }
                return m_lightcomponent;
            }
            private set => m_lightcomponent = value;
        }

        protected override void Update()
        {
            base.Update();
            LightComponent.color = CanSpawn ? CanSpawnColor : CannotSpawnColor;            
        }

    }
}