using System.Collections;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField] GameObject PortalOut;

    private bool catchBall;
    Ball ball;
    float margin = 0.01f;

    private void Start()
    {
        ball = FindObjectOfType<Ball>();
    }

    private void FixedUpdate()
    {
        if (catchBall)
        {
            ball.gameObject.transform.position = Vector2.MoveTowards(ball.gameObject.transform.position, transform.position, 500 * Time.deltaTime);
            if (Vector2.Distance(ball.transform.position, transform.position) < margin)
            {
                catchBall = false;
                StartCoroutine(LaunchBall());
            }
        }
    }

    private IEnumerator LaunchBall()
    {
        ball.gameObject.SetActive(false);
        
        yield return new WaitForSeconds(1);

        if (!ball.GetBallReset())
        {
            ball.transform.position = PortalOut.transform.position;
            ball.gameObject.SetActive(true);
            ThrowOutOfTeleport();
        }
    }

    public void SuckIntoPortal()
    {
        AudioSource audio = GetComponent<AudioSource>();
        audio.Play();

        catchBall = true;
    }

    private void ThrowOutOfTeleport()
    {
        AudioSource audio = GetComponent<AudioSource>();
        audio.Play();

        ball.Launch(PortalOut.transform.up * 30);
    }
}
