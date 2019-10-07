using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRPROJ.Datastructure;

public class MenuControls : MonoBehaviour
{

#pragma warning disable CA1051 // Do not declare visible instance fields
    public GameObject ButtonMenu;
    public GameObject InformationMenu;
    public GameObject ErrorDialog;
#pragma warning restore CA1051 // Do not declare visible instance fields

    private Text fullName = null, 
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
        ResetMenus();
    }
    
    void InitializeTextFields()
    {
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

        InformationMenu.SetActive(false);

    }

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
        // TODO
        // Add keycode for Vive controllers!
        if(Input.GetKeyDown(KeyCode.H))
        {
            // Make it a toggle so you press it again to turn it on and off.
            ButtonMenu.SetActive(!ButtonMenu.activeSelf);
        }
    }

    public void DisplayInformationMenu(SocialNetworkNode snn)
    {
        
        if(fullName == null)
        {
            InitializeTextFields();
        }

        if (snn == null)
        {
            InformationMenu.SetActive(false);
            return;
        }
        else
            InformationMenu.SetActive(true);

        fullName.text = snn.FullName;
        company.text = snn.Company;
        email.text = snn.Email;
        phone.text = snn.Phone;
        address.text = snn.Address;
        registered.text = snn.Registered;
        noFriends.text = "0?"; // TODO CHANGE THIS
    }

    public void ResetMenus()
    {
        ButtonMenu.SetActive(false);
        InformationMenu.SetActive(false);
        ErrorDialog.SetActive(false);
    }
}
