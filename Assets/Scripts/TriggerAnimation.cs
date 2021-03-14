using UnityEngine;

public class TriggerAnimation : MonoBehaviour
{
    Animator anim;

    void Start()
    {
        anim = gameObject.GetComponent<Animator>();
    }

    public void Trigger()
    {
        anim.SetTrigger("Start");
    }
}
