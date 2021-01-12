using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class ChestStatus : MonoBehaviour
{
    [SerializeField] GameObject key1;
    [SerializeField] GameObject key2;
    [SerializeField] GameObject key3;
    [SerializeField] GameObject adsButton;
    [SerializeField] GameObject passPhrase;
    [SerializeField] Scoreboard scoreboard;    
    [SerializeField] Player player;
    [SerializeField] GameObject[] bestPrizesPrefabs;
    [SerializeField] Transform bestPrize;

    Animator key1animator;
    Animator key2animator;
    Animator key3animator;

    float cameraHeightFactor = 1.44f;
    bool bestPrizeReceived = false;
    Navigator navigator;

    TriggerAnimation exitButtonScript;
    TriggerAnimation moreKeysButtonScript;

    GameObject adCancelBg;
    GameObject adCancelWarning;

    GameObject adWarningReceiveButton;
    GameObject adWarningContinueButton;

    TriggerAnimation adWarningReceiveButtonScript;
    TriggerAnimation adWarningContinueButtonScript;

    bool showedAdCancelWarning = false;

    // TODO Opening chests should not be allowed if you quit the game and relaunch not to bug the game

    List<int> rewards = new List<int>() { 0, 10, 10, 10, 10, 25, 25, 25, 50 };
    int reward = -1;
    int keys = 3;
    int openedChests = 0;
    int totalReward = 0;

    int bestPrizeIndex;
    List<int> bestPrizes = new List<int>();

    private void Start()
    {
        if ((float)Screen.height / Screen.width > 2)
            cameraHeightFactor = 1.52f;

        Camera.main.orthographicSize = Screen.height / 6;
        Camera.main.transform.position = new Vector2(Screen.width / 2, Screen.height / cameraHeightFactor);

        key1animator = key1.GetComponent<Animator>();
        key2animator = key2.GetComponent<Animator>();
        key3animator = key3.GetComponent<Animator>();

        navigator = FindObjectOfType<Navigator>();
        player.LoadPlayer();
        scoreboard.SetCoins(player.coins);
        DrawKeys();
        AdManager.ShowBanner();

        GameObject exitButtonObject = GameObject.Find("ExitButton");
        GameObject moreKeysButtonObject = GameObject.Find("MoreKeysButton");
        moreKeysButtonObject.SetActive(false);
        exitButtonScript = exitButtonObject.GetComponent<TriggerAnimation>();
        moreKeysButtonScript = moreKeysButtonObject.GetComponent<TriggerAnimation>();

        SetBestPrize();

        adCancelBg = GameObject.Find("AdCancelBg");
        adCancelWarning = GameObject.Find("ChestAdCancelWarning");
        adWarningReceiveButton = GameObject.Find("AdWarningReceiveButton");
        adWarningContinueButton = GameObject.Find("AdWarningContinueButton");

        adWarningReceiveButtonScript = adWarningReceiveButton.GetComponent<TriggerAnimation>();
        adWarningContinueButtonScript = adWarningContinueButton.GetComponent<TriggerAnimation>();

        adCancelBg.SetActive(false);
        adCancelWarning.SetActive(false);
    }

    private IEnumerator OpenPassPhrase()
    {
        key1.GetComponent<Image>().color = new Color32(0, 0, 0, 0);
        key2.GetComponent<Image>().color = new Color32(0, 0, 0, 0);
        key3.GetComponent<Image>().color = new Color32(0, 0, 0, 0);

        yield return new WaitForSeconds(2);

        if (adsButton.activeSelf)
        {
            passPhrase.SetActive(true);
        }
    }

    private IEnumerator LoadNextLevel()
    {
        yield return new WaitForSeconds(2f);
        NextLevel();
    }

    private void SetKeyAnimation()
    {
        if (keys == 1)
        {
            key1animator.enabled = true;
            key2animator.enabled = false;
            key3animator.enabled = false;
        } else if (keys == 2)
        {
            key1animator.enabled = false;
            key2animator.enabled = true;
            key3animator.enabled = false;
        } else if (keys == 3)
        {
            key1animator.enabled = false;
            key2animator.enabled = false;
            key3animator.enabled = true;
        }
    }

    private void StopAllKeyAnimations()
    {
        key1animator.enabled = false;
        key2animator.enabled = false;
        key3animator.enabled = false;
    }

    private void SetBestPrize()
    {
        for (int i = 0; i < player.unlockedBalls.Length; i++)
        {
            Debug.Log(player.unlockedBalls[i]);
            if (player.unlockedBalls[i] == 0)
            {
                bestPrizes.Add(i);
            }
        }

        bestPrizeIndex = new System.Random().Next(0, bestPrizes.Count);
        GameObject bestPrizeObject = Instantiate(
            bestPrizesPrefabs[bestPrizes[bestPrizeIndex]],
            bestPrize.position,
            Quaternion.identity);

        bestPrizeObject.transform.parent = bestPrize;
    }

    public Sprite GetBestPrizeSprite()
    {
        return bestPrizesPrefabs[bestPrizes[bestPrizeIndex]].GetComponent<SpriteRenderer>().sprite;
    }

    public void NextLevel()
    {
        // Load next level
        player.coins += totalReward;
        scoreboard.SetCoins(player.coins);
        totalReward = 0;

        if (bestPrizeReceived)
        {
            player.unlockedBalls[bestPrizes[bestPrizeIndex]] = 1;
            player.currentBallIndex = bestPrizes[bestPrizeIndex];
        }
        
        player.SavePlayer();
        navigator.LoadNextLevel(player.nextLevelIndex);
    }

    public int OpenChest()
    {
        System.Random rnd = new System.Random();

        reward = rewards[rnd.Next(rewards.Count)]; // Extract reward

        if (reward == 0)
            bestPrizeReceived = true;

        rewards.Remove(reward); // Remove reward
        keys--;
        totalReward += reward;
        openedChests++;
        if (openedChests == 9)
        {
            StopAllKeyAnimations();
            StartCoroutine(LoadNextLevel());
        }
        DrawKeys();

        if (openedChests % 3 == 0 && openedChests != 9)
        {
            adsButton.SetActive(true);
            StartCoroutine(OpenPassPhrase());
        }

        return reward;
    }

    private void DrawKeys()
    {
        key1.GetComponent<Image>().color = new Color32(0, 0, 0, 255);
        key2.GetComponent<Image>().color = new Color32(0, 0, 0, 255);
        key3.GetComponent<Image>().color = new Color32(0, 0, 0, 255);

        if (keys > 0)
            key1.GetComponent<Image>().color = new Color32(255, 240, 0, 255);
        if (keys > 1)
            key2.GetComponent<Image>().color = new Color32(255, 240, 0, 255);
        if (keys > 2)
            key3.GetComponent<Image>().color = new Color32(255, 240, 0, 255);

        SetKeyAnimation();
    }

    public int GetKeys()
    {
        return keys;
    }

    public void GetMoreKeys()
    {
        moreKeysButtonScript.Trigger();
        StartCoroutine(LoadGetMoreKeys());
    }

    public IEnumerator LoadGetMoreKeys()
    {
        yield return new WaitForSeconds(0.20f);
        AdManager.ShowStandardAd(GetMoreKeysSuccess, GetKeysCancel, CloseChest);
    }

    private void GetMoreKeysSuccess()
    {
        adsButton.SetActive(false);
        passPhrase.SetActive(false);
        keys = 3;
        DrawKeys();

        player.SavePlayer();
        player.LoadPlayer();

        showedAdCancelWarning = true;
        adCancelBg.SetActive(false);
        adCancelWarning.SetActive(false);
    }

    public void ClickExitButton()
    {
        exitButtonScript.Trigger();
        StartCoroutine(LoadCloseChest());
    }

    public IEnumerator LoadCloseChest()
    {
        yield return new WaitForSeconds(0.20f);
        CloseChest();
    }

    public void CloseChest()
    {
        NextLevel();
    }

    public void ReceiveKeysButtonClick()
    {
        adWarningReceiveButtonScript.Trigger();

        StartCoroutine(ReceiveKeysButtonCoroutine());
    }

    public IEnumerator ReceiveKeysButtonCoroutine()
    {
        yield return new WaitForSeconds(0.20f);

        AdManager.ShowStandardAd(GetMoreKeysSuccess, GetKeysCancel, GetKeysFail);
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

    private void GetKeysCancel()
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

    private void GetKeysFail()
    {
        showedAdCancelWarning = true;
        adCancelBg.SetActive(false);
        adCancelWarning.SetActive(false);
    }
}
