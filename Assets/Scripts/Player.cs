using UnityEngine;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
    public List<string> unlockedBalls = new List<string>() { "default" };
    public List<int> unlockedChallenges = new List<int>() { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };
    public string currentBall = "default";
    public int coins = 0;
    public int diamonds = 0;
    public int keys = 0;
    public int lives = 0;
    public int nextLevelIndex = 1;
    public string playerName = "";
    public int selectedChallenge = -1;
    public bool nameChanged = false;
    public bool playerCreated = false;

    void Awake()
    {
        transform.SetParent(transform.parent.parent);
        // Singleton
        int instances = FindObjectsOfType<Player>().Length;
        if (instances > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    public void SavePlayer()
    {
        SaveSystem.SavePlayer(this);
    }

    public void ResetPlayer()
    {
        unlockedBalls = new List<string>() { "default" };
        unlockedChallenges = new List<int>() { -2, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 5, 5, 5, 5}; // - 2 solved, -1 locked
        currentBall = "default";
        coins = 44444;
        keys = 0;
        playerName = "";
        diamonds = 555;
        nextLevelIndex = 1;
        lives = 0;
        playerName = "";
        selectedChallenge = 4;
        playerCreated = false;
        nameChanged = false;
        SaveSystem.SavePlayer(this);
    }

    public void LoadPlayer()
    {
        PlayerData data = SaveSystem.LoadPlayer();
        if (data == null)
        {
            ResetPlayer();
            data = SaveSystem.LoadPlayer();
        }

        playerCreated = data.playerCreated;
        playerName = data.playerName;
        unlockedBalls = data.unlockedBalls;
        unlockedChallenges = data.unlockedChallenges;
        currentBall = data.currentBall;
        coins = data.coins;
        keys = data.keys;
        lives = data.lives;
        diamonds = data.diamonds;
        selectedChallenge = data.selectedChallenge;
        nameChanged = data.nameChanged;
        nextLevelIndex = data.nextLevelIndex;
    }
}
