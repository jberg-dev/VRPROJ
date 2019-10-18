﻿using System.Collections;
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
    private Slider minimum;
    private Slider maximum;
    private DataStructure ds;

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
            minimum = minSlider;
        }

        if(maxFriendsSlider.TryGetComponent<Slider>(out Slider maxSlider))
        {
            maxSlider.maxValue = DataStructure.MAXFRIENDS;
            maxSlider.minValue = DataStructure.MINFRIENDS;
            maxSlider.value = maxSlider.maxValue;
            maximum = maxSlider;
        }

        GameObject minCurr = GameObject.FindGameObjectWithTag("minCurrentSelection");
        GameObject maxCurr = GameObject.FindGameObjectWithTag("maxCurrentSelection");

        if(minCurr.TryGetComponent<Text>(out Text minCurrText))
        {
            minCurrText.text = minimum.value.ToString();
            minSelection = minCurrText;
        }

        if (maxCurr.TryGetComponent<Text>(out Text maxCurrText))
        {
            maxCurrText.text = maximum.value.ToString();
            maxSelection = maxCurrText;
        }

        GameObject datastructure = GameObject.FindGameObjectWithTag("DataStructure");
        
        if(datastructure.TryGetComponent<DataStructure>(out DataStructure dataaa))
        {
            ds = dataaa;
        }
        else
        {
            Debug.LogError("Failed to get DataStructure");
        }

    }

    public void minSliderChanged()
    {
        if (!INITIALIZED)
            return;

        if (minimum.value > maximum.value)
            minimum.value = maximum.value;

        minSelection.text = minimum.value.ToString();
        ds.SetRangeOfFriendsVisible((int)minimum.value, (int)maximum.value);
    }

    public void maxSliderChanged()
    {
        if (!INITIALIZED)
            return;

        if (maximum.value < minimum.value)
            maximum.value = minimum.value;

        maxSelection.text = maximum.value.ToString();
        ds.SetRangeOfFriendsVisible((int)minimum.value, (int)maximum.value);
    }
}
