using UnityEngine;
using UnityEngine.UI;
using MoreMountains.NiceVibrations;

public class Chest : MonoBehaviour
{
    [SerializeField] int index;
    [SerializeField] GameObject icon;
    [SerializeField] GameObject bestPrize;

    [SerializeField] GameObject reward;
    [SerializeField] Text rewardText;
    [SerializeField] GameObject frame;

    ChestStatus chestStatus;

    private void Start()
    {
        chestStatus = FindObjectOfType<ChestStatus>();
    }

    public void OpenChest()
    {
        if (chestStatus.GetKeys() > 0)
        {
            int rewardValue = chestStatus.OpenChest();
            icon.SetActive(false); // Hide chest

            if (PlayerPrefs.GetInt("Haptics") == 1)
            {
                MMVibrationManager.Haptic(HapticTypes.Selection);
            }

            if (rewardValue == 0)
            {
                bestPrize.SetActive(true); // Show best prize
                bestPrize.transform.GetChild(1).GetComponent<Image>().sprite = chestStatus.GetBestPrizeSprite();
                if (PlayerPrefs.GetInt("Haptics") == 1)
                {
                    MMVibrationManager.Haptic(HapticTypes.Success);
                }
            }
            else
            {
                reward.SetActive(true); // Show reward
                rewardText.text = rewardValue.ToString();
                frame.SetActive(true); // Show frame
            }
        }
    }
}
