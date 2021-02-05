using System.Collections;
using UnityEngine;

public class LoadStatus : MonoBehaviour
{
    Navigator navigator;
    Player player;
    Server server;

    void Awake()
    {
        server = FindObjectOfType<Server>();
        player = FindObjectOfType<Player>();
        navigator = FindObjectOfType<Navigator>();
    }

    void Start()
    {
        player.LoadPlayer();

        server.CreatePlayer();
    }

    // Create a new player
    public void CreatePlayerSuccess(string response)
    {
        Debug.Log(response);
        StartCoroutine(LoadGame());
    }

    // Player already exists or error while creating
    public void CreatePlayerError(string response)
    {
        Debug.Log(response);
        StartCoroutine(LoadGame());
    }

    private IEnumerator LoadGame()
    {
        yield return new WaitForSeconds(0);
        if (player.nextLevelIndex == 0)
        {
            PlayerPrefs.SetInt("Haptics", 1);
            player.ResetPlayer();
            navigator.LoadMainScene();
        }
        else
        {
            navigator.LoadMainScene();
        }
    }
}
