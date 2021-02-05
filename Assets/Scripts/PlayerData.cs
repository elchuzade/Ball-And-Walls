using System;
using System.Collections.Generic;

[Serializable]
public class PlayerData
{
    public List<string> unlockedBalls = new List<string>() { "default" };
    public string currentBall = "default";
    public int coins = 0;
    public int keys = 0;
    public int diamonds = 0;
    public int nextLevelIndex = 1;
    public bool nameModified = false;

    public PlayerData (Player player)
    {
        unlockedBalls = player.unlockedBalls;
        currentBall = player.currentBall;
        coins = player.coins;
        keys = player.keys;
        diamonds = player.diamonds;
        nextLevelIndex = player.nextLevelIndex;
        nameModified = player.nameModified;
    }
}
