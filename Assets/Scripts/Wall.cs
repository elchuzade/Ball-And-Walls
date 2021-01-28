using UnityEngine;
using MoreMountains.NiceVibrations;

public class Wall : MonoBehaviour
{
    // Radius to rotate ball over, inner radius of the wall minus ball radius
    float radius = 52f;
    // Approximately ball radius to do the subtraction
    float ballRadius = 12f;
    // Direction in which the rotation of the wall will happen
    int angularDirection;
    // Toggle when the ball has entered the wall from either side
    bool readyToRotate = false;
    // This is to switch from if to else if in FixedUpdate
    float margin = 0.0001f;
    // Status that changes when the ball is detected inside the wall
    bool wallEntered = false;
    // This is to fix the bug of sending to the center
    bool startedRotation = false;
    // A variable to toggle when hint is used and correct position of a wall is exposed
    bool showCorrectPosition = false;

    // This is to save default color incase it was assigned via script, to return to, when finger is released
    Color32 initialColor;

    [SerializeField] GameObject background;
    [SerializeField] GameObject initPosition;
    [SerializeField] GameObject arrow;
    [SerializeField] GameObject move;
    [SerializeField] GameObject correctPosition;

    [SerializeField] GameObject clockAssist;
    [SerializeField] GameObject clockAssistLong;
    [SerializeField] GameObject counterClockAssist;
    [SerializeField] GameObject counterClockAssistLong;

    // Holding coordinates of correct position incase the wall is horizontal or vertical to show when the hint is used
    Vector3 correctPositionCoordinates;
    // Holding angle of correct position incase the wall is an angular wall to show when hint is used
    Quaternion correctPositionRotation;

    Ball ball;
    HomeStatus homeStatus;

    // Slight zoom animation when the walll is touched
    Animator dragAnimator;

    // Position for launching the ball, changes based on which side the ball enters from
    Vector3 launchPosition;

    void Awake()
    {
        dragAnimator = GetComponent<Animator>();
        ball = FindObjectOfType<Ball>();
        homeStatus = FindObjectOfType<HomeStatus>();
        initialColor = GetComponent<SpriteRenderer>().color;
    }

    void Start()
    {
        // Correct Coordinates are saved before player can touch and move or rotate the wall
        correctPositionCoordinates = correctPosition.transform.position;
        correctPositionRotation = correctPosition.transform.rotation;
    }

    void FixedUpdate()
    {
        // If in both objects, the ball and the wall the entered status is updated, proceed
        if (ball.entered && wallEntered)
        {
            // Get the position of a ball to track it properly
            Vector3 ballPosition = ball.transform.position;
            // Get the distance from the ball to the center of the wall, to see when it goes off the ballRotate distance
            float distance = Vector3.Distance(ballPosition, transform.position);

            // If the ball has moved enough after enetring to be outside of the rotate distance
            if (distance > radius - ballRadius + margin)
            {
                readyToRotate = true;
                // Stop the ball movement
                ball.StopMoving();
                // Teleport it to the position where it will rotate on the curve
                TeleportBall(ballPosition);
            } else if (readyToRotate)
            {
                // If the ball is on the curve and it is ready to be rotated
                startedRotation = true;
                // Rotate the ball around the center of the wall in the rotate direction with rotate speed
                ball.transform.RotateAround(
                    transform.position,
                    ball.transform.forward,
                    Time.fixedDeltaTime * ball.GetAngularSpeed() * angularDirection);
            }
        }
    }

    // When player touches the wall
    public void OnMouseDown()
    {
        homeStatus.CheckPointerMove();

        // If the level is not the last level make it move or rotate, since last level has static walls for animation
        if (homeStatus.GetNextLevel() != 100)
        {
            // If the hint is used and correct positions are exposed
            if (showCorrectPosition)
            {
                // Move the wall to the correct position
                transform.position = correctPositionCoordinates;
                // Rotate the wall to the correct rotation
                transform.rotation = correctPositionRotation;
                // Update current position and rotation
                correctPosition.transform.position = transform.position;
                correctPosition.transform.rotation = transform.rotation;
                if (PlayerPrefs.GetInt("Haptics") == 1)
                {
                    MMVibrationManager.Haptic(HapticTypes.RigidImpact);
                }
            }
            else
            {
                // If the ball has not been launched yet
                if (!homeStatus.GetBallLaunched())
                {
                    // Change the objects of a wall that show when moving or rotating the wall
                    background.SetActive(true);
                    move.SetActive(true);
                    arrow.SetActive(false);
                    // Run the animation for moving or rotating the wall
                    dragAnimator.SetTrigger("Drag");
                    // Make the assisting lines longer for easier alignment
                    ShowAssistLinesLong();
                }
            }
        }
    }

