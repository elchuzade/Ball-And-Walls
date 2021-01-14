using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum ButtonStates { Disable, Enable };

public class IconDisableButton : MonoBehaviour
{
    //private GameObject background;
    //private GameObject frame;
    //private GameObject icon;
    private GameObject disable;

    private bool buttonDisabled;

    private TriggerAnimation buttonIconDisabledAnimation;

    void Awake()
    {
        //background = transform.Find("Background").gameObject;
        //frame = transform.Find("Frame").gameObject;
        //icon = transform.Find("Icon").gameObject;
        disable = transform.Find("Disable").gameObject;

        buttonIconDisabledAnimation = GetComponent<TriggerAnimation>();
    }

    // Depending on player prefs set whther button is enabled or disabled
    public void SetButtonInitialState(ButtonStates buttonState)
    {
        if (buttonState == ButtonStates.Enable)
        {
            // Hide cross line and set script state to enabled
            buttonDisabled = false;
            disable.SetActive(false);
        } else {
            // Show cross line and set script state to disabled
            buttonDisabled = true;
            disable.SetActive(true);
        }
    }

    // This function will return true if the trigger animation is complete
    public bool GetButtonEnabled()
    {
        return !buttonDisabled;
    }

    // Run this function when button is just clicked
    public void ClickButton(ButtonStates buttonState)
    {
        if (buttonState == ButtonStates.Enable)
        {
            buttonDisabled = false;
            // Enable button by removing cross line
            // When enabling button we first remove cross line then run animation
            disable.SetActive(false);
        } else
        {
            buttonDisabled = true;
        }

        // This is to make sure people do not double click button while in animation
        GetComponent<Button>().interactable = false;

        // Start click animation
        buttonIconDisabledAnimation.Trigger();
    }

    // Run this function when click animation is over
    public void ClickButtonComplete()
    {
        if (buttonDisabled)
        {
            // Disable button by adding cross line
            // When disabling button we first run animation then remove cross line
            disable.SetActive(true);
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
