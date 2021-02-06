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

    GameObject challengeLevelBackground;
    GameObject challengeLevelScreenshot;

    void Awake()
    {
        server = FindObjectOfType<Server>();
        navigator = FindObjectOfType<Navigator>();

        challengeLevelBackground = GameObject.Find("ChallengeLevelBackground");
        challengeLevelScreenshot = GameObject.Find("ChallengeLevelScreenshot");
    }

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>();

        server.GetLatestChallengeMini();
    }

    public void SetBackgroundImage(Sprite sprite)
    {
        challengeLevelBackground.GetComponent<Image>().sprite = sprite;
    }

    public void SetScreenshotImage(Sprite sprite)
    {
        challengeLevelScreenshot.GetComponent<Image>().sprite = sprite;
    }

    public void LatestChallengeMiniSuccess(ChallengeLevelMini challengeLevel)
    {
        Debug.Log(challengeLevel.tried);
        Debug.Log(challengeLevel.solved);
        Debug.Log(challengeLevel.coins);
        Debug.Log(challengeLevel.diamonds);
        Debug.Log(challengeLevel.screenshot);
        Debug.Log(challengeLevel.background);
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