    // When the player has stopped touching the wall
    public void OnMouseUp()
    {
        homeStatus.CheckPointerMove();

        // If the level is not the last level
        if (homeStatus.GetNextLevel() != 100)
        {
            // If the ball is not launched yet and the hint button is not used yet
            if (!homeStatus.GetBallLaunched() && !showCorrectPosition)
            {
                background.SetActive(false);
                move.SetActive(false);
                arrow.SetActive(true);
                // Return the wall state to idle, hiding and showing some objects and shrinking assist lines
                HideAssistLinesLong();
            }
        }
    }

    // Show correct positions for all walls when hint button is used
    public void ShowCorrectPosition()
    {
        correctPosition.transform.position = correctPositionCoordinates;
        correctPosition.transform.rotation = correctPositionRotation;
        correctPosition.SetActive(true);
        showCorrectPosition = true;
    }

    // Check if hint button is touched, return the result
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

    // Move the wall to its correct position if the hint is used
    public void MoveToCorrectPosition()
    {
        transform.position = correctPosition.transform.position;
    }

    private void TeleportBall(Vector3 ballPosition)
    {
        // Find a normalized vector between ball when it has just stepped over the rotating curve inside the wall
        Vector3 normalizedVector = Vector3.Normalize(
        ballPosition - transform.position);

        // Based on the normalized vector and the radius of the wall and ball get the coordinates of the cross of curve and ball vector
        Vector3 radiusDistanceVector = normalizedVector * (radius - ballRadius);
        Vector3 ballFinalPosition = transform.position + radiusDistanceVector;

        // Move the ball to the curve cross
        ball.transform.position = ballFinalPosition;
    }


    // Hide short assist lines and show long assist lines, when the wall is being rotated or moved
    private void ShowAssistLinesLong()
    {
        clockAssistLong.SetActive(true);
        counterClockAssistLong.SetActive(true);

        clockAssist.SetActive(false);
        counterClockAssist.SetActive(false);
    }

    // Show short assist lines and hide long assist lines, when the wall is being rotated or moved
    private void HideAssistLinesLong()
    {
        clockAssistLong.SetActive(false);
        counterClockAssistLong.SetActive(false);

        clockAssist.SetActive(true);
        counterClockAssist.SetActive(true);
    }

    // Check if the wall has detedted a ball with its enter colliders
    public void BallDetected(Vector3 _launchPosition, int _angularDirection)
    {
        angularDirection = _angularDirection;

        // If the ball is inside the wall and it has started rotating
        if (ball.entered && startedRotation)
        {
            wallEntered = false;
            ball.entered = false;
            readyToRotate = false;

            // If the player has not clicked reset button while the ball is rotating inside a wall
            if (!ball.GetBallReset())
            {
                ball.transform.position = launchPosition;

                // Get the two vectors representing ball and wall and find the vector that represents ball's expected launch direction
                Vector3 firstVectorCross = ball.transform.position - transform.position;
                Vector3 secondVectorCross = transform.position - Vector3.forward;
                Vector3 launchVelocity = Vector3.Cross(
                    firstVectorCross, secondVectorCross) * -angularDirection;

                // Launch the ball with that determined vector
                ball.Launch(launchVelocity);
            }
        }
        else
        {
            // If the ball has just entered so it is not supposed to rotate yet, until it crosses over the rotation curve
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

    public Color32 GetInitialColor()
    {
        return initialColor;
    }

    // This is to save color for challenge levels, since the color is being assigned after the object is created,
    // we need to resave initial color for using when finger is released
    public void SaveInitialColor()
    {
        initialColor = GetComponent<SpriteRenderer>().color;
    }
}
