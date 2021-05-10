using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HomeStatus : MonoBehaviour
{
    Player player;
    BallCatcher ballCatcher;
    // This is to indicate that the wall on this level should be shuffled
    [SerializeField] bool shuffle = true;
    // Game object that holds all the walls of the level, for easier access and looping through
    [SerializeField] Transform walls;
    // To show when unlocking a new challege every 10th level
    [SerializeField] Sprite[] challengeScreenshots;
    [SerializeField] GameObject unlockChallengeIcon;
    // All balls prefabs, to show the one that is selected by the player
    [SerializeField] GameObject[] balls;

    // Tutorial stuff
    [SerializeField] GameObject FocusPointer;
    [SerializeField] GameObject FocusPointerAfterHintHorizontal;
    [SerializeField] GameObject FocusPointerAfterHintVertical;
    [SerializeField] GameObject VerticalPointer;
    [SerializeField] GameObject AngularPointer;
    [SerializeField] GameObject HorizontalPointer;
    bool showedFocus = false;
    bool showedPointer = false;
    int tutorialStep = 0;

    // Index of a challenge level, if 0 it means it is a normal level
    [SerializeField] int challengeLevel;
    int normalLevel;

    [SerializeField] bool tutorial;
    // To pass to ad cancel for normal level
    [SerializeField] Sprite hintIcon;
    // To pass to ad cancel for challenge level
    [SerializeField] Sprite lifeIcon;

    // For hint button laoder
    [SerializeField] GameObject hintButtonEmpty;
    [SerializeField] GameObject hintButtonLoader;
    // For hint button ad cancel window
    [SerializeField] GameObject hintButtonReceive;
    [SerializeField] GameObject hintButtonReceiveEmpty;
    [SerializeField] GameObject hintButtonReceiveLoader;
    // For extra life button laoder
    [SerializeField] GameObject extraLifeButtonEmpty;
    [SerializeField] GameObject extraLifeButtonLoader;

    // This should be changed as new balls are being added
    // This is not all balls but all unlockable balls
    int totalBallsAmount = 22;

    // Background Sprites
    [SerializeField] Sprite green;
    [SerializeField] Sprite blue;
    [SerializeField] Sprite purple;
    [SerializeField] Sprite orange;
    [SerializeField] Sprite red;
    [SerializeField] Sprite defaultBg;

    // Canvas objects
    GameObject gameBackground;
    GameObject levelIndex;
    GameObject hintButton;
    GameObject homeButton;
    GameObject resetButton;
    GameObject forwardButton;
    GameObject ballDirectionArrow;
    Scoreboard scoreboard;

    Ball ball;

    Navigator navigator;

    AdCancel adCancel;

    bool showedAdCancelWarning = false;

    // Status of ball being launched or not, for other scripts to access
    bool ballLaunched;
    int keys = 0;
    int coins = 0;
    int diamonds = 0;

    // Challenge level
    int lives;
    Sprite currentBallSprite;
    GameObject livesParent;
    GameObject extraLife;
    GameObject extraLifeButton;

    GameObject passPhrase;
    // This is to not mess around with solved status changing it to its lives value
    bool solved;

    [SerializeField] int challengeCoins;
    [SerializeField] int challengeDiamonds;

    GameObject ballStuff;

    void Awake()
    {
        ballCatcher = FindObjectOfType<BallCatcher>();
        scoreboard = FindObjectOfType<Scoreboard>();
        gameBackground = GameObject.Find("GameBackground");
        homeButton = GameObject.Find("HomeButton");
        resetButton = GameObject.Find("ResetButton");
        forwardButton = GameObject.Find("ForwardButton");
        ballDirectionArrow = GameObject.Find("BallDirectionArrow");
        levelIndex = GameObject.Find("LevelIndex");

        adCancel = FindObjectOfType<AdCancel>();

        navigator = FindObjectOfType<Navigator>();
        ball = FindObjectOfType<Ball>();
        // In order to get proper locations of walls (in case hint button is clicked)
        walls = GameObject.Find("Walls").transform;

        ballStuff = GameObject.Find("BallStuff");

        // Hide the white plane that is there for helping design the level on canvas
        GameObject.Find("GamePlane").SetActive(false);

        if (challengeLevel > 0)
        {
            livesParent = GameObject.Find("Lives");
            extraLife = GameObject.Find("ExtraLife");
            extraLifeButton = GameObject.Find("ExtraLifeButton");
            passPhrase = GameObject.Find("PassPhrase");
            extraLife.SetActive(false);
            extraLife.transform.localScale = new Vector3(1, 1, 1);
        } else
        {
            hintButton = GameObject.Find("HintButton");
        }
    }

    void Start()
    {
        // This is in start so it can destroy the old item before accessing it
        player = FindObjectOfType<Player>();
        //player.ResetPlayer();
        player.LoadPlayer();
        //AdManager.ShowBanner();

        SetButtonFunctions();

        // Hide all the supposedely invisible buttons
        resetButton.SetActive(false);
        forwardButton.SetActive(false);

        ChangeCameraSettings();

        AdManager.ShowBanner();

        scoreboard.SetCoins(player.coins);
        scoreboard.SetDiamonds(player.diamonds);

        if (challengeLevel == 0)
        {
            normalLevel = Int32.Parse(SceneManager.GetActiveScene().name.Remove(0, 6));
        }

        // Set the ball based on which ball index is selected in player data
        SetBallPrefab();
        // Set the background based on the ball
        SetBackground();
        if (challengeLevel > 0)
        {
            ballCatcher.SetDiamondsCoins(5, 10);

            lives = player.unlockedChallenges[challengeLevel - 1];
            if (lives == -1)
            {
                // Level is locked
                navigator.LoadMainScene();
            }
            else if (lives == -2)
            {
                solved = true;
                lives = 5;
            }
            levelIndex.GetComponent<Text>().text = challengeLevel.ToString();
            extraLifeButton.GetComponent<Button>().onClick.AddListener(() => ExtraLifeButtonClick());
            GetCurrentBallSprite();
            SetLives();

            adCancel.GetReceiveButton().GetComponent<Button>().onClick.AddListener(() => ExtraLifeReceiveButtonClick());
            adCancel.GetCancelButton().GetComponent<Button>().onClick.AddListener(() => ExtraLifeCancelButtonClick());
            adCancel.InitializeAdCancel(" life", hintIcon);

            passPhrase.GetComponent<Button>().onClick.AddListener(() => navigator.LoadMainScene());
        } else
        {
            if (player.maxLevelReached)
            {
                levelIndex.GetComponent<Text>().text = "Max";
            } else
            {
                levelIndex.GetComponent<Text>().text = player.nextLevelIndex.ToString();
            }
            // Hide shop and hint in tutorial levels
            if (player.nextLevelIndex <= 3 || (player.nextLevelIndex == 151 && !player.maxLevelReached))
            {
                hintButton.SetActive(false);
            }

            // Set initial keys coins diamonds and canvas values
            keys = player.keys;
            scoreboard.SetKeys(keys);

            adCancel.GetReceiveButton().GetComponent<Button>().onClick.AddListener(() => UseHintReceiveButtonClick());
            adCancel.GetCancelButton().GetComponent<Button>().onClick.AddListener(() => UseHintCancelButtonClick());
            adCancel.InitializeAdCancel(" hint", hintIcon);
        }
        adCancel.gameObject.SetActive(false);
    }

    public void DisableHintButtonLoadingAd()
    {
        hintButton.GetComponent<Button>().interactable = false;
        hintButtonLoader.SetActive(true);
        hintButtonEmpty.SetActive(true);

        hintButtonReceive.GetComponent<Button>().interactable = false;
        hintButtonReceiveEmpty.SetActive(true);
        hintButtonReceiveLoader.SetActive(true);
    }

    public void EnableHintButtonLoadingAd()
    {
        hintButton.GetComponent<Button>().interactable = true;
        hintButtonLoader.SetActive(false);
        hintButtonEmpty.SetActive(false);

        hintButtonReceive.GetComponent<Button>().interactable = true;
        hintButtonReceiveEmpty.SetActive(false);
        hintButtonReceiveLoader.SetActive(false);
    }

    public void DisableExtraLifeButtonLoadingAd()
    {
        extraLifeButton.GetComponent<Button>().interactable = false;
        extraLifeButtonEmpty.SetActive(true);
        extraLifeButtonLoader.SetActive(true);
    }

    public void EnableExtraLifeButtonLoadingAd()
    {
        extraLife.SetActive(false);

        extraLifeButton.GetComponent<Button>().interactable = true;
        extraLifeButtonEmpty.SetActive(false);
        extraLifeButtonLoader.SetActive(false);
    }

    public void ClickPlayRandomLevels()
    {
        player.maxLevelReached = true;
        player.SavePlayer();
        navigator.LoadNextLevel(player.nextLevelIndex, player.maxLevelReached);
    }

    private void ChangeCameraSettings() {
        Canvas canvas = FindObjectOfType<Canvas>();
        // Change the camera zoom based on the screen ration, for very tall or very wide screens
        if ((float)Screen.height / Screen.width > 2)
        {
            Camera.main.orthographicSize = 800;
            if (canvas != null)
            {
                canvas.GetComponent<CanvasScaler>().matchWidthOrHeight = 0;
            }
        } else {
            Camera.main.orthographicSize = 667;
            if (canvas != null)
            {
                canvas.GetComponent<CanvasScaler>().matchWidthOrHeight = 1;
            }
        }

        //if ((float)Screen.width / Screen.height > 0.7)
        //{
        //    canvas.GetComponent<CanvasScaler>().matchWidthOrHeight = 0;
        //}
        //else
        //{
        //    canvas.GetComponent<CanvasScaler>().matchWidthOrHeight = 1;
        //}
    }

    // @access from ball catcher when deciding on showing unlock challenge view
    public void SetUnlockChallengeIcon(int index)
    {
        unlockChallengeIcon.GetComponent<Image>().sprite = challengeScreenshots[index];
    }

    // To access from ball catcher to know whether to drop diamonds or not
    public int GetChallengeLevel()
    {
        return challengeLevel;
    }

    public void SetLifeIcon(Sprite icon)
    {
        lifeIcon = icon;
        if (challengeLevel > 0)
        {
            adCancel.InitializeAdCancel(" life", lifeIcon);
        }
    }

    private void SetButtonFunctions()
    {
        if (challengeLevel == 0 && hintButton != null)
        {
            hintButton.GetComponent<Button>().onClick.AddListener(() => ClickHintButton());
        }
        homeButton.GetComponent<Button>().onClick.AddListener(() => ClickHomeButton());
        resetButton.GetComponent<Button>().onClick.AddListener(() => ClickResetButton());
        forwardButton.GetComponent<Button>().onClick.AddListener(() => ClickForwardButton());
    }

    public bool GetShuffle()
    {
        return shuffle;
    }

    public void ExtraLifeReceiveButtonClick()
    {
        DisableExtraLifeButtonLoadingAd();
        AdManager.ShowStandardAd(ExtraLifeSuccess, RewardAdCancel, RewardAdFail);
    }

    public void ExtraLifeCancelButtonClick()
    {
        navigator.LoadMainScene();
    }

    public void ExtraLifeButtonClick()
    {
        DisableExtraLifeButtonLoadingAd();
        ballStuff.SetActive(true);
        AdManager.ShowStandardAd(ExtraLifeSuccess, RewardAdCancel, RewardAdFail);
    }

    private void ExtraLifeSuccess()
    {
        EnableExtraLifeButtonLoadingAd();

        lives++;

        if (!solved)
        {
            player.unlockedChallenges[challengeLevel - 1] = lives;
            player.SavePlayer();
        }

        SetLives();
        // Incase this is coming from after warning stuff being showed, hide it
        showedAdCancelWarning = true;
        adCancel.gameObject.SetActive(false);
    }

    public void UseHintReceiveButtonClick()
    {
        DisableHintButtonLoadingAd();
        AdManager.ShowStandardAd(UseHintSuccess, RewardAdCancel, RewardAdFail);
    }

    public void UseHintCancelButtonClick()
    {
        adCancel.gameObject.SetActive(false);
    }

    public void ClickHintButton()
    {
        // If the level is tutorial for how to use hint button
        if (player.nextLevelIndex == 4)
        {
            // Run hint results as if the video was watched
            UseHintSuccess();
            // Run the tutorial stuff
            showedPointer = true;
            showedFocus = true;
            CheckPointerFocus();
            ShowFocusPinterAfterHintHorizontal();
        }
        else
        {
            DisableHintButtonLoadingAd();
            AdManager.ShowStandardAd(UseHintSuccess, RewardAdCancel, RewardAdFail);
        }
    }

    private void RewardAdCancel()
    {
        if (hintButton != null)
        {
            EnableHintButtonLoadingAd();
        } else
        {
            EnableExtraLifeButtonLoadingAd();
        }
        // Show the warning stuff if it is the first time of cancelling
        if (!showedAdCancelWarning)
        {
            adCancel.gameObject.SetActive(true);
        } else
        {
            if (challengeLevel > 0)
            {
                navigator.LoadMainScene();
            } else
            {
                adCancel.gameObject.SetActive(false);
            }
        }

        showedAdCancelWarning = true;
    }

    private void RewardAdFail()
    {
        if (hintButton != null)
        {
            EnableHintButtonLoadingAd();
        }
        else
        {
            EnableExtraLifeButtonLoadingAd();
        }
        // Close the warning stuff if for some reason video failed
        showedAdCancelWarning = true;
        adCancel.gameObject.SetActive(false);
    }

    public void UseHintSuccess()
    {
        if (challengeLevel == 0)
        {
            // Save click
            player.hintClicks.Add(normalLevel);
            player.SavePlayer();
        }

        EnableHintButtonLoadingAd();
        // Hide hint button and show every wall's correct position
        hintButton.SetActive(false);
        foreach (Transform child in walls)
        {
            child.GetComponent<Wall>().ShowCorrectPosition();
        }

        // Incase this is coming from after warning stuff being showed, hide it
        showedAdCancelWarning = true;
        adCancel.gameObject.SetActive(false);
    }

    private void SetBallPrefab()
    {
        for (int i = 0; i < balls.Length; i++)
        {
            // Get sprite name from sprite renderer because it is in the game
            if (balls[i].GetComponent<SpriteRenderer>().sprite.name == player.currentBall)
            {
                // Instantiate a ball from all balls array and player data ball index, set its parent to ballStuff
                GameObject ballPrefab = Instantiate(balls[i], ball.transform.position, ball.transform.rotation);

                ballPrefab.transform.SetParent(ball.gameObject.transform);
                break;
            }
        }
    }

    public void LaunchBall()
    {
        // Set the ball launched status to be access by other scripts
        ballLaunched = true;

        // If this is the tutorial on level hide the tutorial stuff
        if (player.nextLevelIndex == 1 && tutorial)
        {
            FocusPointer.SetActive(false);
        }
        // If this is not the last level, show the reset button
        if (player.nextLevelIndex != 150)
        {
            resetButton.SetActive(true);
        }
        forwardButton.SetActive(true);

        // Hide hint and shop button when the ball is in movement, those should be access when ball is idle
        // In not challenge level
        if (challengeLevel == 0)
        {
            hintButton.SetActive(false);
        }
        homeButton.SetActive(false);
        // Hide arrow that shows where the ball will move when launched
        ballDirectionArrow.SetActive(false);

        

        forwardButton.transform.position = Camera.main.WorldToScreenPoint(ball.gameObject.transform.position); ;
        forwardButton.transform.localScale = new Vector3(1.4f, 1.4f, 1.4f);
        forwardButton.transform.Find("Forward").localScale = new Vector3(0.6f, 0.6f, 0.6f);
        forwardButton.transform.Find("Forward").GetComponent<Image>().color = new Color32(255, 255, 255, 100);
    }

    public void CatchBall()
    {
        // Change the ball launched status when the ball has been cought by the wall
        ballLaunched = false;
    }

    public bool GetBallLaunched()
    {
        return ballLaunched;
    }

    public void CollectKey()
    {
        // If a ball collides with a key add it to the level keys and to the scoreboard in canvas
        // If the keys are less than 3, as we dont have 4th key and they get reset after every 3 keys
        // And every level may have at most 1 key
        if (keys < 3)
        {
            keys++;
            scoreboard.SetKeys(keys);
        }
    }

    public void CollectCoin()
    {
        // If a ball collides with a coin add it to the level keys and to the scoreboard in canvas
        coins++;
        scoreboard.SetCoins(player.coins + coins);
    }

    public void CollectDiamond()
    {
        // If a ball collides with a coin add it to the level keys and to the scoreboard in canvas
        diamonds++;
        scoreboard.SetDiamonds(player.diamonds + diamonds);
    }

    public int GetCoins()
    {
        if (challengeLevel > 0)
        {
            return challengeCoins;
        }
        return coins;
    }

    public int GetDiamonds()
    {
        if (challengeLevel > 0)
        {
            return challengeDiamonds;
        }
        return diamonds;
    }

    public int GetKeys()
    {
        return keys;
    }

    // Not to give coins and diamonds if you solved the level twice
    public bool GetChallengeSolved()
    {
        return solved;
    }

    public void ClickResetButton()
    {
        // If the ball has not been reset yet, reset it. This is to remove double clicks
        if (!ball.GetBallReset())
        {
            ball.ResetBall();
        }
        // Hide the forward and reset buttons
        resetButton.SetActive(false);
        forwardButton.SetActive(false);
    }

    public void ClickForwardButton()
    {
        ball.ForwardBall();
        // Switch disable button
        forwardButton.GetComponent<Button>().interactable = false;
        forwardButton.transform.Find("Forward").Find("Disabled").gameObject.SetActive(true);
    }

    private void SetLives()
    {
        for (int i = 0; i < livesParent.transform.childCount; i++)
        {
            livesParent.transform.GetChild(i).GetComponent<Image>().sprite = currentBallSprite;
            if (i < lives)
            {
                livesParent.transform.GetChild(i).GetComponent<Image>().color = new Color32(255, 255, 255, 255);
            }
            else
            {
                livesParent.transform.GetChild(i).GetComponent<Image>().color = new Color32(255, 255, 255, 25);
            }
        }
    }

    public void ResetLevel()
    {
        if (challengeLevel > 0)
        {
            lives--;

            if (lives > -1)
            {
                if (!solved)
                {
                    player.unlockedChallenges[challengeLevel - 1] = lives;
                    player.SavePlayer();
                }
                SetLives();
            }

            if (lives == 0)
            {
                extraLife.SetActive(true);
                ballStuff.SetActive(false);
            }
            homeButton.SetActive(true);
        }
        else
        {
            // If it is not a tutorial level, show the shop and hint buttons
            if (player.nextLevelIndex > 4)
            {
                hintButton.SetActive(true);
            }
            homeButton.SetActive(true);
        }
        // Seet ball to idle state
        ballLaunched = false;
        // Show the ball direction arrow again
        ballDirectionArrow.SetActive(true);

        // This is accessed by a ball when it is reset
        resetButton.SetActive(false);
        forwardButton.SetActive(false);

        forwardButton.GetComponent<Button>().interactable = true;
        forwardButton.transform.Find("Forward").Find("Disabled").gameObject.SetActive(false);
    }

    public void ClickHomeButton()
    {
        navigator.LoadMainScene();
    }
    
    // Tutorial stuff
    public void CheckPointerMove()
    {
        if (tutorial)
        {
            if (player.nextLevelIndex == 1)
            {
                if (HorizontalPointer.activeSelf)
                {
                    HorizontalPointer.SetActive(false);
                }
                else
                {
                    CheckPointerFocus();
                }
            }
            else if (player.nextLevelIndex == 2)
            {
                if (VerticalPointer.activeSelf)
                {
                    VerticalPointer.SetActive(false);
                }
                else
                {
                    CheckPointerFocus();
                }
            }
            else if (player.nextLevelIndex == 3)
            {
                if (AngularPointer.activeSelf)
                {
                    AngularPointer.SetActive(false);
                }
                else
                {
                    CheckPointerFocus();
                }
            }
            showedPointer = true;
        }
    }
    // Tutorial stuff
    public void CheckPointerFocus()
    {
        if (tutorial)
        {
            if ((player.nextLevelIndex == 1 || player.nextLevelIndex == 4) && showedPointer)
            {
                if (FocusPointer.activeSelf || showedFocus)
                {
                    FocusPointer.SetActive(false);
                }
                else
                {
                    FocusPointer.SetActive(true);
                    showedFocus = true;
                }
            }
        }
    }
    // Tutorial stuff
    public bool TutorialPassed()
    {
        if (tutorial)
        {
            if (player.nextLevelIndex == 1 && HorizontalPointer.activeSelf)
            {
                return false;
            }
            else if (player.nextLevelIndex == 2 && VerticalPointer.activeSelf)
            {
                return false;
            }
            else if (player.nextLevelIndex == 3 && AngularPointer.activeSelf)
            {
                return false;
            }
            else if (player.nextLevelIndex == 4 && FocusPointer.activeSelf)
            {
                return false;
            }
        }
        return true;
    }
    // Tutorial stuff
    public void ShowFocusPinterAfterHintHorizontal()
    {
        if (tutorial)
        {
            FocusPointerAfterHintHorizontal.SetActive(true);
            tutorialStep++;
        }
    }
    // Tutorial stuff
    public void HideFocusPinterAfterHintHorizontal()
    {
        if (tutorial)
        {
            if (player.nextLevelIndex == 4)
            {
                if (tutorialStep == 1)
                {
                    FocusPointerAfterHintHorizontal.SetActive(false);
                    FocusPointerAfterHintVertical.SetActive(true);
                    tutorialStep++;
                }
            }
        }
    }
    // Tutorial stuff
    public void HideFocusPinterAfterHintVertical()
    {
        if (tutorial)
        {
            if (player.nextLevelIndex == 4)
            {
                FocusPointerAfterHintVertical.SetActive(false);
            }
        }
    }

    public int GetNextLevel()
    {
        return player.nextLevelIndex;
    }

    public bool AllBallsUnlocked()
    {
        if (!player)
        {
            player = FindObjectOfType<Player>();
            player.LoadPlayer();
        }
        // This is to decide whether the key should be on the level or not
        if (player.unlockedBalls.Count >= totalBallsAmount)
        {
            return true;
        }

        return false;
    }

    // Set the game background based on the ball
    private void SetBallBackground(string ballBackground)
    {
        gameBackground.SetActive(true);
        if (ballBackground == "DEFAULT")
        {
            gameBackground.GetComponent<SpriteRenderer>().sprite = defaultBg;
        }
        else if (ballBackground == "GREEN")
        {
            gameBackground.GetComponent<SpriteRenderer>().sprite = green;
        }
        else if (ballBackground == "BLUE")
        {
            gameBackground.GetComponent<SpriteRenderer>().sprite = blue;
        }
        else if (ballBackground == "PURPLE")
        {
            gameBackground.GetComponent<SpriteRenderer>().sprite = purple;
        }
        else if (ballBackground == "ORANGE")
        {
            gameBackground.GetComponent<SpriteRenderer>().sprite = orange;
        }
        else if (ballBackground == "RED")
        {
            gameBackground.GetComponent<SpriteRenderer>().sprite = red;
        }
    }

    // Disable all buttons when level is passed
    public void DisableAllButtons()
    {
        resetButton.GetComponent<Button>().interactable = false;
        homeButton.GetComponent<Button>().interactable = false;
        forwardButton.GetComponent<Button>().interactable = false;
        if (challengeLevel == 0)
        {
            hintButton.GetComponent<Button>().interactable = false;
        }
    }

    private void GetCurrentBallSprite()
    {
        for (int i = 0; i < balls.Length; i++)
        {
            if (balls[i].name == player.currentBall)
            {
                currentBallSprite = balls[i].GetComponent<SpriteRenderer>().sprite;
            }
        }
    }

    private void SetBackground()
    {
        switch (player.currentBall)
        {
            case "bowling":
            case "default":
            case "pool":
            case "tennis":
            case "volleyball":
            case "football":
            case "basketball":
                SetBallBackground("GREEN");
                break;
            case "darts":
            case "yinyang":
            case "beachball":
            case "watermelon":
            case "pumpkin":
            case "button":
            case "smile":
            case "gear":
            case "pokemon":
                SetBallBackground("BLUE");
                break;
            case "snowball":
            case "cookie":
            case "flower":
            case "burger":
            case "donut":
            case "saturn":
            case "discoball":
            case "virus":
            case "bomb":
            case "coin":
                SetBallBackground("PURPLE");
                break;
            case "atom":
            case "meteor":
            case "blackhole":
            case "candy":
            case "sun":
            case "hypnose":
            case "wheel":
            case "eyeball":
            case "radiation":
                SetBallBackground("ORANGE");
                break;
            case "abbox":
                SetBallBackground("RED");
                break;
            default: 
                SetBallBackground("DEFAULT");
                break;
        }
    }
}
