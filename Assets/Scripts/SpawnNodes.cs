using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System.Text;

public class SpawnNodes : MonoBehaviour
{
    public int num_points;
    public GameObject myPrefab;
    public GameObject centerPoint;
    public GameObject empty;

    private GameObject[] holder;
    private readonly System.Random r = new System.Random();

    // Start is called before the first frame update
    void Start()
    {
        return;

        holder = new GameObject[num_points];

        string jsontext = File.ReadAllText(@"C:\Users\Bergerking\VRPROJ\Assets\Scripts\GenData.json", Encoding.UTF8);
        var result = JsonConvert.DeserializeObject<List<VRPROJ.Datastructure.SocialNetworkNode>>(jsontext);

        /* 
         * "The golden spiral method to distibute points on a sphere using the sunflower"
         * https://stackoverflow.com/a/44164075
         * 
         * The code below is adapted and personalized from the stackoverflow link above.
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
            // Set up the angle where each node will be displayed at.
            double arranged_point = i + 0.5f;
            double phi = Math.Acos(1 - (arranged_point / num_points));
            double theta = Math.PI * (1 + Math.Pow(5, 0.5)) * arranged_point;

            // Calculate what it means in real world positions.
            float x = (float)(Math.Cos(theta) * Math.Sin(phi)/* * (Math.PI/2) */);
            float y = (float)(Math.Sin(theta) * Math.Sin(phi)/* * (Math.PI / 2) */);
            float z = (float)(Math.Cos(phi)/* * (Math.PI / 2)*/);

            // Basically, increase the radius the more objects there are to display by multiplying
            // the positions values with the square root of the amount of nodes, but do not go below the multiplier 1.
            Vector3 vector = new Vector3((float)(y * Math.Max(1, Math.Floor(Math.Sqrt(num_points)))),
                                         (float)(z * Math.Max(1, Math.Floor(Math.Sqrt(num_points)))),
                                         (float)(x * Math.Max(1, Math.Floor(Math.Sqrt(num_points)))));

            // Get the reference center point of the sphere.
            Vector3 centerPointPosition = centerPoint.gameObject.transform.position;

            // Instantiate the object, save the reference to it in an array for later manipulation.
            holder[i] = Instantiate(myPrefab, vector + centerPointPosition, Quaternion.identity) as GameObject;

        }



        //foreach(object o in result)
        //{
        //    Debug.Log(o.ToString());
        //}
        

    }

    Vector3 zero_pos = new Vector3(0, 0, 0);

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("z"))
        {
            return;

            //for (int i = 0; i < num_points; i++)
            //{
            //    Material mat = holder[i].GetComponent<Renderer>().material;
            //    Color color = mat.color;
            //    color.a = (float)r.NextDouble();
            //    mat.color = color;
            //}

            for(int i = 0; i < 10; i++)
            {
                GameObject from = holder[r.Next(0, num_points)];
                GameObject unto = holder[r.Next(0, num_points)];
                GameObject emptyPos = Instantiate(empty, zero_pos, Quaternion.identity);

                LineRenderer lr = emptyPos.AddComponent<LineRenderer>();
                lr.material = new Material(Shader.Find("Sprites/Default"));
                lr.positionCount = 2;
                lr.widthMultiplier = 0.2f;

                Vector3 pos1 = from.gameObject.transform.position;
                Vector3 pos2 = unto.gameObject.transform.position;

                Vector3[] positions = { pos1, pos2 };

                lr.SetPositions(positions);
            }
        }
    }
}
