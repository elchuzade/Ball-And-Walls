using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShopStatus : MonoBehaviour
{
    Player player;
    Scoreboard scoreboard;
    // Current ball to be selected based on player data
    string currentBallName;
    Navigator navigator;
    // Amount of coins that player will receive if he watches ad video
    [SerializeField] int adCoinsAmount;

    GameObject playButton;
    GameObject getCoinsButton;
    GameObject exitButton;
    // Continue watching and receive a reward button when player wanted to skip the video
    GameObject adWarningReceiveButton;
    // Reject reward and continue playing game button when player wanted to skip the video
    GameObject adWarningContinueButton;

    // Background of window that pops up if player wants to switch off reward ad
    GameObject adCancelBg;
    // Window with buttons that pops up if player wants to switch off reward ad
    GameObject adCancelWarning;

    // Track if ad cancel warning has already been shown, not to annoy each time showing the same window
    private bool showedAdCancelWarning = false;

    void Awake()
    {
        player = FindObjectOfType<Player>();

        player.ResetPlayer();

        scoreboard = FindObjectOfType<Scoreboard>();
        navigator = FindObjectOfType<Navigator>();

        playButton = GameObject.Find("PlayButton");
        getCoinsButton = GameObject.Find("GetCoinsButton");
        exitButton = GameObject.Find("ExitButton");

        adCancelBg = GameObject.Find("AdCancelBg");
        adCancelWarning = GameObject.Find("ShopAdCancelWarning");
        adWarningReceiveButton = GameObject.Find("AdWarningReceiveButton");
        adWarningContinueButton = GameObject.Find("AdWarningContinueButton");
    }

    void Start()
    {
        player.LoadPlayer();

        AdManager.ShowBanner();

        // Set currently selected ball with their frames and backgrounds
        currentBallName = player.currentBallName;

        // Set current player coins to the scoreboard
        scoreboard.SetCoins(player.coins);
        // Set current player diamonds to the scoreboard
        scoreboard.SetCoins(player.diamonds);

        // Set ad stuff back to normal as they are shrinked in x axis for visibility by default
        adCancelBg.transform.localScale = new Vector3(1, 1, 1);
        adCancelWarning.transform.localScale = new Vector3(1, 1, 1);

        // Hide ad stuff until the ad is actually skipped by player
        adCancelBg.SetActive(false);
        adCancelWarning.SetActive(false);
    }

    public bool CheckUnlockStatus(string ballName)
    {
        // Check player data for unlocked balls, if ball with given name is unlocked return true
        if (player.unlockedBalls.Contains(ballName))
        {
            return true;
        }

        return false;
    }

    public bool CheckSelectStatus(string ballName)
    {
        // Check player data for selected ball, if ball with given name is selected return true
        if (currentBallName == ballName)
        {
            return true;
        }

        return false;
    }

    public bool SelectItem(string ballName)
    {
        // Check if ball is unlocked, select it and save player data
        if (player.unlockedBalls.Contains(ballName))
        {
            currentBallName = ballName;
            player.currentBallName = currentBallName;
            player.SavePlayer();
            return true;
        }

        return false;
    }

    public bool UnlockItem(string ballName, int priceTag)
    {
        // Check if ball is locked and player has enough coins to unlock it return true
        if (!player.unlockedBalls.Contains(ballName) && player.coins >= priceTag)
        {
            // Change player coins for ball price
            player.coins -= priceTag;
            // Set this ball index as unlocked in player data
            player.unlockedBalls.Add(ballName);
            player.SavePlayer();
            // Load player again to access its data
            player.LoadPlayer();
            // Update coins in scoreboard based on new status of player coins
            scoreboard.SetCoins(player.coins);
            return true;
        }
        return false;
    }

    public void PlayLevel()
    {
        if (playButton.GetComponent<Button>().IsInteractable())
        {
            // Run the trigger button animation and disable button for its duration
            playButton.GetComponent<TriggerButton>().ClickButton(0.2f);
            // Approximately when animation is over, load the game scene
            StartCoroutine(LoadGameSceneCoroutine(0.2f));
        }
    }

    public void GetMoreCoins()
    {
        if (getCoinsButton.GetComponent<Button>().IsInteractable())
        {
            // Run the trigger button animation and disable button for its duration
            getCoinsButton.GetComponent<TriggerButton>().ClickButton(0.2f);
            // Approximately when animation is over, load get more coins ad
            StartCoroutine(LoadGetMoreCoinsCoroutine(0.2f));
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

    public IEnumerator LoadGameSceneCoroutine(float time)
    {
        // Wait for given time and load the game scene
        yield return new WaitForSeconds(time);

        navigator.LoadNextLevel(player.nextLevelIndex);
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
        player.coins += adCoinsAmount;
        // Update number of coins in scoreboard
        scoreboard.SetCoins(player.coins);
        // Save player
        player.SavePlayer();

        // Set a parameter to remmeber that once ad stuff was already cancelled not to ask a player again when he skips another ad
        showedAdCancelWarning = true;
        adCancelBg.SetActive(false);
        adCancelWarning.SetActive(false);
    }
}
