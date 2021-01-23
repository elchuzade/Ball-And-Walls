using UnityEngine;
using UnityEngine.UI;

public class ShopMask : MonoBehaviour
{
    // Distance more than which player needs to swipe to open next or previous list
    float swipeDistance = 0.2f;
    // Speed with which the balls list returns to its normal place when not swiped fully
    float swipeReturnSpeed = 0.3f;
    // Status of eitehr swiping or tapping the screen
    bool swiping;
    // Distance for how much in world space player can drag the list away from either side
    float dragLimit = 0.1f;
    // Tolerance for coordinates not matching during drag and return
    float margin = 0.1f;
    // Which 9 items are shown in the list. left = 0, middleLeft = 1, middleRight = 2, right = 3
    int currentSideIndex = 0;
    // How much of a movement in any direction is considered as swiping
    float touchSwipeDifference = 0.05f;
    // Width for all balls to fit in world space
    float itemsSectionWidth = 0.8f;
    // Since center of the wide list is changing based on which 9 items are shown, we need to hold its center to move
    float itemsCenter = 0.8f * 2;
    // Each item cell width
    float cellWidth = 0.27f;
    // Each item cell height
    float cellHeight = 0.15f;
    // Which ball is touched based on its index
    int itemIndex;

    // Number of transitions that can be made in one direction to reach the edge
    int transitions;

    // Dots that indicate which 9 items are shown on the screen
    GameObject leftDot;
    GameObject midLeftDot;
    GameObject midRightDot;
    GameObject rightDot;

    GameObject leftLimit;
    GameObject rightLimit;

    [SerializeField] GameObject items;

    Vector3 downMousePosition;
    Vector3 upMousePosition;
    Vector3 mouseToCenter;

    Vector3 expectedPosition;

    void Awake()
    {
        leftDot = GameObject.Find("LeftDot");    
        rightDot = GameObject.Find("RightDot");
        midLeftDot = GameObject.Find("MidLeftDot");
        midRightDot = GameObject.Find("MidRightDot");

        leftLimit = GameObject.Find("LeftLimit");
        rightLimit = GameObject.Find("RightLimit");
    }

    void Start()
    {
        // Hide limit stick for dragging the window
        leftLimit.SetActive(false);
        rightLimit.SetActive(false);

        // 9 coz there are 9 items in a single window, -1 coz there are 1 less transitions then windows
        transitions = items.transform.childCount / 9 - 1;

        // This is the position of the total wide list of items in the current location
        // This can be either the next 9 items list, the prev 9 items list or current 9 items list
        expectedPosition = Camera.main.ScreenToViewportPoint(items.transform.position);

        SwitchDots();
    }

    private void SwitchDots()
    {
        // Based on which side of the wide list the player is at, make the corresponding dot lighter
        if (currentSideIndex == 0)
        {
            leftDot.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
            midLeftDot.GetComponent<Image>().color = new Color32(150, 150, 150, 255);
            midRightDot.GetComponent<Image>().color = new Color32(150, 150, 150, 255);
            rightDot.GetComponent<Image>().color = new Color32(150, 150, 150, 255);
        }
        else if (currentSideIndex == 1)
        {
            leftDot.GetComponent<Image>().color = new Color32(150, 150, 150, 255);
            midLeftDot.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
            midRightDot.GetComponent<Image>().color = new Color32(150, 150, 150, 255);
            rightDot.GetComponent<Image>().color = new Color32(150, 150, 150, 255);
        }
        else if (currentSideIndex == 2)
        {
            leftDot.GetComponent<Image>().color = new Color32(150, 150, 150, 255);
            midLeftDot.GetComponent<Image>().color = new Color32(150, 150, 150, 255);
            midRightDot.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
            rightDot.GetComponent<Image>().color = new Color32(150, 150, 150, 255);
        }
        else if (currentSideIndex == 3)
        {
            leftDot.GetComponent<Image>().color = new Color32(150, 150, 150, 255);
            midLeftDot.GetComponent<Image>().color = new Color32(150, 150, 150, 255);
            midRightDot.GetComponent<Image>().color = new Color32(150, 150, 150, 255);
            rightDot.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        }
    }

