namespace RSToolkit.UI.Controls
{
    using UnityEngine;
    using UnityEngine.EventSystems;
    using RSToolkit.Helpers;

    public class UIPopup : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler {

        public bool Draggable = false;
        // Use this for initialization

        void Start(){

            this.transform.SetAsLastSibling();
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

        public void OpenPopup()
        {
            this.gameObject.SetActive(true);
            this.transform.SetAsLastSibling();
        }

        public void ClosePopup()
        {
            this.gameObject.SetActive(false);
        }

        public void DestroyPopup()
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

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
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
            this.transform.SetAsLastSibling();
        }
    }
}
