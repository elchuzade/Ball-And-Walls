using UnityEngine;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
    public List<string> unlockedBalls = new List<string>() { "default" };
    public string currentBall = "default";
    public int coins = 0;
    public int diamonds = 0;
    public int keys = 0;
    public int nextLevelIndex = 1;
    public bool nameModified = false;

    void Awake()
    {
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
        currentBall = "default";
        coins = 56789;
        keys = 0;
        diamonds = 987;
        nextLevelIndex = 1;
        nameModified = false;
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

        unlockedBalls = data.unlockedBalls;
        currentBall = data.currentBall;
        coins = data.coins;
        keys = data.keys;
        diamonds = data.diamonds;
        nextLevelIndex = data.nextLevelIndex;
        nameModified = data.nameModified;
    }
}
