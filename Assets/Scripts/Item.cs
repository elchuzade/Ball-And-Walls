using UnityEngine;
using UnityEngine.UI;
using MoreMountains.NiceVibrations;

public class Item : MonoBehaviour
{
    private ShopStatus shopStatus;
    [SerializeField] Text price;
    [SerializeField] int index;
    [SerializeField] int priceTag;
    [SerializeField] GameObject SelectedFrame;
    [SerializeField] GameObject LockedFrame;

    private void Awake()
    {
        shopStatus = FindObjectOfType<ShopStatus>();
    }

    private void Start()
    {
        if (shopStatus.CheckUnlockStatus(index))
        {
            LockedFrame.SetActive(false);
        }

        if (shopStatus.CheckSelectStatus(index))
        {
            SelectedFrame.SetActive(true);
        }
        price.text = priceTag.ToString();
    }

    private void Update()
    {
        if (shopStatus.CheckSelectStatus(index))
        {
            SelectedFrame.SetActive(true);
        } else
        {
            SelectedFrame.SetActive(false);
        }
    }

    public void UnlockItem()
    {
        if (shopStatus.UnlockItem(index, priceTag))
        {
            LockedFrame.SetActive(false);
            SelectItem();
        }
    }

    public void SelectItem()
    {
        if (shopStatus.SelectItem(index))
        {
            SelectedFrame.SetActive(true);
        }
    }

    public void TouchItem()
    {
        if (shopStatus.CheckUnlockStatus(index))
        {
            if (PlayerPrefs.GetInt("Haptics") == 1)
            {
                MMVibrationManager.Haptic(HapticTypes.Selection);
            }
            SelectItem();
        } else
        {
            if (PlayerPrefs.GetInt("Haptics") == 1)
            {
                MMVibrationManager.Haptic(HapticTypes.Success);
            }
            UnlockItem();
        }
    }
}
