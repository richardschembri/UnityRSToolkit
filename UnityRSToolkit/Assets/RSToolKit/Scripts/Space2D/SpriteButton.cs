using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSToolkit.Space2D
{

    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteButton : MonoBehaviour
    {
        public SpriteRenderer SpriteRendererComponent { get; private set; }
        public Sprite defaultSprite;
        public Sprite pressedSprite;

        public KeyCode mappedKey;
        // Start is called before the first frame update
        void Awake()
        {
            SpriteRendererComponent = this.GetComponent<SpriteRenderer>();
            SpriteRendererComponent.sprite = defaultSprite;
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(mappedKey))
            {
                SpriteRendererComponent.sprite = pressedSprite;
            }

            if (Input.GetKeyUp(mappedKey))
            {
                SpriteRendererComponent.sprite = defaultSprite;
            }
        }
    }
}