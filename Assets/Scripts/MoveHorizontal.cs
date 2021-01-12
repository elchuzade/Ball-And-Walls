using System;
using UnityEngine;
using MoreMountains.NiceVibrations;

public class MoveHorizontal : MonoBehaviour
{
    private GameObject initialPosition;
    // Shuffle is used to indicate if the wall should be shuffled or stay fixed
    private bool shuffle;
    // Snap vertically to 10 pixels
    private int step = 10; // Snap vertically to 10 pixels
    // Distance to which the wall can be moved before it hits the limits
    private int moveDistance = 100;
    // Indicate if player is touching this wall at this time so the rotation can take place
    private bool touching = false;
    // Coordinates from the mouse to the center of the wall when player touches the wall to rotate
    private Vector3 touchMouseToCenter;

    private HomeStatus homeStatus;

    // Position at which the wall was designed, to show after the hint button is clicked
    private Vector3 hintPosition;

    // Limits of the screen space incase shuffle moves the wall outside of them
    private int upClampLimit = 195;
    private int downClampLimit = 555;

    void Awake()
    {
        homeStatus = FindObjectOfType<HomeStatus>();
        initialPosition = transform.Find("InitialPosition").gameObject;
    }

    void Start()
    {
        // Check from home script if this level should shuffle its walls
        shuffle = homeStatus.GetShuffle();

        // Save position of the wall as it was designed for hint usage
        hintPosition = transform.position;

        if (shuffle)
        {
            // Make a random shift in distance from -100 to 100 with step of 10 pixels
            float shift = new System.Random().Next(-100 / step, 100 / step) * step;

            // Snap the coordinate of the wall to the nearest coordinate divisible by step
            shift = SnapHorizontal(shift);

            if (transform.position.x + shift < upClampLimit)
            {
                transform.position = new Vector3(upClampLimit, transform.position.y, transform.position.z);
            }
            else if (transform.position.x + shift > downClampLimit)
            {
                transform.position = new Vector3(downClampLimit, transform.position.y, transform.position.z);
            }
            else
            {
                // Otherwise move the wall with random shift coordinates
                transform.position += new Vector3(shift, 0, 0);
            }
        }

        // Take the movement directing stick outside of the wall so when the wall is moved stick does not move with it
        // Combine all sticks in a single component as children of InitialPositions gameObject
        GameObject initialPositions = GameObject.Find("InitialPositions");
        initialPosition.transform.SetParent(initialPositions.transform, true);

        // Save the correct position of the wall in the wall script that is attached to the same object
        GetComponent<Wall>().UpdateCorrectPosition(hintPosition);
    }

    void FixedUpdate()
    {
        if (touching)
        {
            // If player is touching the wall, move it based on his finger moves
            MoveWall();
        }
    }

    public void OnMouseDown()
    {
        // If player has touhed the wall and the ball is not launched and the hint is not used and this is not the last level
        if (!homeStatus.GetBallLaunched() && !GetComponent<Wall>().GetHintTouched())
        {
            // Make the color of the wall a little darker
            GetComponent<SpriteRenderer>().color = new Color32(200, 200, 200, 255);

            touching = true;
            if (PlayerPrefs.GetInt("Haptics") == 1)
            {
                MMVibrationManager.Haptic(HapticTypes.LightImpact);
            }

            // Save current direction of the touch of the player fingers to move in proportion to the finger movement
            touchMouseToCenter = GetPosition();
        }
        else
        {
            homeStatus.HideFocusPinterAfterHintHorizontal();
        }
    }

    public void OnMouseUp()
    {
        // If ball is not launched yet and hint is not used and mouse is not touched any more, make color darker
        if (!homeStatus.GetBallLaunched() && !GetComponent<Wall>().GetHintTouched())
        {
            GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 255);

            touching = false;
        }
    }

    // Move Wall by player mouse or screen touch
    private void MoveWall()
    {
        // Get current mouse or finger tap coordinates
        Vector3 mouseScreen = Input.mousePosition;
        // Change them to world unit coordinates, since the wall is not on canvas but on the world space
        Vector3 currentMouse = Camera.main.ScreenToWorldPoint(mouseScreen);
        // Find distance from object's position to mouse position
        Vector3 currentMouseToCenter = currentMouse - transform.position;

        // If movement of a mouse in vertical direction is less than move limit,
        // then move based on mouse Y coordinate, else snap it to up and down move limits
        if (Math.Abs(currentMouse.x - touchMouseToCenter.x - initialPosition.transform.position.x) <= moveDistance)
        {
            // Find a new Y coordinate based on the new mouse position
            float newX = transform.position.x + (currentMouseToCenter.x - touchMouseToCenter.x);
            // Snap it to previous or next step of 10 pixels
            newX = SnapHorizontal(newX);

            // Move the wall to the new snapped coordinates
            // within the boundaries
            transform.position = new Vector3(
                newX,
                transform.position.y,
                transform.position.z);

        } else
        {
            // outside the move boundaries
            if (currentMouse.x - touchMouseToCenter.x < initialPosition.transform.position.x - 100)
            {
                // Move the ball to the left limit coordinates
                transform.position = new Vector3(
                initialPosition.transform.position.x - moveDistance,
                transform.position.y,
                transform.position.z);
            }
            else if (currentMouse.x - touchMouseToCenter.x > initialPosition.transform.position.x + 100)
            {
                // Move the ball to the right limit coordinates
                transform.position = new Vector3(
                initialPosition.transform.position.x + moveDistance,
                transform.position.y,
                transform.position.z);
            }
        }
    }

    private Vector3 GetPosition()
    {
        // Get current mouse or finger tap coordinates
        Vector3 mouseScreen = Input.mousePosition;
        // Change them to world unit coordinates, since the wall is not on canvas but on the world space
        Vector3 touchMouse = Camera.main.ScreenToWorldPoint(mouseScreen);
        // Return the resultant distance from object's position to mouse position
        // This is to determine the spot at which the finger tap has taken a place for moving
        return touchMouse - transform.position;
    }

    private float SnapHorizontal(float xCoord)
    {
        // If the coordiante is not divisible by half of a step, snap it to ether previous or next divisible coordinate
        if (xCoord % step < step / 2)
        {
            // Snap previous y coordinate
            return (int)(xCoord / step) * step;
        }
        else
        {
            // Snap next y coordinate
            return (int)(xCoord / step) * step + step;
        }
    }
}
