using UnityEngine;
using UnityEngine.UI;
using MoreMountains.NiceVibrations;

public class OptionsBar : MonoBehaviour
{
    [SerializeField] Image hapticsImage;
    [SerializeField] Image hapticsIcon;
    [SerializeField] Image hapticsIconDisabled;
    [SerializeField] Image hapticsFrame;

    [SerializeField] Image soundsImage;
    [SerializeField] Image soundsIcon;
    [SerializeField] Image soundsIconDisabled;
    [SerializeField] Image soundsFrame;

    // To store haptics status, whether it is are on or off
    private bool haptics;
    // To store sounds status, whether it is are on or off
    private bool sounds;

    // Animation of clicking haptics button
    TriggerAnimation hapticsButtonAnimation;
    // Animation of clicking sounds button
    TriggerAnimation soundsButtonAnimation;

    void Start()
    {
        // Retrieve haptics button status from player prefs
        haptics = PlayerPrefs.GetInt("Haptics") == 1;
        // Retrieve sounds button status from player prefs
        sounds = PlayerPrefs.GetInt("Sounds") == 1;

        SetupInitialValues();

        // Find a haptics button game object and access its trigger animation
        GameObject hapticsButtonObject = GameObject.Find("HapticsButton");
        hapticsButtonAnimation = hapticsButtonObject.GetComponent<TriggerAnimation>();

        // Find a sounds button game object and access its trigger animation
        GameObject soundsButtonObject = GameObject.Find("SoundsButton");
        soundsButtonAnimation = soundsButtonObject.GetComponent<TriggerAnimation>();
    }

    private void SetupInitialValues()
    {
        // Setup initial values for haptics
        if (haptics)
        {
            // Make the frame, the hapctis icon and the background darker as enabled
            EnableHapticsButton();
        }
        else
        {
            // Make the frame, the hapctis icon and the background lighter as disabled
            DisableHapticsButton();
        }
        // Setup initial values for sounds
        if (sounds)
        {
            // Make the frame, the sounds icon and the background darker as enabled
            EnableSoundsButton();
        }
        else
        {
            
            DisableSoundsButton();
        }
    }

    public void ClickHapticsButton()
    {
        if (haptics)
        {
            DisableHapticsButton();

            PlayerPrefs.SetInt("Haptics", 0);
        }
        else
        {
            EnableHapticsButton();

            PlayerPrefs.SetInt("Haptics", 1);
            MMVibrationManager.Haptic(HapticTypes.HeavyImpact);
        }
        hapticsButtonAnimation.Trigger();
    }

    public void ClickSoundsButton()
    {
        if (sounds)
        {
            DisableSoundsButton();

            PlayerPrefs.SetInt("Haptics", 0);
        }
        else
        {
            EnableSoundsButton();

            PlayerPrefs.SetInt("Haptics", 1);
        }
        soundsButtonAnimation.Trigger();
    }

    private void DisableSoundsButton()
    {
        // Change sounds status to off
        sounds = false;
        // TODO: test this??? might not need it
        soundsIcon.gameObject.SetActive(false);
        // Show the cross line in disable sounds button
        soundsIconDisabled.gameObject.SetActive(true);

        // Make the frame, the sounds icon and the background lighter as disabled
        soundsImage.GetComponent<Image>().color = new Color32(0, 0, 0, 50);
        soundsIcon.GetComponent<Image>().color = new Color32(255, 255, 255, 100);
        soundsFrame.GetComponent<Image>().color = new Color32(255, 255, 255, 100);
    }

    private void EnableSoundsButton()
    {
        // Change sounds status to on
        sounds = true;
        // TODO: test this??? might not need it
        soundsIcon.gameObject.SetActive(true);
        // Hide the cross line in disable sounds button
        soundsIconDisabled.gameObject.SetActive(false);

        // Make the frame, the sounds icon and the background darker as enabled
        soundsImage.GetComponent<Image>().color = new Color32(0, 0, 0, 100);
        soundsIcon.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        soundsFrame.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
    }

    private void DisableHapticsButton()
    {
        // Change haptics status to off
        haptics = false;
        // TODO: test this??? might not need it
        hapticsIcon.gameObject.SetActive(false);
        // Show the cross line in disable haptics button
        hapticsIconDisabled.gameObject.SetActive(true);

        // Make the frame, the haptics icon and the background lighter as disabled
        hapticsImage.GetComponent<Image>().color = new Color32(0, 0, 0, 50);
        hapticsIcon.GetComponent<Image>().color = new Color32(255, 255, 255, 100);
        hapticsFrame.GetComponent<Image>().color = new Color32(255, 255, 255, 100);
    }

    private void EnableHapticsButton()
    {
        // Change haptics status to on
        haptics = true;
        // TODO: test this??? might not need it
        hapticsIcon.gameObject.SetActive(true);
        // Hide the cross line in disable haptics button
        hapticsIconDisabled.gameObject.SetActive(false);

        // Make the frame, the haptics icon and the background darker as enabled
        hapticsImage.GetComponent<Image>().color = new Color32(0, 0, 0, 100);
        hapticsIcon.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        hapticsFrame.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
    }
}
