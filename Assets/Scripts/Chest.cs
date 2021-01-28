using UnityEngine;
using UnityEngine.UI;
using MoreMountains.NiceVibrations;

public class Chest : MonoBehaviour
{
    // Index of current chest to track opened closed status
    [SerializeField] int index;
    // Chest icon itself to hide when unlocked
    [SerializeField] GameObject icon;
    // Object that holds the best prize inside the chest
    [SerializeField] GameObject bestPrize;
    // Object that holds the coins reward inside the chest
    [SerializeField] GameObject reward;
    // Text representing amount of coins dropped from this chest
    [SerializeField] Text rewardText;
    // Frame around coins reward
    [SerializeField] GameObject frame;

    ChestStatus chestStatus;

    void Awake()
    {
        chestStatus = FindObjectOfType<ChestStatus>();
    }

    // Try to open a chest when clicked on it
    public void OpenChest()
    {
        // Check if player has at least 1 key to open a chest
        if (chestStatus.GetKeys() > 0)
        {
            // Make the open chest procedure that removes a key from a player
            int rewardValue = chestStatus.OpenChest();
            // Hide chest icon to make it look like it is opened
            icon.SetActive(false);

            if (PlayerPrefs.GetInt("Haptics") == 1)
            {
                MMVibrationManager.Haptic(HapticTypes.Selection);
            }

            // If reward value is 0, this is a best prize
            if (rewardValue == 0)
            {
                // Show best prize object
                bestPrize.SetActive(true);
                // Set its icon to best prize icon
                bestPrize.transform.GetChild(1).GetComponent<Image>().sprite = chestStatus.GetBestPrizeSprite();
                if (PlayerPrefs.GetInt("Haptics") == 1)
                {
                    MMVibrationManager.Haptic(HapticTypes.Success);
                }
            }
            else
            {
                // If the reward is not the best prize show the reward object
                reward.SetActive(true);
                // Set its coins amount to the reward value that was hidden in this chest
                rewardText.text = rewardValue.ToString();
                // Show the frame around the reward
                frame.SetActive(true);
            }
        }
    }
}
