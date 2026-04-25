using UnityEngine;

public class FrogIdentity : MonoBehaviour
{
    public FrogType frogType;

    [Header("Feeding")]
    public int feedCount = 0;
    public int requiredFeeds = 3;

    public bool CanBeSelectedForBreeding()
    {
        return feedCount >= requiredFeeds;
    }

    public void Feed()
    {
        if (feedCount >= requiredFeeds)
            return;

        feedCount++;
    }
}