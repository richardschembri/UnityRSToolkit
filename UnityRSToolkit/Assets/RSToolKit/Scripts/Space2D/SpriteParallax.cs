using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSToolkit.Space2D
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteParallax : MonoBehaviour
    {
        [SerializeField] private float _parallaxEffectMultiplier; 

        private Transform _cameraTransform;
        private Vector3 _lastCameraPosition;        
        private Vector3 _deltaMovement;

        private float _textureUnitSizeX;
        private float _offsetPositionX;

        private void Start(){
            _cameraTransform = Camera.main.transform;
            _lastCameraPosition = _cameraTransform.position;
            Sprite sprite = GetComponent<SpriteRenderer>().sprite;
            _textureUnitSizeX = sprite.texture.width / sprite.pixelsPerUnit;
        }
        private void LateUpdate(){
            _deltaMovement = _cameraTransform.position - _lastCameraPosition;
            
            transform.position += new Vector3(_deltaMovement.x * _parallaxEffectMultiplier, _deltaMovement.y * _parallaxEffectMultiplier, 0f);
            _lastCameraPosition = _cameraTransform.position;
            if (Mathf.Abs(_cameraTransform.position.x - transform.position.x) >= _textureUnitSizeX){
                _offsetPositionX = (_cameraTransform.position.x - transform.position.x) % _textureUnitSizeX;
                transform.position = new Vector3(_cameraTransform.position.x, transform.position.y);
            }
        }
    }
}