using System.Collections;
using UnityEngine;
using MoreMountains.NiceVibrations;

public enum Directions { North, South, East, West };

public class Ball : MonoBehaviour
{
    [SerializeField] GameObject startParticlesPrefab;
    [SerializeField] GameObject loseParticlesPrefab;

    // Area around a ball when it is ready to be launched, for big thumbs
    [SerializeField] Transform ballClick;

    // To set particles angle against the ball direction
    int startParticleAngle;

    // To be toggled when reset button is clicked or ball hit the game borders
    bool ballReset;

    // To indicate that ball has entered a rotation wall, stop moving and let the wall rotate it
    public bool entered;

    // ----------- all these three speeds give the same visual
    // Speed of a ball when launched from rotating walls
    float speed = 10f;
    // Speed of a ball when launched from start position by clicking
    float initSpeed = 30f;
    // Speed of rotation inside a rotating wall
    float angularSpeed = 600f;
    // ----------- all these three speeds give the same visual

    // Indicates accelerated movement of a ball when forward button clicked
    bool forward;

    HomeStatus homeStatus;

    // Prefab of currently selected ball based on player data
    GameObject ballPrefab;

    // Coordinates to return to when ball is reset
    Vector3 initialPosition;

    // Vector that determines direction of a ball to be launched by clicking
    Vector2 launchVector;

    // Set a direction for the ball where to move when launched
    [SerializeField] Directions direction;

    void Awake()
    {
        homeStatus = FindObjectOfType<HomeStatus>();
    }

    void Start()
    {
        initialPosition = transform.position;

        SetLaunchVector();
    }

    public void OnMouseDown()
    {
        // If tutorial level, hide pointers since ball has just been launched
        homeStatus.CheckPointerFocus();

        if (!homeStatus.GetBallLaunched() && homeStatus.TutorialPassed())
        {
            // Run all the actions needed when ball has just been launched
            BallClicked();
        }
    }

    public void BallClicked()
    {
        // Create burst from launching ball with start particle angle and destroy in a second
        GameObject startParticle = Instantiate(startParticlesPrefab, transform.position, Quaternion.Euler(0, 0, startParticleAngle));
        Destroy(startParticle, 1f);

        // If haptics are enabled, make a vibration on launch
        if (PlayerPrefs.GetInt("Haptics") == 1)
        {
            MMVibrationManager.Haptic(HapticTypes.RigidImpact);
        }

        // If sounds are enabled, make a sound on launch
        if (PlayerPrefs.GetInt("Sounds") == 1)
        {
            // Find Audio Source component on Ball Game Object and run its sound source
            AudioSource audio = GetComponent<AudioSource>();
            audio.Play();
        }

        // Launch the ball with determined vector and ball launch speed
        Launch(launchVector * initSpeed);
        // Inform main script that ball is launched, to change canvas buttons etc...
        homeStatus.LaunchBall();
    }

    // Based on the direction selected on the ball determine launch vector and start particles angle to burst
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

    // Used for walls to find out what speed to rotate the ball with
    public float GetAngularSpeed()
    {
        return angularSpeed;
    }

    // Launch the ball from inside walls or from starting position
    public void Launch(Vector2 velocityVector)
    {
        ballReset = false;
        // Add velocity based on the ball speed and velocity vector
        transform.GetComponent<Rigidbody2D>().velocity = velocityVector * speed;
    }

    // Stop moving when entered rotation wall or portal or game wall
    public void StopMoving()
    {
        GetComponent<Rigidbody2D>().velocity = Vector3.zero;
    }

    // If the ball has entered collider with trigger effect on it
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Border")
        {
            // Hit the border, reset the ball
            ResetBall();
            // Stop moving the ball from its velocity
            StopMoving();
        } else if (collision.gameObject.tag == "Portal")
        {
            // Send the ball into a portal
            collision.GetComponent<Portal>().SuckIntoPortal();
            // Stop moving the ball from its velocity
            StopMoving();
        }
    }

    // If the ball has entered collider without trigger effect on it
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Barrier")
        {
            // If haptics are opened set it to vibrate
            if (PlayerPrefs.GetInt("Haptics") == 1)
            {
                // Find an Audio Source on barrier that is hit and play it
                AudioSource audio = collision.gameObject.GetComponent<AudioSource>();
                audio.Play();
            }
        }
    }

    // Accelerate ball when forward button is clicked
    public void ForwardBall()
    {
        if (!forward)
        {
            // Set ball is forwarded status to not be able to increase repetetively
            forward = true;
            // Increase ball velocity
            GetComponent<Rigidbody2D>().velocity *= 1.5f;
            // Increase ball launch velocity from walls and portals
            speed *= 1.5f;
            // Increase ball rotation speed inside a wall
            angularSpeed *= 1.5f;
        }
    }

    // Check if the ball status is reset and return
    // To be used from Wall script and HomeStatus script
    public bool GetBallReset()
    {
        if (ballReset)
        {
            // If ball status is reset, send the ball to its initial position
            transform.position = initialPosition;
        }

        return ballReset;
    }

    // To be used from HomeStatus
    public void ResetBall()
    {
        // Set ball reset status
        ballReset = true;

        // Get the actual ball based on its index. Name cant be used, since balls are different
        ballPrefab = transform.GetChild(2).gameObject;

        // Set big thumb circle around the ball to the same position as ball
        ballClick.position = transform.position;

        // Reset ball speed to normal incase it was accelerated with forward button
        ResetSpeed();

        // If sounds are enabled set it to make a sound
        if (PlayerPrefs.GetInt("Sounds") == 1)
        {
            // Find Lose effect Audio Source from ball's child component and play it
            // It is not placed on the ball itseld, coz it already has launch Audio Source
            AudioSource audio = transform.Find("Center").GetComponent<AudioSource>();
            audio.Play();
        }

        // Reset ball velocity to zero to stop it from moving
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;

        // If haptics are enabled set it to vibrate
        if (PlayerPrefs.GetInt("Haptics") == 1)
        {
            MMVibrationManager.Haptic(HapticTypes.Failure);
        }

        // Create lose particles effect on ball coordinates and destroy it after it has played itself
        GameObject loseParticle = Instantiate(loseParticlesPrefab, transform.position, Quaternion.identity);
        Destroy(loseParticle, 1f);

        // TODO: Check with commenting it out??? This might not be important
        if (ballReset)
        {
            gameObject.SetActive(true);
        }

        // Recreate a ball after a second, this time is for lose particles to play
        StartCoroutine(Reappear(1));
    }

    // Reset all ball speed values to their initial values
    private void ResetSpeed()
    {
        speed = 10f;
        initSpeed = 30f;
        angularSpeed = 600f;
        forward = false;
    }

    private IEnumerator Reappear(float time)
    {
        // Hide the prefab that holds current ball icon
        ballPrefab.SetActive(false);

        // Reset position of a ball to its initial position
        transform.position = initialPosition;

        yield return new WaitForSeconds(time);

        // Inform home status that the ball has been reset, for appropriate canvas changes
        homeStatus.ResetLevel();
        // Show the prefab that holds current ball icon
        ballPrefab.SetActive(true);
    }
}
