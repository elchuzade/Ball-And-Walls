using UnityEngine;

public class BallClick : MonoBehaviour
{
    public void OnMouseDown()
    {
        transform.parent.GetComponent<Ball>().OnMouseDown();
    }
}
