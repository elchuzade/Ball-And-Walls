using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

using static Server;

public class ChallengeStatus : MonoBehaviour
{
    Server server;
    Player player;
    Scoreboard scoreboard;
    Navigator navigator;

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

    GameObject extraLifeAd;

    // To set parent of all barriers
    GameObject barriersParent;

    // To set parent of all walls
    GameObject wallsParent;

    // To set parent of all coins
    GameObject portalsParent;

    // Continue watching and receive a reward button when player wanted to skip the video
    GameObject adWarningReceiveButton;
    // Reject reward and continue playing game button when player wanted to skip the video
    GameObject adWarningContinueButton;
    // Background of window that pops up if player wants to switch off reward ad
    GameObject adCancelBg;
    // Window with buttons that pops up if player wants to switch off reward ad
    GameObject adCancelWarning;

    // For every index of a portal in there should be a portal out to connect
    // Type will be a Red-Green or a Blue-Yellow to show which type color pair of portals to place
    List<ChallengePortal> portalIns = new List<ChallengePortal>();
    List<ChallengePortal> portalOuts = new List<ChallengePortal>();
    List<ChallengeWall> walls = new List<ChallengeWall>();
    List<ChallengeBarrier> barriers = new List<ChallengeBarrier>();

    // Parent that holds all 5 lives
    GameObject livesParent;

    int lives = 5;

    // Reward from server
    int diamonds = 0;
    int coins = 0;

    GameObject ball;
    GameObject ballCatcher;
    GameObject getLifeButton;

    // Track if ad cancel warning has already been shown, not to annoy each time showing the same window
    bool showedAdCancelWarning = false;

    void Awake()
    {
        scoreboard = FindObjectOfType<Scoreboard>();
        navigator = FindObjectOfType<Navigator>();
        server = FindObjectOfType<Server>();

        barriersParent = GameObject.Find("Barriers");    
        wallsParent = GameObject.Find("Walls");
        portalsParent = GameObject.Find("Portals");

        ball = GameObject.Find("Ball");
        ballCatcher = GameObject.Find("BallCatcher");
        livesParent = GameObject.Find("Lives");

        extraLifeAd = GameObject.Find("ExtraLifeAd");
        getLifeButton = GameObject.Find("GetLifeButton");
        adCancelBg = GameObject.Find("AdCancelBg");
        adCancelWarning = GameObject.Find("AdCancelWarning");
        adWarningReceiveButton = GameObject.Find("AdWarningReceiveButton");
        adWarningContinueButton = GameObject.Find("AdWarningContinueButton");
    }

    void Start()
    {
        player = FindObjectOfType<Player>();

        server.GetCurrentChallenge();
        SetLives();

        // Hide it and return its scale to normal
        extraLifeAd.transform.localScale = new Vector3(1, 1, 1);
        extraLifeAd.SetActive(false);
    }

    private void SetLives()
    {
        for (int i = 0; i < livesParent.transform.childCount; i++)
        {
            if (i < lives)
            {
                livesParent.transform.GetChild(i).GetComponent<Image>().color =
                    new Color32(255, 255, 255, 255);
            } else
            {
                livesParent.transform.GetChild(i).GetComponent<Image>().color =
                    new Color32(60, 60, 60, 255);
            }
        }
    }

    private void SetEarnLifeAdScreen()
    {
        Debug.Log("ads");
        extraLifeAd.SetActive(true);
    }

    // When you lose
    public void DecreaseLife()
    {
        if (lives > 0)
        {
            lives--;
            SetLives();

            player.lives--;
            player.SavePlayer();

            if (lives == 0)
            {
                SetEarnLifeAdScreen();
            }
        }
    }

