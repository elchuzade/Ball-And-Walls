using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class TV : MonoBehaviour
{
    private VideoPlayer videoPlayer;

    void Awake()
    {
        videoPlayer = transform.Find("VideoPlayer").GetComponent<VideoPlayer>();    
    }

    public void SetAdLink(string url)
    {
        videoPlayer.url = url;
        videoPlayer.Play();
    }

    public void SetAdButton(string url)
    {
        GetComponent<Button>().onClick.AddListener(() => Application.OpenURL(url));
    }
}
