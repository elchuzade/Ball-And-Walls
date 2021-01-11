using UnityEngine;
using UnityEngine.UI;

public class Scoreboard : MonoBehaviour
{
    [SerializeField] Text coins;
    [SerializeField] GameObject key1;
    [SerializeField] GameObject key2;
    [SerializeField] GameObject key3;

    public void SetCoins(int _coins)
    {
        // Setting up the number of coins provided to the function converting it to string
        coins.text = _coins.ToString();
    }

    public void SetKeys(int _keys)
    {
        // Setting up the number of keys provided to the function coloring them appropriately
        key1.GetComponent<Image>().color = new Color32(150, 150, 150, 255);
        key2.GetComponent<Image>().color = new Color32(150, 150, 150, 255);
        key3.GetComponent<Image>().color = new Color32(150, 150, 150, 255);

        if (_keys > 0)
            key1.GetComponent<Image>().color = new Color32(255, 255, 0, 255);
        if (_keys > 1)
            key2.GetComponent<Image>().color = new Color32(255, 255, 0, 255);
        if (_keys > 2)
            key3.GetComponent<Image>().color = new Color32(255, 255, 0, 255);
    }
}
