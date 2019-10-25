using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRPROJ.Datastructure;
using Valve.VR;

public class MenuControls : MonoBehaviour
{

    public GameObject MainMenu;
    public GameObject FilterMenu;
    public GameObject InformationMenu;
    public GameObject FileSelectionMenu;

    private DataStructure data;
    private Slider activeSlider;
    ISteamVR_Action_Vector2 modSlider;
    SteamVR_Action_Boolean MenuPress;

    // Text fields of the InformationMenu
    private Text 
        fullName = null, 
        company = null, 
        email = null, 
        phone = null, 
        address = null, 
        registered = null, 
        noFriends = null;

    /// <summary>
    /// Make sure there are no Menus visible on the start of the application.
    /// </summary>
    void Awake()
    {

    }

    void Start()
    {
        modSlider = SteamVR_Actions.default_ChangeSliderValue;
        SteamVR_Actions.default_MenuPress.AddOnStateDownListener(TriggerMenuDown, SteamVR_Input_Sources.Any);
        ResetMenus();
        
    }

    void TriggerMenuDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromController)
    {
        MainMenu.SetActive(!MainMenu.activeSelf);
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
        if(activeSlider != null && modSlider.axis != Vector2.zero)
        {
            Debug.Log("Entered modify active slider");

            // Axis.Y can range from -1.0 to 1.0. Add 1 to range it from 0 to 2.
            float val = modSlider.axis.y + 1.0f;
            
            // Get the total range of avaliable values, max minus min. divide the previous val with 2 to get
            // a percent value of how far on the trackpad you have moved. Take the floor value of that, since
            // we're only interested in whole numbers.
            float percentEndVal = Mathf.Floor((activeSlider.maxValue - activeSlider.minValue) * (val/2f));

            // Set the slider value to the final value, add min val to get a proper representation.
            activeSlider.value = percentEndVal + activeSlider.minValue;

            Debug.Log(activeSlider.value);
            // Attempt to invoke the change to properly use the other written code.
            //activeSlider.onValueChanged.Invoke(percentEndVal + activeSlider.minValue);
        }

        if(Input.GetKeyDown(KeyCode.H))
        {
            // Make it a toggle so you press it again to turn it on and off.
            MainMenu.SetActive(!MainMenu.activeSelf);
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

        // Assign the currently selected node to the keeper value;
        data.CURRENTSELECTED = snn;

        // Trigger friend line recalc to decide what lines should be displayed;
        data.TriggerFriendLineRecalc();

        // Assure that the friend lines follow with the node when we move around the GameObjects in space;
        data.AssureLinesFollow();

        // Grab the data from the node and set the displaying text fields;
        fullName.text = "Name: " + snn.FullName;
        company.text = "Company: " + snn.Company;
        email.text = "Email: " + snn.Email;
        phone.text = "Phone: " + snn.Phone;
        address.text = "Address: " + snn.Address;
        registered.text = "Joined: " + snn.Registered;
        noFriends.text = "Number of friends: " + snn.NumberFriends;
    }

    public void ToggleFilterMenu()
    {
        FilterMenu.SetActive(!FilterMenu.activeSelf);
        activeSlider = null;
    }

    public void ToggleMainMenu()
    {
        MainMenu.SetActive(!MainMenu.activeSelf);            
    }

    public void ToggleFileSelectionMenu()
    {
        FileSelectionMenu.SetActive(!FileSelectionMenu.activeSelf);
    }

    public void SelectFile(int file)
    {
        if (data == null)
            InitializeData();
        
        data.LoadFileToNodes(Application.dataPath + "/" + file.ToString() + ".json");
    }

    public void SetCondition(int condition)
    {
        if (data == null)
            InitializeData();

        data.SetConditions(condition);
    }

    public void SetActiveSlider(Slider slide)
    {

    }

    private void InitializeData()
    {
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

    /// <summary>
    /// Helper method to reset all the menus from showing.
    /// </summary>
    public void ResetMenus()
    {
        FilterMenu.SetActive(false);
        InformationMenu.SetActive(false);
        FileSelectionMenu.SetActive(false);
        MainMenu.SetActive(false);
    }
}
