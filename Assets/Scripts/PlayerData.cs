using System;

[Serializable]
public class PlayerData
{
    public int[] unlockedBalls = { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    public int currentBallIndex = 0;
    public int coins = 0;
    public int keys = 0;
    public int diamonds = 0;
    public int nextLevelIndex = 1;

    public PlayerData (Player player)
    {
        unlockedBalls = player.unlockedBalls;
        currentBallIndex = player.currentBallIndex;
        coins = player.coins;
        keys = player.keys;
        diamonds = player.diamonds;
        nextLevelIndex = player.nextLevelIndex;
    }
}
