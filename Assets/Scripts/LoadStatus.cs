using System.Collections;
using UnityEngine;

public class LoadStatus : MonoBehaviour
{
    Navigator navigator;
    Player player;

    void Start()
    {
        player = FindObjectOfType<Player>();
        navigator = FindObjectOfType<Navigator>();
        player.LoadPlayer();
        StartCoroutine(LoadGame());
    }

    private IEnumerator LoadGame()
    {
        yield return new WaitForSeconds(0);
        if (player.nextLevelIndex == 0)
        {
            PlayerPrefs.SetInt("Haptics", 1);
            player.ResetPlayer();
            navigator.LoadNextLevel(1);
        }
        else
        {
            navigator.LoadNextLevel(player.nextLevelIndex);
        }
    }
}
