using System;
using System.Collections.Generic;

[Serializable]
public class PlayerData
{
    public List<string> unlockedBalls = new List<string>() { "default" };
    public List<int> unlockedChallenges = new List<int>() { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };
    public string currentBall = "default";
    public int coins = 0;
    public int keys = 0;
    public int lives = 0;
    public int diamonds = 0;
    public int nextLevelIndex = 1;
    public string playerName = "";
    public bool playerCreated = false;
    public bool nameChanged = false;
    public bool privacyPolicyAccepted = false;
    public bool privacyPolicyDeclined = false;

    public PlayerData (Player player)
    {
        unlockedBalls = player.unlockedBalls;
        unlockedChallenges = player.unlockedChallenges;
        currentBall = player.currentBall;
        coins = player.coins;
        keys = player.keys;
        lives = player.lives;
        diamonds = player.diamonds;
        nextLevelIndex = player.nextLevelIndex;
        playerName = player.playerName;
        playerCreated = player.playerCreated;
        nameChanged = player.nameChanged;
        privacyPolicyAccepted = player.privacyPolicyAccepted;
        privacyPolicyDeclined = player.privacyPolicyDeclined;
    }
}
