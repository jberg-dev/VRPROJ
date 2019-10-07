using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

/// <summary>
/// A Collection of what is expected to manage all the data about the nodes and their connections in the network.
/// </summary>
namespace VRPROJ.Datastructure
{

    /// <summary>
    /// Manager Class that handles initialization, and later calls for filtering and/or redrawing.
    /// </summary>
    public class DataStructure : MonoBehaviour
    {

        public static bool debugging = true;
        private static List<SocialNetworkNode> nodes;
        public GameObject centerPoint;
        public GameObject myPrefab;

        /*
         * Planned class structure;
         * 
         * 1) At Start(), make the user choose the data set to work with, make that a generic file selector 
         *    external call that can be repeatedly called at a later point if desired.
         *    
         * 2) Take the selected file, of JSON type, and parse it in to a desired structure/class.
         * 
         * 3) Delegate the instantiation of objects to the virtual space. Remember to set it to display proper
         *    amounts of line connections based on the user settings in the menu. STORE the instantiated objects
         *    in a hashmap for quick access and further conditional processing.
         */



        // Start is called before the first frame update
        void Start()
        {
            if(!debugging)
            {
                // CALL FILE MANAGER HERE FOR SELECTION!
            }
            else
            {
                string jsontext = File.ReadAllText(@"C:\Users\Bergerking\VRPROJ\Assets\Scripts\GenData.json", Encoding.UTF8);
                //string jsontext = File.ReadAllText(@"C:\Users\Bergerking\VRPROJ\Assets\Scripts\SimplifiedGenData.json", Encoding.UTF8);
                nodes = JsonConvert.DeserializeObject<List<VRPROJ.Datastructure.SocialNetworkNode>>(jsontext);
            }

            if(!SpawnNodes())
            {
                // TODO
                // Display error popup here.
            }
        }

