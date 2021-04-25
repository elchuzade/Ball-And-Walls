using System.Collections;
using UnityEngine;

public class Portal : MonoBehaviour
{
    // If it is a portal in, conenct it to a portal out
    [SerializeField] GameObject PortalOut;
    HomeStatus homeStatus;

    // Variable to determine that the ball has been caught by a portal in
    bool catchBall;
    Ball ball;
    // This is used coz sometimes there may be a small difference in tolerance of coordinates
    float margin = 0.01f;
    // Speed with which the portal in will suck the ball inside
    int portalSpeed = 500;
    // Speed with which teleport out will throw the ball
    int releaseSpeed = 30;

    void Start()
    {
        ball = FindObjectOfType<Ball>();
        homeStatus = FindObjectOfType<HomeStatus>();
    }

    // If there is a mechanical action, use FixedUpdate if canvas action use Update
    void FixedUpdate()
    {
        // Check if the ball has been caught by a portal in
        if (catchBall)
        {
            // Move the ball into the center of the portal in with the portal suck in speed
            ball.gameObject.transform.position = Vector2.MoveTowards(
                ball.gameObject.transform.position,
                transform.position,
                portalSpeed * Time.deltaTime);
            // If the ball is close enough to the portal center, teleport it and launch it
            if (Vector2.Distance(ball.transform.position, transform.position) < margin)
            {
                // Change the ball caught status, since it is about to be released
                catchBall = false;
                // Release the ball from portal out
                StartCoroutine(LaunchBall(1));
            }
        }
    }

    private IEnumerator LaunchBall(float time)
    {
        // Hide the ball for its teleport duration
        ball.gameObject.SetActive(false);

        if (PlayerPrefs.GetInt("Sounds") == 1)
        {
            AudioSource audio = GetComponent<AudioSource>();
            audio.Play();
        }
        
        yield return new WaitForSeconds(time);

        // If the ball has not been reset by a playerwhile inside teleport teleport and launch the ball
        if (!ball.GetBallReset())
        {
            // Teleport the ball to portal out coordinates
            ball.transform.position = PortalOut.transform.position;
            // make the ball visible again
            ball.gameObject.SetActive(true);
            // Launch the ball
            ThrowOutOfTeleport();
        }
    }

    public void SuckIntoPortal()
    {
        // Change the ball caught status if it has been sucked into the portal
        catchBall = true;
    }

    private void ThrowOutOfTeleport()
    {
        // Launch the ball in the direction of the teleport with the teleport release speed
        ball.Launch(PortalOut.transform.up * releaseSpeed);
        if (PlayerPrefs.GetInt("Sounds") == 1)
        {
            AudioSource audio = GetComponent<AudioSource>();
            audio.Play();
        }
    }

    // This is to set portal out from the script for Challenge levels
    public void SetPortalOut(GameObject portal)
    {
        PortalOut = portal;
    }
}
