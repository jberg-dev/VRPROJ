using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using VRPROJ.Datastructure;

public class Rotate : MonoBehaviour
{
    public GameObject centerpoint;
    private DataStructure data;
    private ISteamVR_Action_Vector2 rotator;

    // Start is called before the first frame update
    void Start()
    {
        centerpoint = GameObject.FindGameObjectWithTag("CenterPoint");
        rotator = SteamVR_Actions.default_RotateSphere;

        GameObject go = GameObject.FindGameObjectWithTag("DataStructure");
        if (go.TryGetComponent<DataStructure>(out DataStructure attempt))
        {
            data = attempt;
            Debug.Log("Loaded datastructure successfully");
        }
        else
        {
            Debug.LogError("Failed to load the datastructure.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(rotator.delta != Vector2.zero)
        {
            transform.RotateAround(centerpoint.transform.position, new Vector3(rotator.axis.x, rotator.axis.y),
                Mathf.Sqrt(Mathf.Pow(rotator.axis.x, 2f) + Mathf.Pow(rotator.axis.y, 2f)) * 20 * Time.deltaTime);

            data.AssureLinesFollow();
        }
    }
}
