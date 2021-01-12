using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HomeStatus : MonoBehaviour
{
    [SerializeField] Player player;
    [SerializeField] bool shuffle = true;

    [SerializeField] Transform walls; // to loop through walls
    [SerializeField] Sprite[] backgroundSprites;

    [SerializeField] GameObject[] balls;

    [SerializeField] GameObject FocusPointer;
    [SerializeField] GameObject FocusPointerAfterHintHorizontal;
    [SerializeField] GameObject FocusPointerAfterHintVertical;
    [SerializeField] GameObject VerticalPointer;
    [SerializeField] GameObject AngularPointer;
    [SerializeField] GameObject HorizontalPointer;

    [SerializeField] Sprite table;
    [SerializeField] Sprite space;
    [SerializeField] Sprite stadium;
    [SerializeField] Sprite words;

    GameObject gameBackground;
    GameObject hintButton;
    GameObject shopButton;
    GameObject resetButton;
    GameObject forwardButton;
    GameObject ballDirectionArrow;
    Scoreboard scoreboard;

    TriggerAnimation hintButtonScript;
    TriggerAnimation shopButtonScript;
    TriggerAnimation resetButtonScript;
    TriggerAnimation forwardButtonScript;

    Ball ball;

    Navigator navigator;

    GameObject adCancelBg;
    GameObject adCancelWarning;

    GameObject adWarningReceiveButton;
    GameObject adWarningContinueButton;

    TriggerAnimation adWarningReceiveButtonScript;
    TriggerAnimation adWarningContinueButtonScript;

    bool ballLaunched;
    int keys = 0;
    int coins = 0;

    bool showedFocus = false;
    bool showedPointer = false;
    bool showedAdCancelWarning = false;

    int tutorialStep = 0;

    private void Awake()
    {
        scoreboard = FindObjectOfType<Scoreboard>();
        gameBackground = GameObject.Find("GameBackground");
        hintButton = GameObject.Find("HintButton");
        shopButton = GameObject.Find("ShopButton");
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

        GameObject.Find("GamePlane").SetActive(false);

        navigator = FindObjectOfType<Navigator>();

        hintButtonScript = hintButton.GetComponent<TriggerAnimation>();
        shopButtonScript = shopButton.GetComponent<TriggerAnimation>();
        resetButtonScript = resetButton.GetComponent<TriggerAnimation>();
        forwardButtonScript = forwardButton.GetComponent<TriggerAnimation>();

        adWarningReceiveButtonScript = adWarningReceiveButton.GetComponent<TriggerAnimation>();
        adWarningContinueButtonScript = adWarningContinueButton.GetComponent<TriggerAnimation>();

        resetButton.SetActive(false);
        forwardButton.SetActive(false);
        adCancelBg.SetActive(false);
        adCancelWarning.SetActive(false);
    }

    private void Start()
    {
        if ((float)Screen.height / Screen.width > 2)
        {
            Camera.main.orthographicSize = 800;
        } else {
            Camera.main.orthographicSize = 667;
        }

        ball = FindObjectOfType<Ball>();
        AdManager.ShowBanner();
        navigator = FindObjectOfType<Navigator>();
        player.LoadPlayer();
        keys = player.keys;
        scoreboard.SetCoins(player.coins + coins);
        scoreboard.SetKeys(keys);

        // Hide shop and hint in tutorial levels
        if (player.nextLevelIndex <= 3 || player.nextLevelIndex == 100)
        {
            hintButton.SetActive(false);
        }

       // player.ResetPlayer();
        SetBallPrefab();
        SetBackground();
    }

    public bool GetShuffle()
    {
        if (player.nextLevelIndex > 3)
        {
            return true;
        }
        return shuffle;
    }

    public void ReceiveHintButtonClick()
    {
        adWarningReceiveButtonScript.Trigger();

        StartCoroutine(ReceiveHintButtonCoroutine());
    }

    public IEnumerator ReceiveHintButtonCoroutine()
    {
        yield return new WaitForSeconds(0.20f);

        AdManager.ShowStandardAd(UseHintSuccess, UseHintCancel, UseHintFail);
    }

    public void ContinuePlayingButtonClick()
    {
        adWarningContinueButtonScript.Trigger();

        StartCoroutine(ContinuePlayingButtonCoroutine());
    }

    public IEnumerator ContinuePlayingButtonCoroutine()
    {
        yield return new WaitForSeconds(0.20f);

        adCancelBg.SetActive(false);
        adCancelWarning.SetActive(false);
    }

    private void SetBallPrefab()
    {
        GameObject ballPrefab = Instantiate(balls[player.currentBallIndex], ball.transform.position, ball.transform.rotation);

        ballPrefab.transform.SetParent(ball.gameObject.transform);
    }

    public void UseHint()
    {
        hintButtonScript.Trigger();

        StartCoroutine(HintButtonCoroutine());
    }

    public IEnumerator HintButtonCoroutine()
    {
        yield return new WaitForSeconds(0.20f);

        if (player.nextLevelIndex == 4)
        {
            UseHintSuccess();
            showedPointer = true;
            showedFocus = true;
            CheckPointerFocus();
            ShowFocusPinterAfterHintHorizontal();
        }
        else
        {
            AdManager.ShowStandardAd(UseHintSuccess, UseHintCancel, UseHintFail);
        }
    }


    private void UseHintCancel()
    {
        
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
        showedAdCancelWarning = true;
        adCancelBg.SetActive(false);
        adCancelWarning.SetActive(false);
    }

    public void UseHintSuccess()
    {
        hintButton.SetActive(false);
        foreach (Transform child in walls)
        {
            child.GetComponent<Wall>().ShowCorrectPosition();
        }

        showedAdCancelWarning = true;
        adCancelBg.SetActive(false);
        adCancelWarning.SetActive(false);
    }

    public void LaunchBall()
    {
        ballLaunched = true;
        if (player.nextLevelIndex == 1)
        {
            FocusPointer.SetActive(false);
        }

        if (player.nextLevelIndex != 100)
        {
            resetButton.SetActive(true);
        }
        forwardButton.SetActive(true);

        hintButton.SetActive(false);
        shopButton.SetActive(false);
        ballDirectionArrow.SetActive(false);
    }

    public void CatchBall()
    {
        ballLaunched = false;
    }

    public bool GetBallLaunched()
    {
        return ballLaunched;
    }

    public void CollectKey()
    {
        if (keys < 3)
        {
            keys++;
            scoreboard.SetKeys(keys);
        }
    }

    public void CollectCoin()
    {
        coins++;
        scoreboard.SetCoins(player.coins + coins);
    }

    public int GetCoins()
    {
        return coins;
    }

    public int GetKeys()
    {
        return keys;
    }

    public void ResetButtonClick()
    {
        resetButtonScript.Trigger();

        StartCoroutine(ResetButtonCoroutine());
    }

    public IEnumerator ResetButtonCoroutine()
    {
        if (!ball.GetBallReset())
        {
            ball.ResetBall();
        }

        yield return new WaitForSeconds(0.20f);
        
        resetButton.SetActive(false);
        forwardButton.SetActive(false);
    }

    public void ResetForwardButton()
    {
        forwardButton.GetComponent<Button>().interactable = true;
        forwardButton.transform.GetChild(1).gameObject.SetActive(true); // icon
        forwardButton.transform.GetChild(2).gameObject.SetActive(false); // disabled icon
    }

    public void ForwardButtonClick()
    {
        forwardButtonScript.Trigger();
        forwardButton.GetComponent<Button>().interactable = false;
        forwardButton.transform.GetChild(1).gameObject.SetActive(false); // icon
        forwardButton.transform.GetChild(2).gameObject.SetActive(true); // disabled icon

        StartCoroutine(ForwardButtonCoroutine());
    }

    public IEnumerator ForwardButtonCoroutine()
    {
        yield return new WaitForSeconds(0.20f);

        ball.ForwardBall();
    }

    public void ResetLevel()
    {
        ResetForwardButton();

        resetButton.SetActive(false);
        forwardButton.SetActive(false);

        ballLaunched = false;
        ballDirectionArrow.SetActive(true);
        if (player.nextLevelIndex > 4)
        {
            shopButton.SetActive(true);
            hintButton.SetActive(true);
        }
        // if key or coins are collected, remove them and place them back
        //navigator.LoadNextLevel(player.nextLevelIndex);
    }

    public void LoadShopScene()
    {
        shopButtonScript.Trigger();

        StartCoroutine(LoadShopSceneCoroutine());
    }

    public IEnumerator LoadShopSceneCoroutine()
    {
        yield return new WaitForSeconds(0.20f);
        navigator.LoadShop();
    }

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

    public void ShowFocusPinterAfterHintHorizontal()
    {
        FocusPointerAfterHintHorizontal.SetActive(true);
        tutorialStep++;
    }

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
        for (int i = 0; i < player.unlockedBalls.Length; i++)
        {
            if (player.unlockedBalls[i] == 0)
            {
                return false;
            }
        }

        return true;
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
