using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MoreMountains.NiceVibrations;

public class BallCatcher : MonoBehaviour
{
    private string[] winWords = { "Congrats!", "Excellent!", "Good Job!", "Perfect!", "Amazing!", "Superb!", "Great!" };

    Navigator navigator;
    HomeStatus homeStatus;
    [SerializeField] Player player;
    [SerializeField] GameObject winParticlesPrefab;
    [SerializeField] GameObject winTextPrefab;
    [SerializeField] GameObject coinPrefab;

    GameObject canvas;

    TextMeshProUGUI winWordText;

    TriggerAnimation coinsIcon;

    private GameObject scoreboardCoinPrefab;
    private int dropCoinsAmount;
    private int dropCoinDistance = 25;
    private List<int> dropCoinAngles = new List<int>();
    private List<GameObject> dropCoins = new List<GameObject>();
    private bool coinsDropped = false;
    private bool moveDroppedCoins = false;
    private float margin = 0.1f;
    private bool triggerAnimTriggered = false;

    private int collectedCoins = 0;

    private void Start()
    {
        scoreboardCoinPrefab = GameObject.Find("TotalCoinsIcon");
        coinsIcon = scoreboardCoinPrefab.GetComponent<TriggerAnimation>();
        dropCoinsAmount = Random.Range(5, 11); // from 5 to 10 coins
        canvas = GameObject.Find("Canvas");
        homeStatus = FindObjectOfType<HomeStatus>();
        navigator = FindObjectOfType<Navigator>();
    }

    private void FixedUpdate()
    {
        if (coinsDropped)
        {
            MoveCoinsToScoreboard();
        } else if (moveDroppedCoins)
        {
            MoveDroppedCoins();
        }
    }

    private void MoveDroppedCoins()
    {
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

    private void MoveCoinsToScoreboard()
    {
        for (int i = 0; i < dropCoinsAmount; i++)
        {
            dropCoins[i].transform.position = Vector2.MoveTowards(
                dropCoins[i].transform.position,
                scoreboardCoinPrefab.transform.position,
                1500 * Time.deltaTime);

            if (Vector3.Distance(
                dropCoins[i].transform.position,
                scoreboardCoinPrefab.transform.position) < margin)
            {
                dropCoins[i].SetActive(false);
                if (collectedCoins < dropCoinsAmount)
                {
                    if (collectedCoins < dropCoinsAmount)
                    {
                        homeStatus.CollectCoin();
                        collectedCoins++;
                        if (!triggerAnimTriggered)
                        {
                            coinsIcon.Trigger();
                            triggerAnimTriggered = true;
                        }
                    }
                }
            }
        }
    }

    private void MakeDropCoinAngles()
    {
        moveDroppedCoins = true;
        StartCoroutine(CoinsDropped());
        for (int i = 0; i < dropCoinsAmount; i++)
        {
            int angle = Random.Range(0, 360 / dropCoinsAmount);
            dropCoinAngles.Add(angle + i * 360 / dropCoinsAmount);

            GameObject coin = Instantiate(
                coinPrefab,
                transform.position,
                Quaternion.identity);

            coin.transform.parent = transform;
            coin.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);

            dropCoins.Add(coin);
        }
    }

    private IEnumerator CoinsDropped()
    {
        yield return new WaitForSeconds(1);
        coinsDropped = true;
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ball")
        {
            if (PlayerPrefs.GetInt("Haptics") == 1)
            {
                MMVibrationManager.Haptic(HapticTypes.Success);
            }

            if ((player.nextLevelIndex + 1) % 5 == 0 && player.nextLevelIndex != 99)
            {
                AdManager.ShowStandardAd(() => { }, () => { }, () => { });
            }

            MakeDropCoinAngles();
            GetComponent<SpriteRenderer>().enabled = false;

            GameObject winEffect = Instantiate(
                winParticlesPrefab,
                new Vector2(Screen.width / 2, Screen.height / 2),
                Quaternion.identity);

            Destroy(winEffect, 1.5f);

            GameObject winText = Instantiate(
                winTextPrefab,
                new Vector2(Screen.width / 2, Screen.height / 2),
                Quaternion.identity);

            winText.transform.SetParent(canvas.transform);
            
            winText.SetActive(true);

            string winWord = winWords[Random.Range(0, winWords.Length)];

            winWordText = winText.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

            winWordText.text = winWord;

            player.coins += homeStatus.GetCoins();
            if (player.keys < homeStatus.GetKeys())
            {
                player.keys += homeStatus.GetKeys() - player.keys;
            }

            player.nextLevelIndex++;
            Destroy(collision.gameObject);

            StartCoroutine(LoadNextScreen());
        }
    }

    private IEnumerator LoadNextScreen()
    {
        yield return new WaitForSeconds(3f);
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
