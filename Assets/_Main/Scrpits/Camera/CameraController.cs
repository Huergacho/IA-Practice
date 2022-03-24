using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    CameraModel _model;
    public float cooldown;
    // Start is called before the first frame update
    void Start()
    {
        _model = GetComponent<CameraModel>();
    }

    // Update is called once per frame
    void Update()
    {
            _model.CheckTargets();  
        
    }
}