    private void Update()
    {
        if (!swiping)
        {
            // If player has released his finger after swiping find the X coordinate for moving player towards expected 9 items center
            float newX = Mathf.Lerp(
                Camera.main.ScreenToViewportPoint(items.transform.position).x,
                expectedPosition.x,
                swipeReturnSpeed);

            // Move the list towards expected position
            items.transform.position = Camera.main.ViewportToScreenPoint(new Vector3(
                newX,
                expectedPosition.y,
                expectedPosition.z));

            // Hide both drag limit sticks
            leftLimit.SetActive(false);
            rightLimit.SetActive(false);
        }
    }

    public void OnPointerDrag()
    {
        // If you dragged within the drag limits move the wide list based on finger
        if(Camera.main.ScreenToViewportPoint(items.transform.position).x > (itemsCenter + margin - dragLimit - itemsSectionWidth * transitions) &&
            Camera.main.ScreenToViewportPoint(items.transform.position).x < (itemsCenter + margin + dragLimit))
        {
            items.transform.position =
            new Vector3(Input.mousePosition.x,
                items.transform.position.y,
                items.transform.position.z) -
            new Vector3(Camera.main.ViewportToScreenPoint(mouseToCenter).x, 0, 0);
        } else
        {
            // Otherwise show the drag limit sticks
            if (currentSideIndex == 0)
            {
                leftLimit.SetActive(true);
            } else if (currentSideIndex == transitions)
            {
                rightLimit.SetActive(true);
            }
        }
    }    

    public void OnPointerDown()
    {
        // When started swiping save coordinates of where the touch has happened based on center
        swiping = true;
        downMousePosition = Input.mousePosition;
        downMousePosition = Camera.main.ScreenToViewportPoint(downMousePosition);
        mouseToCenter = downMousePosition - Camera.main.ScreenToViewportPoint(items.transform.position);
    }

    public void OnPointerUp()
    {
        // When stopped swiping 
        swiping = false;
        // Save mouse positions of where the finger was released
        upMousePosition = Input.mousePosition;
        upMousePosition = Camera.main.ScreenToViewportPoint(upMousePosition);

        // If the difference of coordinates of touching and releasing is less than swipe distance consider it a click not a swipe
        if (Vector3.Distance(upMousePosition, downMousePosition) < touchSwipeDifference)
        {
            // Get index of item that was clicked
            itemIndex = GetPointItemIndex(downMousePosition.x, downMousePosition.y);

            // Call touch item to either unlock or select the item
            items.transform.GetChild(itemIndex).GetComponent<Item>().TouchItem();
        } else
        {
            // Otherwsie the distance of swiping is large enough to consider a swipe action
            // Finger release is on the left of finger touch swipe left
            if (upMousePosition.x < downMousePosition.x)
            {
                // Swipe Left
                // Check if the right most limit has not been reached yet
                if (currentSideIndex < transitions)
                {
                    // If the swipe is big enough to open the next list show next 9 items
                    if (Mathf.Abs(upMousePosition.x - downMousePosition.x) > swipeDistance)
                    {
                        SnapNextItems();
                    }
                }
            }
            // Finger release is on the right of finger touch swipe right
            else if (upMousePosition.x > downMousePosition.x)
            {
                // Swipe Right
                // Check if the left most limit has not been reached yet
                if (currentSideIndex > 0)
                {
                    // If the swipe is big enough to open the previous list show previous 9 items
                    if (Mathf.Abs(upMousePosition.x - downMousePosition.x) > swipeDistance)
                    {
                        SnapPreviousItems();
                    }
                }
            }

            // Update expected position of the list based on a new 9 items list
            expectedPosition = new Vector3(
                itemsCenter + margin - itemsSectionWidth * currentSideIndex,
                expectedPosition.y,
                0);
        }
    }

    private void SnapNextItems()
    {
        // Update current side and update dots accordingly to next one
        currentSideIndex++;

        SwitchDots();
    }

    private void SnapPreviousItems()
    {
        // Update current side and update dots accordingly to previous one
        currentSideIndex--;

        SwitchDots();
    }

    private int GetPointItemIndex(float x, float y)
    {
        x -= 0.1f; // To map to 0,0 Axis
        y -= 0.725f; // To map to 0,0 Axis
        y = -y; // To map to 0,0 Axis

        // Find index of an item clicked on the screen based on its x and y coordinates
        // and index of a 9 items list
        int xIndex = (int)(x / cellWidth);
        int yIndex = (int)(y / cellHeight);

        return yIndex * transitions + xIndex + currentSideIndex * 9;
    }
}
