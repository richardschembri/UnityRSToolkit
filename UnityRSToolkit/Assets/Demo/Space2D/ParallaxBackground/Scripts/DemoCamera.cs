using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoCamera : MonoBehaviour
{
    [SerializeField] private float _speed = .1f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(Input.GetKey(KeyCode.D)){
            this.transform.position =  this.transform.position + new Vector3(_speed, 0f, 0f);
        }else if(Input.GetKey(KeyCode.A)){
            this.transform.position =  this.transform.position + new Vector3(-_speed, 0f, 0f);
        }
        
    }
}
