using UnityEngine;

public class ChallengeItem : MonoBehaviour
{
    GameObject lockedFrame;

    void Awake()
    {
        lockedFrame = transform.Find("LockedFrame").gameObject;
    }

    private string id;
    private int index;
    private bool locked;

    public void SetIndex(int _index)
    {
        index = _index;
    }

    public int GetIndex()
    {
        return index;
    }

    public void SetLocked(bool _locked)
    {
        locked = _locked;
        if (!_locked)
        {
            lockedFrame.SetActive(false);
        }
    }

    public bool GetLocked()
    {
        return locked;
    }

    public void SetId(string _id)
    {
        id = _id;
    }

    public string GetId()
    {
        return id;
    }
}
