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

    public class ChallengePortal
    {
        public string type;
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

    void Awake()
    {
        barriersParent = GameObject.Find("Barriers");    
        wallsParent = GameObject.Find("Walls");
        coinsParent = GameObject.Find("Coins");
        portalsParent = GameObject.Find("Portals");
    }

    void Start()
    {
        SendData();
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
                // Create portal in of type given in data sent from server
                GameObject portalIn = Instantiate(GetPortalPrefabFromType(portalIns[i].type).Item1, portalIns[i].position, Quaternion.Euler(portalIns[i].rotation));
                // Create portal out of type given in data sent from server
                GameObject portalOut = Instantiate(GetPortalPrefabFromType(portalOuts[i].type).Item2, portalOuts[i].position, Quaternion.Euler(portalOuts[i].rotation));
                // Connect portal out to portal in
                portalIn.GetComponent<Portal>().SetPortalOut(portalOut);
                // Move both portals to Portals game object in the game scene
                portalIn.transform.SetParent(portalsParent.transform);
                portalOut.transform.SetParent(portalsParent.transform);
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

        // Extracting barriers array json from overall data
        string[] barriersJson = JsonHelper.GetJsonObjectArray(jsonData, "barriers");
        PopulateBarriersData(barriersJson);

        // Extracting portals in and portals out array json from overall data
        string[] portalInsJson = JsonHelper.GetJsonObjectArray(jsonData, "portalIns");
        string[] portalOutsJson = JsonHelper.GetJsonObjectArray(jsonData, "portalOuts");
        PopulatePortalsData(portalInsJson, portalOutsJson);

        // Extracting coins array json from overall data
        string[] coinsJson = JsonHelper.GetJsonObjectArray(jsonData, "coins");
        PopulateCoinsData(coinsJson);
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
            // Parsing barriers json into barriers class object
            ChallengeBarrier barriersInfo = JsonUtility.FromJson<ChallengeBarrier>(barriersJson[i]);

            // Creating tuple that a game level would understand
            // Changing color string position string and rotation string to Color32 and Vector3
            (string type, Color32 color, Vector3 position, Vector3 rotation) barrierObject =
                (barriersInfo.type, JsonStringToColor32(barriersInfo.color), JsonStringToVector3(barriersInfo.position), JsonStringToVector3(barriersInfo.rotation));

            // Adding newly parsed and foramtter wall to the walls list to draw on the map
            barriers.Add(barrierObject);
        }

        DrawBarriers();
    }

    private void PopulatePortalsData(string[] portalInsJson, string[] portalOutsJson)
    {
        // Length of portal ins must be equal to length of portals out, so every portal in has its own portal out
        // They should also be ordered, so for every index of portal in there is an index of portal out to connect
        if (portalInsJson.Length == portalOutsJson.Length)
        {
            for (int i = 0; i < portalInsJson.Length; i++)
            {
                // Parsing portalIns json into portalIn class object
                ChallengePortal portalInsInfo = JsonUtility.FromJson<ChallengePortal>(portalInsJson[i]);

                // Creating tuple that a game level would understand
                // Changing position string and rotation string to Vector3
                (string type, Vector3 position, Vector3 rotation) portalInsObject =
                    (portalInsInfo.type, JsonStringToVector3(portalInsInfo.position), JsonStringToVector3(portalInsInfo.rotation));
                
                portalIns.Add(portalInsObject);
            }
            for (int i = 0; i < portalOutsJson.Length; i++)
            {
                // Parsing portalOuts json into portalIn class object
                ChallengePortal portalOutsInfo = JsonUtility.FromJson<ChallengePortal>(portalOutsJson[i]);

                // Creating tuple that a game level would understand
                // Changing position string and rotation string to Vector3
                (string type, Vector3 position, Vector3 rotation) portalOutsObject =
                    (portalOutsInfo.type, JsonStringToVector3(portalOutsInfo.position), JsonStringToVector3(portalOutsInfo.rotation));

                portalOuts.Add(portalOutsObject);
            }
        }

        DrawPortals();
    }

    private void PopulateCoinsData(string[] coinsJson)
    {
        // Populating barriers for each barrierJson inside walls json
        for (int i = 0; i < coinsJson.Length; i++)
        {
            // Parsing coins json into coins class object
            ChallengeCoin coinsInfo = JsonUtility.FromJson<ChallengeCoin>(coinsJson[i]);

            // Creating tuple that a game level would understand
            Vector3 coinsObject = JsonStringToVector3(coinsInfo.position);

            // Adding newly parsed and foramtter wall to the walls list to draw on the map
            coins.Add(coinsObject);
        }

        DrawCoins();
    }

    private (GameObject, GameObject) GetPortalPrefabFromType(string type)
    {
        if (type == "BlueYellow")
        {
            return (portalInBlueYellow, portalOutBlueYellow);
        }
        else if (type == "RedGreen")
        {
            return (portalInRedGreen, portalOutRedGreen);
        }
        return (null, null);
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
