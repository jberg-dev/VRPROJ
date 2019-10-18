using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRPROJ.Datastructure;

public class FilterController : MonoBehaviour
{

    public GameObject maxFriendsSlider;
    public GameObject minFriendsSlider;
    private Text minSelection;
    private Text maxSelection;

    public bool INITIALIZED
    {
        private set; get;
    }


    // Start is called before the first frame update
    void Start()
    {
        INITIALIZED = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(!INITIALIZED && DataStructure.INITIALIZED)
        {
            InitializeFiltermenu();
            INITIALIZED = true;
        }
    }

    void InitializeFiltermenu()
    {
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("MINFRIENDS"))
        {
            if (go.TryGetComponent<Text>(out Text textComponent))
            {
                textComponent.text = DataStructure.MINFRIENDS.ToString();
            }
        }

        foreach (GameObject go in GameObject.FindGameObjectsWithTag("MAXFRIENDS"))
        {
            if (go.TryGetComponent<Text>(out Text textComponent))
            {
                textComponent.text = DataStructure.MAXFRIENDS.ToString();
            }
        }

        if(minFriendsSlider.TryGetComponent<Slider>(out Slider minSlider))
        {
            minSlider.maxValue = DataStructure.MAXFRIENDS;
            minSlider.minValue = DataStructure.MINFRIENDS;
            minSlider.value = minSlider.minValue;
        }

        if(maxFriendsSlider.TryGetComponent<Slider>(out Slider maxSlider))
        {
            maxSlider.maxValue = DataStructure.MAXFRIENDS;
            maxSlider.minValue = DataStructure.MINFRIENDS;
            maxSlider.value = maxSlider.maxValue;
        }
    }
}
