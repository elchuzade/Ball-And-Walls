using System.Collections;
using UnityEngine;
using MoreMountains.NiceVibrations;

public class Ball : MonoBehaviour
{
    private enum Directions { North, South, East, West };

    [SerializeField] Player player;

    [SerializeField] GameObject startParticlesPrefab;
    [SerializeField] GameObject loseParticlesPrefab;
    [SerializeField] Transform ballClick;

    int startParticleAngle;

    private bool ballReset = false;

    public bool entered = false;
    private float speed = 10f;
    private float initSpeed = 30f;
    private float angularSpeed = 600f;

    private bool forward = false;

    HomeStatus homeStatus;

    GameObject ballPrefab;

    Vector3 initialPosition;

    Vector2 launchVector;

    [SerializeField] Directions direction;

    private void Start()
    {
        initialPosition = transform.position;

        homeStatus = FindObjectOfType<HomeStatus>();

        SetLaunchVector();
    }

    public void OnMouseDown()
    {
        homeStatus.CheckPointerFocus();

        if (!homeStatus.GetBallLaunched() && homeStatus.TutorialPassed())
        {
            BallClicked();
        }
    }

    public float GetAngularSpeed()
    {
        return angularSpeed;
    }

    public float GetSpeed()
    {
        return speed;
    }

    public void BallClicked()
    {
        GameObject startParticle = Instantiate(startParticlesPrefab, transform.position, Quaternion.Euler(0, 0, startParticleAngle));
        Destroy(startParticle, 1f);

        if (PlayerPrefs.GetInt("Haptics") == 1)
        {
            MMVibrationManager.Haptic(HapticTypes.RigidImpact);
        }
        AudioSource audio = GetComponent<AudioSource>();
        audio.Play();
        Launch(launchVector * initSpeed);
        homeStatus.LaunchBall();
    }

    private void SetLaunchVector()
    {
        switch (direction)
        {
            case Directions.North:
                launchVector = new Vector2(0, 1);
                startParticleAngle = 0;
                break;
            case Directions.South:
                launchVector = new Vector2(0, -1);
                startParticleAngle = 180;
                break;
            case Directions.East:
                launchVector = new Vector2(1, 0);
                startParticleAngle = -90;
                break;
            case Directions.West:
                launchVector = new Vector2(-1, 0);
                startParticleAngle = 90;
                break;
        }
    }

    public void Launch(Vector2 velocityVector)
    {
        ballReset = false;
        transform.GetComponent<Rigidbody2D>().velocity = velocityVector * speed;
    }

    public void StopMoving()
    {
        GetComponent<Rigidbody2D>().velocity = Vector3.zero;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Border")
        {
            ResetBall();
            StopMoving();
        } else if (collision.gameObject.tag == "Portal")
        {
            collision.GetComponent<Portal>().SuckIntoPortal();
            StopMoving();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Barrier")
        {
            AudioSource audio = collision.gameObject.GetComponent<AudioSource>();

            Debug.Log(audio);

            audio.Play();
        }
    }

    public void ForwardBall()
    {
        if (!forward)
        {
            forward = true;
            GetComponent<Rigidbody2D>().velocity *= 1.5f;
            speed *= 1.5f;
            angularSpeed *= 1.5f;
        }
    }

    public bool GetBallReset()
    {
        if (ballReset)
            transform.position = initialPosition;

        return ballReset;
    }

    public void ResetBall()
    {
        ballReset = true;

        ballPrefab = transform.GetChild(2).gameObject;

        ballClick.position = transform.position;

        ResetSpeed();

        AudioSource audio = transform.Find("Center").GetComponent<AudioSource>();
        audio.Play();

        GetComponent<Rigidbody2D>().velocity = Vector2.zero;

        if (PlayerPrefs.GetInt("Haptics") == 1)
        {
            MMVibrationManager.Haptic(HapticTypes.Failure);
        }
        GameObject loseParticle = Instantiate(loseParticlesPrefab, transform.position, Quaternion.identity);
        Destroy(loseParticle, 1f);

        if (ballReset)
        {
            gameObject.SetActive(true);
        }
        StartCoroutine(Reappear());
    }

    private void ResetSpeed()
    {
        speed = 10f;
        initSpeed = 30f;
        angularSpeed = 600f;
        forward = false;
    }

    private IEnumerator Reappear()
    {
        ballPrefab.SetActive(false);
        GetComponent<TrailRenderer>().time = 0;
        transform.position = initialPosition;

        yield return new WaitForSeconds(1);

        homeStatus.ResetLevel();
        ballPrefab.SetActive(true);
        GetComponent<TrailRenderer>().time = 100;

        homeStatus.ResetForwardButton();
    }
}
