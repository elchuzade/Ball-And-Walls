using UnityEngine;
using System.Collections;

public class ShopStatus : MonoBehaviour
{
    [SerializeField] Player player;
    private Scoreboard scoreboard;
    // Current ball to be selected based on player data
    private int currentBallIndex;
    private Navigator navigator;
    // Amount of coins that player will receive if he watches ad video
    [SerializeField] int adCoinsAmount;

    private GameObject hapticsButton;
    private GameObject soundsButton;

    // Animation to be played when play button is clicked
    private TriggerAnimation playButtonScript;
    // Animation to be played when get coins button is clicked
    private TriggerAnimation getCoinsButtonScript;

    // Background of window that pops up if player wants to switch off reward ad
    private GameObject adCancelBg;
    // Window with buttons that pops up if player wants to switch off reward ad
    private GameObject adCancelWarning;

    // Continue watching and receive a reward button when player wanted to skip the video
    private GameObject adWarningReceiveButton;
    // Reject reward and continue playing game button when player wanted to skip the video
    private GameObject adWarningContinueButton;

    // Animation to be played when Receive a reward button is clicked
    private TriggerAnimation adWarningReceiveButtonScript;
    // Animation to be played when Continue Playing Game button is clicked
    private TriggerAnimation adWarningContinueButtonScript;

    // Track if ad cancel warning has already been shown, not to annoy each time showing the same window
    private bool showedAdCancelWarning = false;

    void Awake()
    {
        scoreboard = FindObjectOfType<Scoreboard>();
        navigator = FindObjectOfType<Navigator>();

        hapticsButton = GameObject.Find("HapticsButton");
        soundsButton = GameObject.Find("SoundsButton");

        GameObject playButtonObject = GameObject.Find("PlayButton");
        GameObject getCoinsButtonObject = GameObject.Find("GetCoinsButton");

        playButtonScript = playButtonObject.GetComponent<TriggerAnimation>();
        getCoinsButtonScript = getCoinsButtonObject.GetComponent<TriggerAnimation>();

        adCancelBg = GameObject.Find("AdCancelBg");
        adCancelWarning = GameObject.Find("ShopAdCancelWarning");
        adWarningReceiveButton = GameObject.Find("AdWarningReceiveButton");
        adWarningContinueButton = GameObject.Find("AdWarningContinueButton");

        adWarningReceiveButtonScript = adWarningReceiveButton.GetComponent<TriggerAnimation>();
        adWarningContinueButtonScript = adWarningContinueButton.GetComponent<TriggerAnimation>();
    }

    void Start()
    {
        player.LoadPlayer();

        AdManager.ShowBanner();

        // Set currently selected ball with their frames and backgrounds
        currentBallIndex = player.currentBallIndex;

        // Set whether haptics and sound buttons are enabled or disabled initially
        SetButtonInitialState();

        // Set current player coins to the scoreboard
        scoreboard.SetCoins(player.coins);

        // Set ad stuff back to normal as they are shrinked in x axis for visibility by default
        adCancelBg.transform.localScale = new Vector3(1, 1, 1);
        adCancelWarning.transform.localScale = new Vector3(1, 1, 1);

        // Hide ad stuff until the ad is actually skipped by player
        adCancelBg.SetActive(false);
        adCancelWarning.SetActive(false);
    }

    public bool CheckUnlockStatus(int index)
    {
        // Check player data for unlocked balls, if ball with given index is unlocked return true
        if (player.unlockedBalls[index] == 1)
        {
            return true;
        }

        return false;
    }

    public bool CheckSelectStatus(int index)
    {
        // Check player data for selected ball, if ball with given index is selected return true
        if (currentBallIndex == index)
        {
            return true;
        }

        return false;
    }

    public bool SelectItem(int index)
    {
        // Check if ball is unlocked, select it and save player data
        if (player.unlockedBalls[index] == 1)
        {
            currentBallIndex = index;
            player.currentBallIndex = currentBallIndex;
            player.SavePlayer();
            return true;
        }

        return false;
    }

    public bool UnlockItem(int index, int priceTag)
    {
        // Check if ball is locked and player has enough coins to unlock it return true
        if (player.unlockedBalls[index] == 0 && player.coins >= priceTag)
        {
            // Change player coins for ball price
            player.coins -= priceTag;
            // Set this ball index as unlocked in player data
            player.unlockedBalls[index] = 1;
            player.SavePlayer();
            // Load player again to access its data
            player.LoadPlayer();
            // Update coins in scoreboard based on new status of player coins
            scoreboard.SetCoins(player.coins);
            return true;
        }
        return false;
    }

