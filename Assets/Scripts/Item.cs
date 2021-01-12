using UnityEngine;
using UnityEngine.UI;
using MoreMountains.NiceVibrations;

// Item in the shop
public class Item : MonoBehaviour
{
    private ShopStatus shopStatus;
    // Text field under the item icon
    [SerializeField] Text price;
    // Item index to be able to unlock it in player data
    [SerializeField] int index;
    // Actual price of an item to be charged
    [SerializeField] int priceTag;
    // Frame around an item that shows when an item is selected
    [SerializeField] GameObject SelectedFrame;
    // Frame around an icon that shows when item is locked
    [SerializeField] GameObject LockedFrame;

    void Awake()
    {
        // This finds a script of ShopStatus, can be used if there is only one item that holds this script
        shopStatus = FindObjectOfType<ShopStatus>();
    }

    void Start()
    {
        // Check from the player data if the item is unlocked for this player
        if (shopStatus.CheckUnlockStatus(index))
        {
            // Remove lock frame if unlocked
            LockedFrame.SetActive(false);
        }

        // Check from the player data if the item is selected for this player
        if (shopStatus.CheckSelectStatus(index))
        {
            // Show selected frame if selected
            SelectedFrame.SetActive(true);
        }
        // Set price of an item. Siince price text is a child of a locked frame object,
        // it is not visible when item is unlocked
        price.text = priceTag.ToString();
    }

    void Update()
    {
        // TODO: come up with a better method of shopping in other games, update this one too
        // For each item check if the item has been selected at this frame
        if (shopStatus.CheckSelectStatus(index))
        {
            // Show selected frame
            SelectedFrame.SetActive(true);
        } else
        {
            // Hide selected frame
            SelectedFrame.SetActive(false);
        }
    }

    public void UnlockItem()
    {
        // Try to unlock the item in player data based on its index and price
        if (shopStatus.UnlockItem(index, priceTag))
        {
            // If player had enough money then remove the unlock frame
            LockedFrame.SetActive(false);
            // Show selected frame on this item
            SelectItem();
        }
    }

    public void SelectItem()
    {
        // If this item is selected in the player data put selected frame on this item
        if (shopStatus.SelectItem(index))
        {
            SelectedFrame.SetActive(true);
        }
    }

    public void TouchItem()
    {
        // Check if item is unlocked for this player, if so select it
        if (shopStatus.CheckUnlockStatus(index))
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
