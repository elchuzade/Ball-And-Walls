using System;
using UnityEngine;
using MoreMountains.NiceVibrations;

public class MoveVertical : MonoBehaviour
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

            shift = SnapVertical(shift);

            if (transform.position.y + shift < 260)
            {
                transform.position = new Vector3(transform.position.x, 260, transform.position.z);
            }
            else if (transform.position.y + shift > 940)
            {
                transform.position = new Vector3(transform.position.x, 940, transform.position.z);
            }
            else
            {
                transform.position += new Vector3(0, shift, 0);
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
        if (!homeStatus.GetBallLaunched() &&
            !GetComponent<Wall>().GetHintTouched() &&
            homeStatus.GetNextLevel() != 100)
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
            homeStatus.HideFocusPinterAfterHintVertical();
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

        if (Math.Abs(currentMouse.y - touchMouseToCenter.y - initialPosition.transform.position.y) <= moveDistance)
        {
            float newY = transform.position.y + (currentMouseToCenter.y - touchMouseToCenter.y);
            newY = SnapVertical(newY);

            // withing the move boundaries
            transform.position = new Vector3(
                transform.position.x,
                newY,
                transform.position.z);
        }
        else
        {
            // outside the move boundaries
            if (currentMouse.y - touchMouseToCenter.y < initialPosition.transform.position.y - 100)
            {
                // Left end
                transform.position = new Vector3(
                transform.position.x,
                initialPosition.transform.position.y - moveDistance,
                transform.position.z);
            }
            else if (currentMouse.y - touchMouseToCenter.y > initialPosition.transform.position.y + 100)
            {
                // Right end
                transform.position = new Vector3(
                transform.position.x,
                initialPosition.transform.position.y + moveDistance,
                transform.position.z);
            }
        }
    }

    private float SnapVertical(float yCoord)
    {
        if (yCoord % step < step / 2)
        {
            // Snap previous y coordinate
            return (int)(yCoord / step) * step;
        } else
        {
            // Snap next y coordinate
            return (int)(yCoord / step) * step + step;
        }
    }
}
