using UnityEngine;
using UnityEngine.UI;

public class AdCancel : MonoBehaviour
{
    // Continue watching and receive a reward button when player wanted to skip the video
    [SerializeField] GameObject receiveButton;
    // Continue watching and receive a reward button's text when player wanted to skip the video
    [SerializeField] GameObject receiveButtonText;
    // Reject reward and continue playing game button when player wanted to skip the video
    [SerializeField] GameObject cancelButton;
    // Background of window that pops up if player wants to switch off reward ad
    [SerializeField] GameObject background;
    // Window with buttons that pops up if player wants to switch off reward ad
    [SerializeField] GameObject warning;
    // Repreents what is being offered for the ad
    [SerializeField] GameObject text;
    // Represents the icon of offer
    [SerializeField] GameObject icon;

    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = new Vector3(1, 1, 1);
    }

    public void InitializeAdCancel(string _text, Sprite _icon)
    {
        text.GetComponent<Text>().text += _text;
        icon.GetComponent<Image>().sprite = _icon;
        receiveButtonText.GetComponent<Text>().text += _text;
    }

    // To assign a function on click of the button
    public GameObject GetReceiveButton()
    {
        return receiveButton;
    }

    // To assign a function on click of the button
    public GameObject GetCancelButton()
    {
        return cancelButton;
    }
}
