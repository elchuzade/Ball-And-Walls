using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// To show whether the button state is enabled or disabled, for showing cross icon
public enum ButtonStates { Disable, Enable };
// To act differently depending on what type of a button it is
public enum ButtonType { Default, Text, Disable };

public class TriggerButton : MonoBehaviour
{
    // Disable cross line if applicable
    GameObject disable;
    public ButtonType type;

    // Status of the feature this button toggles
    // This is not used if there is no disabled cross line on the button
    bool isEnabled;

    // Script to run the animation
    TriggerAnimation clickAnimation;

    void Awake()
    {
        if (type == ButtonType.Disable)
        {
            // Find disable cross line if exists
            disable = transform.Find("Components").Find("Disable").gameObject;
        }

        // Trigger animation script must be attached to parent component same as current script
        clickAnimation = GetComponent<TriggerAnimation>();
    }

    // This is accessed from other scripts to set initial value to disabled or enabled
    public void SetButtonState(ButtonStates initState)
    {
        // If initState passed to this button is enabled, save it in local variable isEnabled
        if (initState == ButtonStates.Disable)
        {
            isEnabled = false;
            if (type == ButtonType.Disable)
            {
                disable.SetActive(true);
            }
        } else
        {
            isEnabled = true;
            if (type == ButtonType.Disable)
            {
                disable.SetActive(false);
            }
        }

        // If initState is disabled, show the cross line representing a feature being disabled
        if (!isEnabled && type == ButtonType.Disable)
        {
            disable.SetActive(true);
        }
    }

    // Click the button function, passing in how much time needed before the button can be clicked again
    // This is important to differentiate time between reset button clicks. Should not be clicked again
    // until the ball is reset. Other buttons, should wait until the trigger animation is over
    public void ClickButton(float resetTime)
    {
        float waitTime = 0.2f;
        // Toggle button state
        isEnabled = !isEnabled;
        // Wait for reset time which is minimum 0.2f
        if (resetTime > 0.2)
        {
            waitTime = resetTime;
        }

        StartCoroutine(ResetButton(waitTime));
    }

    IEnumerator ResetButton(float time)
    {
        // Run the click animation, that takes roughly 0.2 seconds
        clickAnimation.Trigger();

        // Make the button not interactable until the time is up to reset the button
        GetComponent<Button>().interactable = false;

        yield return new WaitForSeconds(time);

        // Make the button interactable after the time is up to reset the button
        GetComponent<Button>().interactable = true;
    }
}
