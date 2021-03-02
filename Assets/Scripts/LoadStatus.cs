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

        player.LoadPlayer();

        if (!player.playerCreated)
        {
            Debug.Log("creating");
            server.CreatePlayer();
        } else
        {
            StartCoroutine(LoadGame());
        }
    }

    // Create a new player
    public void CreatePlayerSuccess()
    {
        player.playerCreated = true;
        player.SavePlayer();
        StartCoroutine(LoadGame());
    }

    // Player already exists or error while creating
    public void CreatePlayerError()
    {
        StartCoroutine(LoadGame());
    }

    private IEnumerator LoadGame()
    {
        yield return new WaitForSeconds(0);
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
