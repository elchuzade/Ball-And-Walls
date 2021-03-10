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
        navigator = FindObjectOfType<Navigator>();
    }

    void Start()
    {
        player = FindObjectOfType<Player>();
        player.ResetPlayer();
        player.LoadPlayer();

        if (!player.playerCreated)
        {
            server.CreatePlayer();
            StartCoroutine(LoadGame(2f));
        } else
        {
            StartCoroutine(LoadGame(0));
        }
    }

    // Create a new player
    public void CreatePlayerSuccess()
    {
        player.playerCreated = true;
        player.SavePlayer();
        StartCoroutine(LoadGame(0));
    }

    // Player already exists or error while creating
    public void CreatePlayerError()
    {
        StartCoroutine(LoadGame(0));
    }

    private IEnumerator LoadGame(float time)
    {
        yield return new WaitForSeconds(time);
        if (player.nextLevelIndex == 0 && !player.playerCreated)
        {
            PlayerPrefs.SetInt("Haptics", 1);
            PlayerPrefs.SetInt("Sounds", 1);
            player.ResetPlayer();
            navigator.LoadMainScene();
        }
        else
        {
            navigator.LoadMainScene();
        }
    }
}
