using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

public class ChallengeStatus : MonoBehaviour
{
    public class ChallengeWall
    {
        public string type;
        public string color;
        public string position;
        public string rotation;
    }

    public class ChallengeBarrier
    {
        public string type;
        public string color;
        public string position;
        public string rotation;
    }

    public class ChallengePortalIn
    {
        public string type;
        public string color;
        public string position;
        public string rotation;
    }

    public class ChallengePortalOut
    {
        public string type;
        public string color;
        public string position;
        public string rotation;
    }

    public class ChallengeCoin
    {
        public string position;
    }

    [SerializeField] GameObject angular_135;
    [SerializeField] GameObject angular_180;
    [SerializeField] GameObject angular_225;
    [SerializeField] GameObject angular_270;

    [SerializeField] GameObject horizontal_135;
    [SerializeField] GameObject horizontal_180;
    [SerializeField] GameObject horizontal_225;
    [SerializeField] GameObject horizontal_270;

    [SerializeField] GameObject vertical_135;
    [SerializeField] GameObject vertical_180;
    [SerializeField] GameObject vertical_225;
    [SerializeField] GameObject vertical_270;

    // Each wall and barrier should be described as type, x, y and rotation

    [SerializeField] GameObject barrier_150;
    [SerializeField] GameObject barrier_200;
    [SerializeField] GameObject barrier_250;
    [SerializeField] GameObject barrier_300;

    // Each portal should be described as type, x, y and rotation
    [SerializeField] GameObject portalInBlueYellow;
    [SerializeField] GameObject portalOutBlueYellow;
    [SerializeField] GameObject portalInRedGreen;
    [SerializeField] GameObject portalOutRedGreen;

    // Each coin should be described as x, y
    [SerializeField] GameObject coinPrefab;

    // To set parent of all barriers
    private GameObject barriersParent;

    // To set parent of all walls
    private GameObject wallsParent;

    // To set parent of all coins
    private GameObject coinsParent;

    // To set parent of all coins
    private GameObject portalsParent;

    private List<Vector3> coins = new List<Vector3>();

    // For every index of a portal in there should be a portal out to connect
    // Type will be a Red-Green or a Blue-Yellow to show which type color pair of portals to place
    private List<(string type, Vector3 position, Vector3 rotation)> portalIns = new List<(string type, Vector3 position, Vector3 rotation)>();
    private List<(string type, Vector3 position, Vector3 rotation)> portalOuts = new List<(string type, Vector3 position, Vector3 rotation)>();
    private List<(string type, Color32 color, Vector3 position, Vector3 rotation)> walls = new List<(string type, Color32 color, Vector3 position, Vector3 rotation)>();
    private List<(string type, Color32 color, Vector3 position, Vector3 rotation)> barriers = new List<(string type, Color32 color, Vector3 position, Vector3 rotation)>();

