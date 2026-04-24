using UnityEngine;

public class FrogFeeding : MonoBehaviour
{
    public int maxFeed = 3;
    private int currentFeed = 0;

    public bool CanBeFed()
    {
        return currentFeed < maxFeed;
    }

    public void Feed()
    {
        if (!CanBeFed()) return;

        currentFeed++;

        Debug.Log(name + " fed! (" + currentFeed + "/" + maxFeed + ")");

        if (currentFeed >= maxFeed)
        {
            Debug.Log(name + " is full!");
        }
    }

    public int GetFeedCount()
    {
        return currentFeed;
    }
}