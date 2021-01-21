using UnityEngine;
using UnityEngine.UI;

public class ChallengeSaverStatus : MonoBehaviour
{
    public class ChallengeWall
    {
        public string type;
        public string color;
        public string position;
        public string rotation;
    }

    public class ChallengeBarrier
    {
        public string type;
        public Color32 color;
        public Vector3 position;
        public Vector3 rotation;
    }

    public class ChallengePortal
    {
        public string type;
        public string position;
        public string rotation;
    }

    public class ChallengeCoin
    {
        public string position;
    }

    GameObject saveJsonButton;

    GameObject barriers;

    void Awake()
    {
        saveJsonButton = GameObject.Find("SaveJsonButton");
        barriers = GameObject.Find("Barriers");
    }

    void Start()
    {
        saveJsonButton.GetComponent<Button>().onClick.AddListener(() => SaveLevelJson());
    }

    public void SaveLevelJson()
    {
        Debug.Log("saving");
        // Saving barriers
        GameObject barrier = barriers.transform.GetChild(0).gameObject;

        ChallengeBarrier barrierInfo = new ChallengeBarrier();

        Debug.Log(barrier.GetComponent<SpriteRenderer>().sprite.name);

        barrierInfo.type = "Barrier150";
        barrierInfo.color = barrier.GetComponent<SpriteRenderer>().color;
        barrierInfo.position = barrier.transform.position;
        barrierInfo.rotation = new Vector3(
            barrier.transform.rotation.x,
            barrier.transform.rotation.y,
            barrier.transform.rotation.z);

        string barrierJson = JsonUtility.ToJson(barrierInfo);
        System.IO.File.WriteAllText(Application.dataPath + "/Barriers.json", barrierJson);
    }
}
