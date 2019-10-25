using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

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
        public static bool realVR = false;
        private static List<SocialNetworkNode> nodes;
        public GameObject centerPoint;
        public GameObject myPrefab;
        private int currMin, currMax, currCondition;
        
        public SocialNetworkNode CURRENTSELECTED
        {
            set; get;
        }

        static public int MINFRIENDS
        {
            private set; get;
        }
        static public int MAXFRIENDS
        {
            private set; get;
        }

        static public bool INITIALIZED
        {
            private set; get;
        }


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

        void Awake()
        {
            currCondition = 1;
            INITIALIZED = false;
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        /// <summary>
        /// Parse the provided file into nodes.
        /// </summary>
        /// <param name="path">The file to parse</param>
        /// <returns></returns>
        private bool ReadDataIntoNodes(string path)
        {
            if(File.Exists(path))
            {
                if (nodes != null && nodes.Count != 0)
                {
                    // Clear out the current data.
                    ResetNodes();

                    // Need to set this to false to re-init the filter menu.
                    FilterController.INITIALIZED = false;
                }

                // Read in from file.
                string jsontext = File.ReadAllText(path, Encoding.UTF8);

                // Parse the data to the provided structures.
                nodes = JsonConvert.DeserializeObject<List<SocialNetworkNode>>(jsontext);

                return true;
            }
            else
            {
                Debug.LogError("PATH DID NOT EXIST!");
            }

            return false;
        }

        /// <summary>
        /// Clear the current active data structure.
        /// </summary>
        private void ResetNodes()
        {
            if (nodes == null)
                return;

            foreach (SocialNetworkNode node in nodes)
            {
                node.SelfDestruct();
            }

            currMin = 0;
            currMax = 0;
            nodes.Clear();
        }

        /// <summary>
        /// Load a specified file and parse it into data nodes and set up the surrounding data.
        /// </summary>
        /// <param name="path">The path to the file to parse</param>
        public void LoadFileToNodes(string path)
        {
            Debug.Log("File path: " + path);

            if(ReadDataIntoNodes(path))
            {
                if(SpawnNodes())
                {
                    SetUpPublicData();
                    SetNodeSizesBasedOnFriends();
                    TriggerFriendLineRecalc();
                    INITIALIZED = true;
                }
                else
                {
                    Debug.Log("Could not spawn nodes with the provided data");
                    INITIALIZED = false;
                }                
            }
            else
            {
                Debug.Log("Could not read the file and failed to get new nodes.");
                INITIALIZED = false;
            }
        }

        void SetNodeSizesBasedOnFriends()
        {

            foreach (SocialNetworkNode snn in nodes)
            {
                if (!snn.isInstantiated())
                    continue;

                float percentVal = MAXFRIENDS - MINFRIENDS;
                float baseVal = (snn.CountNumberFriends / percentVal);
                Vector3 oldVect = snn.GetNode().transform.localScale;
                
                float freshX = Mathf.Max(baseVal * oldVect.x, 0.1f);
                float freshY = Mathf.Max(baseVal * oldVect.y, 0.1f);
                float freshZ = Mathf.Max(baseVal * oldVect.z, 0.1f);

                snn.GetNode().transform.localScale = new Vector3(freshX, freshY, freshZ);
            }
        }

        Vector3 zero_pos = new Vector3(0, 0, 0);
        readonly System.Random r = new System.Random();
        public GameObject empty;

        // Update is called once per frame
        void Update()
        {
            // DEBUG CODE that should be gotten rid of.
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
            if(Input.GetKeyDown(KeyCode.L))
            {
                ResetNodes();
            }
        }

        /// <summary>
        /// Public reference data about the data structure active at the moment.
        /// </summary>
        void SetUpPublicData()
        {
            if (nodes.Count == 0)
            {
                Debug.LogError("NODES NOT INITIALIZED, NOT SETTING UP FILTER MENU");
                return;
            }

            MINFRIENDS = nodes[0].CountNumberFriends;
            MAXFRIENDS = nodes[0].CountNumberFriends;

            foreach(SocialNetworkNode n in nodes)
            {
                if(n.CountNumberFriends < MINFRIENDS)
                {
                    MINFRIENDS = n.CountNumberFriends;
                }
                if(n.CountNumberFriends > MAXFRIENDS)
                {
                    MAXFRIENDS = n.CountNumberFriends;
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
                Vector3 vector = new Vector3((float)(x * Math.Max(3, Math.Floor(Math.Sqrt(nodes.Count)))),
                                             (float)(y * Math.Max(3, Math.Floor(Math.Sqrt(nodes.Count)))),
                                             (float)(z * Math.Max(3, Math.Floor(Math.Sqrt(nodes.Count)))));

                //Y Z X for top

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

        /// <summary>
        /// Conditionalize the required friends to be displayed.
        /// </summary>
        /// <param name="minimum">The minimum amount of friends to have to be displayed</param>
        /// <param name="maximum">The maximum amount of friends to have to be displayed</param>
        public void SetRangeOfFriendsVisible(int minimum, int maximum)
        {
            // Error check.
            if(minimum < MINFRIENDS || maximum > MAXFRIENDS)
            {
                Debug.LogError(String.Format("ERROR! min: {0}, minfriends: {1} | max: {2}, maxfriends: {3}", 
                    minimum, MINFRIENDS, maximum, MAXFRIENDS));
                return;
            }

            Debug.Log("Current policy selected is: " + currCondition);

            currMax = maximum;
            currMin = minimum;

            // Set those that should be visible, visible. Invisible those that should be invisible.
            foreach(SocialNetworkNode snn in nodes)
            {
                if (MatchesConditions(snn) && (snn.CountNumberFriends >= minimum && snn.CountNumberFriends <= maximum))
                    snn.Visible = true;
                else
                    snn.Visible = false;
            }

            // Assert that all the connections from the visible nodes display.
            foreach(SocialNetworkNode active in nodes)
            {
                if (active.Visible)
                    active.AssertConnectionsVisible();
            }
        }

        /// <summary>
        /// Helper method to check if the node matches the current line rendering policy.
        /// </summary>
        /// <param name="snn">The node to check</param>
        /// <returns>The result of the match against the policy. True if matching.</returns>
        public bool MatchesConditions(SocialNetworkNode snn)
        {
            switch(currCondition)
            {
                case 0:
                    // Never true.
                    return false; 
                case 1:
                    // Only true if matches the current selected.
                    return snn.Equals(CURRENTSELECTED); 
                case 2:
                    // Only true if in the current selected has them on their friends list
                    return (CURRENTSELECTED != null && CURRENTSELECTED.IsFriendPresent(snn));
                case 3:
                    // Always true.
                    return true;
                default:
                    Debug.LogError("Failed to match a current condition!");
                    return false;
            }
        }

        /// <summary>
        /// Shorthand method to trigger the re-calculation of the friend lines.
        /// </summary>
        public void TriggerFriendLineRecalc()
        {
            if(currMin == 0 && currMax == 0)
            {
                currMin = MINFRIENDS;
                currMax = MAXFRIENDS;
            }
            Debug.Log("Triggering Recalculation of friend lines....");
            SetRangeOfFriendsVisible(currMin, currMax);
        }

        /// <summary>
        /// Set and save the friend line rendering conditions.
        /// </summary>
        /// <param name="condition">The condition to set.</param>
        public void SetConditions(int condition)
        {
            switch(condition)
            {
                case 0: // DO NOT RENDER NODES FRIENDS
                    Debug.Log("Condition: No nodes friends rendering.");
                    break;
                case 1: // RENDER SELECTED NODES FRIENDS
                    Debug.Log("Condition: Selected nodes friends rendering.");
                    break;
                case 2: // RENDER 1 DEEP FROM SELECTED NODES FRIENDS
                    Debug.Log("Condition: One deep friends' connections rendering.");
                    break;
                case 3: // RENDER ALL NODES FRIENDS
                    Debug.Log("Condition: All nodes friends rendering.");
                    break;
                default:
                    Debug.LogError("Unknown value for condition: " + condition);
                    return;
            }

            // Save the current conditon of friends rendering.
            currCondition = condition;

            // Trigger recalc of friends rendering.
            TriggerFriendLineRecalc();
        }

        /// <summary>
        /// Method to make sure LineRenderers follow when the position of the GameObjects rotate in space.
        /// </summary>
        public void AssureLinesFollow()
        {
            foreach (SocialNetworkNode snn in nodes)
            {
                if(snn.Visible)
                {
                    snn.AssureLinesFollow();
                }
            }
        }

        //Vector3 previous = new Vector3(0,0,0);

        //public void RotateNodes(Vector3 delta)
        //{
        //    float angle = Vector3.Angle(previous, delta);

        //    foreach (SocialNetworkNode snn in nodes)
        //    {
        //        if(snn.isInstantiated())
        //        {
        //            snn.GetNode().transform.Rotate(centerPoint.transform.position, angle);
        //        }
        //    }
        //}
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
        private volatile bool m_visible = true;

        #region properties
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

        public string NumberFriends
        {
            get
            {
                return friendRelations.Count.ToString();
            }
        }

        public int CountNumberFriends
        {
            get
            {
                return friendRelations.Count;
            }
        }

        public bool Visible
        {
            get
            {
                return m_visible;
            }
            set
            {
                if(game_node != null)
                {
                    m_visible = value;

                    if (value)
                        SetNodeAlpha(1.0f);
                    else
                        SetNodeAlpha(0.3f);

                    foreach(LineRenderer lr in friendRelations.Values)
                    {
                        lr.enabled = value;
                    }
                }
                else
                {
                    Debug.LogWarning("Attempting to set visibility on null game node SNN for " + FullName);
                }
            }
        }
#endregion

        public SocialNetworkNode()
        {
            friendRelations = new Dictionary<SocialNetworkNode, LineRenderer>();
        }

        public void AssureLinesFollow()
        {
            LineRenderer lr;
            SocialNetworkNode snn;

            for(int i = 0; i < friendRelations.Count; i++)
            {
                snn = friendRelations.Keys.ElementAt(i);
                lr = friendRelations.Values.ElementAt(i);

                if (!isInstantiated() || !snn.isInstantiated())
                    continue;

                lr.SetPosition(0, game_node.transform.position);
                lr.SetPosition(1, snn.GetNode().transform.position);
            }
        }

        void SetNodeAlpha(float alpha)
        {
            if(game_node != null && alpha >= 0 && alpha <= 1.0f)
            {
                Material mat = game_node.gameObject.GetComponent<Renderer>().material;
                Color color = mat.color;
                color.a = alpha;
                mat.color = color;
            }
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

        public void AssertConnectionsVisible()
        {
            if(Visible)
            {
                foreach (LineRenderer show in friendRelations.Values)
                {
                    show.enabled = true;
                }
            }
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

        public void SelfDestruct()
        {
            if (isInstantiated())
                GameObject.Destroy(game_node);

            foreach(LineRenderer lr in friendRelations.Values)
            {
                if (lr != null)
                    GameObject.Destroy(lr.gameObject);
            }
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
