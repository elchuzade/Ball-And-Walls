using UnityEngine;
using MoreMountains.NiceVibrations;
using System.Collections;

public enum Type { Key, Coin, Diamond }

public class Collectable : MonoBehaviour
{
    // Particle effect to show when the object is collected
    [SerializeField] GameObject collectParticlePrefab;
    HomeStatus homeStatus;

    // Select whether it is a key or coin in the inspector
    [SerializeField] Type type;

    void Awake()
    {
        homeStatus = FindObjectOfType<HomeStatus>();
    }

    void Start()
    {
        // If the player has unlocked all of the balls, do not show him keys anymore
        if (type == Type.Key && homeStatus.AllBallsUnlocked())
        {
            Destroy(gameObject);
        }
    }

    public void OnTriggerEnter2D(Collider2D collider)
    {
        // If a ball has collided with the collectable item
        if (collider.gameObject.tag == "Ball")
        {
            // Create a collectParticle instance at the location of collectable item
            GameObject collectParticle = Instantiate(
                collectParticlePrefab, transform.position, Quaternion.identity);

            // Remove it in a second
            Destroy(collectParticle, 1);

            if (PlayerPrefs.GetInt("Haptics") == 1)
            {
                MMVibrationManager.Haptic(HapticTypes.SoftImpact);
            }

            // If the item that holds this script was a key add 1 key to player
            if (type == Type.Key)
            {
                homeStatus.CollectKey();
            }

            // If the item that holds this script was a coin add 1 coin to player
            else if (type == Type.Coin)
            {
                homeStatus.CollectCoin();
            }
            // Run all the actions needed for destroying this collectable item after 1 second
            StartCoroutine(DestroyCollectable(1));
        }
    }

    private IEnumerator DestroyCollectable(float time)
    {
        // Move it outside of the screen so player can not see or interact with it again
        gameObject.transform.position = new Vector3(-1000, -1000, -1000);

        // wait for some time so that sound effect can be played and destroy it
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
}