    //private (string type, Color32 color, Vector3 position, Vector3 rotation)[] mockWalls =
    //{
    //    ("Angular180", new Color32(255, 0, 0, 255), new Vector3(616, 979.8f, 0), new Vector3(0, 0, -135.525f)),
    //    ("Angular180", new Color32(255, 0, 0, 255), new Vector3(258, 1025, 0), new Vector3(0, 0, -90)),
    //    ("Angular135", new Color32(255, 0, 0, 255), new Vector3(291, 503, 0), new Vector3(0, 0, 175.685f)),
    //    ("Angular180", new Color32(255, 0, 0, 255), new Vector3(338.3f, 702, 0), new Vector3(0, 0, 90)),
    //    ("Angular135", new Color32(255, 0, 0, 255), new Vector3(407.7f, 891.8f, 0), new Vector3(0, 0, 42))
    //};
    //private (string type, Vector3 position, Vector3 rotation)[] mockBarriers =
    //{
    //    ("Barrier150", new Vector3(242, 789, 0), new Vector3(0, 0, -51.883f)),
    //    ("Barrier300", new Vector3(587.5f, 174.3615f, 0), new Vector3(0, 0, 0)),
    //    ("Barrier300", new Vector3(498.5f, 255.5f, 0), new Vector3(0, 0, -90)),
    //    ("Barrier300", new Vector3(412.3f, 171.231f, 0), new Vector3(0, 0, 0)),
    //    ("Barrier150", new Vector3(641, 540, 0), new Vector3(0, 0, -179.684f)),
    //    ("Barrier200", new Vector3(577.5f, 774, 0), new Vector3(0, 0, -90)),
    //    ("Barrier200", new Vector3(35, 567, 0), new Vector3(0, 0, -7.424f)),
    //    ("Barrier250", new Vector3(433.8f, 309.2f, 0), new Vector3(0, 0, -50)),
    //    ("Barrier250", new Vector3(133.6338f, 1037, 0), new Vector3(0, 0, 0)),
    //    ("Barrier250", new Vector3(133.6338f, 909, 0), new Vector3(0, 0, 0)),
    //    ("Barrier250", new Vector3(561.8f, 310.2f, 0), new Vector3(0, 0, 50)),
    //    ("Barrier300", new Vector3(89.65347f, 126.8252f, 0), new Vector3(0, 0, -90.43301f)),
    //    ("Barrier300", new Vector3(87.84814f, 302.0439f, 0), new Vector3(0, 0, -90.43301f))
    //};
    private Vector3[] mockCoins =
    {
        new Vector3(572.6772f, 858.2982f, 0),
        new Vector3(637.932f, 389.8024f, 0),
        new Vector3(184.495f, 864.991f, 0),
        new Vector3(221.3054f, 384.7828f, 0),
        new Vector3(356.8345f, 388.1292f, 0),
        new Vector3(505.7493f, 547.0831f, 0),
        new Vector3(154.3774f, 550.4295f, 0)
    };
    private (string type, Vector3 position, Vector3 rotation)[] mockPortalIns =
    {
        ("Blue-Yellow", new Vector3(220.3657f, 816.3812f, 0), new Vector3(0, 0, 217.637f)),
        ("Blue-Yellow", new Vector3(576.7657f, 804.6249f, 0), new Vector3(0, 0, 180)),
        ("Blue-Yellow", new Vector3(411.2657f, 336.1812f, 0), new Vector3(0, 0, -140.186f)),
        ("Red-Green", new Vector3(68.24689f, 876.3812f, 0), new Vector3(0, 0, -180)),
        ("Blue-Yellow", new Vector3(166.3657f, 334.3812f, 0), new Vector3(0, 0, 140.239f)),
        ("Red-Green", new Vector3(81.58411f, 164.9251f, 0), new Vector3(0, 0, 180))
    };
    private (string type, Vector3 position, Vector3 rotation)[] mockPortalOuts =
    {
        ("Blue-Yellow", new Vector3(606, 542, 0), new Vector3(0, 0, 91.189f)),
        ("Blue-Yellow", new Vector3(66.36572f, 561.3812f, 0), new Vector3(0, 0, -99.08801f)),
        ("Blue-Yellow", new Vector3(69.02356f, 1069.381f, 0), new Vector3(0, 0, 180)),
        ("Red-Green", new Vector3(589.0657f, 335.6812f, 0), new Vector3(0, 0, -40.624f)),
        ("Blue-Yellow", new Vector3(81.58411f, 266.5785f, 0), new Vector3(0, 0, 180)),
        ("Red-Green", new Vector3(498.4164f, 224.0699f, 0), new Vector3(0, 0, 180))
    };

    void Awake()
    {
        barriersParent = GameObject.Find("Barriers");    
        wallsParent = GameObject.Find("Walls");
        coinsParent = GameObject.Find("Coins");
        portalsParent = GameObject.Find("Portals");
    }

    void Start()
    {
        //PopulateMockWalls();
        //PopulateMockBarriers();
        PopulateMockCoins();
        PopulateMockPortals();

        SendData();
    }

    //private void PopulateMockWalls()
    //{
    //    for (int i = 0; i < mockWalls.Length; i++)
    //    {
    //        walls.Add(mockWalls[i]);
    //    }
    //    DrawWalls();
    //}

    //private void PopulateMockBarriers()
    //{
    //    for (int i = 0; i < mockBarriers.Length; i++)
    //    {
    //        barriers.Add(mockBarriers[i]);
    //    }
    //    DrawBarriers();
    //}

    private void PopulateMockCoins()
    {
        for (int i = 0; i < mockCoins.Length; i++)
        {
            coins.Add(mockCoins[i]);
        }
        DrawCoins();
    }

    private void PopulateMockPortals()
    {
        // Length of portal ins must be equal to length of portals out, so every portal in has its own portal out
        // They should also be ordered, so for every index of portal in there is an index of portal out to connect
        if(mockPortalIns.Length == mockPortalOuts.Length)
        {
            for (int i = 0; i < mockPortalIns.Length; i++)
            {
                portalIns.Add(mockPortalIns[i]);
            }
            for (int i = 0; i < mockPortalOuts.Length; i++)
            {
                portalOuts.Add(mockPortalOuts[i]);
            }
        }
        DrawPortals();
    }

