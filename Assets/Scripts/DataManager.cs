using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRPROJ.Datastructure;

/// <summary>
/// A class to attach to the representative "node" objects to keep a doubly-linked reference
/// between the graphical representation and the data representation of a node.
/// 
/// Expected to handle a List of all the lines drawn from/to it at a later point, too.
/// </summary>
public class DataManager : MonoBehaviour
{

    public SocialNetworkNode nodeData;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SaveNodeData(SocialNetworkNode in_node)
    {
        if(in_node != null)
        {
            this.nodeData = in_node;
        }            
        else
            Debug.LogError("in_node was null!");
    }
}
