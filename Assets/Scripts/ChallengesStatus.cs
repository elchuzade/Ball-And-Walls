using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
// Bring classes from Server
using static Server;

public class ChallengesStatus : MonoBehaviour
{
    Navigator navigator;
    Player player;
    Server server;


    GameObject tried;
    GameObject solved;
    GameObject challengeLevelScreenshot;

    void Awake()
    {
        server = FindObjectOfType<Server>();
        navigator = FindObjectOfType<Navigator>();

        challengeLevelScreenshot = GameObject.Find("ChallengeLevelScreenshot");
        tried = GameObject.Find("TriedText");
        solved = GameObject.Find("SolvedText");
    }

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>();

        server.GetLatestChallengeMini();
    }

    public void SetScreenshotImage(Sprite sprite)
    {
        challengeLevelScreenshot.GetComponent<Image>().sprite = sprite;
    }

    public void LatestChallengeMiniSuccess(ChallengeLevelMini challengeLevel)
    {
        tried.GetComponent<Text>().text = challengeLevel.tried.ToString() + " tried";
        solved.GetComponent<Text>().text = challengeLevel.tried.ToString() + " solved";
        Debug.Log(challengeLevel.coins);
        Debug.Log(challengeLevel.diamonds);
    }

    public void LatestChallengeMiniError()
    {
        Debug.Log("error");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
