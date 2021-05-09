using System;
using UnityEngine;
using UnityEngine.UI;

public class ChallengesStatus : MonoBehaviour
{
    AdMobManager adMobManager;

    Navigator navigator;
    Player player;
    Scoreboard scoreboard;

    [SerializeField] Sprite[] ballSprites;

    Sprite currentBallSprite;

    GameObject allChallengesScrollContent;
    GameObject allChallengesScrollbar;

    GameObject diamondLives;
    GameObject videoLives;
    GameObject playChallenge;

    GameObject selectedChallenge;

    AdCancel adCancel;

    bool showedAdCancelWarning = false;

    void Awake()
    {
        adMobManager = FindObjectOfType<AdMobManager>();

        navigator = FindObjectOfType<Navigator>();
        scoreboard = FindObjectOfType<Scoreboard>();
        adCancel = FindObjectOfType<AdCancel>();

        diamondLives = GameObject.Find("DiamondLives");
        videoLives = GameObject.Find("VideoLives");
        playChallenge = GameObject.Find("PlayChallenge");

        allChallengesScrollContent = GameObject.Find("AllChallengesScrollContent");
        allChallengesScrollbar = GameObject.Find("AllChallengesScrollbar");
    }

    // Start is called before the first frame update
    void Start()
    {
        if ((float)Screen.width / Screen.height > 0.7)
        {
            GameObject canvas = GameObject.Find("Canvas");
            canvas.GetComponent<CanvasScaler>().matchWidthOrHeight = 1;
        }

        playChallenge.GetComponent<Button>().onClick.AddListener(() => ClickPlayButton());
        diamondLives.GetComponent<Button>().onClick.AddListener(() => ClickGetLivesForDiamondButton());
        videoLives.GetComponent<Button>().onClick.AddListener(() => ClickGetLifeForVideoButton());

        diamondLives.SetActive(false);
        videoLives.SetActive(false);
        playChallenge.SetActive(false);

        AdMobManager.ShowAdmobBanner();
        // AdManager.ShowBanner();

        player = FindObjectOfType<Player>();
        //player.ResetPlayer();
        player.LoadPlayer();

        GetCurrentBallSprite();
        scoreboard.SetDiamonds(player.diamonds);
        scoreboard.SetCoins(player.coins);

        adCancel.InitializeAdCancel(" life", currentBallSprite);
        adCancel.GetReceiveButton().GetComponent<Button>().onClick.AddListener(() => ReceiveButtonClick());
        adCancel.GetCancelButton().GetComponent<Button>().onClick.AddListener(() => CancelButtonClick());

        adCancel.gameObject.SetActive(false);

        allChallengesScrollbar.GetComponent<Scrollbar>().value = 0.001f;

        PopulateChallenges();

        // Save click
        DateTimeOffset now = DateTimeOffset.UtcNow;
        long date = now.ToUnixTimeMilliseconds();
        player.challengesClicks.Add(date);
        player.SavePlayer();
    }

    public void SelectChallenge(GameObject challenge)
    {
        for (int i = 0; i < allChallengesScrollContent.transform.childCount; i++)
        {
            allChallengesScrollContent.transform.GetChild(i).GetComponent<ChallengeItem>().SelectChallengeStatus(false);
        }

        selectedChallenge = challenge;

        if (selectedChallenge.GetComponent<ChallengeItem>().GetChallengeStatus() > 0 ||
            selectedChallenge.GetComponent<ChallengeItem>().GetChallengeStatus() == -2)
        {
            playChallenge.SetActive(true);
            diamondLives.SetActive(false);
            videoLives.SetActive(false);
        } else if (selectedChallenge.GetComponent<ChallengeItem>().GetChallengeStatus() == 0)
        {
            playChallenge.SetActive(false);
            diamondLives.SetActive(true);
            videoLives.SetActive(true);
        }
    }

    private void PopulateChallenges()
    {
        for (int i = 0; i < player.unlockedChallenges.Count; i++)
        {
            GameObject challenge = allChallengesScrollContent.transform.GetChild(i).gameObject;
            challenge.GetComponent<ChallengeItem>().SetData(player.unlockedChallenges[i], currentBallSprite);
        }
        SelectChallenge(allChallengesScrollContent.transform.GetChild(0).gameObject);
    }

    public void ClickGetLifeForVideoButton()
    {
        adMobManager.ShowAdmobRewardedAd(GetLifeSuccess, GetLifeCancel, GetLifeFail);
        //AdManager.ShowStandardAd(GetLifeSuccess, GetLifeCancel, GetLifeFail);
    }

    public void ClickGetLivesForDiamondButton()
    {
        if (player.diamonds > 0)
        {
            adCancel.gameObject.SetActive(false);
            int index = selectedChallenge.GetComponent<ChallengeItem>().GetChallengeIndex();

            player.diamonds--;
            player.unlockedChallenges[index - 1] = 5;
            player.SavePlayer();

            scoreboard.SetDiamonds(player.diamonds);

            playChallenge.SetActive(true);
            diamondLives.SetActive(false);
            videoLives.SetActive(false);

            selectedChallenge.GetComponent<ChallengeItem>().SetLives(5, currentBallSprite);
        }
    }

    public void ReceiveButtonClick()
    {
        adMobManager.ShowAdmobRewardedAd(GetLifeSuccess, GetLifeCancel, GetLifeFail);
        //AdManager.ShowStandardAd(GetLifeSuccess, GetLifeCancel, GetLifeFail);
    }

    public void CancelButtonClick()
    {
        adCancel.gameObject.SetActive(false);
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
        int index = selectedChallenge.GetComponent<ChallengeItem>().GetChallengeIndex();

        player.unlockedChallenges[index - 1] = 1;
        player.SavePlayer();

        playChallenge.SetActive(true);
        diamondLives.SetActive(false);
        videoLives.SetActive(false);

        selectedChallenge.GetComponent<ChallengeItem>().SetLives(1, currentBallSprite);
    }

    public void ClickExitButton()
    {
        navigator.LoadMainScene();
    }

    public void ClickPlayButton()
    {
        int index = selectedChallenge.GetComponent<ChallengeItem>().GetChallengeIndex();
        navigator.LoadChallengeLevel(index);
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
}
