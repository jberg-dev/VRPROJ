using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;

public class RayCastingManager : MonoBehaviour
{
    LineRenderer lr;
    public GameObject textObject;
    public static bool emulator = false;
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

        SteamVR_Actions.default_GrabPinch.AddOnStateDownListener(TriggerDown, leftController);
        SteamVR_Actions.default_GrabPinch.AddOnStateUpListener(TriggerUp, leftController);

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
    private Button potential_button = null;
    private string nameOfHovered = string.Empty;
    private bool rightControllerTriggerDown = false;
    private bool rightControllerTriggerUp = false;
    private GameObject rControllerModel = null;
    Vector3 origin, target;

    // Update is called once per frame
    void Update()
    {
        // Null out the previous runs vars
        nameOfHovered = string.Empty;
        potential_hit = null;
        potential_manager = null;
        origin = Vector3.zero;
        target = Vector3.zero;

        // Get the controller model for when you're not in emulator mode.
        if (!emulator && rControllerModel == null)
        {
            rControllerModel = GameObject.FindGameObjectWithTag("LeftController");
        }
        
        // Decide the raycasting origins
        if (emulator)
        {
            origin = Camera.main.ViewportToWorldPoint(new Vector3(1.5f, 1.5f, 0.05f));
            target = Camera.main.transform.forward * 999f;
        }
        else
        {
            if (rControllerModel == null)
                Debug.LogError("COULD NOT FIND RIGHT CONTROLLER MODEL");
            origin = rControllerModel.transform.position;
            target = rControllerModel.transform.forward * 999f; // Mathf.Infinity;
        }

        if (renderLaserLine)
        {
            if (DoRaycast(out potential_hit, origin, target))
            {
                if (potential_hit.TryGetComponent<DataManager>(out potential_manager))
                {
                    nameOfHovered = potential_manager.nodeData.FullName;
                }
                else if(potential_hit.TryGetComponent<Button>(out potential_button))
                {
                    nameOfHovered = potential_button.name;
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
            else if(potential_button != null)
            {
                controlMenu.DisplayInformationMenu(null);
                potential_button.onClick.Invoke();
            }

            if(potential_hit != null)
                Debug.Log("Potential hit: " + potential_hit.ToString());

            renderLaserLine = false;
            lr.enabled = false;
            rightControllerTriggerUp = false;
        }

        // Update the laser lines position as long as the line is to be displayed.
        if (renderLaserLine)
        {

            
            AttemptToSetText(nameOfHovered);

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

    bool DoRaycast(out GameObject hit, Vector3 source, Vector3 target)
    {
        RaycastHit hitCast;

        if (Physics.Raycast(source, target, out hitCast, 99f))
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
