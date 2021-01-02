using UnityEngine;
using MoreMountains.NiceVibrations;
using UnityEngine.UI;

public class MoveAngular : MonoBehaviour
{
    private bool shuffle;

    int step = 6; // Snap to 6 degrees
    bool touching = false;

    HomeStatus homeStatus;

    float initWallAngle;
    Vector2 initMouseDirection;

    Quaternion hintRotation;

    private void Start()
    {
        homeStatus = FindObjectOfType<HomeStatus>();

        shuffle = homeStatus.GetShuffle();

        hintRotation = transform.rotation;

        if (shuffle)
        {
            float shift = new System.Random().Next(0, 360 / step) * step;
            shift = SnapAngle(shift);

            transform.Rotate(0, 0, shift);
        }

        GetComponent<Wall>().UpdateCorrectRotation(hintRotation);
    }

    private void FixedUpdate()
    {
        if (touching)
        {
            RotateWall();
        }
    }

    public void OnMouseDown()
    {
        if (!homeStatus.GetBallLaunched() && !GetComponent<Wall>().GetHintTouched())
        {
            GetComponent<SpriteRenderer>().color = new Color32(200, 200, 200, 255);

            touching = true;
            if (PlayerPrefs.GetInt("Haptics") == 1)
            {
                MMVibrationManager.Haptic(HapticTypes.LightImpact);
            }

            initMouseDirection = GetDirection();

            initWallAngle = transform.rotation.eulerAngles.z;
        }
    }

    public void OnMouseUp()
    {
        if (!homeStatus.GetBallLaunched() && !GetComponent<Wall>().GetHintTouched())
        {
            GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 255);

            touching = false;
        }
    }

    // Rotate Wall by player mouse or screen touch
    private void RotateWall()
    {
        Vector2 currentMouseDirection = GetDirection();

        float angle = Vector2.SignedAngle(initMouseDirection, initMouseDirection + currentMouseDirection);

        angle = SnapAngle(angle);

        transform.rotation = Quaternion.Euler(0, 0, angle + initWallAngle);
    }

    private Vector2 GetDirection()
    {
        Vector3 mouseScreen = Input.mousePosition;
        Vector3 mouse = Camera.main.ScreenToWorldPoint(mouseScreen);
        return new Vector2(
            mouse.x - transform.position.x,
            mouse.y - transform.position.y);
    }

    private float SnapAngle(float angle)
    {
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
