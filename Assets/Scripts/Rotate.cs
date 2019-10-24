using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class Rotate : MonoBehaviour
{
    public GameObject centerpoint;
    private ISteamVR_Action_Vector2 rotator;

    // Start is called before the first frame update
    void Start()
    {
        centerpoint = GameObject.FindGameObjectWithTag("CenterPoint");
        rotator = SteamVR_Actions.default_RotateSphere;        
    }

    // Update is called once per frame
    void Update()
    {
        if(rotator.delta != Vector2.zero)
        {
            transform.RotateAround(centerpoint.transform.position, new Vector3(rotator.delta.x, rotator.delta.y),
                Mathf.Sqrt(Mathf.Pow(rotator.delta.x, 2f) + Mathf.Pow(rotator.delta.y, 2f)) * 20);
        }
    }
}
