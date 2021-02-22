using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MoreMountains.NiceVibrations;

public class BallCatcher : MonoBehaviour
{
    [SerializeField] bool challengeLevel;

    // List of words to show when level is passed
    string[] winWords = { "Congrats!", "Excellent!", "Good Job!", "Perfect!", "Amazing!", "Superb!", "Great!", "Wonderful!", "Brilliant!" };

    Navigator navigator;
    HomeStatus homeStatus;

    Player player;
    [SerializeField] GameObject winParticlesPrefab;
    [SerializeField] GameObject winTextPrefab;
    // To drop when level is passed
    [SerializeField] GameObject coinPrefab;
    // To drop when level is passed for challenge level
    [SerializeField] GameObject diamondPrefab;

    // To set text to it as a child
    GameObject canvas;

    TextMeshProUGUI winWordText;

    // Animation on scoreboard coin zooming when coins go to it
    TriggerAnimation coinsIcon;

    // Animation on scoreboard diamond zooming when diamonds go to it
    TriggerAnimation diamondsIcon;

    GameObject scoreboardCoinPrefab;
    GameObject scoreboardDiamondPrefab;

    // Number of coins dropped
    int dropCoinsAmount;
    // Distance to which every coin may travel after being dropped
    int dropCoinDistance = 25;

    // Number of diamonds dropped
    int dropDiamondsAmount;
    // Distance to which every coin may travel after being dropped
    int dropDiamondDistance = 10;

    // Angles list for all coins dropped
    List<int> dropCoinAngles = new List<int>();
    // List of game objcts of coins that were dropped
    List<GameObject> dropCoins = new List<GameObject>();

    // Angles list for all coins dropped
    List<int> dropDiamondAngles = new List<int>();
    // List of game objcts of coins that were dropped
    List<GameObject> dropDiamonds = new List<GameObject>();

    // Status of coins starting to move when they have reached their drop location
    bool coinsDropped = false;
    // Status of coins being dropped when level is passed
    bool moveDroppedCoins = false;

    // Status of coins starting to move when they have reached their drop location
    bool diamondsDropped = false;
    // Status of coins being dropped when level is passed
    bool moveDroppedDiamonds = false;

    // Tolerance issues
    float margin = 0.1f;
    // Status of coins animation is triggered, not to keep zooming for each coin reaching coins icon
    bool triggerAnimTriggered = false;
    // Number of collected coins to add to player data
    int collectedCoins = 0;
    int collectedDiamonds = 0;

    void Awake()
    {
        homeStatus = FindObjectOfType<HomeStatus>();
        navigator = FindObjectOfType<Navigator>();

        scoreboardCoinPrefab = GameObject.Find("TotalCoinsIcon");
        scoreboardDiamondPrefab = GameObject.Find("TotalDiamondsIcon");
        canvas = GameObject.Find("Canvas");

        // Animate scoreboard coin zooming when coins go to it
        coinsIcon = scoreboardCoinPrefab.GetComponent<TriggerAnimation>();
        // Animate scoreboard coin zooming when coins go to it
        diamondsIcon = scoreboardDiamondPrefab.GetComponent<TriggerAnimation>();
    }

    void Start()
    {
        player = FindObjectOfType<Player>();

        player.LoadPlayer();
        // Assing random number to amount of coins to drop from 7 to 13
        dropCoinsAmount = Random.Range(7, 14);
        // Assing random number to amount of coins to drop from 7 to 13
        dropDiamondsAmount = Random.Range(7, 14);
    }

    void FixedUpdate()
    {
        // If the coins have reached their destination move them to canvas
        if (coinsDropped)
        {
            MoveCoinsToScoreboard();
        }
        // Else move the coins to drop location
        else if (moveDroppedCoins)
        {
            MoveDroppedCoins();
        }

        if (challengeLevel)
        {
            // If the diamonds have reached their destination move them to canvas
            if (diamondsDropped)
            {
                MoveDiamondsToScoreboard();
            }
            // Else move the coins to drop location
            else if (moveDroppedDiamonds)
            {
                MoveDroppedDiamonds();
            }
        }
    }

    private void MoveDroppedCoins()
    {
        // For all drop coins amount move every coin to its drop location
        for (int i = 0; i < dropCoinsAmount; i++)
        {
            Vector3 finalPosition = new Vector2(
                transform.position.x + (Mathf.Cos(dropCoinAngles[i]) * dropCoinDistance),
                transform.position.y + (Mathf.Sin(dropCoinAngles[i]) * dropCoinDistance));

            dropCoins[i].transform.position = Vector2.MoveTowards(
                dropCoins[i].transform.position,
                finalPosition,
                200 * Time.deltaTime);
        }
    }

