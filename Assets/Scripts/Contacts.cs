using UnityEngine;

public class Contacts : MonoBehaviour
{
    public void ClickTelegram()
    {
        Application.OpenURL("https://telegram.com");
    }

    public void ClickDiscord()
    {
        Application.OpenURL("https://discord.com");
    }

    public void ClickWebsite()
    {
        Application.OpenURL("https://abbox.com");
    }
}
