using UnityEngine;
using UnityEngine.SceneManagement;


public class Navigator : MonoBehaviour
{
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
}
