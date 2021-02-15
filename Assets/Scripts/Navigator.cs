using UnityEngine;
using UnityEngine.SceneManagement;


public class Navigator : MonoBehaviour
{
    void Awake()
    {
        // Singleton
        int instances = FindObjectsOfType<Navigator>().Length;
        if (instances > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
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

    public void LoadChallengeLevel()
    {
        SceneManager.LoadScene("ChallengeLevel");
    }

    public void LoadChallengesScene()
    {
        SceneManager.LoadScene("ChallengesScene");
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
