using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ShopStatus : MonoBehaviour
{
    [SerializeField] Player player;
    [SerializeField] Scoreboard scoreboard;
    private int[] unlockedBalls;
    private int currentBallIndex;
    Navigator navigator;
    [SerializeField] int adCoinsAmount;

    private GameObject hapticsButton;
    private GameObject soundsButton;

    //TriggerAnimation adsButtonScript;
    TriggerAnimation playButtonScript;
    TriggerAnimation getCoinsButtonScript;

    GameObject adCancelBg;
    GameObject adCancelWarning;

    GameObject adWarningReceiveButton;
    GameObject adWarningContinueButton;

    TriggerAnimation adWarningReceiveButtonScript;
    TriggerAnimation adWarningContinueButtonScript;

    bool showedAdCancelWarning = false;

    private void Awake()
    {
        navigator = FindObjectOfType<Navigator>();
        player.LoadPlayer();
        SetUnlockedAndCurrent();
    }

    void Start()
    {
        hapticsButton = GameObject.Find("HapticsButton");
        soundsButton = GameObject.Find("SoundsButton");

        // For haptics and sounds
        SetButtonInitialState();

        scoreboard.SetCoins(player.coins);
        AdManager.ShowBanner();

        //GameObject adsButtonObject = GameObject.Find("AdsButton");
        GameObject playButtonObject = GameObject.Find("PlayButton");
        GameObject getCoinsButtonObject = GameObject.Find("GetCoinsButton");
        //adsButtonScript = adsButtonObject.GetComponent<TriggerAnimation>();
        playButtonScript = playButtonObject.GetComponent<TriggerAnimation>();
        getCoinsButtonScript = getCoinsButtonObject.GetComponent<TriggerAnimation>();

        adCancelBg = GameObject.Find("AdCancelBg");
        adCancelWarning = GameObject.Find("ShopAdCancelWarning");
        adWarningReceiveButton = GameObject.Find("AdWarningReceiveButton");
        adWarningContinueButton = GameObject.Find("AdWarningContinueButton");

        adWarningReceiveButtonScript = adWarningReceiveButton.GetComponent<TriggerAnimation>();
        adWarningContinueButtonScript = adWarningContinueButton.GetComponent<TriggerAnimation>();

        // Set ad stuff back to normal as they are shrinked in x axis for visibility by default
        adCancelBg.transform.localScale = new Vector3(1, 1, 1);
        adCancelWarning.transform.localScale = new Vector3(1, 1, 1);

        adCancelBg.SetActive(false);
        adCancelWarning.SetActive(false);
    }

    public bool CheckUnlockStatus(int index)
    {

        if (unlockedBalls[index] == 1)
            return true;

        return false;
    }

    public bool CheckSelectStatus(int index)
    {
        if (currentBallIndex == index)
            return true;

        return false;
    }

    public bool SelectItem(int index)
    {
        if (unlockedBalls[index] == 1)
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
        if (unlockedBalls[index] == 0 && player.coins >= priceTag)
        {
            player.coins -= priceTag;
            unlockedBalls[index] = 1;
            player.unlockedBalls = unlockedBalls;
            player.SavePlayer();
            scoreboard.SetCoins(player.coins);
            return true;
        }
        return false;
    }

    private void SetUnlockedAndCurrent()
    {
        unlockedBalls = player.unlockedBalls;
        currentBallIndex = player.currentBallIndex;
    }

    public void ResetPlayer()
    {
        player.ResetPlayer();
    }

    public void CloseShop()
    {
        playButtonScript.Trigger();
        StartCoroutine(LoadGameSceneCoroutine());
    }

    public void GetMoreCoins()
    {
        getCoinsButtonScript.Trigger();
        StartCoroutine(LoadGetMoreCoins());
    }

    public void BuyAdsFree()
    {
        //adsButtonScript.Trigger();
    }

    public IEnumerator LoadGameSceneCoroutine()
    {
        yield return new WaitForSeconds(0.20f);
        navigator.LoadNextLevel(player.nextLevelIndex);
    }

    public IEnumerator LoadGetMoreCoins()
    {
        yield return new WaitForSeconds(0.20f);
        AdManager.ShowStandardAd(GetCoinsSuccess, GetCoinsCancel, GetCoinsFail);
    }

    public void ReceiveCoinsButtonClick()
    {
        adWarningReceiveButtonScript.Trigger();

        StartCoroutine(ReceiveCoinsButtonCoroutine());
    }

    public IEnumerator ReceiveCoinsButtonCoroutine()
    {
        yield return new WaitForSeconds(0.20f);

        AdManager.ShowStandardAd(GetCoinsSuccess, GetCoinsCancel, GetCoinsFail);
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

    private void GetCoinsCancel()
    {

        if (!showedAdCancelWarning)
        {
            adCancelBg.SetActive(true);
            adCancelWarning.SetActive(true);
        }
        else
        {
            adCancelBg.SetActive(false);
            adCancelWarning.SetActive(false);
        }

        showedAdCancelWarning = true;
    }

    private void GetCoinsFail()
    {
        showedAdCancelWarning = true;
        adCancelBg.SetActive(false);
        adCancelWarning.SetActive(false);
    }

    public void GetCoinsSuccess()
    {
        player.coins += adCoinsAmount;
        scoreboard.SetCoins(player.coins);
        player.SavePlayer();
        player.LoadPlayer();

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
