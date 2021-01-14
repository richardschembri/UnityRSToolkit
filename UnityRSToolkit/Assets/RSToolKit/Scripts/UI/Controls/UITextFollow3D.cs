using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace RSToolkit.UI.Controls
{
    public class UITextFollow3D : MonoBehaviour
    {
        public Transform Target;
        public Vector3 Offset = Vector3.zero;
        protected StringBuilder _sbDebugText;
        protected Text _text;

        public float DisplayAtDistance = 100f;

        protected virtual void GenerateDebugText()
        {
            _sbDebugText.Clear();
            _sbDebugText.AppendLine($"-=[{Target.name}]=-");
        }

        #region MonoBehaviour Functions
        protected virtual void Awake()
        {
            _text = GetComponent<Text>();
            if(_text == null)
            {
                _text = GetComponentInChildren<Text>();
            }
            _sbDebugText = new StringBuilder();
        }

        // Update is called once per frame
        protected virtual void Update()
        {
            if(Target != null && Camera.allCamerasCount > 0 && Vector3.Distance(Camera.allCameras[0].transform.position, Target.position ) <= DisplayAtDistance)
            {
                GenerateDebugText();
                _text.text = _sbDebugText.ToString();
                // transform.position = Camera.main.WorldToScreenPoint(Target.position) + Offset;
                transform.position = Camera.allCameras[0].WorldToScreenPoint(Target.position) + Offset;
            }
        }
        #endregion MonoBehaviour Functions
    }
}