    private void DrawWalls()
    {
        walls.ForEach(item =>
        {
            // Create and place walls from angular-135 prefab based on position and rotation given by the server
            GameObject wall = Instantiate(GetWallPrefabFromType(item.type), item.position, Quaternion.Euler(item.rotation));
            // Put the wall into Walls folder
            wall.transform.SetParent(wallsParent.transform);
            // Change its color based on the server data
            wall.GetComponent<SpriteRenderer>().color = item.color;
        });
    }

    private void DrawBarriers()
    {
        barriers.ForEach(item =>
        {
            // Create and place barriers from barrier-300 prefab based on position and rotation given by the server
            GameObject barrier = Instantiate(GetBarrierPrefabFromType(item.type), item.position, Quaternion.Euler(item.rotation));
            // Put the barrier into Barriers folder
            barrier.transform.SetParent(barriersParent.transform);
            // Change its color based on the server data
            barrier.GetComponent<SpriteRenderer>().color = item.color;
        });
    }

    private void DrawCoins()
    {
        coins.ForEach(item => {
            // Create and place coins from coin prefab based on the position
            GameObject coin = Instantiate(coinPrefab, item, Quaternion.identity);
            // Shrink the coin size to normal level size coins of 0.8
            coin.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            coin.transform.SetParent(coinsParent.transform);
        });
    }

    private void DrawPortals()
    {
        // If count of port ins is equal to ocunt of portal outs, every portal in has its own portal out
        if (portalIns.Count == portalOuts.Count)
        {
            // loop through portal ins and create them
            for (int i = 0; i < portalIns.Count; i++)
            {
                // If portal in is of type Blue-Yellow, create that combination
                if (portalIns[i].type == "Blue-Yellow")
                {
                    // Create portal in of type blue
                    GameObject portalIn = Instantiate(portalInBlueYellow, portalIns[i].position, Quaternion.Euler(portalIns[i].rotation));
                    // Create portal out of type yellow
                    GameObject portalOut = Instantiate(portalOutBlueYellow, portalOuts[i].position, Quaternion.Euler(portalOuts[i].rotation));
                    // Connect portal out to portal in
                    portalIn.GetComponent<Portal>().SetPortalOut(portalOut);
                    // Move both portals to Portals game object in the game scene
                    portalIn.transform.SetParent(portalsParent.transform);
                    portalIn.transform.SetParent(portalsParent.transform);
                }
                // otherwise create combination of Red-Green portals
                else if (portalIns[i].type == "Red-Green")
                {
                    // Create portal in of type red
                    GameObject portalIn = Instantiate(portalInRedGreen, portalIns[i].position, Quaternion.Euler(portalIns[i].rotation));
                    // Create portal out of type green
                    GameObject portalOut = Instantiate(portalOutRedGreen, portalOuts[i].position, Quaternion.Euler(portalOuts[i].rotation));
                    // Connect portal out to portal in
                    portalIn.GetComponent<Portal>().SetPortalOut(portalOut);
                    // Move both portals to Portals game object in the game scene
                    portalIn.transform.SetParent(portalsParent.transform);
                    portalIn.transform.SetParent(portalsParent.transform);
                }
            }
        }
    }

    // This function will take in stringified json file that represents vector 3 and return actual vector 3
    private Vector3 JsonStringToVector3(string jsonData)
    {
        // jsonData is expected to be of format (0.123, -534.234, 3123)
        // Remove all spaces between coordinates and split data with commas to get x y and z coordinates
        string[] values = jsonData.Trim(' ').Split(',');

        // If the jsonData format is correct and trim and split worked well then we should receive an array of 3 values
        if (values.Length == 3)
        {
            return new Vector3(float.Parse(values[0]), float.Parse(values[1]), float.Parse(values[2]));
        }

        Debug.Log("Error parsing jsonData to Vector3: " + jsonData);
        return new Vector3(0, 0, 0);
    }

