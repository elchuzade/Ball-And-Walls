using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChallengeStatus : MonoBehaviour
{
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
    private List<(string type, Vector3 position, Vector3 rotation)> walls = new List<(string type, Vector3 position, Vector3 rotation)>();
    private List<(string type, Vector3 position, Vector3 rotation)> barriers = new List<(string type, Vector3 position, Vector3 rotation)>();

    private (string type, Vector3 position, Vector3 rotation)[] mockWalls =
    {
        ("Angular180", new Vector3(616, 979.8f, 0), new Vector3(0, 0, -135.525f)),
        ("Angular180", new Vector3(258, 1025, 0), new Vector3(0, 0, -90)),
        ("Angular135", new Vector3(291, 503, 0), new Vector3(0, 0, 175.685f)),
        ("Angular180", new Vector3(338.3f, 702, 0), new Vector3(0, 0, 90)),
        ("Angular135", new Vector3(407.7f, 891.8f, 0), new Vector3(0, 0, 42)),
        ("Horizontal270", new Vector3(407.7f, 891.8f, 0), new Vector3(0, 0, 42)),
        ("Vertical225", new Vector3(407.7f, 891.8f, 0), new Vector3(0, 0, 42))
    };
    private (string type, Vector3 position, Vector3 rotation)[] mockBarriers =
    {
        ("Barrier150", new Vector3(-133, 122, 0), new Vector3(0, 0, -51.883f)),
        ("Barrier300", new Vector3(212.5f, -492.6385f, 0), new Vector3(0, 0, 0)),
        ("Barrier300", new Vector3(123.5f, -411.5f, 0), new Vector3(0, 0, -90)),
        ("Barrier300", new Vector3(37.29999f, -495.769f, 0), new Vector3(0, 0, 0)),
        ("Barrier150", new Vector3(266, -127, 0), new Vector3(0, 0, -179.684f)),
        ("Barrier200", new Vector3(202.5f, 107, 0), new Vector3(0, 0, -90)),
        ("Barrier200", new Vector3(-340, -100, 0), new Vector3(0, 0, -7.424f)),
        ("Barrier250", new Vector3(58.8f, -357.8f, 0), new Vector3(0, 0, -50)),
        ("Barrier250", new Vector3(-241.3662f, 370, 0), new Vector3(0, 0, 0)),
        ("Barrier250", new Vector3(-241.3662f, 242, 0), new Vector3(0, 0, 0)),
        ("Barrier250", new Vector3(186.8f, -356.8f, 0), new Vector3(0, 0, 50)),
        ("Barrier300", new Vector3(-285.3465f, -540.1748f, 0), new Vector3(0, 0, -90.43301f)),
        ("Barrier300", new Vector3(-287.1519f, -364.9561f, 0), new Vector3(0, 0, -90.43301f))
    };
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
        PopulateMockWalls();
        PopulateMockBarriers();
        PopulateMockCoins();
        PopulateMockPortals();
    }

    private void PopulateMockWalls()
    {
        for (int i = 0; i < mockWalls.Length; i++)
        {
            walls.Add(mockWalls[i]);
        }
        DrawWalls();
    }

    private void PopulateMockBarriers()
    {
        for (int i = 0; i < mockBarriers.Length; i++)
        {
            barriers.Add(mockBarriers[i]);
        }
        DrawBarriers();
    }

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
            if (item.type == "Angular135")
            {
                // Create and place walls from angular-135 prefab based on position and rotation given by the server
                GameObject wall = Instantiate(angular_135, item.position, Quaternion.Euler(item.rotation));
                wall.transform.SetParent(wallsParent.transform);
            }
            else if (item.type == "Angular180")
            {
                // Create and place walls from angular-180 prefab based on position and rotation given by the server
                GameObject wall = Instantiate(angular_135, item.position, Quaternion.Euler(item.rotation));
                wall.transform.SetParent(wallsParent.transform);
            }
            else if (item.type == "Angular225")
            {
                // Create and place walls from angular-225 prefab based on position and rotation given by the server
                GameObject wall = Instantiate(angular_135, item.position, Quaternion.Euler(item.rotation));
                wall.transform.SetParent(wallsParent.transform);
            }
            else if (item.type == "Angular270")
            {
                // Create and place walls from angular-270 prefab based on position and rotation given by the server
                GameObject wall = Instantiate(angular_135, item.position, Quaternion.Euler(item.rotation));
                wall.transform.SetParent(wallsParent.transform);
            }
            else if (item.type == "Horizontal135")
            {
                // Create and place walls from horizontal-135 prefab based on position and rotation given by the server
                GameObject wall = Instantiate(horizontal_135, item.position, Quaternion.Euler(item.rotation));
                wall.transform.SetParent(wallsParent.transform);
            }
            else if (item.type == "Horizontal180")
            {
                // Create and place walls from horizontal-180 prefab based on position and rotation given by the server
                GameObject wall = Instantiate(horizontal_180, item.position, Quaternion.Euler(item.rotation));
                wall.transform.SetParent(wallsParent.transform);
            }
            else if (item.type == "Horizontal225")
            {
                // Create and place walls from horizontal-225 prefab based on position and rotation given by the server
                GameObject wall = Instantiate(horizontal_225, item.position, Quaternion.Euler(item.rotation));
                wall.transform.SetParent(wallsParent.transform);
            }
            else if (item.type == "Horizontal270")
            {
                // Create and place walls from horizontal-270 prefab based on position and rotation given by the server
                GameObject wall = Instantiate(horizontal_270, item.position, Quaternion.Euler(item.rotation));
                wall.transform.SetParent(wallsParent.transform);
            }
            else if (item.type == "Vertical135")
            {
                // Create and place walls from vertical-135 prefab based on position and rotation given by the server
                GameObject wall = Instantiate(vertical_135, item.position, Quaternion.Euler(item.rotation));
                wall.transform.SetParent(wallsParent.transform);
            }
            else if (item.type == "Vertical180")
            {
                // Create and place walls from vertical-180 prefab based on position and rotation given by the server
                GameObject wall = Instantiate(vertical_180, item.position, Quaternion.Euler(item.rotation));
                wall.transform.SetParent(wallsParent.transform);
            }
            else if (item.type == "Vertical225")
            {
                // Create and place walls from vertical-225 prefab based on position and rotation given by the server
                GameObject wall = Instantiate(vertical_225, item.position, Quaternion.Euler(item.rotation));
                wall.transform.SetParent(wallsParent.transform);
            }
            else if (item.type == "Vertical270")
            {
                // Create and place walls from vertical-270 prefab based on position and rotation given by the server
                GameObject wall = Instantiate(vertical_270, item.position, Quaternion.Euler(item.rotation));
                wall.transform.SetParent(wallsParent.transform);
            }
        });
    }

    private void DrawBarriers()
    {
        barriers.ForEach(item =>
        {
            if (item.type == "Barrier150")
            {
                // Create and place barriers from barrier-150 prefab based on position and rotation given by the server adding parent position
                GameObject barrier = Instantiate(barrier_150, item.position, Quaternion.Euler(item.rotation));
                barrier.transform.SetParent(barriersParent.transform);
            }
            else if (item.type == "Barrier200")
            {
                // Create and place barriers from barrier-200 prefab based on position and rotation given by the server adding parent position
                GameObject barrier = Instantiate(barrier_200, item.position, Quaternion.Euler(item.rotation));
                barrier.transform.SetParent(barriersParent.transform);
            }
            else if (item.type == "Barrier250")
            {
                // Create and place barriers from barrier-250 prefab based on position and rotation given by the server adding parent position
                GameObject barrier = Instantiate(barrier_250, item.position, Quaternion.Euler(item.rotation));
                barrier.transform.SetParent(barriersParent.transform);
            }
            else if (item.type == "Barrier300")
            {
                // Create and place barriers from barrier-300 prefab based on position and rotation given by the server adding parent position
                GameObject barrier = Instantiate(barrier_300, item.position, Quaternion.Euler(item.rotation));
                barrier.transform.SetParent(barriersParent.transform);
            }
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
}
