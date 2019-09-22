using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class SpawnNodes : MonoBehaviour
{
    public int num_points;
    public GameObject myPrefab;
    public GameObject centerPoint;

    // Start is called before the first frame update
    void Start()
    {
        /* 
         * Referens för att distribuera noder på en yta med "repulsive force" så att alla noder får
         * så stort utrymme som det är möjligt.
         * https://pdfs.semanticscholar.org/97a6/7e367e39762baf631f519c00fbfd1d5c009a.pdf
         * 
         * "The golden spiral method to distibute points on a sphere using the sunflower"
         * https://stackoverflow.com/a/44164075
         *  
         */

        double[] arranged_points = new double[num_points];

        for (int i = 0; i < num_points; i++)
        {
            arranged_points[i] = i + 0.5f;
        }

        double[] phi = new double[num_points];

        for (int j = 0; j < num_points; j++)
        {
            phi[j] = Math.Acos(1 - ((2*arranged_points[j])/num_points));
        }

        double[] theta = new double[num_points];

        for (int k = 0; k < num_points; k++)
        {
            theta[k] = Math.PI * (1 + Math.Pow(5, 0.5)) * arranged_points[k];
        }

        Vector3[] vectors = new Vector3[num_points];

        for (int l = 0; l < num_points; l++)
        {
            float x = (float)(Math.Cos(theta[l]) * Math.Sin(phi[l])); 
            float y = (float)(Math.Sin(theta[l]) * Math.Sin(phi[l]));
            float z = (float)Math.Cos(phi[l]);


            vectors[l] = new Vector3(x, y, z);
        }

        for(int m = 0; m < num_points; m++)
        {
            Instantiate(myPrefab, vectors[m] + centerPoint.gameObject.transform.position, Quaternion.identity);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
