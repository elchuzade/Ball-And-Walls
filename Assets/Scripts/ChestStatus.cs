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

    // Zoom animations on keys
    private Animator key1animator;
    private Animator key2animator;
    private Animator key3animator;

    // This is needed because we are mixing canvas and world game object
    float cameraHeightFactor = 1.44f;
    bool bestPrizeReceived = false;
    Navigator navigator;

    GameObject exitButton;
    GameObject moreKeysButton;
    GameObject passPhraseButton;

    GameObject adCancelBg;
    GameObject adCancelWarning;

    GameObject adWarningReceiveButton;
    GameObject adWarningContinueButton;

    bool showedAdCancelWarning = false;

    // TODO Opening chests should not be allowed if you quit the game and relaunch not to bug the game

    List<int> rewards = new List<int>() { 0, 10, 10, 10, 10, 10, 25, 25, 50 };
    // This is to indicate a number outside of rewards
    int reward = -1;
    // Initially player gets 3 keys if he enterred a chestroom
    int keys = 3;
    int openedChests = 0;
    //Rewards are being added here
    int totalReward = 0;

    // Balls that the player has not unlocked yet
    List<int> bestPrizes = new List<int>();
    // Index of the ball from the list of balls the player has not unlocked yet to offer as the best prize
    int bestPrizeIndex;

    void Awake()
    {
        key1animator = key1.GetComponent<Animator>();
        key2animator = key2.GetComponent<Animator>();
        key3animator = key3.GetComponent<Animator>();

        navigator = FindObjectOfType<Navigator>();

        exitButton = GameObject.Find("ExitButton");
        moreKeysButton = GameObject.Find("MoreKeysButton");
        passPhraseButton = GameObject.Find("PassPhrase");

        adCancelBg = GameObject.Find("AdCancelBg");
        adCancelWarning = GameObject.Find("ChestAdCancelWarning");
        adWarningReceiveButton = GameObject.Find("AdWarningReceiveButton");
        adWarningContinueButton = GameObject.Find("AdWarningContinueButton");
    }

    void Start()
    {
        moreKeysButton.SetActive(false);
        passPhraseButton.SetActive(false);

        // Adjust camera zoom for different ratio screens to put the best prize in correct position
        if ((float)Screen.height / Screen.width > 2)
        {
            cameraHeightFactor = 1.52f;
        }
        Camera.main.orthographicSize = Screen.height / 6;
        Camera.main.transform.position = new Vector2(Screen.width / 2, Screen.height / cameraHeightFactor);

        player.LoadPlayer();
        scoreboard.SetCoins(player.coins);
        DrawKeys();
        AdManager.ShowBanner();

        // Choose which of locked balls will be the best prize
        SetBestPrize();

        // Set ad stuff back to normal as they are shrinked in x axis for visibility by default
        adCancelBg.transform.localScale = new Vector3(1, 1, 1);
        adCancelWarning.transform.localScale = new Vector3(1, 1, 1);

        adCancelBg.SetActive(false);
        adCancelWarning.SetActive(false);
    }

    // Show no thanks button after all the keys are used
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

    // Based on which key is being used to open a chest make it animate
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
        // Loop through all unlocked keys and find the ones that are locked
        // -3 from Length is implemented in order to remove last three balls from the bestPrize list
        for (int i = 0; i < player.unlockedBalls.Length - 3; i++)
        {
            if (player.unlockedBalls[i] == 0)
            {
                bestPrizes.Add(i);
            }
        }

        // Get a random number in the range of locked keys and choose the best prize
        bestPrizeIndex = new System.Random().Next(0, bestPrizes.Count);
        GameObject bestPrizeObject = Instantiate(
            bestPrizesPrefabs[bestPrizes[bestPrizeIndex]],
            bestPrize.position,
            Quaternion.identity);

        // Assign that ball to the best prize object to show above chests
        bestPrizeObject.transform.parent = bestPrize;
    }

    public Sprite GetBestPrizeSprite()
    {
        return bestPrizesPrefabs[bestPrizes[bestPrizeIndex]].GetComponent<SpriteRenderer>().sprite;
    }

    public void NextLevel()
    {
        // Save all collected coins and best prize if received and Load next level
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
        // When opening any chest, get a random index within all rewards and find the corresponsing reward
        System.Random rnd = new System.Random();

        reward = rewards[rnd.Next(rewards.Count)]; // Extract reward

        // 0 reward represents the best prize
        if (reward == 0)
        {
            bestPrizeReceived = true;
        }

        // Remove that reward from all the rewards list not to repeat the same reward twice
        rewards.Remove(reward); // Remove reward
        // Remove the key as it has been used
        keys--;
        // Add the reward to the total rewards
        totalReward += reward;
        // Increase a count of opened chests, not to exceed 3 in total
        openedChests++;
        // If all the ads are being watched and all the chests are being opened
        if (openedChests == 9)
        {
            // Stop all the key animations, as there are no more keys and no more chests, and load the next level
            StopAllKeyAnimations();
            StartCoroutine(LoadNextLevel());
        }
        DrawKeys();

        // If opened chests are 3 or 6 make an ad button available for more keys
        if (openedChests % 3 == 0 && openedChests != 9)
        {
            adsButton.SetActive(true);
            StartCoroutine(OpenPassPhrase());
        }

        // Return a reward value received from the current chest
        return reward;
    }

    private void DrawKeys()
    {
        // Draw the keys that are used in black the rest in yellow color
        key1.GetComponent<Image>().color = new Color32(0, 0, 0, 255);
        key2.GetComponent<Image>().color = new Color32(0, 0, 0, 255);
        key3.GetComponent<Image>().color = new Color32(0, 0, 0, 255);

        if (keys > 0)
            key1.GetComponent<Image>().color = new Color32(255, 240, 0, 255);
        if (keys > 1)
            key2.GetComponent<Image>().color = new Color32(255, 240, 0, 255);
        if (keys > 2)
            key3.GetComponent<Image>().color = new Color32(255, 240, 0, 255);

        // Animate the next key to be used
        SetKeyAnimation();
    }

    public int GetKeys()
    {
        return keys;
    }

    public void ClickPassPhrase()
    {
        // Run the animation of a click of get more keys button and run the ad
        if (passPhraseButton.GetComponent<Button>().IsInteractable())
        {
            passPhraseButton.GetComponent<TriggerButton>().ClickButton(0.2f);
            StartCoroutine(LoadPassPhraseCoroutine(0.2f));
        }
    }

    public IEnumerator LoadPassPhraseCoroutine(float time)
    {
        // Wait for given time and load the ad
        yield return new WaitForSeconds(time);

        NextLevel();
    }

    public void GetMoreKeys()
    {
        // Run the animation of a click of get more keys button and run the ad
        if (moreKeysButton.GetComponent<Button>().IsInteractable())
        {
            moreKeysButton.GetComponent<TriggerButton>().ClickButton(0.2f);
            StartCoroutine(LoadGetMoreKeysCoroutine(0.2f));
        }
    }

    public IEnumerator LoadGetMoreKeysCoroutine(float time)
    {
        // Wait for given time and load the ad
        yield return new WaitForSeconds(time);
        AdManager.ShowStandardAd(GetMoreKeysSuccess, GetKeysCancel, CloseChest);
    }

    private void GetMoreKeysSuccess()
    {
        // If the ad has been watched till the end, Hide no thanks phrase and get 3 more keys button
        adsButton.SetActive(false);
        passPhrase.SetActive(false);
        // Reset the keys amount ot 3 and draw them again with animation on the first one
        keys = 3;
        DrawKeys();

        // Incase the success funcion is called from the rewathing the ad after skipping once,
        // Hide the warning buttons and toggle the variable indicating that the warning has been seen
        showedAdCancelWarning = true;
        adCancelBg.SetActive(false);
        adCancelWarning.SetActive(false);
    }

    public void ClickExitButton()
    {
        if (exitButton.GetComponent<Button>().IsInteractable())
        {
            // Run the trigger button animation and disable button for its duration
            exitButton.GetComponent<TriggerButton>().ClickButton(0.2f);
            // Approximately when animation is over, load level scene
            StartCoroutine(LoadCloseChest(0.2f));
        }
    }

    public IEnumerator LoadCloseChest(float time)
    {
        yield return new WaitForSeconds(time);
        CloseChest();
    }

    public void CloseChest()
    {
        NextLevel();
    }

    // Button that shows watch the ad till the end and receive the gift in the warning of ad cancel
    public void ReceiveKeysButtonClick()
    {
        if (adWarningReceiveButton.GetComponent<Button>().IsInteractable())
        {
            // Run animation of clicking receive coins and watch the ad button
            adWarningReceiveButton.GetComponent<TriggerButton>().ClickButton(0.2f);
            // Approximately when animation is finished, load the ad screen
            StartCoroutine(ReceiveKeysButtonCoroutine(0.2f));
        }
    }

    public IEnumerator ReceiveKeysButtonCoroutine(float time)
    {
        yield return new WaitForSeconds(time);

        AdManager.ShowStandardAd(GetMoreKeysSuccess, GetKeysCancel, GetKeysFail);
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

        adCancelBg.SetActive(false);
        adCancelWarning.SetActive(false);
    }

    // Incase ad has been cancelled, show the warning screen
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

    // Incase ad failed for some network issues or whatever
    private void GetKeysFail()
    {
        showedAdCancelWarning = true;
        adCancelBg.SetActive(false);
        adCancelWarning.SetActive(false);
    }
}
