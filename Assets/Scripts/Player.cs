using UnityEngine;

public class Player : MonoBehaviour
{
    public int[] unlockedBalls = { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    public int currentBallIndex = 0;
    public int coins = 0;
    public int keys = 0;
    public int nextLevelIndex = 1;

    public void SavePlayer()
    {
        SaveSystem.SavePlayer(this);
    }

    public void ResetPlayer()
    {
        unlockedBalls = new int[] { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        currentBallIndex = 0;
        coins = 0;
        keys = 0;
        nextLevelIndex = 1;
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
        currentBallIndex = data.currentBallIndex;
        coins = data.coins;
        keys = data.keys;
        nextLevelIndex = data.nextLevelIndex;
    }
}
