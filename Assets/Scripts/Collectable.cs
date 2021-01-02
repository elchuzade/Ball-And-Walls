using UnityEngine;
using MoreMountains.NiceVibrations;

public class Collectable : MonoBehaviour
{
    private enum Type { Key, Coin }

    [SerializeField] GameObject collectParticle;
    HomeStatus homeStatus;

    [SerializeField] Type type;

    private void Start()
    {
        homeStatus = FindObjectOfType<HomeStatus>();

        if (type == Type.Key && homeStatus.AllBallsUnlocked())
        {
            Destroy(gameObject);
        }
    }

    public void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Ball")
        {
            GameObject collect = Instantiate(
                collectParticle, transform.position, Quaternion.identity);

            Destroy(collect, 1);

            if (PlayerPrefs.GetInt("Haptics") == 1)
            {
                MMVibrationManager.Haptic(HapticTypes.SoftImpact);
            }

            if (type == Type.Key)
            {
                homeStatus.CollectKey();
            } else if (type == Type.Coin)
            {
                homeStatus.CollectCoin();
            }

            Destroy(gameObject);
        }
    }
}
