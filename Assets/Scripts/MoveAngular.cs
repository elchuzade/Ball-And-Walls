using UnityEngine;
using MoreMountains.NiceVibrations;

public class MoveAngular : MonoBehaviour
{
    // Shuffle is used to indicate if the wall should be shuffled or stay fixed
    private bool shuffle;
    // Snap to 6 degrees
    private int step = 6;
    // Indicate if player is touching this wall at this time so the rotation can take place
    private bool touching = false;

    private HomeStatus homeStatus;

    // Angle at which the rotation has started,
    // this is to make rotation always proportional to the moved angle of player fingers
    private float initWallAngle;
    // Coordinates at which the rotation has started,
    // this is to make always rotation proportional to the moved angle of player fingers
    private Vector2 initMouseDirection;

    // Angle at which the wall was designed, to show after the hint button is clicked
    private Quaternion hintRotation;

    void Awake()
    {
        homeStatus = FindObjectOfType<HomeStatus>();
    }

    void Start()
    {
        // Check from home script if this level should shuffle its walls
        shuffle = homeStatus.GetShuffle();

        // Save angle of the wall as it was designed for hint usage
        hintRotation = transform.rotation;

        if (shuffle)
        {
            // Make a random shift in angle from 0 to 360 with step of 6 degrees
            float shift = new System.Random().Next(0, 360 / step) * step;
            // Snap the angle of the wall to the nearest angle divisible by step
            shift = SnapAngle(shift);

            // Rotate the wall with that random shift, to hide the solution from a player
            transform.Rotate(0, 0, shift);
        }
        // Save the correct rotation of the wall in the wall script that is attached to the same object
        GetComponent<Wall>().UpdateCorrectRotation(hintRotation);
    }

    void FixedUpdate()
    {
        if (touching)
        {
            // If player is touching the wall, rotate it based on his finger moves
            RotateWall();
        }
    }

    public void OnMouseDown()
    {
        // If player has touhed the wall and the ball is not launched and the hint is not used
        if (!homeStatus.GetBallLaunched() && !GetComponent<Wall>().GetHintTouched())
        {
            // Make the color of the wall a little darker
            GetComponent<SpriteRenderer>().color = new Color32(200, 200, 200, 255);

            touching = true;
            if (PlayerPrefs.GetInt("Haptics") == 1)
            {
                MMVibrationManager.Haptic(HapticTypes.LightImpact);
            }
            // Save current direction of the touch of the player fingers to rotate in proportion to the finger movement
            initMouseDirection = GetDirection();
            // Set the angle of the wall at the time of touching to add to the rotation, to keep it proportional
            initWallAngle = transform.rotation.eulerAngles.z;
        }
    }

    public void OnMouseUp()
    {
        // When the finger is released and ball is not launched and hint is not used
        if (!homeStatus.GetBallLaunched() && !GetComponent<Wall>().GetHintTouched())
        {
            // Make the wall color back to light
            GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 255);

            touching = false;
        }
    }

    // Rotate Wall by player mouse or screen touch
    private void RotateWall()
    {
        // When rotating the wall find direction of the finger at every frame
        Vector2 currentMouseDirection = GetDirection();

        // Find an angle between initial direction and current direction, to get the difference in rotation
        float angle = Vector2.SignedAngle(initMouseDirection, initMouseDirection + currentMouseDirection);

        // Snap that angle to either previous or next divisible by step angle
        angle = SnapAngle(angle);

        // Rotate the wall based on that snapped angle
        transform.rotation = Quaternion.Euler(0, 0, angle + initWallAngle);
    }

    private Vector2 GetDirection()
    {
        // Get current mouse or finger tap coordinates
        Vector3 mouseScreen = Input.mousePosition;
        // Change them to world unit coordinates, since the wall is not on canvas but on the world space
        Vector3 mouse = Camera.main.ScreenToWorldPoint(mouseScreen);
        // Return the resultant distance from object's position to mouse position
        // This is to determine the spot at which the finger tap has taken a place for rotation
        return new Vector2(
            mouse.x - transform.position.x,
            mouse.y - transform.position.y);
    }

    private float SnapAngle(float angle)
    {
        // If the angle is not divisible by step, snap it to ether previous or next divisible angle
        if (angle % step < step)
        {
            // Snap previous angle
            return (int)(angle / step) * step;
        } else
        {
            // Snap next angle
            return (int)(angle / step) * step + step;
        }
    }
}
