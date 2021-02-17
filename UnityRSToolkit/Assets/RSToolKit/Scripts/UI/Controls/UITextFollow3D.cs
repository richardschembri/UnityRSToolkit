using System;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RSToolkit.UI.Controls
{
    public class UITextFollow3D<T> : RSMonoBehaviour where T : MonoBehaviour
    {
        private Camera _targetCamera = null;

        [SerializeField]
        private T _target;
        public T Target
        {
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
        protected bool _is3D = false;

        private Action<string> SetText;

        protected virtual void SetTarget(T target)
        {
            _target = target;
            CheckActive();
        }

        protected virtual void GenerateDebugText()
        {
            _sbDebugText.Clear();
            _sbDebugText.AppendLine($"-=[{Target.name}]=-");
        }

        public override bool Init(bool force = false)
        {
            if(base.Init(force)){
                CheckActive();
                return true;
            }

            return false;
        }

        public void CheckActive()
        {
            this.gameObject.SetActive(_target != null);
        }

        #region MonoBehaviour Functions
        protected override void Awake()
        {
            base.Awake();
            if (_targetCamera == null)
            {
                _targetCamera = Camera.allCameras[0];
            }

            _text = GetComponent<Text>();
            if (_text == null)
            {
                _text = GetComponentInChildren<Text>();
            }

            _textMeshPro = GetComponent<TextMeshPro>(); ;
            if (_textMeshPro == null)
            {
                _textMeshPro = GetComponentInChildren<TextMeshPro>(); ;
            }

            _textMesh = GetComponent<TextMesh>(); ;

            if (_textMesh == null)
            {
                _textMesh = GetComponentInChildren<TextMesh>(); ;
            }

            if (_text != null)
            {
                _is3D = false;
                SetText = (string value) => { _text.text = value; };
            }
            else if (_textMeshPro != null)
            {
                _is3D = true;
                SetText = (string value) => { _textMeshPro.text = value; };
            }
            else if (_textMesh != null)
            {
                _is3D = true;
                SetText = (string value) => { _textMesh.text = value; };
            }

            _sbDebugText = new StringBuilder();
        }

        // Update is called once per frame
        protected virtual void Update()
        {
            if (Target != null && _targetCamera != null)
            // && Vector3.Distance(_targetCamera.transform.position, Target.transform.position ) <= DisplayAtDistance)
            {
                GenerateDebugText();
                SetText(_sbDebugText.ToString());

                if (_is3D)
                {
                    transform.position = Target.transform.position + OffsetPosition;
                }
                else
                {
                    transform.position = _targetCamera.WorldToScreenPoint(Target.transform.position) + OffsetPosition;
                }

            }
            else
            {
                this.gameObject.SetActive(false);
            }
        }
        #endregion MonoBehaviour Functions
    }
}
