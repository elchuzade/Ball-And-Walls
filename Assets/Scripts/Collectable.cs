using UnityEngine;
using MoreMountains.NiceVibrations;
using System.Collections;

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
                AudioSource audio2 = GetComponent<AudioSource>();
                audio2.Play();

                homeStatus.CollectCoin();
            }
            StartCoroutine(DestroyCollectable(1));
        }
    }

    private IEnumerator DestroyCollectable(float time)
    {
        gameObject.transform.position = new Vector3(-200, -200, -200);

        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
}
