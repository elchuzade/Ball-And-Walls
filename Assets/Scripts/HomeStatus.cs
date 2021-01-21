using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HomeStatus : MonoBehaviour
{
    [SerializeField] Player player;
    // This is to indicate that the wall on this level should be shuffled
    [SerializeField] bool shuffle = true;

    // Game object that holds all the walls of the level, for easier access and looping through
    [SerializeField] Transform walls;

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

    // Background Sprites
    [SerializeField] Sprite table;
    [SerializeField] Sprite space;
    [SerializeField] Sprite stadium;
    [SerializeField] Sprite words;

    // Canvas objects
    GameObject gameBackground;
    GameObject hintButton;
    GameObject homeButton;
    GameObject resetButton;
    GameObject forwardButton;
    GameObject ballDirectionArrow;
    Scoreboard scoreboard;

    Ball ball;

    Navigator navigator;

    // Ad cancel warning stuff
    GameObject adCancelBg;
    GameObject adCancelWarning;
    GameObject adWarningReceiveButton;
    GameObject adWarningContinueButton;

    bool showedAdCancelWarning = false;

    // Status of ball being launched or not, for other scripts to access
    bool ballLaunched;
    int keys = 0;
    int coins = 0;
    int diamonds = 0;

    void Awake()
    {
        scoreboard = FindObjectOfType<Scoreboard>();
        gameBackground = GameObject.Find("GameBackground");
        hintButton = GameObject.Find("HintButton");
        homeButton = GameObject.Find("HomeButton");
        resetButton = GameObject.Find("ResetButton");
        forwardButton = GameObject.Find("ForwardButton");
        ballDirectionArrow = GameObject.Find("BallDirectionArrow");

        adCancelBg = GameObject.Find("AdCancelBg");
        adCancelWarning = GameObject.Find("AdCancelWarning");
        adWarningReceiveButton = GameObject.Find("AdWarningReceiveButton");
        adWarningContinueButton = GameObject.Find("AdWarningContinueButton");

        // Set ad stuff back to normal as they are shrinked in x axis for visibility by default
        adCancelBg.transform.localScale = new Vector3(1, 1, 1);
        adCancelWarning.transform.localScale = new Vector3(1, 1, 1);

        navigator = FindObjectOfType<Navigator>();
        ball = FindObjectOfType<Ball>();
    }

    void Start()
    {
        forwardButton.GetComponent<TriggerButton>().SetButtonState(ButtonStates.Enable);
        SetButtonFunctions();

        AdManager.ShowBanner();
        // Hide the white plane that is there for helping design the level on canvas
        GameObject.Find("GamePlane").SetActive(false);
        // Hide all the supposedely invisible buttons
        resetButton.SetActive(false);
        forwardButton.SetActive(false);
        adCancelBg.SetActive(false);
        adCancelWarning.SetActive(false);

        // Change the camera zoom based on the screen ration, for very tall or very wide screens
        if ((float)Screen.height / Screen.width > 2)
        {
            Camera.main.orthographicSize = 800;
        } else {
            Camera.main.orthographicSize = 667;
        }

        player.LoadPlayer();

        // Set initial keys coins diamonds and canvas values
        keys = player.keys;
        scoreboard.SetCoins(player.coins + coins);
        scoreboard.SetDiamonds(player.diamonds + diamonds);
        scoreboard.SetKeys(keys);

        // Hide shop and hint in tutorial levels
        if (player.nextLevelIndex <= 3 || player.nextLevelIndex == 100)
        {
            hintButton.SetActive(false);
        }
        // Set the ball based on which ball index is selected in player data
        SetBallPrefab();
        // Set the background based on the ball
        SetBackground();
    }

    private void SetButtonFunctions()
    {
        homeButton.GetComponent<Button>().onClick.AddListener(() => ClickHomeButton());
        hintButton.GetComponent<Button>().onClick.AddListener(() => ClickHintButton());
        resetButton.GetComponent<Button>().onClick.AddListener(() => ClickResetButton());
        forwardButton.GetComponent<Button>().onClick.AddListener(() => ClickForwardButton());

        adWarningReceiveButton.GetComponent<Button>().onClick.AddListener(() => ReceiveHintButtonClick());
        adWarningContinueButton.GetComponent<Button>().onClick.AddListener(() => ContinuePlayingButtonClick());
    }

    public bool GetShuffle()
    {
        // Return if the walls on the level should be shuffled, accessed by each wall
        if (player.nextLevelIndex > 3)
        {
            return true;
        }
        return shuffle;
    }

    public void ReceiveHintButtonClick()
    {
        if (adWarningReceiveButton.GetComponent<Button>().IsInteractable())
        {
            // Run animation of clicking receive coins and watch the ad button
            adWarningReceiveButton.GetComponent<TriggerButton>().ClickButton(0.2f);
            // Approximately when animation is finished, load the ad screen
            StartCoroutine(ReceiveHintButtonCoroutine(0.2f));
        }
    }

    public IEnumerator ReceiveHintButtonCoroutine(float time)
    {
        // Wait for given time and load the ad screen
        yield return new WaitForSeconds(time);
        // Load the ad screen
        AdManager.ShowStandardAd(UseHintSuccess, UseHintCancel, UseHintFail);
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
        // Wait for given time and hide the ad and warning stuff
        yield return new WaitForSeconds(time);

        adCancelBg.SetActive(false);
        adCancelWarning.SetActive(false);
    }

    private void SetBallPrefab()
    {
        // Instantiate a ball from all balls array and player data ball index, set its parent to ballStuff
        GameObject ballPrefab = Instantiate(balls[player.currentBallIndex], ball.transform.position, ball.transform.rotation);

        ballPrefab.transform.SetParent(ball.gameObject.transform);
    }

    public void ClickHintButton()
    {
        if (hintButton.GetComponent<Button>().IsInteractable())
        {
            // Run button click animation and show the hint ad window
            hintButton.GetComponent<TriggerButton>().ClickButton(0.2f);
            StartCoroutine(HintButtonCoroutine(0.2f));
        }
    }

    public IEnumerator HintButtonCoroutine(float time)
    {
        // Wait for given time for animation to finish
        yield return new WaitForSeconds(time);

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
            // Run the ad for hint
            AdManager.ShowStandardAd(UseHintSuccess, UseHintCancel, UseHintFail);
        }
    }


    private void UseHintCancel()
    {
        // Show the warning stuff if it is the first time of cancelling
        if (!showedAdCancelWarning)
        {
            adCancelBg.SetActive(true);
            adCancelWarning.SetActive(true);
        } else
        {
            adCancelBg.SetActive(false);
            adCancelWarning.SetActive(false);
        }

        showedAdCancelWarning = true;
    }

    private void UseHintFail()
    {
        // Close the warning stuff if for some reason video failed
        showedAdCancelWarning = true;
        adCancelBg.SetActive(false);
        adCancelWarning.SetActive(false);
    }

    public void UseHintSuccess()
    {
        // Hide hint button and show every wall's correct position
        hintButton.SetActive(false);
        foreach (Transform child in walls)
        {
            child.GetComponent<Wall>().ShowCorrectPosition();
        }

        // Incase this is coming from after warning stuff being showed, hide it
        showedAdCancelWarning = true;
        adCancelBg.SetActive(false);
        adCancelWarning.SetActive(false);
    }

    public void LaunchBall()
    {
        // Set the ball launched status to be access by other scripts
        ballLaunched = true;
        // If this is the tutorial on level hide the tutorial stuff
        if (player.nextLevelIndex == 1)
        {
            FocusPointer.SetActive(false);
        }
        // If this is not the last level, show the reset button
        if (player.nextLevelIndex != 100)
        {
            resetButton.SetActive(true);
        }
        forwardButton.SetActive(true);

        // hide hint and shop button when the ball is in movement, those should be access when ball is idle
        hintButton.SetActive(false);
        homeButton.SetActive(false);
        // Hide arrow that shows where the ball will move when launched
        ballDirectionArrow.SetActive(false);
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

    public void CollectDiamonds()
    {
        // If a ball collides with a coin add it to the level keys and to the scoreboard in canvas
        diamonds++;
        scoreboard.SetCoins(player.diamonds + diamonds);
    }

    public int GetCoins()
    {
        return coins;
    }

    public int GetDiamonds()
    {
        return diamonds;
    }

    public int GetKeys()
    {
        return keys;
    }

    public void ClickResetButton()
    {
        if (resetButton.GetComponent<Button>().IsInteractable())
        {
            // Run the button click animation and reset the level
            resetButton.GetComponent<TriggerButton>().ClickButton(0.2f);
            StartCoroutine(ResetButtonCoroutine(0.2f));
        }
    }

    public IEnumerator ResetButtonCoroutine(float time)
    {
        // If the ball has not been reset yet, reset it. This is to remove double clicks
        if (!ball.GetBallReset())
        {
            ball.ResetBall();
        }

        yield return new WaitForSeconds(time);

        // Hide the forward and reset buttons
        resetButton.SetActive(false);
        forwardButton.SetActive(false);
    }

    public void ClickForwardButton()
    {
        if (forwardButton.GetComponent<Button>().IsInteractable())
        {
            // Run the button click animation and accelerate the ball
            forwardButton.GetComponent<TriggerButton>().ClickButton(0.2f);
            // Show disable cross on the button when clicked
            forwardButton.GetComponent<TriggerButton>().SetButtonState(ButtonStates.Disable);
            StartCoroutine(ForwardButtonCoroutine(0.2f));
        }
    }

    public IEnumerator ForwardButtonCoroutine(float time)
    {
        // WAit for given time and run the ball's forward function that will accelerate the ball speed
        yield return new WaitForSeconds(time);
        // Make button not clickable any more until level reset or passed
        forwardButton.GetComponent<Button>().interactable = false;
        ball.ForwardBall();
    }

    public void ResetLevel()
    {
        // This is accessed by a ball when it is reset
        resetButton.SetActive(false);
        forwardButton.SetActive(false);

        forwardButton.GetComponent<TriggerButton>().SetButtonState(ButtonStates.Enable);
        forwardButton.GetComponent<Button>().interactable = true;

        // Seet ball to idle state
        ballLaunched = false;
        // Show the ball direction arrow again
        ballDirectionArrow.SetActive(true);
        // If it is not a tutorial level, show the shop and hint buttons
        if (player.nextLevelIndex > 4)
        {
            homeButton.SetActive(true);
            hintButton.SetActive(true);
        }
    }

    public void ClickHomeButton()
    {
        if (homeButton.GetComponent<Button>().IsInteractable())
        {
            // Run the button click animation and load the home scene
            homeButton.GetComponent<TriggerButton>().ClickButton(0.2f);
            StartCoroutine(LoadHomeSceneCoroutine(0.2f));
        }
    }

    public IEnumerator LoadHomeSceneCoroutine(float time)
    {
        // Wait for given time for animation to finish and load the shop scene
        yield return new WaitForSeconds(time);
        navigator.LoadMainScene();
    }
    
    // Tutorial stuff
    public void CheckPointerMove()
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
    // Tutorial stuff
    public void CheckPointerFocus()
    {
        if ((player.nextLevelIndex == 1 || player.nextLevelIndex == 4) && showedPointer)
        {
            if (FocusPointer.activeSelf || showedFocus)
            {
                FocusPointer.SetActive(false);
            } else
            {
                FocusPointer.SetActive(true);
                showedFocus = true;
            }
        }
    }
    // Tutorial stuff
    public bool TutorialPassed()
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
        return true;
    }

    // Tutorial stuff
    public void ShowFocusPinterAfterHintHorizontal()
    {
        FocusPointerAfterHintHorizontal.SetActive(true);
        tutorialStep++;
    }
    // Tutorial stuff
    public void HideFocusPinterAfterHintHorizontal()
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
    // Tutorial stuff
    public void HideFocusPinterAfterHintVertical()
    {
        if (player.nextLevelIndex == 4)
        {
            FocusPointerAfterHintVertical.SetActive(false);
        }
    }

    public int GetNextLevel()
    {
        return player.nextLevelIndex;
    }

    public bool AllBallsUnlocked()
    {
        // Loop through all the balls and see if there is any ball to be unlocked
        // This is to decide whether the key should be on the level or not
        for (int i = 0; i < player.unlockedBalls.Length; i++)
        {
            if (player.unlockedBalls[i] == 0)
            {
                return false;
            }
        }

        return true;
    }

    // Set the game background based on the ball
    private void SetBallBackground(string ballBackground)
    {
        gameBackground.SetActive(true);
        if (ballBackground == "DEFAULT")
        {
            gameBackground.GetComponent<SpriteRenderer>().sprite = words;
        }
        else if (ballBackground == "STADIUM")
        {
            gameBackground.GetComponent<SpriteRenderer>().sprite = stadium;
        }
        else if (ballBackground == "SPACE")
        {
            gameBackground.GetComponent<SpriteRenderer>().sprite = space;
        }
        else if (ballBackground == "TABLE")
        {
            gameBackground.GetComponent<SpriteRenderer>().sprite = table;
        }
    }

    // Disable all buttons when level is passed
    public void DisableAllButtons()
    {
        resetButton.GetComponent<Button>().interactable = false;
        homeButton.GetComponent<Button>().interactable = false;
        forwardButton.GetComponent<Button>().interactable = false;
        hintButton.GetComponent<Button>().interactable = false;
    }

    private void SetBackground()
    {
        switch (player.currentBallIndex)
        {
            case 0:
                // Default - DEFAULT
                SetBallBackground("DEFAULT");
                break;
            case 1:
                // Bowling - STADIUM
                SetBallBackground("STADIUM");
                break;
            case 2:
                // Beachball - STADIUM
                SetBallBackground("STADIUM");
                break;
            case 3:
                // Poolball - STADIUM
                SetBallBackground("STADIUM");
                break;
            case 4:
                // Tennis - STADIUM
                SetBallBackground("STADIUM");
                break;
            case 5:
                // Volleyball - STADIUM
                SetBallBackground("STADIUM");
                break;
            case 6:
                // Footbal - STADIUM
                SetBallBackground("STADIUM");
                break;
            case 7:
                // Basketball - STADIUM
                SetBallBackground("STADIUM");
                break;
            case 8:
                // Watermelon - TABLE
                SetBallBackground("TABLE");
                break;
            case 9:
                // Smile - DEFAULT
                SetBallBackground("DEFAULT");
                break;
            case 10:
                // Pokemon - DEFAULT
                SetBallBackground("DEFAULT");
                break;
            case 11:
                // Donut - TABLE
                SetBallBackground("TABLE");
                break;
            case 12:
                // Darts - DEFAULT
                SetBallBackground("DEFAULT");
                break;
            case 13:
                // Cookie - TABLE
                SetBallBackground("TABLE");
                break;
            case 14:
                // Meteor - SPACE
                SetBallBackground("SPACE");
                break;
            case 15:
                // Flower - DEFAULT
                SetBallBackground("DEFAULT");
                break;
            case 16:
                // Pumpkin - TABLE
                SetBallBackground("TABLE");
                break;
            case 17:
                // Virus - DEFAULT
                SetBallBackground("DEFAULT");
                break;
            case 18:
                // Candy - TABLE
                SetBallBackground("TABLE");
                break;
            case 19:
                // Hypnose - DEFAULT
                SetBallBackground("DEFAULT");
                break;
            case 20:
                // Saturn - SPACE
                SetBallBackground("SPACE");
                break;
            case 21:
                // Wheel - DEFAULT
                SetBallBackground("DEFAULT");
                break;
            case 22:
                // Coin - DEFAULT
                SetBallBackground("DEFAULT");
                break;
            case 23:
                // Bomb - DEFAULT
                SetBallBackground("DEFAULT");
                break;
            case 24:
                // Sun - SPACE
                SetBallBackground("SPACE");
                break;
            case 25:
                // Blackhole - SPACE
                SetBallBackground("SPACE");
                break;
            case 26:
                // Atom - DEFAULT
                SetBallBackground("DEFAULT");
                break;
        }
    }
}
