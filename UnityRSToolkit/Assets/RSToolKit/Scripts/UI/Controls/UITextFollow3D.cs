using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RSToolkit.UI.Controls
{
    public class UITextFollow3D<T> : RSMonoBehaviour where T : MonoBehaviour
    {
        [SerializeField]
        private T _target;
        public T Target {
            get { return _target; }
            set
            {
                SetTarget(value);
            }
        }

        public Vector3 OffsetPosition = Vector3.zero;
        protected StringBuilder _sbDebugText;
        protected Text _text;
        protected TextMesh _textMesh;
        protected TextMeshPro _textMeshPro;

        protected virtual void SetTarget(T target)
        {
            _target = target;
            CheckActive();
        }

        public float DisplayAtDistance = 100f;

        protected virtual void GenerateDebugText()
        {
            
            _sbDebugText.Clear();
            _sbDebugText.AppendLine($"-=[{Target.name}]=-");
        }
       
        protected override void Init()
        {
            base.Init();

            CheckActive();
        }

        public void CheckActive()
        {
            this.gameObject.SetActive(_target != null);
        }

        #region MonoBehaviour Functions
        protected override void Awake()
        {
            base.Awake();

            _text = GetComponent<Text>();
            if(_text == null)
            {
                _text = GetComponentInChildren<Text>();
            }

            _textMeshPro = GetComponent<TextMeshPro>(); ;
            if (_textMeshPro == null)
            {
                _textMeshPro = GetComponentInChildren<TextMeshPro>();;
            }

            _textMesh = GetComponent<TextMesh>(); ;
            if (_textMesh == null)
            {
                _textMesh = GetComponentInChildren<TextMesh>();;
            }

            _sbDebugText = new StringBuilder();
        }

        // Update is called once per frame
        protected virtual void Update()
        {
            if(Target != null && Camera.allCamerasCount > 0 && Vector3.Distance(Camera.allCameras[0].transform.position, Target.transform.position ) <= DisplayAtDistance)
            {
                GenerateDebugText();
                bool is3D = true;
                if (_text != null)
                {
                    _text.text = _sbDebugText.ToString();
                    is3D = false;
                } else if (_textMeshPro != null)
                {
                    _textMeshPro.text = _sbDebugText.ToString();
                }else if (_textMesh != null)
                {
                    _textMesh.text = _sbDebugText.ToString();
                }

                if (is3D)
                {
                    transform.position = Target.transform.position + OffsetPosition;
                }
                else
                {
                    // transform.position = Camera.main.WorldToScreenPoint(Target.position) + Offset;
                    transform.position = Camera.allCameras[0].WorldToScreenPoint(Target.transform.position) + OffsetPosition;
                }

            }
        }
        #endregion MonoBehaviour Functions
    }
}
