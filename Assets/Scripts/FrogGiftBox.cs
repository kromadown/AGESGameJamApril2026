using UnityEngine;

public class FrogGiftBox : MonoBehaviour
{
    public FrogIdentity giver;

    private bool opened = false;

    public void Init(FrogIdentity source)
    {
        giver = source;
    }

    public void Open()
    {
        Debug.Log("OPEN CALLED! Frame: " + Time.frameCount);
        Debug.Log("CALL STACK:\n" + System.Environment.StackTrace);   

        if (opened) return;
        opened = true;

        FrogGiftSystem system = FindFirstObjectByType<FrogGiftSystem>();

        if (system != null && giver != null)
        {
            system.GiveGift(giver, transform.position);
        }

        Destroy(gameObject);
    }
}