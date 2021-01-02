using UnityEngine;
using UnityEngine.UI;
using MoreMountains.NiceVibrations;

public class OptionsBar : MonoBehaviour
{
    [SerializeField] Image hapticsImage;
    [SerializeField] Image hapticsIcon;
    [SerializeField] Image hapticsIconDisabled;
    [SerializeField] Image hapticsFrame;

    private bool haptics;

    public bool visible;

    TriggerAnimation hapticsButton;

    void Start()
    {
        haptics = PlayerPrefs.GetInt("Haptics") == 1;
        SetupInitialValues();

        GameObject hapticsButtonObject = GameObject.Find("HapticsButton");
        hapticsButton = hapticsButtonObject.GetComponent<TriggerAnimation>();
    }

    private void SetupInitialValues()
    {
        if (haptics)
        {
            hapticsImage.GetComponent<Image>().color = new Color32(0, 0, 0, 100);
            hapticsIcon.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
            hapticsFrame.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        }
        else
        {
            hapticsImage.GetComponent<Image>().color = new Color32(0, 0, 0, 50);
            hapticsIcon.GetComponent<Image>().color = new Color32(255, 255, 255, 100);
            hapticsFrame.GetComponent<Image>().color = new Color32(255, 255, 255, 100);
        }
    }

    public void ClickHapticsButton()
    {
        if (haptics)
        {
            haptics = false;
            hapticsIcon.gameObject.SetActive(false);
            hapticsIconDisabled.gameObject.SetActive(true);
            hapticsImage.GetComponent<Image>().color = new Color32(0, 0, 0, 50);
            hapticsIcon.GetComponent<Image>().color = new Color32(255, 255, 255, 100);
            hapticsFrame.GetComponent<Image>().color = new Color32(255, 255, 255, 100);
            PlayerPrefs.SetInt("Haptics", 0);
        }
        else
        {
            haptics = true;
            hapticsIcon.gameObject.SetActive(true);
            hapticsIconDisabled.gameObject.SetActive(false);
            hapticsImage.GetComponent<Image>().color = new Color32(0, 0, 0, 100);
            hapticsIcon.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
            hapticsFrame.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
            PlayerPrefs.SetInt("Haptics", 1);
            MMVibrationManager.Haptic(HapticTypes.HeavyImpact);
        }
        hapticsButton.Trigger();
    }
}
