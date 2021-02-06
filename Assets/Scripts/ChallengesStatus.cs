using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Bring classes from Server
using static Server;

public class ChallengesStatus : MonoBehaviour
{
    Navigator navigator;
    Player player;
    Server server;

    void Awake()
    {
        server = FindObjectOfType<Server>();
        navigator = FindObjectOfType<Navigator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>();

        server.GetLatestChallengeMini();
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
