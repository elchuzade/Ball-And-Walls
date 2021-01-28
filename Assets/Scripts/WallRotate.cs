using UnityEngine;

// This script is placed in both enters of rotation walls
public class WallRotate : MonoBehaviour
{
    // Direction in which the wall will rotate a ball
    [SerializeField] bool clockwise = true;
    // Transform that holds a position of a clockwise launching spot
    Transform clockLaunch;
    // Transform that holds a position of a counter clockwise launching spot
    Transform counterClockLaunch;

    // Position that will be passed based on where the ball has entered from
    Vector3 launchPosition;
    // Direction to launch the ball based on the wall assiting lines direction
    int angularDirection;

    void Awake()
    {
        clockLaunch = transform.parent.Find("ClockLaunch");
        counterClockLaunch = transform.parent.Find("CounterClockLaunch");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // If the very center of the ball has entered any of the wall entries
        // (clockwise or counterclockwise) send appropriate launch position and direction
        if (collision.gameObject.tag == "BallCenter")
        {
            if (clockwise)
            {
                // Used to transform word clockwise into mathematical value
                angularDirection = -1;
                // launch position is determined based on the entry of the ball
                launchPosition = clockLaunch.transform.position;
            }
            else
            {
                // Used to transform word counter clockwise into mathematical value
                angularDirection = 1;
                // launch position is determined based on the entry of the ball
                launchPosition = counterClockLaunch.transform.position;
            }

            // Inform a wall that has these entry points that the ball has been detected
            // And pass it position to launch from and rotation direction (clockwise counterclockwise)
            transform.parent.GetComponent<Wall>().BallDetected(launchPosition, angularDirection);
        }
    }
}
