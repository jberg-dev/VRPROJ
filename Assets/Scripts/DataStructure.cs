using Newtonsoft.Json;
using System;
using UnityEngine;


namespace VRPROJ.Datastructure
{
    public class DataStructure : MonoBehaviour
    {

        public static bool debugging = true;

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
                // SET UP DEFAULT TESTING SET HERE.
            }
        }

        // Update is called once per frame
        void Update()
        {

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

        public SocialNetworkNode()
        {

        }

        public bool instantiateNode(GameObject gameObject)
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

        public bool isInstantiated()
        {
            return game_node == null;
        }

        public override string ToString()
        {
            return String.Format("[{0} {1}] - {2} | {3}", m_firstName, m_lastName, m_company, m_email);
        }
    }

}