    private void MoveDroppedDiamonds()
    {
        // For all drop diamonds amount move every diamond to its drop location
        for (int i = 0; i < dropDiamondsAmount; i++)
        {
            Vector3 finalPosition = new Vector2(
                transform.position.x + (Mathf.Cos(dropDiamondAngles[i]) * dropDiamondDistance),
                transform.position.y + (Mathf.Sin(dropDiamondAngles[i]) * dropDiamondDistance));

            dropDiamonds[i].transform.position = Vector2.MoveTowards(
                dropDiamonds[i].transform.position,
                finalPosition,
                200 * Time.deltaTime);
        }
    }

    private void MoveCoinsToScoreboard()
    {
        // For all drop coins amount move every coin to canvas location of coins icon
        for (int i = 0; i < dropCoinsAmount; i++)
        {
            dropCoins[i].transform.position = Vector2.MoveTowards(
                dropCoins[i].transform.position,
                scoreboardCoinPrefab.transform.position,
                1500 * Time.deltaTime);

            // If the coin game object is close enough to the coins icon
            if (Vector3.Distance(
                dropCoins[i].transform.position,
                scoreboardCoinPrefab.transform.position) < margin)
            {
                // Hide the coin
                dropCoins[i].SetActive(false);
                // The coin is not counted yet into player data, collect it
                if (collectedCoins < dropCoinsAmount)
                {
                    homeStatus.CollectCoin();
                    collectedCoins++;
                    // If zoom animation of coins icon has not been run yet,
                    // run it and set its status to run not to repeat it
                    if (!triggerAnimTriggered)
                    {
                        coinsIcon.Trigger();
                        triggerAnimTriggered = true;
                    }
                }
            }
        }
    }

    private void MoveDiamondsToScoreboard()
    {
        // For all drop diamonds amount move every diamond to canvas location of diamonds icon
        for (int i = 0; i < dropDiamondsAmount; i++)
        {
            dropDiamonds[i].transform.position = Vector2.MoveTowards(
                dropDiamonds[i].transform.position,
                scoreboardDiamondPrefab.transform.position,
                1500 * Time.deltaTime);

            // If the diamond game object is close enough to the diamonds icon
            if (Vector3.Distance(
                dropDiamonds[i].transform.position,
                scoreboardDiamondPrefab.transform.position) < margin)
            {
                // Hide the diamond
                dropDiamonds[i].SetActive(false);
                // The diamond is not counted yet into player data, collect it
                if (collectedDiamonds < dropDiamondsAmount)
                {
                    homeStatus.CollectDiamond();
                    collectedDiamonds++;
                    // If zoom animation of diamonds icon has not been run yet,
                    // run it and set its status to run not to repeat it
                    if (!triggerAnimTriggered)
                    {
                        diamondsIcon.Trigger();
                        triggerAnimTriggered = true;
                    }
                }
            }
        }
    }

    private void MakeDropCoinAngles()
    {
        // It is to indecate that coins are dropping towards their destination
        moveDroppedCoins = true;
        // It takes less than a second to reach their destination
        // So after 1 second move them towards canvas coins icon
        StartCoroutine(CoinsDropped(1));
        // For every coins to be dropped
        for (int i = 0; i < dropCoinsAmount; i++)
        {
            // Make arandom angle 
            int angle = Random.Range(0, 360 / dropCoinsAmount);
            // Add it to the drop angles list
            dropCoinAngles.Add(angle + i * 360 / dropCoinsAmount);

            // Make a coin instance
            GameObject coin = Instantiate(
                coinPrefab,
                transform.position,
                Quaternion.identity);

            // Set its parent to Bacll Catcher not to clutter the game tree
            coin.transform.parent = transform;
            // Shrink coin to not look too big compared to the ball and ball catcher
            coin.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            // Add the game object to the coins list to proceed further
            dropCoins.Add(coin);
        }
    }

    private void MakeDropDiamondAngles()
    {
        // It is to indecate that coins are dropping towards their destination
        moveDroppedDiamonds = true;
        // It takes less than a second to reach their destination
        // So after 1 second move them towards canvas coins icon
        StartCoroutine(DiamondsDropped(1));
        // For every coins to be dropped
        for (int i = 0; i < dropDiamondsAmount; i++)
        {
            // Make arandom angle 
            int angle = Random.Range(0, 360 / dropDiamondsAmount);
            // Add it to the drop angles list
            dropDiamondAngles.Add(angle + i * 360 / dropDiamondsAmount);

            // Make a coin instance
            GameObject diamond = Instantiate(
                diamondPrefab,
                transform.position,
                Quaternion.identity);

            // Set its parent to Bacll Catcher not to clutter the game tree
            diamond.transform.parent = transform;
            // Shrink coin to not look too big compared to the ball and ball catcher
            diamond.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            // Add the game object to the coins list to proceed further
            dropDiamonds.Add(diamond);
        }
    }

