using UnityEngine;
using UnityEngine.UI;

public class ChallengeItem : MonoBehaviour
{
    [SerializeField] GameObject lockFrame;
    [SerializeField] GameObject lives;
    [SerializeField] GameObject selectFrame;

    [SerializeField] int levelIndex;
    [SerializeField] int levelCoins;
    [SerializeField] int levelDiamonds;
    [SerializeField] Text challengeLevel;
    [SerializeField] GameObject diamondsText;
    [SerializeField] GameObject coinsText;
    [SerializeField] GameObject solvedText;

    private bool unlocked;

    private int status;

    private void Awake()
    {
        lockFrame.SetActive(true);
        gameObject.GetComponent<Button>().onClick.AddListener(() => SelectChallenge());
    }

    public void SelectChallenge()
    {
        if (unlocked)
        {
            ChallengesStatus challengesStatus = FindObjectOfType<ChallengesStatus>();
            challengesStatus.SelectChallenge(gameObject);
            SelectChallengeStatus(true);
        }
    }

    public int GetChallengeStatus()
    {
        return status;
    }

    public int GetChallengeIndex()
    {
        return levelIndex;
    }

    public void SelectChallengeStatus(bool status)
    {
        if (status)
        {
            selectFrame.SetActive(true);
        } else
        {
            selectFrame.SetActive(false);
        }
    }

    public void SetData(int challengeStatus, Sprite _ball)
    {
        status = challengeStatus;
        SetLives(challengeStatus, _ball);
        if (challengeStatus == -2)
        {
            unlocked = true;
            solvedText.SetActive(true);
            diamondsText.SetActive(false);
            coinsText.SetActive(false);
            lockFrame.SetActive(false);
        }
        else if (challengeStatus != -1)
        {
            unlocked = true;
            lockFrame.SetActive(false);
            string coinsString = "";
            string diamondsString = "";
            coinsString += levelCoins;
            diamondsString += levelDiamonds;
            diamondsText.transform.Find("Text").GetComponent<Text>().text = diamondsString;
            coinsText.transform.Find("Text").GetComponent<Text>().text = coinsString;
        }
    }

    public void SetLives(int _lives, Sprite _ball)
    {
        // Incase lives are set after watching video or spending diamond
        if (status != _lives)
        {
            status = _lives;
        }

        for (int i = 0; i < lives.transform.childCount; i++)
        {
            lives.transform.GetChild(i).GetComponent<Image>().sprite = _ball;
            if (i < status || status == - 2)
            {
                lives.transform.GetChild(i).GetComponent<Image>().color = new Color32(255, 255, 255, 255);
            } else
            {
                lives.transform.GetChild(i).GetComponent<Image>().color = new Color32(255, 255, 255, 25);
            }
        }
    }
}
