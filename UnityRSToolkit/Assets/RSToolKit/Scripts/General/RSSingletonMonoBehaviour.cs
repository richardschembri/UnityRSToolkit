using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RSToolkit{
    public class RSSingletonMonoBehaviour<T> : RSMonoBehaviour where T: RSSingletonMonoBehaviour<T>
    {
        public static T Instance { get; protected set; }
    
        protected override void Awake()
        {
            base.Awake();
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                throw new System.Exception("An instance of this singleton already exists.");
            }
            else
            {
                Instance = (T)this;
            }
        }
    }

}