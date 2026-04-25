using UnityEngine;

public class FrogFeeding : MonoBehaviour
{
    public int maxFeed = 3;
    private int currentFeed = 0;

    public bool hasMated = false;

    public bool IsReadyToMate()
    {
        return currentFeed >= maxFeed && !hasMated;
    }

    public bool CanBeFed()
    {
        return currentFeed < maxFeed && !hasMated;
    }

    public void Feed()
    {
        if (!CanBeFed()) return;

        currentFeed++;

        Debug.Log(name + " fed " + currentFeed + "/" + maxFeed);
    }

    public int GetFeedCount()
    {
        return currentFeed;
    }
}