    // Access from challenge status
    public void SetDiamondsCoins(int diamonds, int coins)
    {
        dropCoinsAmount = coins;
        dropDiamondsAmount = diamonds;
    }

    private IEnumerator CoinsDropped(int time)
    {
        yield return new WaitForSeconds(time);
        // Change coins dropped status to move coins to canvas coins icon
        coinsDropped = true;
    }

    private IEnumerator DiamondsDropped(int time)
    {
        yield return new WaitForSeconds(time);
        // Change coins dropped status to move coins to canvas coins icon
        diamondsDropped = true;
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        // If the ball catcher has hit the ball, level should be passed
        if (collision.gameObject.tag == "Ball")
        {
            if (challengeLevel)
            {
                Server server = FindObjectOfType<Server>();
                server.ChangeChallengeStatus(PlayerPrefs.GetString("challengeId"), "solved");
            }

            // Disable all buttons from canvas so after the level is passed buttons do not get clicked during win animation
            homeStatus.DisableAllButtons();

            if (PlayerPrefs.GetInt("Haptics") == 1)
            {
                MMVibrationManager.Haptic(HapticTypes.Success);
            }

            // Show an ad before every 5th level except the 100th one which is the last level
            if (!challengeLevel && (player.nextLevelIndex) % 5 == 0 && player.nextLevelIndex != 99)
            {
                AdManager.ShowStandardAd(() => { }, () => { }, () => { });
            }

            if (PlayerPrefs.GetInt("Sounds") == 1)
            {
                AudioSource audio = GetComponent<AudioSource>();
                audio.Play();
            }

            // Make angles for all coins that will drop and then drop them
            MakeDropCoinAngles();

            if (challengeLevel)
            {
                // Make angles for all diamonds that will drop and then drop them
                MakeDropDiamondAngles();
            }

            // Hide ball catcher image to not clutter when coins are dropping
            GetComponent<SpriteRenderer>().enabled = false;

            // Make particle effects for passing the level and destroy them after 1.5 seconds
            GameObject winEffect = Instantiate(
                winParticlesPrefab,
                new Vector2(Screen.width / 2, Screen.height / 2),
                Quaternion.identity);

            Destroy(winEffect, 1.5f);

            // Make a win text to congratulate the player with randomly selected text from all thext list
            GameObject winText = Instantiate(
                winTextPrefab,
                new Vector2(Screen.width / 2, Screen.height / 2),
                Quaternion.identity);

            // Move the text instance into canvas
            winText.transform.SetParent(canvas.transform);
            winText.SetActive(true);

            // Find a random word from win text list
            string winWord = winWords[Random.Range(0, winWords.Length)];

            // Assign to the first child of winText object
            winWordText = winText.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            winWordText.text = winWord;

            // Add coins that were collected from this level
            player.coins += homeStatus.GetCoins();

            if (challengeLevel)
            {
                // Add diamonds that were collected from this level
                player.diamonds += homeStatus.GetDiamonds();
            }

            // Check if in this level player has collected any keys, add them if so
            if (player.keys < homeStatus.GetKeys())
            {
                player.keys += homeStatus.GetKeys() - player.keys;
            }

            // Destroy the ball
            Destroy(collision.gameObject);

            StartCoroutine(LoadNextScreen(3));
        }
    }

    private IEnumerator LoadNextScreen(int time)
    {
        yield return new WaitForSeconds(time);

        if (challengeLevel)
        {
            player.coins += homeStatus.GetCoins();
            player.diamonds += homeStatus.GetDiamonds();
            player.SavePlayer();
            navigator.LoadChallengesScene();
        } else
        {
            // Update the next level for the player
            player.nextLevelIndex++;

            // Wait for some time then update player data and load either next level or chest room
            if (player.keys == 3)
            {
                player.keys = 0;
                player.SavePlayer();
                navigator.LoadChestRoom();
            }
            else
            {
                player.coins += homeStatus.GetCoins();
                player.SavePlayer();
                navigator.LoadNextLevel(player.nextLevelIndex);
            }
        }
    }
}
