using UnityEngine;
using MoreMountains.NiceVibrations;

public class Wall : MonoBehaviour
{
    float radius = 52f;
    float ballRadius = 12f;
    int angularDirection;
    bool readyToRotate = false;
    float margin = 0.0001f; // This is to switch from if to else if in FixedUpdate

    bool wallEntered = false;
    bool startedRotation = false; // This is to fix the bug of sending to the center
    bool showCorrectPosition = false;

    [SerializeField] GameObject background;
    [SerializeField] GameObject initPosition;
    [SerializeField] GameObject arrow;
    [SerializeField] GameObject move;
    [SerializeField] GameObject correctPosition;

    [SerializeField] GameObject clockAssist;
    [SerializeField] GameObject clockAssistLong;
    [SerializeField] GameObject counterClockAssist;
    [SerializeField] GameObject counterClockAssistLong;

    Vector3 correctPositionCoordinates;
    Quaternion correctPositionRotation;

    Ball ball;
    HomeStatus homeStatus;

    Animator dragAnimator;

    //Vector3 mouseTouchToCenter;
    Vector3 launchPosition;

    // Start is called before the first frame update
    void Start()
    {
        dragAnimator = GetComponent<Animator>();

        correctPositionCoordinates = correctPosition.transform.position;
        correctPositionRotation = correctPosition.transform.rotation;
        ball = FindObjectOfType<Ball>();
        homeStatus = FindObjectOfType<HomeStatus>();
    }

    private void FixedUpdate()
    {
        if (ball.entered && wallEntered)
        {
            Vector3 ballPosition = ball.transform.position;
            float distance = Vector3.Distance(ballPosition, transform.position);

            if (distance > radius - ballRadius + margin)
            {
                readyToRotate = true;
                ball.StopMoving();
                TeleportBall(ballPosition);
            } else if (readyToRotate)
            {
                startedRotation = true;
                ball.transform.RotateAround(
                    transform.position,
                    ball.transform.forward,
                    Time.fixedDeltaTime * ball.GetAngularSpeed() * angularDirection); // Speed inside wall
            }
        }
    }

    public void OnMouseDown()
    {
        homeStatus.CheckPointerMove();

        if (homeStatus.GetNextLevel() != 100)
        {
            if (showCorrectPosition)
            {
                transform.position = correctPositionCoordinates;
                transform.rotation = correctPositionRotation;
                correctPosition.transform.position = transform.position;
                correctPosition.transform.rotation = transform.rotation;
                if (PlayerPrefs.GetInt("Haptics") == 1)
                {
                    MMVibrationManager.Haptic(HapticTypes.RigidImpact);
                }
            }
            else
            {
                if (!homeStatus.GetBallLaunched())
                {
                    background.SetActive(true);
                    move.SetActive(true);
                    arrow.SetActive(false);
                    dragAnimator.SetTrigger("Drag");
                    ShowAssistLinesLong();
                }
            }
        }
    }

    public void OnMouseUp()
    {
        homeStatus.CheckPointerMove();

        if (homeStatus.GetNextLevel() != 100)
        {
            if (!homeStatus.GetBallLaunched() && !showCorrectPosition)
            {
                background.SetActive(false);
                move.SetActive(false);
                arrow.SetActive(true);
                HideAssistLinesLong();
            }
        }
    }

    public void ShowCorrectPosition()
    {
        correctPosition.transform.position = correctPositionCoordinates;
        correctPosition.transform.rotation = correctPositionRotation;
        correctPosition.SetActive(true);
        showCorrectPosition = true;
    }

    public bool GetHintTouched()
    {
        if (correctPositionCoordinates == transform.position &&
            correctPositionRotation == transform.rotation &&
            showCorrectPosition)
        {
            return true;
        }
        return false;
    }

    public void MoveToCorrectPosition()
    {
        transform.position = correctPosition.transform.position;
    }

    private void TeleportBall(Vector3 ballPosition)
    {

        Vector3 normalizedVector = Vector3.Normalize(
        ballPosition - transform.position);

        Vector3 radiusDistanceVector = normalizedVector * (radius - ballRadius);

        Vector3 ballFinalPosition = transform.position + radiusDistanceVector;

        ball.transform.position = ballFinalPosition;
    }

    private void ShowAssistLinesLong()
    {
        clockAssistLong.SetActive(true);
        counterClockAssistLong.SetActive(true);

        clockAssist.SetActive(false);
        counterClockAssist.SetActive(false);
    }

    private void HideAssistLinesLong()
    {
        clockAssistLong.SetActive(false);
        counterClockAssistLong.SetActive(false);

        clockAssist.SetActive(true);
        counterClockAssist.SetActive(true);
    }

    public void BallDetected(Vector3 _launchPosition, int _angularDirection)
    {
        angularDirection = _angularDirection;

        if (ball.entered && startedRotation)
        {
            wallEntered = false;
            ball.entered = false;
            readyToRotate = false;

            if (!ball.GetBallReset())
            {
                ball.transform.position = launchPosition;

                // Launch the Ball
                Vector3 firstVectorCross = ball.transform.position - transform.position;
                Vector3 secondVectorCross = transform.position - Vector3.forward;
                Vector3 launchVelocity = Vector3.Cross(
                    firstVectorCross, secondVectorCross) * -angularDirection;

                ball.Launch(launchVelocity);
            }
        }
        else
        {
            launchPosition = _launchPosition;
            ball.entered = true;
            wallEntered = true;

            if (PlayerPrefs.GetInt("Haptics") == 1)
            {
                MMVibrationManager.Haptic(HapticTypes.RigidImpact);
            }
        }
    }

    public void UpdateCorrectPosition(Vector3 _correctPosition)
    {
        correctPosition.transform.position = _correctPosition;
    }

    public void UpdateCorrectRotation(Quaternion _correctRotation)
    {
        correctPosition.transform.rotation = _correctRotation;
    }
}