    // When you watch ads
    public void IncreaseLife()
    {
        if (lives == 0)
        {
            lives++;
            SetLives();
        }
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
        
        ball.GetComponent<Ball>().SetPosition(ballData.position);
        ball.GetComponent<Ball>().SetDirection(ballData.direction);

        // Set ballCatcher position
        ballCatcher.transform.position = ballCatcherData.position;

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
            return portalOutBlueYellow;
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

    public void GetMoreCoins()
    {
        if (getLifeButton.GetComponent<Button>().IsInteractable())
        {
            // Run the trigger button animation and disable button for its duration
            getLifeButton.GetComponent<TriggerButton>().ClickButton(0.2f);
            // Approximately when animation is over, load get more coins ad
            StartCoroutine(LoadGetMoreCoinsCoroutine(0.2f));
        }
    }

    public IEnumerator LoadGetMoreCoinsCoroutine(float time)
    {
        // Wait for given time and load the ad screen
        yield return new WaitForSeconds(time);

        AdManager.ShowStandardAd(GetCoinsSuccess, GetCoinsCancel, GetCoinsFail);
    }


    public void ReceiveCoinsButtonClick()
    {
        if (adWarningReceiveButton.GetComponent<Button>().IsInteractable())
        {
            // Run animation of clicking receive coins and watch the ad button
            adWarningReceiveButton.GetComponent<TriggerButton>().ClickButton(0.2f);
            // Approximately when animation is finished, load the ad screen
            StartCoroutine(ReceiveCoinsButtonCoroutine(0.2f));
        }
    }

    // This is similar to LoadGetMoreCoins. But for consistency, it is better to keep it separately
    public IEnumerator ReceiveCoinsButtonCoroutine(float time)
    {
        // Wait for given time and load the ad screen
        yield return new WaitForSeconds(time);
        // Load the ad screen
        AdManager.ShowStandardAd(GetCoinsSuccess, GetCoinsCancel, GetCoinsFail);
    }

    public void ContinuePlayingButtonClick()
    {
        if (adWarningContinueButton.GetComponent<Button>().IsInteractable())
        {
            // Wait for given time and load the ad screen
            adWarningContinueButton.GetComponent<TriggerButton>().ClickButton(0.2f);
            // Approximately when animation is finished, load the ad screen
            StartCoroutine(ContinuePlayingButtonCoroutine(0.2f));
        }
    }

    public IEnumerator ContinuePlayingButtonCoroutine(float time)
    {
        yield return new WaitForSeconds(time);

        // Wait for some time and hide all the ad stuff, since player has cancelled reward and refused to watch the ad
        adCancelBg.SetActive(false);
        adCancelWarning.SetActive(false);
    }

    private void GetCoinsCancel()
    {
        // Playe has canceled get extra ccoins video
        if (!showedAdCancelWarning)
        {
            // If it is the first time, show him a warnigng screen with ad stuff
            adCancelBg.SetActive(true);
            adCancelWarning.SetActive(true);
        }
        else
        {
            // else hide a warning screen with ad stuff
            adCancelBg.SetActive(false);
            adCancelWarning.SetActive(false);
        }

        // Set a parameter to remmeber that once ad stuff was already cancelled not to ask a player again when he skips another ad
        showedAdCancelWarning = true;
    }

    private void GetCoinsFail()
    {
        // If a video for receiving coins fails, hide the warning page about cancelling, not to annoy the player
        // Set a parameter to remmeber that once ad stuff was already cancelled not to ask a player again when he skips another ad
        showedAdCancelWarning = true;
        adCancelBg.SetActive(false);
        adCancelWarning.SetActive(false);
    }

    public void GetCoinsSuccess()
    {
        // If video has been played suvvessfully till the end give the reward
        // Increase player coins by ad reward amount
        if (player.lives == 0)
        {
            player.lives++;
            IncreaseLife();
            player.SavePlayer();
        }

        // Set a parameter to remmeber that once ad stuff was already cancelled not to ask a player again when he skips another ad
        showedAdCancelWarning = true;
        adCancelBg.SetActive(false);
        adCancelWarning.SetActive(false);
    }
}
