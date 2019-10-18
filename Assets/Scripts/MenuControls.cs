using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRPROJ.Datastructure;
using Valve.VR;

public class MenuControls : MonoBehaviour
{

    public GameObject ButtonMenu;
    public GameObject InformationMenu;
    public GameObject ErrorDialog;
    SteamVR_Action_Boolean menuPress;

    // Text fields of the InformationMenu
    private Text 
        fullName = null, 
        company = null, 
        email = null, 
        phone = null, 
        address = null, 
        registered = null, 
        noFriends = null;

    private bool menuPressed = false;

    /// <summary>
    /// Make sure there are no Menus visible on the start of the application.
    /// </summary>
    void Awake()
    {
        ButtonMenu.SetActive(true);
    }

    void Start()
    {
        //ResetMenus();
        menuPress.AddOnStateDownListener(TriggerMenuDown, SteamVR_Input_Sources.Any);
    }

    void TriggerMenuDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromController)
    {
        menuPressed = true;
    }

        /// <summary>
        /// Lazy initializer for the text fields of the Information Menu.
        /// </summary>
        void InitializeInformationMenuTextFields()
    {
        // Save the current state of active.
        bool previousState = InformationMenu.activeSelf;

        // Menu MUST be active for these objects to be gettable/exist.
        InformationMenu.SetActive(true);

        GameObject fullNameObj = GameObject.FindGameObjectWithTag("IM_NAME");
        GameObject companyObj = GameObject.FindGameObjectWithTag("IM_COMPANY");
        GameObject emailObj = GameObject.FindGameObjectWithTag("IM_EMAIL");
        GameObject phoneObj = GameObject.FindGameObjectWithTag("IM_PHONE");
        GameObject addressObj = GameObject.FindGameObjectWithTag("IM_ADDRESS");
        GameObject registeredObj = GameObject.FindGameObjectWithTag("IM_REGISTERED");
        GameObject noFriendsObj = GameObject.FindGameObjectWithTag("IM_NOFRIENDS");

        GameObject[] menufields = { fullNameObj, companyObj, emailObj, phoneObj, addressObj, registeredObj, noFriendsObj };
        Text[] textFields = { fullName, company, email, phone, address, registered, noFriends };

        for (int i = 0; i < menufields.Length; i++)
        {
            if (attemptGetTextFromParent(menufields[i], out Text attempt))
                textFields[i] = attempt;
            else
                Debug.LogError("Object " + i + " failed quietly?");
        }

        fullName = textFields[0];
        company = textFields[1];
        email = textFields[2];
        phone = textFields[3];
        address = textFields[4];
        registered = textFields[5];
        noFriends = textFields[6];

        // Return the menu to previous state of active.
        InformationMenu.SetActive(previousState);

    }

    /// <summary>
    /// Helper method to get a text field from an object.
    /// </summary>
    /// <param name="parent">The GameObject to get the text field from</param>
    /// <param name="field">The out parameter of the Text object to return</param>
    /// <returns>True if the getting were successful, otherwise false</returns>
    private bool attemptGetTextFromParent(GameObject parent, out Text field)
    {
        if (parent.TryGetComponent<Text>(out field))
            return true;

        Debug.LogError("Failed to get text for object with tag [" + parent.tag + "]");
        field = null;
        return false;
    }

    /// <summary>
    /// Keep track of all the menu calls and their handling.
    /// </summary>
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.H) || menuPressed)
        {
            // Make it a toggle so you press it again to turn it on and off.
            ButtonMenu.SetActive(!ButtonMenu.activeSelf);
            menuPressed = false;
        }
    }

    /// <summary>
    /// Helper function to display a SocialNetworkNodes' information on the screen.
    /// </summary>
    /// <param name="snn">The node which data should be displayed. Accepts null, but it closes the menu</param>
    public void DisplayInformationMenu(SocialNetworkNode snn)
    {
        // Have to initialize the fields before usage.
        if(fullName == null)
        {
            InitializeInformationMenuTextFields();
        }

        // Hide the menu if you click outside a node
        if (snn == null)
        {
            InformationMenu.SetActive(false);
            return;
        }
        else
            InformationMenu.SetActive(true);

        // Grab the data from the node and set the displaying text fields;
        fullName.text = "Name: " + snn.FullName;
        company.text = "Company: " + snn.Company;
        email.text = "Email: " + snn.Email;
        phone.text = "Phone: " + snn.Phone;
        address.text = "Address: " + snn.Address;
        registered.text = "Joined: " + snn.Registered;
        noFriends.text = "Number of friends: " + snn.NumberFriends;
    }

    /// <summary>
    /// Helper method to reset all the menus from showing.
    /// </summary>
    public void ResetMenus()
    {
        ButtonMenu.SetActive(false);
        InformationMenu.SetActive(false);
        ErrorDialog.SetActive(false);
    }
}
