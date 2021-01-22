using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;
using System;

public class ChallengeSaverStatus : MonoBehaviour
{
    string[] barrierTypes = { "Barrier-150", "Barrier-200", "Barrier-250", "Barrier-300" };
    string[] wallTypes = { "Angular-135", "Angular-180", "Angular-225", "Angular-270", "Horizontal-135", "Horizontal-180", "Horizontal-225", "Horizontal-270", "Vertical-135", "Vertical-180", "Vertical-225", "Vertical-270" };
    string[] portalTypes = { "Portal-In-Blue-Yellow", "Portal-Out-Blue-Yellow", "Portal-In-Red-Green", "Portal-Out-Red-Green" };

    public class ChallengeData
    {
        public List<string> walls = new List<string>();
        public List<string> barriers = new List<string>();
        public List<string> portals = new List<string>();
    }

    public class ChallengeWall
    {
        public string type;
        public Color32 color;
        public Vector3 position;
        public Vector3 rotation;
    }

    public class ChallengeBarrier
    {
        public string type;
        public Color32 color;
        public Vector3 position;
        public Vector3 rotation;
    }

    public class ChallengePortal
    {
        public string type;
        public Vector3 position;
        public Vector3 rotation;
    }

    GameObject saveJsonButton;

    GameObject barriers;
    GameObject walls;
    GameObject portals;

    ChallengeWall[] challengeWalls;
    ChallengeBarrier[] challengeBarriers;
    ChallengePortal[] challengePortals;

    ChallengeData saveData;

    void Awake()
    {
        saveJsonButton = GameObject.Find("SaveJsonButton");
        barriers = GameObject.Find("Barriers");
        walls = GameObject.Find("Walls");
        portals = GameObject.Find("Portals");
    }

    void Start()
    {
        saveData = new ChallengeData();

        saveJsonButton.GetComponent<Button>().onClick.AddListener(() => SaveLevelJson());
    }

    public void SaveLevelJson()
    {
        // Saving barriers, walls and portals
        SaveBarriers();
        SaveWalls();
        SavePortals();

        string saveDataJson = JsonUtility.ToJson(saveData);
        System.IO.File.WriteAllText(Application.dataPath + "/SaveData.json", saveDataJson);
    }

    private void SaveBarriers()
    {
        for (int i = 0; i < barriers.transform.childCount; i++)
        {
            SaveBarrier(barriers.transform.GetChild(i).gameObject);
        }
    }

    // Save each barrier from the list of barriers in the scene
    private void SaveBarrier(GameObject barrier)
    {
        ChallengeBarrier barrierInfo = new ChallengeBarrier();

        // Make sure all objects are named correctly
        TestBarrierType(barrier.name);
        // Get the name of object. 
        barrierInfo.type = barrier.name;
        barrierInfo.color = barrier.GetComponent<SpriteRenderer>().color;
        barrierInfo.position = barrier.transform.position;
        barrierInfo.rotation = new Vector3(
            barrier.transform.rotation.x,
            barrier.transform.rotation.y,
            barrier.transform.rotation.z);

        // Save data about every barrier into the list
        string barrierJson = JsonUtility.ToJson(barrierInfo);
        saveData.barriers.Add(barrierJson);
    }

    // Check if the name of the object in scene is correct
    private void TestBarrierType(string barrierType)
    {
        if (!barrierTypes.Contains(barrierType))
        {
            // If at least one barrier is named incorrect, stop the program
            throw new Exception("Barrier Name is incorrect: " + barrierType);
        }
    }

    private void SaveWalls()
    {
        for (int i = 0; i < walls.transform.childCount; i++)
        {
            SaveWall(walls.transform.GetChild(i).gameObject);
        }
    }

    // Save each barrier from the list of barriers in the scene
    private void SaveWall(GameObject wall)
    {
        ChallengeWall wallInfo = new ChallengeWall();

        // Make sure all objects are named correctly
        TestWallType(wall.name);
        // Get the name of object. 
        wallInfo.type = wall.name;
        wallInfo.color = wall.GetComponent<SpriteRenderer>().color;
        wallInfo.position = wall.transform.position;
        wallInfo.rotation = new Vector3(
            wall.transform.rotation.x,
            wall.transform.rotation.y,
            wall.transform.rotation.z);

        // Save data about every barrier into the list
        string wallJson = JsonUtility.ToJson(wallInfo);
        saveData.walls.Add(wallJson);
    }

    // Check if the name of the object in scene is correct
    private void TestWallType(string wallType)
    {
        if (!wallTypes.Contains(wallType))
        {
            // If at least one barrier is named incorrect, stop the program
            throw new Exception("Wall Name is incorrect: " + wallType);
        }
    }

    private void SavePortals()
    {
        for (int i = 0; i < portals.transform.childCount; i++)
        {
            SavePortal(portals.transform.GetChild(i).gameObject);
        }
    }

    // Save each barrier from the list of barriers in the scene
    private void SavePortal(GameObject portal)
    {
        ChallengePortal portalInfo = new ChallengePortal();

        // Make sure all objects are named correctly
        TestPortalType(portal.name);
        // Get the name of object. 
        portalInfo.type = portal.name;
        portalInfo.position = portal.transform.position;
        portalInfo.rotation = new Vector3(
            portal.transform.rotation.x,
            portal.transform.rotation.y,
            portal.transform.rotation.z);

        // Save data about every barrier into the list
        string portalJson = JsonUtility.ToJson(portalInfo);
        saveData.portals.Add(portalJson);
    }

    // Check if the name of the object in scene is correct
    private void TestPortalType(string portalType)
    {
        if (!portalTypes.Contains(portalType))
        {
            // If at least one barrier is named incorrect, stop the program
            throw new Exception("Portal Name is incorrect: " + portalType);
        }
    }
}
