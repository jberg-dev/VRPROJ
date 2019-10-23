using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSelection : MonoBehaviour
{

    public GameObject debugRig;
    public GameObject liveRig;
    public GameObject marker;

    // Start is called before the first frame update
    void Awake()
    {
        // This is a marker for the scene building, not live rendering.
        marker.SetActive(false);
        

        if(RayCastingManager.emulator)
        {
            debugRig.SetActive(true);
            liveRig.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            debugRig.SetActive(false);
            liveRig.SetActive(true);
        }



    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