    // This function will take in stringified json file that represents vector 3 and return actual vector 3
    private Color32 JsonStringToColor32(string jsonData)
    {
        // jsonData is expected to be of format (142, 0, 231, 255). RGBA, positive less than 256
        // Remove all spaces between coordinates and split data with commas to get r g b and a values
        string[] values = jsonData.Trim(' ').Split(',');

        // If the jsonData format is correct and trim and split worked well then we should receive an array of 4 values
        if (values.Length == 4)
        {
            // Parse them from strings into integers and then into bytes, as Color32 accepts bytes only
            return new Color32((byte)Int32.Parse(values[0]), (byte)Int32.Parse(values[1]), (byte)Int32.Parse(values[2]), (byte)Int32.Parse(values[3]));
        }

        Debug.Log("Error parsing jsonData to Color32: " + jsonData);
        return new Color32(0, 0, 0, 0);
    }

    private void SendData()
    {
        string challengeurl = "http://localhost:5001/1/v1/challenge";

        StartCoroutine(GetAdLinkCoroutine(challengeurl));
    }

    IEnumerator GetAdLinkCoroutine(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            if (webRequest.isNetworkError)
            {
                Debug.Log(pages[page] + ": Error: " + webRequest.error);
            }
            else
            {
                //Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                PopulateData(webRequest.downloadHandler.text);
            }
        }
    }

    private void PopulateData(string jsonData)
    {
        // Extracting walls array json from overall data
        string[] wallsJson = JsonHelper.GetJsonObjectArray(jsonData, "walls");
        PopulateWallsData(wallsJson);

        // Extracting walls array json from overall data
        string[] barriersJson = JsonHelper.GetJsonObjectArray(jsonData, "barriers");
        PopulateBarriersData(barriersJson);

    }

    private void PopulateWallsData(string[] wallsJson)
    {
        // Populating walls for each wallJson inside walls json
        for (int i = 0; i < wallsJson.Length; i++)
        {
            // Parsing walls json into walls class object
            ChallengeWall wallInfo = JsonUtility.FromJson<ChallengeWall>(wallsJson[i]);

            // Creating tuple that a game level would understand
            // Changing color string position string and rotation string to Color32 and Vector3
            (string type, Color32 color, Vector3 position, Vector3 rotation) wallObject =
                (wallInfo.type, JsonStringToColor32(wallInfo.color), JsonStringToVector3(wallInfo.position), JsonStringToVector3(wallInfo.rotation));

            // Adding newly parsed and foramtter wall to the walls list to draw on the map
            walls.Add(wallObject);
        }

        DrawWalls();
    }

    private void PopulateBarriersData(string[] barriersJson)
    {
        // Populating barriers for each barrierJson inside walls json
        for (int i = 0; i < barriersJson.Length; i++)
        {
            // Parsing walls json into walls class object
            ChallengeWall barriersInfo = JsonUtility.FromJson<ChallengeWall>(barriersJson[i]);

            // Creating tuple that a game level would understand
            // Changing color string position string and rotation string to Color32 and Vector3
            (string type, Color32 color, Vector3 position, Vector3 rotation) barrierObject =
                (barriersInfo.type, JsonStringToColor32(barriersInfo.color), JsonStringToVector3(barriersInfo.position), JsonStringToVector3(barriersInfo.rotation));

            // Adding newly parsed and foramtter wall to the walls list to draw on the map
            barriers.Add(barrierObject);
        }

        DrawBarriers();
    }

    private GameObject GetBarrierPrefabFromType(string type)
    {
        if (type == "Barrier150")
        {
            return barrier_150;
        }
        else if (type == "Barrier200")
        {
            return barrier_200;
        }
        else if (type == "Barrier250")
        {
            return barrier_250;
        }
        else if (type == "Barrier300")
        {
            return barrier_300;
        }
        return null;
    }

    private GameObject GetWallPrefabFromType(string type)
    {
        if (type == "Angular135")
        {
            return angular_135;
        }
        else if (type == "Angular180")
        {
            return angular_180;
        }
        else if (type == "Angular225")
        {
            return angular_225;
        }
        else if (type == "Angular270")
        {
            return angular_270;
        }
        else if (type == "Horizontal135")
        {
            return horizontal_135;
        }
        else if (type == "Horizontal180")
        {
            return horizontal_180;
        }
        else if (type == "Horizontal225")
        {
            return horizontal_225;
        }
        else if (type == "Horizontal270")
        {
            return horizontal_270;
        }
        else if (type == "Vertical135")
        {
            return vertical_135;
        }
        else if (type == "Vertical180")
        {
            return vertical_180;
        }
        else if (type == "Vertical225")
        {
            return vertical_225;
        }
        else if (type == "Vertical270")
        {
            return vertical_270;
        }
        return null;
    }
}
