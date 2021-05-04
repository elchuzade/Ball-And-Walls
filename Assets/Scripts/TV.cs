using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class TV : MonoBehaviour
{
    Server server;
    Player player;
    private VideoPlayer videoPlayer;

    string id;

    void Awake()
    {
        videoPlayer = transform.Find("VideoPlayer").GetComponent<VideoPlayer>();
        server = FindObjectOfType<Server>();
        player = FindObjectOfType<Player>();
    }

    private void VideoClick(string url)
    {
        server.SendVideoClick(player.privacyPolicyAccepted, id, url);
        Application.OpenURL(url);
    }

    public void SetAdLink(string url)
    {
        videoPlayer.url = url;
        videoPlayer.Play();
    }

    public void SetAdButton(string url)
    {
        GetComponent<Button>().onClick.AddListener(() => VideoClick(url));
    }

    public void SetAdId(string _id)
    {
        id = _id;
    }
}
