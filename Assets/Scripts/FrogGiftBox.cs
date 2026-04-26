using UnityEngine;

public class FrogGiftBox : MonoBehaviour
{
    public FrogIdentity giver;

    private bool opened = false;

    public void Init(FrogIdentity source)
    {
        giver = source;
        Debug.Log($"GiftBox initialized by: {giver.frogType}");
    }

    public void Open()
    {
        if (opened)
        {
            Debug.Log("GiftBox already opened");
            return;
        }

        opened = true;

        Debug.Log("GiftBox clicked!");

        FrogGiftSystem system = FindFirstObjectByType<FrogGiftSystem>();

        if (system == null)
        {
            Debug.LogError("No FrogGiftSystem found in scene!");
            return;
        }

        if (giver == null)
        {
            Debug.LogError("GiftBox has no giver assigned!");
            return;
        }

        system.GiveGift(giver, transform.position);

        Destroy(gameObject);
    }
}