using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class Rotate : MonoBehaviour
{
    

    // Start is called before the first frame update
    void Start()
    {
        SteamVR_Actions.default_RotateSphere.AddOnAxisListener(RotateSphereAround, SteamVR_Input_Sources.RightHand);
        //SteamVR_Actions.default_RotateSphere.AddOnChangeListener(RotateSphereAround, SteamVR_Input_Sources.RightHand);
        
    }

    private void RotateSphereAround(SteamVR_Action_Vector2 fromAction, SteamVR_Input_Sources fromSource, bool active)
    {
        Debug.Log("RotateSphere Active:");

    }

    private void RotateSphereAround(SteamVR_Action_Vector2 fromAction, SteamVR_Input_Sources fromSource, Vector2 axis, Vector2 delta)
    {
        Debug.Log("RotateSphere:");
        Debug.Log(String.Format("Axis: {0} | Delta: {1}", fromAction.axis, fromAction.delta));
    }


    // Update is called once per frame
    void Update()
    {
        // Spin the object around the world origin at 20 degrees/second.

    }
}
