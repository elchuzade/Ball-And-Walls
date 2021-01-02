using UnityEngine;
using UnityEngine.UI;

public class ShopMask : MonoBehaviour
{
    float swipeDistance = 0.2f;
    float swipeReturnSpeed = 0.3f;
    bool swiping;
    float dragLimit = 0.1f;
    float margin = 0.1f;
    int currentSideIndex = 0; // left, middle, right

    float touchSwipeDifference = 0.05f;

    float itemsSectionWidth = 0.8f;
    float itemsCenter = 0.8f * 3 / 2;

    float cellWidth = 0.27f;
    float cellHeight = 0.15f;

    int itemIndex;

    [SerializeField] GameObject[] ballsArray;

    [SerializeField] GameObject leftDot;
    [SerializeField] GameObject middleDot;
    [SerializeField] GameObject rightDot;

    [SerializeField] GameObject leftLimit;
    [SerializeField] GameObject rightLimit;

    [SerializeField] GameObject items;

    Vector3 downMousePosition;
    Vector3 upMousePosition;
    Vector3 mouseToCenter;

    Vector3 expectedPosition;

    private void Start()
    {
        expectedPosition = Camera.main.ScreenToViewportPoint(items.transform.position);

        SwitchDots();
    }

    private void SwitchDots()
    {
        if (currentSideIndex == 0)
        {
            leftDot.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
            middleDot.GetComponent<Image>().color = new Color32(150, 150, 150, 255);
            rightDot.GetComponent<Image>().color = new Color32(150, 150, 150, 255);
        } else if (currentSideIndex == 1)
        {
            leftDot.GetComponent<Image>().color = new Color32(150, 150, 150, 255);
            middleDot.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
            rightDot.GetComponent<Image>().color = new Color32(150, 150, 150, 255);
        } else if (currentSideIndex == 2)
        {
            leftDot.GetComponent<Image>().color = new Color32(150, 150, 150, 255);
            middleDot.GetComponent<Image>().color = new Color32(150, 150, 150, 255);
            rightDot.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        }
    }

    private void Update()
    {
        if (!swiping)
        {
            float newX = Mathf.Lerp(
                Camera.main.ScreenToViewportPoint(items.transform.position).x,
                expectedPosition.x,
                swipeReturnSpeed);

            items.transform.position = Camera.main.ViewportToScreenPoint(new Vector3(
                newX,
                expectedPosition.y,
                expectedPosition.z));

            leftLimit.SetActive(false);
            rightLimit.SetActive(false);
        }
    }

    public void OnPointerDrag()
    {
        if(Camera.main.ScreenToViewportPoint(items.transform.position).x > (itemsCenter + margin - dragLimit - itemsSectionWidth * 2) &&
            Camera.main.ScreenToViewportPoint(items.transform.position).x < (itemsCenter + margin + dragLimit))
        {
            items.transform.position =
            new Vector3(Input.mousePosition.x,
                items.transform.position.y,
                items.transform.position.z) -
            new Vector3(Camera.main.ViewportToScreenPoint(mouseToCenter).x, 0, 0);
        } else
        {
            if (currentSideIndex == 0)
            {
                leftLimit.SetActive(true);
            } else if (currentSideIndex == 2)
            {
                rightLimit.SetActive(true);
            }
        }
    }    

    public void OnPointerDown()
    {
        swiping = true;
        downMousePosition = Input.mousePosition;
        downMousePosition = Camera.main.ScreenToViewportPoint(downMousePosition);
        mouseToCenter = downMousePosition - Camera.main.ScreenToViewportPoint(items.transform.position);
    }

    public void OnPointerUp()
    {
        swiping = false;
        upMousePosition = Input.mousePosition;
        upMousePosition = Camera.main.ScreenToViewportPoint(upMousePosition);

        if (Vector3.Distance(upMousePosition, downMousePosition) < touchSwipeDifference)
        {
            itemIndex = GetPointItemIndex(downMousePosition.x, downMousePosition.y);

            ballsArray[itemIndex].GetComponent<Item>().TouchItem();
        } else
        {
            // Snap to the correct position
            if (upMousePosition.x < downMousePosition.x)
            {
                // Swipe left
                if (currentSideIndex < 2)
                {
                    if (Mathf.Abs(upMousePosition.x - downMousePosition.x) > swipeDistance)
                    {
                        SnapNextItems();
                    }
                }

            }
            else if (upMousePosition.x > downMousePosition.x)
            {
                // swipe right
                if (currentSideIndex > 0)
                {
                    if (Mathf.Abs(upMousePosition.x - downMousePosition.x) > swipeDistance)
                    {
                        SnapPreviousItems();
                    }
                }
            }

            expectedPosition = new Vector3(
                itemsCenter + margin - itemsSectionWidth * currentSideIndex,
                expectedPosition.y,
                0);
        }
    }

    private void SnapNextItems()
    {
        currentSideIndex++;

        SwitchDots();
    }

    private void SnapPreviousItems()
    {
        currentSideIndex--;

        SwitchDots();
    }

    private int GetPointItemIndex(float x, float y)
    {
        x -= 0.1f; // To map to 0,0 Axis
        y -= 0.725f; // To map to 0,0 Axis
        y = -y; // To map to 0,0 Axis


        int xIndex = (int)(x / cellWidth);
        int yIndex = (int)(y / cellHeight);

        return yIndex * 3 + xIndex + currentSideIndex * 9;
    }
}