    // Debugging purpose. Reset all player data
    //public void ResetPlayer()
    //{
    //    player.ResetPlayer();
    //}

    public void CloseShop()
    {
        // Play the animation of play button clikcing
        playButtonScript.Trigger();
        // Approximately when animation is over, load the game scene
        StartCoroutine(LoadGameSceneCoroutine(0.2f));
    }

    public void GetMoreCoins()
    {
        // Play the animation of get more coins button clikcing
        getCoinsButtonScript.Trigger();
        // Approximately when animation is over, load get more coins ad
        StartCoroutine(LoadGetMoreCoins(0.2f));
    }

    //public void BuyAdsFree()
    //{
    //    adsButtonScript.Trigger();
    //}

    public IEnumerator LoadGameSceneCoroutine(float time)
    {
        // Wait for given time and load the game scene
        yield return new WaitForSeconds(time);

        navigator.LoadNextLevel(player.nextLevelIndex);
    }

    public IEnumerator LoadGetMoreCoins(float time)
    {
        // Wait for given time and load the ad screen
        yield return new WaitForSeconds(time);

        AdManager.ShowStandardAd(GetCoinsSuccess, GetCoinsCancel, GetCoinsFail);
    }

    public void ReceiveCoinsButtonClick()
    {
        // Run animation of clicking receive coins and watch the ad button
        adWarningReceiveButtonScript.Trigger();

        // Approximately when animation is finished, load the ad screen
        StartCoroutine(ReceiveCoinsButtonCoroutine(0.2f));
    }

    // This is similar to LoadGetMoreCoins. But for consistency, it is better to keep it separately
    public IEnumerator ReceiveCoinsButtonCoroutine(float time)
    {
        // Wait for given time and load the ad screen
        yield return new WaitForSeconds(time);

        AdManager.ShowStandardAd(GetCoinsSuccess, GetCoinsCancel, GetCoinsFail);
    }

    public void ContinuePlayingButtonClick()
    {
        // Wait for given time and load the ad screen
        adWarningContinueButtonScript.Trigger();

        StartCoroutine(ContinuePlayingButtonCoroutine(0.2f));
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

    // Set initial states of haptics and sounds buttons based on player prefs
    private void SetButtonInitialState()
    {
        if (PlayerPrefs.GetInt("Haptics") == 1)
        {
            hapticsButton.GetComponent<IconDisableButton>().SetButtonInitialState(ButtonStates.Enable);
        }
        else
        {
            hapticsButton.GetComponent<IconDisableButton>().SetButtonInitialState(ButtonStates.Disable);
        }
        if (PlayerPrefs.GetInt("Sounds") == 1)
        {
            soundsButton.GetComponent<IconDisableButton>().SetButtonInitialState(ButtonStates.Enable);
        }
        else
        {
            soundsButton.GetComponent<IconDisableButton>().SetButtonInitialState(ButtonStates.Disable);
        }
    }

    public void ClickHapticsButton()
    {
        if (PlayerPrefs.GetInt("Haptics") == 1)
        {
            // Run the click button animation, disable interactible feature until the end of animation
            // Set button state to disabled
            hapticsButton.GetComponent<IconDisableButton>().ClickButton(ButtonStates.Disable);
            // If haptics are turned on => turn them off
            PlayerPrefs.SetInt("Haptics", 0);
        }
        else
        {
            // Run the click button animation, disable interactible feature until the end of animation
            // Set button state to enabled
            hapticsButton.GetComponent<IconDisableButton>().ClickButton(ButtonStates.Enable);
            // If haptics are turned off => turn them on
            PlayerPrefs.SetInt("Haptics", 1);
        }
    }

    public void ClickSoundsButton()
    {
        if (PlayerPrefs.GetInt("Sounds") == 1)
        {
            // Run the click button animation, disable interactible feature until the end of animation
            // Set button state to disabled
            soundsButton.GetComponent<IconDisableButton>().ClickButton(ButtonStates.Disable);
            // If sounds are turned on => turn them off
            PlayerPrefs.SetInt("Sounds", 0);
        }
        else
        {
            // Run the click button animation, disable interactible feature until the end of animation
            // Set button state to enabled
            soundsButton.GetComponent<IconDisableButton>().ClickButton(ButtonStates.Enable);
            // If sounds are turned off => turn them on
            PlayerPrefs.SetInt("Sounds", 1);
        }
    }
}
