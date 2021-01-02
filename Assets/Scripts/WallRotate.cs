using UnityEngine;

public class WallRotate : MonoBehaviour
{
    [SerializeField] bool clockwise = true;
    [SerializeField] GameObject clockLaunch;
    [SerializeField] GameObject counterClockLaunch;

    Vector3 launchPosition;
    int angularDirection;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "BallCenter")
        {
            if (clockwise)
            {
                angularDirection = -1;
                launchPosition = clockLaunch.transform.position;
            }
            else
            {
                angularDirection = 1;
                launchPosition = counterClockLaunch.transform.position;
            }
            
            transform.parent.GetComponent<Wall>().BallDetected(launchPosition, angularDirection);
        }
    }
}
