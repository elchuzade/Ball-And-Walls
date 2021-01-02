using UnityEngine;
using System.Collections;

public class ShopStatus : MonoBehaviour
{
    [SerializeField] Player player;
    [SerializeField] Scoreboard scoreboard;
    private int[] unlockedBalls;
    private int currentBallIndex;
    Navigator navigator;
    [SerializeField] int adCoinsAmount;

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
}
