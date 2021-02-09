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
    GameObject challengeScreenshot;

    [SerializeField] GameObject challengeItem;
    GameObject allChallengesScrollContent;

    // To store all levels to scroll from
    List<PastChallenge> pastChallenges;
    // To store currently selected level
    PastChallenge selectedChallenge;

    void Awake()
    {
        server = FindObjectOfType<Server>();
        navigator = FindObjectOfType<Navigator>();

        allChallengesScrollContent = GameObject.Find("AllChallengesScrollContent");
        challengeScreenshot = GameObject.Find("ChallengeScreenshot");
        tried = GameObject.Find("TriedText");
        solved = GameObject.Find("SolvedText");
    }

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>();

        server.GetPastChallenges();
    }

    public void SetScreenshotImage(Sprite sprite)
    {
        challengeScreenshot.GetComponent<Image>().sprite = sprite;
    }

    public void PastChallengesSuccess(List<PastChallenge> challenge)
    {
        pastChallenges = challenge;
        PopulateChallenges();
    }

    public void PastChallengesError()
    {
        Debug.Log("error");
    }

    private void UnlockChallengeLevel(GameObject level)
    {
        level.GetComponent<ChallengeItem>().SetLocked(false);
    }

    private void ClickChallengeLevel(GameObject level)
    {
        selectedChallenge = pastChallenges[level.GetComponent<ChallengeItem>().GetIndex()];

        // Extract and set screenshot of selected challenge
        StartCoroutine(server.ExtractPastChallengeScreenshotImage(selectedChallenge));

        // Set tried and solved and load the screenshot of selected level
        SetSelectedChallenge(selectedChallenge);
        Debug.Log(selectedChallenge.screenshot);
    }

    public void SetSelectedChallenge(PastChallenge challenge)
    {
        tried.GetComponent<Text>().text = challenge.tried.ToString() + "tried";
        solved.GetComponent<Text>().text = challenge.solved.ToString() + "solved";
    }

    private void PlaySelectedChallenge()
    {
        // Set selected challenge level id to player prefs
        PlayerPrefs.SetString("challengeId", selectedChallenge.id);
    }

    private void PopulateChallenges()
    {
        for (int i = 0; i < pastChallenges.Count; i++)
        {
            GameObject challengeInstance = Instantiate(challengeItem, transform.position, Quaternion.identity);

            challengeInstance.GetComponent<ChallengeItem>().SetIndex(i);

            if (pastChallenges[i].status != "new" || i == 0)
            {
                // This level is unlocked, hide the locked frame
                challengeInstance.transform.Find("LockedFrame").gameObject.SetActive(false);
            }

            challengeInstance.transform.Find("LevelTop")
                    .GetComponent<Button>().onClick.AddListener(() => ClickChallengeLevel(challengeInstance));

            challengeInstance.transform.Find("LockedFrame")
                .GetComponent<Button>().onClick.AddListener(() => UnlockChallengeLevel(challengeInstance));


            challengeInstance.transform.SetParent(allChallengesScrollContent.transform);
        }
    }
}
