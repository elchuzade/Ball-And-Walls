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
    // To pass to ad cancel
    [SerializeField] Sprite coinIcon;

    GameObject playButton;
    GameObject getCoinsButton;
    GameObject exitButton;

    AdCancel adCancel;

    // Track if ad cancel warning has already been shown, not to annoy each time showing the same window
    bool showedAdCancelWarning = false;

    // All items are held here
    GameObject items;

    void Awake()
    {
        scoreboard = FindObjectOfType<Scoreboard>();
        navigator = FindObjectOfType<Navigator>();
        adCancel = FindObjectOfType<AdCancel>();

        playButton = GameObject.Find("PlayButton");
        getCoinsButton = GameObject.Find("GetCoinsButton");
        exitButton = GameObject.Find("ExitButton");

        items = GameObject.Find("Items");
    }

    void Start()
    {
        player = FindObjectOfType<Player>();

        player.LoadPlayer();

        // Set currently selected ball with their frames and backgrounds
        currentBallName = player.currentBall;

        AdManager.ShowBanner();

        // Set current player coins to the scoreboard
        scoreboard.SetCoins(player.coins);
        // Set current player diamonds to the scoreboard
        scoreboard.SetDiamonds(player.diamonds);

        adCancel.InitializeAdCancel(" coins", coinIcon);
        adCancel.GetReceiveButton().GetComponent<Button>().onClick.AddListener(() => ReceiveButtonClick());
        adCancel.GetCancelButton().GetComponent<Button>().onClick.AddListener(() => CancelButtonClick());
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

    public bool GetPlayerLoaded()
    {
        Debug.Log(player.unlockedBalls.Count);
        if (player != null && player.unlockedBalls.Count > 0)
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
            player.currentBall = currentBallName;
            player.SavePlayer();

            // Remove the select frame from previous item and place in new item
            for (int i = 0; i < items.transform.childCount; i++)
            {
                items.transform.GetChild(i).GetComponent<Item>().CheckSelectFrame();
            }
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

            // Remove the select frame from previous item and place in new item
            for (int i = 0; i < items.transform.childCount; i++)
            {
                items.transform.GetChild(i).GetComponent<Item>().CheckSelectFrame();
            }
            return true;
        }
        return false;
    }

    public bool UnlockDiamondItem(string ballName, int diamondTag)
    {
        // Check if ball is locked and player has enough coins to unlock it return true
        if (!player.unlockedBalls.Contains(ballName) && player.diamonds >= diamondTag)
        {
            // Change player coins for ball price
            player.diamonds -= diamondTag;
            // Set this ball index as unlocked in player data
            player.unlockedBalls.Add(ballName);
            player.SavePlayer();
            // Load player again to access its data
            player.LoadPlayer();
            // Update diamonds in scoreboard based on new status of player diamonds
            scoreboard.SetDiamonds(player.diamonds);
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

    // This is similar to LoadGetMoreCoins. But for consistency, it is better to keep it separately
    public IEnumerator ReceiveButtonCoroutine(float time)
    {
        // Wait for given time and load the ad screen
        yield return new WaitForSeconds(time);
        // Load the ad screen
        AdManager.ShowStandardAd(GetCoinsSuccess, GetCoinsCancel, GetCoinsFail);
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
        yield return new WaitForSeconds(time);

        // Wait for some time and hide all the ad stuff, since player has cancelled reward and refused to watch the ad
        adCancel.gameObject.SetActive(false);
    }

    private void GetCoinsCancel()
    {
        // Playe has canceled get extra ccoins video
        if (!showedAdCancelWarning)
        {
            // If it is the first time, show him a warnigng screen with ad stuff
            adCancel.gameObject.SetActive(true);
        }
        else
        {
            // else hide a warning screen with ad stuff
            adCancel.gameObject.SetActive(false);
        }

        // Set a parameter to remmeber that once ad stuff was already cancelled not to ask a player again when he skips another ad
        showedAdCancelWarning = true;
    }

    private void GetCoinsFail()
    {
        // If a video for receiving coins fails, hide the warning page about cancelling, not to annoy the player
        // Set a parameter to remmeber that once ad stuff was already cancelled not to ask a player again when he skips another ad
        showedAdCancelWarning = true;
        adCancel.gameObject.SetActive(false);
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
        adCancel.gameObject.SetActive(false);
    }
}
