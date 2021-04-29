using UnityEngine;
using UnityEngine.UI;
using MoreMountains.NiceVibrations;
using System.Collections;

// Item in the shop
public class Item : MonoBehaviour
{
    ShopStatus shopStatus;
    // Text field under the item icon
    [SerializeField] Text price;
    // Item name from sprite
    string ballName;
    // Actual coins price of an item to be charged
    [SerializeField] int priceTag;
    // Actual diamonds price of an item to be charged
    [SerializeField] int diamondTag;
    // Frame around an item that shows when an item is selected
    [SerializeField] GameObject SelectedFrame;
    // Frame around an icon that shows when item is locked
    [SerializeField] GameObject LockedFrame;
    // Coin and Diamon icons
    GameObject coin;
    GameObject diamond;

    void Awake()
    {
        // This finds a script of ShopStatus, can be used if there is only one item that holds this script
        shopStatus = FindObjectOfType<ShopStatus>();
        coin = transform.Find("LockedFrame").Find("Coin").gameObject;
        diamond = transform.Find("LockedFrame").Find("Diamond").gameObject;
        // Get sprite name from image because it is in the canvas
        ballName = transform.Find("Icon").GetComponent<Image>().sprite.name;
    }

    // To run on each ball from shopStatus
    public void CheckBallData()
    {
        // Check from the player data if the item is unlocked for this player
        if (shopStatus.CheckUnlockStatus(ballName))
        {
            // Remove lock frame if unlocked
            LockedFrame.SetActive(false);
        }

        // Check from the player data if the item is selected for this player
        if (shopStatus.CheckSelectStatus(ballName))
        {
            // Show selected frame if selected
            SelectedFrame.SetActive(true);
        }
        // Set price of an item. Siince price text is a child of a locked frame object,
        // it is not visible when item is unlocked
        if (diamondTag > 0)
        {
            price.text = diamondTag.ToString();
            coin.SetActive(false);
        }
        else
        {
            price.text = priceTag.ToString();
            diamond.SetActive(false);
        }
        CheckSelectFrame();
    }

    public void CheckSelectFrame()
    {
        if (shopStatus.CheckSelectStatus(ballName))
        {
            // Show selected frame
            SelectedFrame.SetActive(true);
        }
        else
        {
            // Hide selected frame
            SelectedFrame.SetActive(false);
        }
    }

    public string GetBallName()
    {
        return ballName;
    }

    public void UnlockItem()
    {
        if (diamondTag > 0)
        {
            // Try to unlock the item in player data based on its index and price
            if (shopStatus.UnlockDiamondItem(ballName, diamondTag))
            {
                // If player had enough money then remove the unlock frame
                LockedFrame.SetActive(false);
                // Show selected frame on this item
                SelectItem();
            }
        } else
        {
            // Try to unlock the item in player data based on its index and price
            if (shopStatus.UnlockItem(ballName, priceTag))
            {
                // If player had enough money then remove the unlock frame
                LockedFrame.SetActive(false);
                // Show selected frame on this item
                SelectItem();
            }
        }
    }

    public void SelectItem()
    {
        // If this item is selected in the player data put selected frame on this item
        if (shopStatus.SelectItem(ballName))
        {
            SelectedFrame.SetActive(true);
        }
    }

    public void TouchItem()
    {
        // Check if item is unlocked for this player, if so select it
        if (shopStatus.CheckUnlockStatus(ballName))
        {
            if (PlayerPrefs.GetInt("Haptics") == 1)
            {
                MMVibrationManager.Haptic(HapticTypes.Selection);
            }
            SelectItem();
        } else
        {
            // Else try to unlock the item if money is sufficient
            if (PlayerPrefs.GetInt("Haptics") == 1)
            {
                MMVibrationManager.Haptic(HapticTypes.Success);
            }
            UnlockItem();
        }
    }
}
