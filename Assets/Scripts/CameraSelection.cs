using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSelection : MonoBehaviour
{

    public GameObject debugRig;
    public GameObject liveRig;

    // Start is called before the first frame update
    void Start()
    {

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
