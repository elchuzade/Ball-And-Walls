using UnityEngine;
using UnityEngine.UI;

public class Scoreboard : MonoBehaviour
{
    Text coins;
    Text diamonds;
    GameObject key1;
    GameObject key2;
    GameObject key3;

    void Awake()
    {
        coins = transform.Find("TotalCoins").Find("Text").GetComponent<Text>();
        diamonds = transform.Find("TotalDiamonds").Find("Text").GetComponent<Text>();

        if (transform.Find("Keys"))
        {
            key1 = transform.Find("Keys").Find("Key-1").gameObject;
            key2 = transform.Find("Keys").Find("Key-2").gameObject;
            key3 = transform.Find("Keys").Find("Key-3").gameObject;
        }
    }

    public void SetCoins(int _coins)
    {
        // Setting up the number of coins provided to the function converting it to string
        coins.text = _coins.ToString();
    }

    public void SetDiamonds(int _diamonds)
    {
        // Setting up the number of coins provided to the function converting it to string
        diamonds.text = _diamonds.ToString();
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
