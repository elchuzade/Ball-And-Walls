using UnityEngine;
using UnityEngine.SceneManagement;


public class Navigator : MonoBehaviour
{
    public static Navigator instance;

    void Awake()
    {
        // Singleton
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadShop()
    {
        SceneManager.LoadScene("ShopScene");
    }

    public void LoadNextLevel(int nextLevelIndex)
    {
        SceneManager.LoadScene("Level-" + nextLevelIndex);
    }

    public void LoadChestRoom()
    {
        SceneManager.LoadScene("ChestScene");
    }

    public void LoadChallengeScene()
    {
        SceneManager.LoadScene("ChallengeScene");
    }

    public void LoadLeaderboardScene()
    {
        SceneManager.LoadScene("LeaderboardScene");
    }

    public void LoadMainScene()
    {
        SceneManager.LoadScene("MainScene");
    }
}
