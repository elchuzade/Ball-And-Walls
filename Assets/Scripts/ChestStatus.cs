using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class ChestStatus : MonoBehaviour
{
    //private List<string> allBalls = new List<string>()
    //    { "abbox", "atom", "basketball", "beach", "blackhole", "bomb",
    //    "bowling", "burger", "button", "candy", "coin", "cookie",
    //    "darts", "default", "disco", "donut", "eye", "flower",
    //    "football", "gear", "hypnose", "yinyang", "meteor", "pokemon",
    //    "pool", "pumpkin", "radiation", "saturn", "smile", "snowball",
    //    "sun", "tennis", "virus", "volleyball", "watermelon", "wheel" };

    // Match this with the prefabs given to the BestPrizesPrefabs 23 balls
    private List<string> allBestPrizeBalls = new List<string>()
        { "basketball", "beach", "bomb", "bowling", "burger", "button",
        "coin", "cookie", "darts", "donut", "flower", "football",
        "gear", "yinyang", "pokemon", "pool", "pumpkin", "saturn",
        "smile", "snowball", "tennis", "volleyball", "watermelon" };

    Player player;
    [SerializeField] GameObject key1;
    [SerializeField] GameObject key2;
    [SerializeField] GameObject key3;
    [SerializeField] GameObject adsButton;
    [SerializeField] GameObject passPhrase;
    Scoreboard scoreboard;    
    
    [SerializeField] GameObject[] bestPrizesPrefabs;
    [SerializeField] Transform bestPrize;
    // To pass to ad cancel
    [SerializeField] Sprite keyIcon;

    // Zoom animations on keys
    Animator key1animator;
    Animator key2animator;
    Animator key3animator;

    // This is needed because we are mixing canvas and world game object
    float cameraHeightFactor = 1.44f;
    bool bestPrizeReceived = false;
    Navigator navigator;

    GameObject moreKeysButton;
    GameObject passPhraseButton;

    AdCancel adCancel;

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
    List<string> bestPrizes = new List<string>();

    // Name of the best prize ball
    string bestPrizeName;

    void Awake()
    {
        scoreboard = FindObjectOfType<Scoreboard>();
        navigator = FindObjectOfType<Navigator>();
        adCancel = FindObjectOfType<AdCancel>();

        key1animator = key1.GetComponent<Animator>();
        key2animator = key2.GetComponent<Animator>();
        key3animator = key3.GetComponent<Animator>();

        moreKeysButton = GameObject.Find("MoreKeysButton");
        passPhraseButton = GameObject.Find("PassPhrase");
    }

    void Start()
    {
        if ((float)Screen.width / Screen.height > 0.7)
        {
            GameObject canvas = GameObject.Find("Canvas");
            canvas.GetComponent<CanvasScaler>().matchWidthOrHeight = 1;
        }

        player = FindObjectOfType<Player>();
        player.LoadPlayer();
        moreKeysButton.SetActive(false);
        passPhraseButton.SetActive(false);

        // Adjust camera zoom for different ratio screens to put the best prize in correct position
        if ((float)Screen.height / Screen.width > 2)
        {
            cameraHeightFactor = 1.52f;
        }
        Camera.main.orthographicSize = Screen.height / 6;
        Camera.main.transform.position = new Vector2(Screen.width / 2, Screen.height / cameraHeightFactor);

        scoreboard.SetCoins(player.coins);
        scoreboard.SetDiamonds(player.diamonds);
        DrawKeys();
        AdManager.ShowBanner();

        // Choose which of locked balls will be the best prize
        SetBestPrize();

        adCancel.InitializeAdCancel(" keys", keyIcon);
        adCancel.GetReceiveButton().GetComponent<Button>().onClick.AddListener(() => ReceiveButtonClick());
        adCancel.GetCancelButton().GetComponent<Button>().onClick.AddListener(() => CancelButtonClick());
        adCancel.gameObject.SetActive(false);
    }

    // Show no thanks button after all the keys are used
    private IEnumerator OpenPassPhrase()
    {
        key1.GetComponent<Image>().color = new Color32(0, 0, 0, 0);
        key2.GetComponent<Image>().color = new Color32(0, 0, 0, 0);
        key3.GetComponent<Image>().color = new Color32(0, 0, 0, 0);

        yield return new WaitForSeconds(1);

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
        // Check from all possible best prizes for the ones that are not unlocked by the player
        for (int i = 0; i < allBestPrizeBalls.Count; i++)
        {
            if (!player.unlockedBalls.Contains(allBestPrizeBalls[i]))
            {
                bestPrizes.Add(allBestPrizeBalls[i]);
            }
        }

        // Get a random number in the range of locked keys and choose the best prize
        int bestPrizeIndex = new System.Random().Next(0, allBestPrizeBalls.Count);
        Debug.Log(bestPrizeIndex);
        bestPrizeName = bestPrizes[bestPrizeIndex];

        for (int i = 0; i < allBestPrizeBalls.Count; i++)
        {
            if (bestPrizesPrefabs[i].GetComponent<SpriteRenderer>().sprite.name == bestPrizeName)
            {
                GameObject bestPrizeObject = Instantiate(
                    bestPrizesPrefabs[i], bestPrize.position, Quaternion.identity);

                // Assign that ball to the best prize object to show above chests
                bestPrizeObject.transform.SetParent(bestPrize.transform);
            }
        }
    }

    public Sprite GetBestPrizeSprite()
    {
        for (int i = 0; i < allBestPrizeBalls.Count; i++)
        {
            if (bestPrizesPrefabs[i].GetComponent<SpriteRenderer>().sprite.name == bestPrizeName)
            {
                return bestPrizesPrefabs[i].GetComponent<SpriteRenderer>().sprite;
            }
        }
        return null;
    }

    public void NextLevel()
    {
        // Save all collected coins and best prize if received and Load next level
        player.coins += totalReward;
        scoreboard.SetCoins(player.coins);
        totalReward = 0;

        if (bestPrizeReceived)
        {
            player.unlockedBalls.Add(bestPrizeName);
            player.currentBall = bestPrizeName;
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
        CloseChest();
    }

    public void GetMoreKeys()
    {
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
        adCancel.gameObject.SetActive(false);
        bestPrize.gameObject.SetActive(true);
    }

    public void ClickExitButton()
    {
        CloseChest();
    }

    public void CloseChest()
    {
        NextLevel();
    }

    // Button that shows watch the ad till the end and receive the gift in the warning of ad cancel
    public void ReceiveButtonClick()
    {
        AdManager.ShowStandardAd(GetMoreKeysSuccess, GetKeysCancel, GetKeysFail);
    }

    public void CancelButtonClick()
    {
        adCancel.gameObject.SetActive(false);
    }

    // Incase ad has been cancelled, show the warning screen
    private void GetKeysCancel()
    {
        if (!showedAdCancelWarning)
        {
            adCancel.gameObject.SetActive(true);
            // Hide ball
            bestPrize.gameObject.SetActive(false);
        }
        else
        {
            adCancel.gameObject.SetActive(false);
            bestPrize.gameObject.SetActive(true);
        }

        showedAdCancelWarning = true;
    }

    // Incase ad failed for some network issues or whatever
    private void GetKeysFail()
    {
        showedAdCancelWarning = true;
        adCancel.gameObject.SetActive(false);
        bestPrize.gameObject.SetActive(true);
    }
}
