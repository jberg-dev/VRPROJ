using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTransparency : MonoBehaviour
{
    System.Random r = new System.Random();
    public GameObject transparencyTester;
    private Material mat;

    // Start is called before the first frame update
    void Start()
    {
        mat = transparencyTester.gameObject.GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            SetAlpha((float)r.NextDouble());
            Debug.Log("Spacebar down!");
        }
            
    }

    private void SetAlpha(float alpha)
    {
        Color color = mat.color;
        color.a = alpha;
        mat.color = color;
    }
}
