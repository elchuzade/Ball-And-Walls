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
    Scoreboard scoreboard;

    [SerializeField] Sprite[] ballSprites;

    Sprite currentBallSprite;

    GameObject tried;
    GameObject solved;
    GameObject challengeScreenshot;

    [SerializeField] GameObject challengeItem;
    GameObject allChallengesScrollContent;

    GameObject diamondLives;
    GameObject videoLives;
    GameObject playChallenge;
    GameObject exitButton;

    AdCancel adCancel;

    bool showedAdCancelWarning = false;

    // To store all levels to scroll from
    List<PastChallenge> pastChallenges;
    // To store currently selected level
    PastChallenge selectedChallenge;

    void Awake()
    {
        server = FindObjectOfType<Server>();
        navigator = FindObjectOfType<Navigator>();
        scoreboard = FindObjectOfType<Scoreboard>();
        adCancel = FindObjectOfType<AdCancel>();

        diamondLives = GameObject.Find("DiamondLives");
        videoLives = GameObject.Find("VideoLives");
        playChallenge = GameObject.Find("PlayChallenge");
        exitButton = GameObject.Find("ExitButton");

        allChallengesScrollContent = GameObject.Find("AllChallengesScrollContent");
        challengeScreenshot = GameObject.Find("ChallengeScreenshot");
        tried = GameObject.Find("TriedText");
        solved = GameObject.Find("SolvedText");
    }

    // Start is called before the first frame update
    void Start()
    {
        diamondLives.GetComponent<Button>().onClick.AddListener(() => ClickGetLivesForDiamondButton());
        videoLives.GetComponent<Button>().onClick.AddListener(() => ClickGetLifeForVideoButton());
        playChallenge.GetComponent<Button>().onClick.AddListener(() => ClickPlaySelectedChallengeButton());

        diamondLives.SetActive(false);
        videoLives.SetActive(false);
        playChallenge.SetActive(false);

        player = FindObjectOfType<Player>();
        player.LoadPlayer();
        GetCurrentBallSprite();

        scoreboard.SetDiamonds(player.diamonds);

        server.GetPastChallenges();

        adCancel.InitializeAdCancel(" life", currentBallSprite);
        adCancel.GetReceiveButton().GetComponent<Button>().onClick.AddListener(() => ReceiveButtonClick());
        adCancel.GetCancelButton().GetComponent<Button>().onClick.AddListener(() => CancelButtonClick());
    }

    public void ClickGetLifeForVideoButton()
    {
        if (videoLives.GetComponent<Button>().IsInteractable())
        {
            // Run the trigger button animation and disable button for its duration
            videoLives.GetComponent<TriggerButton>().ClickButton(0.2f);
            // Approximately when animation is over, load get more coins ad
            StartCoroutine(LoadGetLifeForVideoCoroutine(0.2f));
        }
    }

    public IEnumerator LoadGetLifeForVideoCoroutine(float time)
    {
        // Wait for given time and load the main scene
        yield return new WaitForSeconds(time);

        AdManager.ShowStandardAd(GetLifeSuccess, GetLifeCancel, GetLifeFail);
    }

    public void ReceiveButtonClick()
    {
        GameObject receiveButton = adCancel.GetReceiveButton();
        if (receiveButton.GetComponent<Button>().IsInteractable())
        {
            // Run animation of clicking receive coins and watch the ad button
            receiveButton.GetComponent<TriggerButton>().ClickButton(0.2f);
            // Approximately when animation is finished, load the ad screen
            StartCoroutine(ReceiveButtonCoroutine(0.2f));
        }
    }

    public IEnumerator ReceiveButtonCoroutine(float time)
    {
        // Wait for given time and load the ad screen
        yield return new WaitForSeconds(time);
        // Load the ad screen
        AdManager.ShowStandardAd(GetLifeSuccess, GetLifeCancel, GetLifeFail);
    }

    private void GetLifeCancel()
    {
        // Show the warning stuff if it is the first time of cancelling
        if (!showedAdCancelWarning)
        {
            adCancel.gameObject.SetActive(true);
        }
        else
        {
            adCancel.gameObject.SetActive(false);
        }

        showedAdCancelWarning = true;
    }

    private void GetLifeFail()
    {
        // Close the warning stuff if for some reason video failed
        showedAdCancelWarning = true;
        adCancel.gameObject.SetActive(false);
    }

    private void GetLifeSuccess()
    {
        // Incase this is coming from after warning stuff being showed, hide it
        showedAdCancelWarning = true;
        adCancel.gameObject.SetActive(false);
        server.GetLifeForVideoOrDiamond(selectedChallenge.id, 1);
    }

    public void CancelButtonClick()
    {
        GameObject cancelButton = adCancel.GetCancelButton();
        if (cancelButton.GetComponent<Button>().IsInteractable())
        {
            // Wait for given time and load the ad screen
            cancelButton.GetComponent<TriggerButton>().ClickButton(0.2f);
            // Approximately when animation is finished, load the ad screen
            StartCoroutine(CancelButtonCoroutine(0.2f));
        }
    }

    public IEnumerator CancelButtonCoroutine(float time)
    {
        // Wait for given time and hide the ad and warning stuff
        yield return new WaitForSeconds(time);

        adCancel.gameObject.SetActive(false);
    }

    public void GetLifeForVideoOrDiamondSuccess(string challengeId, int count)
    {
        Debug.Log("successfully updated lifes for the level: " + challengeId);
        for (int i = 0; i < pastChallenges.Count; i++)
        {
            if (pastChallenges[i].id == challengeId)
            {
                // Yuo have successfully watched an ad or paid with diamond
                pastChallenges[i].lives = count;

                if (count == 5)
                {
                    DecreasePlayerDiamonds();
                }

                // Loop through all challenge game objects and find the one that holds the same id
                foreach (Transform child in allChallengesScrollContent.transform)
                {
                    Debug.Log(child.GetComponent<ChallengeItem>().GetId());
                    if (child.GetComponent<ChallengeItem>().GetId() == challengeId)
                    {
                        Debug.Log("gameobject ids match");
                        GameObject challengeInstance = child.gameObject;
                        // Make that level change its lives from 0 to 1
                        SetChallengeIconLives(challengeInstance, pastChallenges[i].lives);
                        // If selected challenge has not been changed
                        // so player waited for the server response then show play button
                        if (selectedChallenge.id == challengeId)
                        {
                            diamondLives.SetActive(false);
                            videoLives.SetActive(false);
                            playChallenge.SetActive(true);
                        }
                        break;
                    }
                }
            }
        }
    }

    public void GetLifeForVideoOrDiamondError(string challengeId, int count)
    {
        if (count == 1)
        {
            Debug.Log("Error trying to get lives for video: " + challengeId);
        } else
        {
            Debug.Log("Error trying to get lives for diamond: " + challengeId);
        }
    }

    public void ClickExitButton()
    {
        if (exitButton.GetComponent<Button>().IsInteractable())
        {
            // Run the trigger button animation and disable button for its duration
            exitButton.GetComponent<TriggerButton>().ClickButton(0.2f);
            // Approximately when animation is over, load get more coins ad
            StartCoroutine(LoadMainSceneCoroutine(0.2f));
        }
    }

    public IEnumerator LoadMainSceneCoroutine(float time)
    {
        // Wait for given time and load the main scene
        yield return new WaitForSeconds(time);

        navigator.LoadMainScene();
    }

    public void ClickGetLivesForDiamondButton()
    {
        if (diamondLives.GetComponent<Button>().IsInteractable())
        {
            // Run the trigger button animation and disable button for its duration
            diamondLives.GetComponent<TriggerButton>().ClickButton(0.2f);
            // Approximately when animation is over, load get more coins ad
            StartCoroutine(LoadGetLivesForDiamondCoroutine(0.2f));
        }
    }

    public IEnumerator LoadGetLivesForDiamondCoroutine(float time)
    {
        // Wait for given time and load the main scene
        yield return new WaitForSeconds(time);

        GetLivesForDiamond();
    }

    public void ClickPlaySelectedChallengeButton()
    {
        if (playChallenge.GetComponent<Button>().IsInteractable())
        {
            // Run the trigger button animation and disable button for its duration
            playChallenge.GetComponent<TriggerButton>().ClickButton(0.2f);
            // Approximately when animation is over, load get more coins ad
            StartCoroutine(LoadPlaySelectedChallengeCoroutine(0.2f));
        }
    }

    public IEnumerator LoadPlaySelectedChallengeCoroutine(float time)
    {
        // Wait for given time and load the main scene
        yield return new WaitForSeconds(time);

        PlaySelectedChallenge();
    }

    private void GetCurrentBallSprite()
    {
        for (int i = 0; i < ballSprites.Length; i++)
        {
            if (ballSprites[i].name == player.currentBall)
            {
                currentBallSprite = ballSprites[i];
            }
        }
    }

    public void UnlockChallengeSuccess()
    {
        // Decrease diamonds successfully unlocked challenge
        Debug.Log("decareasing diamonds");
    }

    public void UnlockChallengeError()
    {
        Debug.Log("error");
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
        selectedChallenge = pastChallenges[level.GetComponent<ChallengeItem>().GetIndex()];

        server.UnlockChallenge(selectedChallenge.id);

        level.GetComponent<ChallengeItem>().SetLocked(false);
        level.transform.Find("LevelTop").gameObject.SetActive(true);
    }

    private void ClickChallengeLevel(GameObject level)
    {
        selectedChallenge = pastChallenges[level.GetComponent<ChallengeItem>().GetIndex()];

        // Extract and set screenshot of selected challenge
        StartCoroutine(server.ExtractPastChallengeScreenshotImage(selectedChallenge));

        // Set tried and solved and load the screenshot of selected level
        SetSelectedChallenge(selectedChallenge);

        if (selectedChallenge.lives == 0)
        {
            diamondLives.SetActive(true);
            videoLives.SetActive(true);
            playChallenge.SetActive(false);
        }
        else
        {
            diamondLives.SetActive(false);
            videoLives.SetActive(false);
            playChallenge.SetActive(true);
        }
    }

    public void SetSelectedChallenge(PastChallenge challenge)
    {
        tried.GetComponent<Text>().text = challenge.tried.ToString() + " tried";
        solved.GetComponent<Text>().text = challenge.solved.ToString() + " solved";
    }

    private void GetLivesForDiamond()
    {
        server.GetLifeForVideoOrDiamond(selectedChallenge.id, 5);
    }

    private void DecreasePlayerDiamonds()
    {
        player.diamonds--;
        scoreboard.SetDiamonds(player.diamonds);
        player.SavePlayer();
    }

    private void PlaySelectedChallenge()
    {
        // Set selected challenge level id to player prefs
        PlayerPrefs.SetString("challengeId", selectedChallenge.id);
        navigator.LoadChallengeLevel();
    }

    private void PopulateChallenges()
    {
        for (int i = 0; i < pastChallenges.Count; i++)
        {
            GameObject challengeInstance = Instantiate(challengeItem, transform.position, Quaternion.identity);

            // Set challenge index text
            challengeInstance.transform.Find("LevelTop").Find("LevelCount").GetComponent<Text>().text = (pastChallenges.Count - i).ToString();

            // Set challenge index into a challnge item script of each challenge
            challengeInstance.GetComponent<ChallengeItem>().SetIndex(i);
            challengeInstance.GetComponent<ChallengeItem>().SetId(pastChallenges[i].id);

            // If the level is not locked open the lock frame
            if (pastChallenges[i].status != "locked" || i == 0)
            {
                // This level is unlocked, hide the locked frame
                challengeInstance.transform.Find("LockedFrame").gameObject.SetActive(false);
                SetChallengeIconLives(challengeInstance, pastChallenges[i].lives);
            } else {
                // Hide the lives, icon and index box to show lock clear
                challengeInstance.transform.Find("LevelTop").gameObject.SetActive(false);
            }

            // Add function to click on a challenge to play it
            challengeInstance.transform.Find("LevelTop")
                .GetComponent<Button>().onClick.AddListener(() => ClickChallengeLevel(challengeInstance));
            // Add function to click on a challenge to unlock it
            challengeInstance.transform.Find("LockedFrame")
                .GetComponent<Button>().onClick.AddListener(() => UnlockChallengeLevel(challengeInstance));

            // Set parent of challenge to a scroll content
            challengeInstance.transform.SetParent(allChallengesScrollContent.transform);

            if (i == 0)
            {
                ClickChallengeLevel(challengeInstance);
            }
        }
    }

    private void SetChallengeIconLives(GameObject challenge, int lives)
    {
        // Color lives in the challenge icon
        Transform livesObject = challenge.transform.Find("LevelTop").Find("Lives");

        for (int i = 0; i < livesObject.childCount; i++)
        {
            if (i < lives)
            {
                livesObject.GetChild(i).GetComponent<Image>().sprite = currentBallSprite;
                livesObject.GetChild(i).GetComponent<Image>().color = new Color32(255, 255, 255, 255);
            }
        }
    }
}
