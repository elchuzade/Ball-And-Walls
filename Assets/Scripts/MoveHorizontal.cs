using System;
using UnityEngine;
using MoreMountains.NiceVibrations;

public class MoveHorizontal : MonoBehaviour
{
    [SerializeField] GameObject initialPosition;
    private bool shuffle;

    int step = 10; // Snap vertically to 10 pixels

    int moveDistance = 100;
    bool touching = false;
    Vector3 touchMouseToCenter;

    HomeStatus homeStatus;

    Vector3 hintPosition;

    private void Start()
    {
        homeStatus = FindObjectOfType<HomeStatus>();

        shuffle = homeStatus.GetShuffle();

        hintPosition = transform.position;

        if (shuffle)
        {
            float shift = new System.Random().Next(-100 / step, 100 / step) * step;

            shift = SnapHorizontal(shift);

            if (transform.position.x + shift < 195)
            {
                transform.position = new Vector3(195, transform.position.y, transform.position.z);
            }
            else if (transform.position.x + shift > 555)
            {
                transform.position = new Vector3(555, transform.position.y, transform.position.z);
            }
            else
            {
                transform.position += new Vector3(shift, 0, 0);
            }
        }

        GameObject initialPositions = GameObject.Find("InitialPositions");
        initialPosition.transform.SetParent(initialPositions.transform, true);

        GetComponent<Wall>().UpdateCorrectPosition(hintPosition);
    }

    private void FixedUpdate()
    {
        if (touching)
        {
            MoveWall();
        }
    }

    public void OnMouseDown()
    {
        if (!homeStatus.GetBallLaunched() && !GetComponent<Wall>().GetHintTouched())
        {
            GetComponent<SpriteRenderer>().color = new Color32(200, 200, 200, 255);

            Vector3 mouseScreen = Input.mousePosition;
            Vector3 touchMouse = Camera.main.ScreenToWorldPoint(mouseScreen);
            touchMouseToCenter = touchMouse - transform.position;

            touching = true;
            if (PlayerPrefs.GetInt("Haptics") == 1)
            {
                MMVibrationManager.Haptic(HapticTypes.LightImpact);
            }
        }
        else
        {
            homeStatus.HideFocusPinterAfterHintHorizontal();
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

    // Move Wall by player mouse or screen touch
    private void MoveWall()
    {
        Vector3 mouseScreen = Input.mousePosition;
        Vector3 currentMouse = Camera.main.ScreenToWorldPoint(mouseScreen);
        Vector3 currentMouseToCenter = currentMouse - transform.position;

        if (Math.Abs(currentMouse.x - touchMouseToCenter.x - initialPosition.transform.position.x) <= moveDistance)
        {
            float newX = transform.position.x + (currentMouseToCenter.x - touchMouseToCenter.x);

            newX = SnapHorizontal(newX);

            // withing the move boundaries
            transform.position = new Vector3(
                newX,
                transform.position.y,
                transform.position.z);

        } else
        {
            // outside the move boundaries
            if (currentMouse.x - touchMouseToCenter.x < initialPosition.transform.position.x - 100)
            {
                // Left end
                transform.position = new Vector3(
                initialPosition.transform.position.x - moveDistance,
                transform.position.y,
                transform.position.z);
            }
            else if (currentMouse.x - touchMouseToCenter.x > initialPosition.transform.position.x + 100)
            {
                // Right end
                transform.position = new Vector3(
                initialPosition.transform.position.x + moveDistance,
                transform.position.y,
                transform.position.z);
            }
        }
    }

    private float SnapHorizontal(float xCoord)
    {
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
