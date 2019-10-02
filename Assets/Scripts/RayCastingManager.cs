using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCastingManager : MonoBehaviour
{
    LineRenderer lr;

    // Start is called before the first frame update
    void Start()
    {
        lr = GetComponent<LineRenderer>();
    }


    bool renderLaserLine = false;

    // Update is called once per frame
    void Update()
    {

#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            renderLaserLine = true;
            lr.enabled = true;
        }

        else if (Input.GetMouseButtonUp(0))
        {
            DoRaycast();
        }

        // TODO
        // BELOW NEEDS REWORK TO PROPERLY WORK WITH THE HANDHELDS!
#else
        if(Input.GetKeyDown("fire1"))
        {
            renderLaserLine = true;
            lr.enabled = true;
        }
        else if (Input.GetKeyUp("fire1"))
        {
            DoRaycast();
        }
#endif

        // Update the laser lines position as long as the line is to be displayed.
        if (renderLaserLine)
        {

            Vector3 origin, target;

#if UNITY_EDITOR

            origin = Camera.main.ViewportToWorldPoint(new Vector3(1.5f, 1.5f, 0.05f));
            target = Camera.main.transform.forward * 99f;

#else

            // TODO
            // INSERT ORIGIN/TARGET FOR HANDHELDS HERE

#endif
            lr.SetPosition(0, origin);
            lr.SetPosition(1, target);
        }
    }

    void DoRaycast()
    {
        RaycastHit hit;

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 99f))
        {
            // TODO
            // Upgrade this to TryGetComponent();
            Debug.Log(hit.collider.gameObject.GetComponent<DataManager>().name);
        }

        renderLaserLine = false;
        lr.enabled = false;
    }
}
