using System;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] Text powerUp;

    AdCancel adCancel;

    // For hint button laoder
    [SerializeField] GameObject getCoinsButton;
    [SerializeField] GameObject getCoinsButtonEmpty;
    [SerializeField] GameObject getCoinsButtonLoader;
    // For hint button ad cancel window
    [SerializeField] GameObject getCoinsButtonReceive;
    [SerializeField] GameObject getCoinsButtonReceiveEmpty;
    [SerializeField] GameObject getCoinsButtonReceiveLoader;

    // Dots that indicate which 9 items are shown on the screen
    [SerializeField] GameObject leftDot;
    [SerializeField] GameObject midLeftDot;
    [SerializeField] GameObject midRightDot;
    [SerializeField] GameObject rightDot;

    // Track if ad cancel warning has already been shown, not to annoy each time showing the same window
    bool showedAdCancelWarning = false;

    // All items are held here
    GameObject items;

    [SerializeField] Scrollbar scrollbar;
    void Awake()
    {
        scoreboard = FindObjectOfType<Scoreboard>();
        navigator = FindObjectOfType<Navigator>();
        adCancel = FindObjectOfType<AdCancel>();

        items = GameObject.Find("Items");
    }

    void Start()
    {
        if ((float)Screen.width / Screen.height > 0.7)
        {
            GameObject canvas = GameObject.Find("Canvas");
            canvas.GetComponent<CanvasScaler>().matchWidthOrHeight = 1;
        }

        player = FindObjectOfType<Player>();
        //player.coins = 100000;
        //player.SavePlayer();
        //return;
        player.LoadPlayer();

        scrollbar.GetComponent<Scrollbar>().value = 0;
        scrollbar.GetComponent<Scrollbar>().onValueChanged.AddListener(value => SwipeShop(value));

        // Set currently selected ball with their frames and backgrounds
        currentBallName = player.currentBall;

        AdManager.ShowBanner();

        // Set current player coins to the scoreboard
        scoreboard.SetCoins(player.coins);
        // Set current player diamonds to the scoreboard
        scoreboard.SetDiamonds(player.diamonds);

        adCancel.InitializeAdCancel(" coins", coinIcon);
        adCancel.GetReceiveButton().GetComponent<Button>().onClick.AddListener(() => ClickReceiveGetCoins());
        adCancel.GetCancelButton().GetComponent<Button>().onClick.AddListener(() => CancelButtonClick());
        SetBallItems();
        adCancel.gameObject.SetActive(false);

        // Save click
        DateTimeOffset now = DateTimeOffset.UtcNow;
        long date = now.ToUnixTimeMilliseconds();
        player.shopClicks.Add(date);
        player.SavePlayer();
    }

    private void SwitchDots()
    {
        int currentSideIndex;
        if (scrollbar.value <= 0.25)
        {
            currentSideIndex = 0;
        }
        else if (scrollbar.value <= 0.5)
        {
            currentSideIndex = 1;
        }
        else if (scrollbar.value <= 0.75)
        {
            currentSideIndex = 2;
        }
        else
        {
            currentSideIndex = 3;
        }

        // Based on which side of the wide list the player is at, make the corresponding dot lighter
        if (currentSideIndex == 0)
        {
            leftDot.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
            midLeftDot.GetComponent<Image>().color = new Color32(150, 150, 150, 255);
            midRightDot.GetComponent<Image>().color = new Color32(150, 150, 150, 255);
            rightDot.GetComponent<Image>().color = new Color32(150, 150, 150, 255);
        }
        else if (currentSideIndex == 1)
        {
            leftDot.GetComponent<Image>().color = new Color32(150, 150, 150, 255);
            midLeftDot.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
            midRightDot.GetComponent<Image>().color = new Color32(150, 150, 150, 255);
            rightDot.GetComponent<Image>().color = new Color32(150, 150, 150, 255);
        }
        else if (currentSideIndex == 2)
        {
            leftDot.GetComponent<Image>().color = new Color32(150, 150, 150, 255);
            midLeftDot.GetComponent<Image>().color = new Color32(150, 150, 150, 255);
            midRightDot.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
            rightDot.GetComponent<Image>().color = new Color32(150, 150, 150, 255);
        }
        else if (currentSideIndex == 3)
        {
            leftDot.GetComponent<Image>().color = new Color32(150, 150, 150, 255);
            midLeftDot.GetComponent<Image>().color = new Color32(150, 150, 150, 255);
            midRightDot.GetComponent<Image>().color = new Color32(150, 150, 150, 255);
            rightDot.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        }
    }

    private void SetBallItems()
    {
        for (int i = 0; i < items.transform.childCount; i++)
        {
            items.transform.GetChild(i).GetComponent<Item>().CheckBallData();
            // Display powerup value according to the selected ball
            if (items.transform.GetChild(i).GetComponent<Item>().GetBallName() == player.currentBall)
            {
                powerUp.text = items.transform.GetChild(i).transform.Find("PowerUp").GetComponent<Text>().text;
                //Debug.Log(items.transform.GetChild(i).transform.Find("PowerUp").GetComponent<Text>().text);
            }
        }
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
            SetBallItems();
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

    public void ClickPlayButton()
    {
        navigator.LoadNextLevel(player.nextLevelIndex, player.maxLevelReached);
    }

    public void ClickGetCoins()
    {
        DisableCoinsButtonLoadingAd();
        AdManager.ShowStandardAd(GetCoinsSuccess, GetCoinsCancel, GetCoinsFail);
    }

    public void ClickReceiveGetCoins()
    {
        // Save click
        DateTimeOffset now = DateTimeOffset.UtcNow;
        long date = now.ToUnixTimeMilliseconds();

        DisableCoinsButtonLoadingAd();
        AdManager.ShowStandardAd(GetCoinsSuccess, GetCoinsCancel, GetCoinsFail);
    }

    public void ClickExitButton()
    {
        navigator.LoadMainScene();
    }

    public void CancelButtonClick()
    {
        adCancel.gameObject.SetActive(false);
    }

    public void GetCoinsCancel()
    {
        EnableGetCoinsButtonLoadingAd();
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

    public void GetCoinsFail()
    {
        EnableGetCoinsButtonLoadingAd();
        // If a video for receiving coins fails, hide the warning page about cancelling, not to annoy the player
        // Set a parameter to remmeber that once ad stuff was already cancelled not to ask a player again when he skips another ad
        scoreboard.gameObject.SetActive(false);
        showedAdCancelWarning = true;
        adCancel.gameObject.SetActive(false);
    }

    public void DisableCoinsButtonLoadingAd()
    {
        getCoinsButton.GetComponent<Button>().interactable = false;
        getCoinsButtonLoader.SetActive(true);
        getCoinsButtonEmpty.SetActive(true);

        getCoinsButtonReceive.GetComponent<Button>().interactable = false;
        getCoinsButtonReceiveEmpty.SetActive(true);
        getCoinsButtonReceiveLoader.SetActive(true);
    }

    public void EnableGetCoinsButtonLoadingAd()
    {
        getCoinsButton.GetComponent<Button>().interactable = true;
        getCoinsButtonLoader.SetActive(false);
        getCoinsButtonEmpty.SetActive(false);

        getCoinsButtonReceive.GetComponent<Button>().interactable = true;
        getCoinsButtonReceiveEmpty.SetActive(false);
        getCoinsButtonReceiveLoader.SetActive(false);
    }

    public void GetCoinsSuccess()
    {
        // Save click
        DateTimeOffset now = DateTimeOffset.UtcNow;
        long date = now.ToUnixTimeMilliseconds();
        player.getTenMoreCoinsClicks.Add(date);
        player.SavePlayer();

        EnableGetCoinsButtonLoadingAd();
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

    public void SwipeShop(float value)
    {
        SwitchDots();
    }

    public void ClickShopItem(GameObject item)
    {
        item.GetComponent<Item>().TouchItem();
    }
}
