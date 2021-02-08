using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using static Server;

public class ChallengeStatus : MonoBehaviour
{
    Server server;

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
    GameObject barriersParent;

    // To set parent of all walls
    GameObject wallsParent;

    // To set parent of all coins
    GameObject portalsParent;

    // For every index of a portal in there should be a portal out to connect
    // Type will be a Red-Green or a Blue-Yellow to show which type color pair of portals to place
    List<ChallengePortal> portalIns = new List<ChallengePortal>();
    List<ChallengePortal> portalOuts = new List<ChallengePortal>();
    List<ChallengeWall> walls = new List<ChallengeWall>();
    List<ChallengeBarrier> barriers = new List<ChallengeBarrier>();

    int lives = 5;

    // Reward from server
    int diamonds = 0;
    int coins = 0;

    GameObject ball;
    GameObject ballDirectionArrow;
    GameObject ballCatcher;

    string challengeId = "601fce7f2169911a30dbbe42";

    void Awake()
    {
        server = FindObjectOfType<Server>();
        barriersParent = GameObject.Find("Barriers");    
        wallsParent = GameObject.Find("Walls");
        portalsParent = GameObject.Find("Portals");

        ball = GameObject.Find("Ball");
        ballDirectionArrow = GameObject.Find("BallDirectionArrow");
        ballCatcher = GameObject.Find("BallCatcher");
    }

    void Start()
    {
        server.GetCurrentChallenge();
    }

    public void CurrentChallengeError()
    {
        Debug.Log("error loading latest challenge");
    }

    public void CurrentChallengeSuccess(
        List<ChallengeWall> wallsData,
        List<ChallengeBarrier> barriersData,
        List<ChallengePortal> portalsData,
        ChallengeBall ballData,
        ChallengeBallCatcher ballCatcherData)
    {
        // Set ball position and direction
        ball.transform.position = ballData.position;
        ball.GetComponent<Ball>().SetDirection(ballData.direction);

        // Set ballCatcher position
        ballCatcher.transform.position = ballCatcherData.position;

        Debug.Log(wallsData[0].type);

        // Set walls
        walls = wallsData;

        DrawWalls();

        // Set walls
        barriers = barriersData;

        DrawBarriers();

        for (int i = 0; i < portalsData.Count; i++)
        {
            if (portalsData[i].type == "Portal-In-Blue-Yellow" ||
                portalsData[i].type == "Portal-In-Red-Green")
            {
                portalIns.Add(portalsData[i]);
            } else if (portalsData[i].type == "Portal-Out-Blue-Yellow" ||
                portalsData[i].type == "Portal-Out-Red-Green")
            {
                portalOuts.Add(portalsData[i]);
            }
        }

        DrawPortals();
    }

    private void DrawWalls()
    {
        walls.ForEach(item =>
        {
            Debug.Log(item.type);
            Debug.Log(item.color);
            Debug.Log(item.position);
            Debug.Log(item.rotation);
            // Create and place walls from angular-135 prefab based on position and rotation given by the server
            GameObject wall = Instantiate(GetWallPrefabFromType(item.type), item.position, Quaternion.Euler(item.rotation));
            // Put the wall into Walls folder
            wall.transform.SetParent(wallsParent.transform);
            // Change its color based on the server data
            wall.GetComponent<SpriteRenderer>().color = item.color;
            wall.GetComponent<Wall>().SaveInitialColor();
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
                GameObject portalIn = Instantiate(GetPortalPrefabFromType(portalIns[i].type), portalIns[i].position, Quaternion.Euler(portalIns[i].rotation));
                // Create portal out of type given in data sent from server
                GameObject portalOut = Instantiate(GetPortalPrefabFromType(portalOuts[i].type), portalOuts[i].position, Quaternion.Euler(portalOuts[i].rotation));
                // Connect portal out to portal in
                portalIn.GetComponent<Portal>().SetPortalOut(portalOut);
                // Move both portals to Portals game object in the game scene
                portalIn.transform.SetParent(portalsParent.transform);
                portalOut.transform.SetParent(portalsParent.transform);
            }
        }
    }

    private GameObject GetPortalPrefabFromType(string type)
    {
        if (type == "Portal-In-Blue-Yellow")
        {
            return portalInBlueYellow;
        }
        else if (type == "Portal-In-Red-Green")
        {
            return portalInRedGreen;
        }
        else if (type == "Portal-Out-Blue-Yellow")
        {
            return portalOutRedGreen;
        }
        else if (type == "Portal-Out-Red-Green")
        {
            return portalOutRedGreen;
        }
        return null;
    }

    private GameObject GetBarrierPrefabFromType(string type)
    {
        if (type == "Barrier-150")
        {
            return barrier_150;
        }
        else if (type == "Barrier-200")
        {
            return barrier_200;
        }
        else if (type == "Barrier-250")
        {
            return barrier_250;
        }
        else if (type == "Barrier-300")
        {
            return barrier_300;
        }
        return null;
    }

    private GameObject GetWallPrefabFromType(string type)
    {
        if (type == "Angular-135")
        {
            return angular_135;
        }
        else if (type == "Angular-180")
        {
            return angular_180;
        }
        else if (type == "Angular-225")
        {
            return angular_225;
        }
        else if (type == "Angular-270")
        {
            return angular_270;
        }
        else if (type == "Horizontal-135")
        {
            return horizontal_135;
        }
        else if (type == "Horizontal-180")
        {
            return horizontal_180;
        }
        else if (type == "Horizontal-225")
        {
            return horizontal_225;
        }
        else if (type == "Horizontal-270")
        {
            return horizontal_270;
        }
        else if (type == "Vertical-135")
        {
            return vertical_135;
        }
        else if (type == "Vertical-180")
        {
            return vertical_180;
        }
        else if (type == "Vertical-225")
        {
            return vertical_225;
        }
        else if (type == "Vertical-270")
        {
            return vertical_270;
        }
        return null;
    }
}
