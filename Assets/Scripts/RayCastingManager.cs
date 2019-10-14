using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;

public class RayCastingManager : MonoBehaviour
{
    LineRenderer lr;
    public GameObject textObject;
    private Text hudDisplayText;
    private MenuControls controlMenu;
    private SteamVR_Input_Sources leftController = SteamVR_Input_Sources.LeftHand;
    private SteamVR_Input_Sources rightController = SteamVR_Input_Sources.RightHand;

    // Start is called before the first frame update
    void Start()
    {
        lr = GetComponent<LineRenderer>();


        GameObject menu = GameObject.FindGameObjectWithTag("MenuControl");
        if (menu == null)
            Debug.LogError("FAILED TO LOCATE MENU OBJECT");

        if (menu.TryGetComponent<MenuControls>(out MenuControls attempt))
        {
            controlMenu = attempt;
        }
        else
            Debug.LogError("FAILED TO LOCATE MENUS MENU CONTROLLER");

        SteamVR_Actions.default_GrabPinch.AddOnStateDownListener(TriggerDown, rightController);
        SteamVR_Actions.default_GrabPinch.AddOnStateUpListener(TriggerUp, rightController);
    }

    void TriggerDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromController)
    {
        rightControllerTriggerDown = true;
    }

    void TriggerUp(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromController)
    {
        rightControllerTriggerUp = true;
    }

    // Keep the frequent use objects as class variables to prevent
    // churning memory too much from allocating new objects every frame.
    private bool renderLaserLine = false;
    private GameObject potential_hit = null;
    private DataManager potential_manager = null;
    private string nameOfHovered = string.Empty;
    private bool rightControllerTriggerDown = false;
    private bool rightControllerTriggerUp = false;

    // Update is called once per frame
    void Update()
    {
        nameOfHovered = string.Empty;
        potential_hit = null;
        potential_manager = null;

        if (renderLaserLine)
        {
            // TODO
            // UPDATE SO IT CAN HIT OTHER THINGS LIKE MENUS TOO.

            if (DoRaycast(out potential_hit))
            {
                if (potential_hit.TryGetComponent<DataManager>(out potential_manager))
                {
                    nameOfHovered = potential_manager.nodeData.FullName;
                }
            }
        }


        if (Input.GetMouseButtonDown(0) || rightControllerTriggerDown)
        {
            renderLaserLine = true;
            lr.enabled = true;
            rightControllerTriggerDown = false;
        }
        else if (Input.GetMouseButtonUp(0) || rightControllerTriggerUp)
        {
            if (potential_manager != null)
            {
                Debug.Log(potential_manager.nodeData.ToString());
                controlMenu.DisplayInformationMenu(potential_manager.nodeData);
            }
            else
                controlMenu.DisplayInformationMenu(null);

            renderLaserLine = false;
            lr.enabled = false;
            rightControllerTriggerUp = false;
        }

        // Update the laser lines position as long as the line is to be displayed.
        if (renderLaserLine)
        {

            Vector3 origin, target;
            AttemptToSetText(nameOfHovered);

#if UNITY_EDITOR

            origin = Camera.main.ViewportToWorldPoint(new Vector3(1.5f, 1.5f, 0.05f));
            target = Camera.main.transform.forward * 99f;

#else
            origin = GameObject.FindGameObjectWithTag("RightController").transform.forward;
            target = GameObject.FindGameObjectWithTag("RightController").transform.forward * Mathf.Infinity;

#endif


            lr.SetPosition(0, origin);
            lr.SetPosition(1, target);
        }
    }

    void AttemptToSetText(string textToSet)
    {
        if(hudDisplayText == null)
        {
            Text potential_text;

            if (textObject.TryGetComponent<Text>(out potential_text))
            {
                hudDisplayText = potential_text;
            }
            else
            {
                Debug.LogWarning("Failed to get Text Component");
            }
        }
        else
        {
            hudDisplayText.text = textToSet;
        }
    }

    bool DoRaycast(out GameObject hit)
    {
        RaycastHit hitCast;

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitCast, 99f))
        {
            hit = hitCast.collider.gameObject;
            return true;
        }
        else
        {
            hit = null;
            return false;
        }
    }
}
