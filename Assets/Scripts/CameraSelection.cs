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
        

#if UNITY_EDITOR
        debugRig.SetActive(true);
        liveRig.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
#else
        debugRig.setActive(false);
        liveRig.setActive(true);
#endif
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
