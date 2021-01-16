using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class IconButton : MonoBehaviour
{
    private bool buttonDisabled;

    private TriggerAnimation buttonIconAnimation;

    void Awake()
    {
        buttonIconAnimation = GetComponent<TriggerAnimation>();
    }

    // Run this function when button is just clicked
    public void ClickButton()
    {
        // This is to make sure people do not double click button while in animation
        GetComponent<Button>().interactable = false;

        // Start click animation
        buttonIconAnimation.Trigger();
    }

    // Run this function when click animation is over
    public void ClickButtonComplete()
    {
        if (buttonDisabled)
        {
            // Disable button by adding cross line
            // When disabling button we first run animation then remove cross line
            // Toggle button
            buttonDisabled = !buttonDisabled;
        }

        // Make button clickable after half a second
        StartCoroutine(ActivateButton(0.5f));
    }

    private IEnumerator ActivateButton(float time)
    {
        yield return new WaitForSeconds(time);

        // This is to make sure people do not double click button while in animation
        GetComponent<Button>().interactable = true;
    }
}