        Vector3 zero_pos = new Vector3(0, 0, 0);
        System.Random r = new System.Random();
        public GameObject empty;

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Z))
            { 
                for (int i = 0; i < 10; i++)
                {
                    GameObject from = nodes[r.Next(0, nodes.Count)].GetNode();
                    GameObject unto = nodes[r.Next(0, nodes.Count)].GetNode();
                    GameObject emptyPos = Instantiate(empty, zero_pos, Quaternion.identity);

                    LineRenderer lr = emptyPos.AddComponent<LineRenderer>();
                    lr.material = new Material(Shader.Find("Sprites/Default"));
                    lr.positionCount = 2;
                    lr.widthMultiplier = 0.1f;

                    Vector3 pos1 = from.gameObject.transform.position;
                    Vector3 pos2 = unto.gameObject.transform.position;

                    Vector3[] positions = { pos1, pos2 };

                    lr.SetPositions(positions);
                }
            }
        }

        /// <summary>
        /// Spawn nodes to the virtual canvas based on the data that has been passed in to the nodes data
        /// structure in the previous step.
        /// </summary>
        /// <returns>True if any kind of instantiation succeeded, false if errors.</returns>
        bool SpawnNodes()
        {
            // Error check node count so we don't crash the application
            if(nodes == null || nodes.Count == 0)
            {
                Debug.LogError("CRITICAL ERROR: NO NODES LOADED, RETURNING FALSE.");
                return false;
            }

            /*
             * "The golden spiral method to distibute points on a sphere using the sunflower"
             * https://stackoverflow.com/a/44164075
             * 
             * The code below is adapted and personalized from the stackoverflow link above.
             */

            for (int i = 0; i < nodes.Count; i++)
            {
                // Set up the angle where each node will be displayed at.
                double arranged_point = i + 0.5f;
                double phi = Math.Acos(1 - (arranged_point / nodes.Count));
                double theta = Math.PI * (1 + Math.Pow(5, 0.5)) * arranged_point;

                // Calculate what it means in real world positions.
                float x = (float)(Math.Cos(theta) * Math.Sin(phi));
                float y = (float)(Math.Sin(theta) * Math.Sin(phi));
                float z = (float)(Math.Cos(phi));

                // Basically, increase the radius the more objects there are to display by multiplying
                // the positions values with the square root of the amount of nodes, but do not go below the multiplier 1.
                Vector3 vector = new Vector3((float)(y * Math.Max(3, Math.Floor(Math.Sqrt(nodes.Count)))),
                                             (float)(z * Math.Max(3, Math.Floor(Math.Sqrt(nodes.Count)))),
                                             (float)(x * Math.Max(3, Math.Floor(Math.Sqrt(nodes.Count)))));

                // Get the reference center point of the sphere.
                Vector3 centerPointPosition = centerPoint.gameObject.transform.position;

                // Instantiate the object and catch it for storage.
                GameObject justMade = Instantiate(myPrefab, vector + centerPointPosition, Quaternion.identity) as GameObject;

                // Get the Data Manager entity from the spawned node for manipulation
                DataManager manager = justMade.GetComponent<DataManager>();

                // Link the data of the node with the manager
                manager.SaveNodeData(nodes[i]);

                // Link the node with the data of the node
                nodes[i].SaveNode(justMade);

            }

            // TODO 
            // REMOVE THE CODE BELOW AND MAKE THE PROPER IMPLEMENTATION!
            GenerateFakeFriends();


            return true;
        }

        /// <summary>
        /// Temporary testing structure to verify friend relation functionality.
        /// </summary>
        void GenerateFakeFriends()
        {
            // Local instance for consistent results.
            System.Random localRandom = new System.Random(1);
            for(int count = 0; count < nodes.Count; count++)
            {
                SocialNetworkNode currentActive = nodes[count];
                int randomAmountOfFriends = localRandom.Next(0, 15); //(int)Math.Ceiling(nodes.Count / 5d));
                for(int inner = 0; inner < randomAmountOfFriends; inner++)
                {
                    // Get a random index of the friend to add.
                    int randomFriend = localRandom.Next(0, nodes.Count);
                    
                    // Not adding youself as a friend.
                    if (randomFriend == count)
                        continue;
                    
                    // Fish out the friend from the node list.
                    SocialNetworkNode selected = nodes[randomFriend];

                    // Make sure that we do not already have this friend added.
                    if(!currentActive.IsFriendPresent(selected))
                    {
                        GameObject emptyPos = Instantiate(empty, zero_pos, Quaternion.identity);

                        LineRenderer lr = emptyPos.AddComponent<LineRenderer>();
                        lr.material = new Material(Shader.Find("Sprites/Default"));
                        lr.positionCount = 2;
                        lr.widthMultiplier = 0.1f;

                        Vector3 pos1 = selected.GetNode().transform.position;
                        Vector3 pos2 = currentActive.GetNode().transform.position;

                        Vector3[] positions = { pos1, pos2 };

                        lr.SetPositions(positions);

                        currentActive.AddFriend(selected, lr);
                        selected.AddFriend(currentActive, lr);
                    }
                    else
                        continue;
                    
                }
            }
        }
    }


    public class SocialNetworkNode
    {
        // Static defaults
        private static string default_name = "DEFAULT";

        // Class variables
        private string m_id         = default_name;
        private int    m_index      = 0;
        private string m_firstName  = default_name;
        private string m_lastName   = default_name;
        private string m_company    = default_name;
        private string m_email      = default_name;
        private string m_phone      = default_name;
        private string m_address    = default_name;
        private string m_registered = default_name;

        private volatile Dictionary<SocialNetworkNode, LineRenderer> friendRelations;
        private volatile GameObject game_node = null;

        [JsonProperty("_id")]
        public string ID
        {
            get
            {
                return this.m_id;
            }
            private set
            {
                if (!String.IsNullOrWhiteSpace(value))
                    m_id = value;
                else if (DataStructure.debugging)
                    Debug.LogError(String.Format("String of name were empty or just plain whitespace! = [{0}]", value));
            }
        }

        [JsonProperty("index")]
        public int Index
        {
            get
            {
                return this.m_index;
            }
            private set
            {
                if (value >= 0)
                    m_index = value;
                else if (DataStructure.debugging)
                    Debug.LogError(String.Format("Value of amount of friends were less than 0! [{0}] for {1}!", value, m_firstName));
            }
        }
        
        [JsonProperty("first_name")]
        public string FirstName
        {
            get
            {
                return this.m_firstName;
            }
            private set
            {
                if (!String.IsNullOrWhiteSpace(value))
                    m_firstName = value;
                else if (DataStructure.debugging)
                    Debug.LogError(String.Format("String of name were empty or just plain whitespace! = [{0}]", value));
            }
        }

        [JsonProperty("last_name")]
        public string LastName
        {
            get
            {
                return this.m_lastName;
            }
            private set
            {
                if (!String.IsNullOrWhiteSpace(value))
                    m_lastName = value;
                else if (DataStructure.debugging)
                    Debug.LogError(String.Format("String of name were empty or just plain whitespace! = [{0}]", value));
            }
        }

        [JsonProperty("company")]
        public string Company
        {
            get
            {
                return this.m_company;
            }
            private set
            {
                if (!String.IsNullOrWhiteSpace(value))
                    m_company = value;
                else if (DataStructure.debugging)
                    Debug.LogError(String.Format("String of name were empty or just plain whitespace! = [{0}]", value));
            }
        }

        [JsonProperty("email")]
        public string Email
        {
            get
            {
                return this.m_email;
            }
            private set
            {
                if (!String.IsNullOrWhiteSpace(value))
                    m_email = value;
                else if (DataStructure.debugging)
                    Debug.LogError(String.Format("String of name were empty or just plain whitespace! = [{0}]", value));
            }
        }

        [JsonProperty("phone")]
        public string Phone
        {
            get
            {
                return this.m_phone;
            }
            private set
            {
                if (!String.IsNullOrWhiteSpace(value))
                    m_phone = value;
                else if (DataStructure.debugging)
                    Debug.LogError(String.Format("String of name were empty or just plain whitespace! = [{0}]", value));
            }
        }

        [JsonProperty("address")]
        public string Address
        {
            get
            {
                return this.m_address;
            }
            private set
            {
                if (!String.IsNullOrWhiteSpace(value))
                    m_address = value;
                else if (DataStructure.debugging)
                    Debug.LogError(String.Format("String of name were empty or just plain whitespace! = [{0}]", value));
            }
        }

        [JsonProperty("registered")]
        public string Registered
        {
            get
            {
                return this.m_registered;
            }
            private set
            {
                if (!String.IsNullOrWhiteSpace(value))
                    m_registered = value;
                else if (DataStructure.debugging)
                    Debug.LogError(String.Format("String of name were empty or just plain whitespace! = [{0}]", value));
            }
        }

        public string FullName
        {
            get
            {
                return m_firstName + " " + m_lastName;
            }
        }

        public SocialNetworkNode()
        {
            friendRelations = new Dictionary<SocialNetworkNode, LineRenderer>();
        }

        public bool SaveNode(GameObject gameObject)
        {
            if(game_node == null)
            {
                if(gameObject != null)
                {
                    game_node = gameObject;
                }
                else if(DataStructure.debugging)
                {
                    Debug.LogError(String.Format("Game object were NULL for [{0}]", m_firstName));
                }
                    
            }
            else if(DataStructure.debugging)
            {
                Debug.LogError(String.Format("Attempting to instantiate a node that already is instantiated! [{0}]", m_firstName));
            }

            return false;
        }

        public LineRenderer GetLineForFriend(SocialNetworkNode other)
        {
            if (friendRelations != null && friendRelations.TryGetValue(other, out LineRenderer lr))
                return lr;
            else
            {
                if (DataStructure.debugging && friendRelations == null)
                    Debug.LogError("FriendRelations were NULL!");
                return null;
            }
                
        }

        public bool AddFriend(SocialNetworkNode friend, LineRenderer line)
        {
            if(!IsFriendPresent(friend))
            {
                friendRelations.Add(friend, line);
                return true;
            }

            return false;
        }

        public bool IsFriendPresent(SocialNetworkNode friend)
        {
            return friendRelations.ContainsKey(friend);
        }

        public GameObject GetNode()
        {
            if (game_node != null)
                return game_node;
            else
            {
                Debug.LogError("Attempting to get a null game node for + " + FullName);
                return null;
            }
        }

        /// <summary>
        /// Used for re-rendering nodes in other configurations.
        /// </summary>
        public void ResetNode()
        {
            this.game_node = null;
            Debug.Log("RESETTING NODE FOR: " + FullName);
        }

        /// <summary>
        /// Shorthand to make sure that tha node is able to be worked on.
        /// </summary>
        /// <returns></returns>
        public bool isInstantiated()
        {
            return game_node != null;
        }

        public void PutToDisplay()
        {
            GameObject controlDummy = GameObject.FindGameObjectWithTag("InformationMenu");
            MenuControls menuController;

            if (controlDummy.TryGetComponent<MenuControls>(out menuController))
            {
                menuController.ResetMenus();
            }
            else
            {
                Debug.LogWarning("Failed to locate the menu control");
            }
        }

        /// <summary>
        /// Custom ToString() to give a short overview of what the data represents.
        /// </summary>
        /// <returns>A short introduction.</returns>
        public override string ToString()
        {
            return String.Format("[{0} {1}] - {2} | {3}", m_firstName, m_lastName, m_company, m_email);
        }
    }

}
