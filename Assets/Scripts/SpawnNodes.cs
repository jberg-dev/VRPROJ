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
         * The code below is adapted from the stackoverflow link above.
         *  
         */
        // This is currently demo code for how the nodes of the social network will be implemented.
        //
        // TODO for real implementation:
        // 1) Data structure to keep track of nodes
        // 2) Refactor function into a proper method instead of a random script attached to a random object.
        //      a) This means that it will be connected with the data parser, most likely.
        //      b) Don't have no data to parse currently, so nothing to do about that........
        // 3) Insert data into the nodes so they properly reperesent the network "node", I.E.;
        //      a) Size from how many connections there are
        //      b) Color from what gender the node represents(?)
        //      c) Make a structure for the "links" to be drawn between "friends".
        for (int i = 0; i < num_points; i++)
        {
            double arranged_point = i + 0.5f;
            double phi = Math.Acos(1 - ((2 * arranged_point) / num_points));
            double theta = Math.PI * (1 + Math.Pow(5, 0.5)) * arranged_point;

            float x = (float)(Math.Cos(theta) * Math.Sin(phi));
            float y = (float)(Math.Sin(theta) * Math.Sin(phi));
            float z = (float)Math.Cos(phi);

            // Basically, increase the radius the more objects there are to display by multiplying
            // the positions values with the square root of the amount of nodes, but do not go below the multiplier 1.
            Vector3 vector = new Vector3((float)(x * Math.Max(1, Math.Floor(Math.Sqrt(num_points)))),
                                         (float)(y * Math.Max(1, Math.Floor(Math.Sqrt(num_points)))),
                                         (float)(z * Math.Max(1, Math.Floor(Math.Sqrt(num_points)))));

            Vector3 centerPointPosition = centerPoint.gameObject.transform.position;

            Instantiate(myPrefab, vector + centerPointPosition, Quaternion.identity);


        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
