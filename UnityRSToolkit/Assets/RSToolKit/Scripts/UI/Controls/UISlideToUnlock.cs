namespace RSToolkit.UI.Controls
{
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;
    using RSToolkit.Helpers;

    [RequireComponent(typeof(Scrollbar))]
    public class UISlideToUnlock : MonoBehaviour
    {
        public float springSpeed = 0.5f;
        Scrollbar m_scrollbarComponent;

        public UnityEvent onUnlock = new UnityEvent();


        Scrollbar ScrollbarComponent
        {
            get
            {
                if (m_scrollbarComponent == null)
                {
                    m_scrollbarComponent = this.GetComponent<Scrollbar>();
                }
                return m_scrollbarComponent;
            }
        }
        // Use this for initialization
        void Start()
        {
            ScrollbarComponent.onValueChanged.AddListener((float val) => { onValueChanged(val); });
            onUnlock.AddListener(() => { Debug.Log("Unlock Event Triggered!"); });
        }

        void onValueChanged(float val)
        {
            if (val >= 1f)
            {

                ResetValue();
                onUnlock.Invoke();
            }
        }
        public void ResetValue()
        {
            ScrollbarComponent.value = 0f;
        }

        private void Awake()
        {
        }

        // Update is called once per frame
        void Update()
        {
            if (!Input.GetMouseButton(0))
            {
                if (ScrollbarComponent.value > 0)
                {
                    ScrollbarComponent.value = MathHelpers.Berp(ScrollbarComponent.value, 0f, Time.deltaTime * springSpeed);
                }
            }
        }

    }
}
