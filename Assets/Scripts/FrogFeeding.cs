using UnityEngine;
using System;

public class FrogFeeding : MonoBehaviour
{
    public int mateFeedThreshold = 3;
    public int maxFeed = 6;

    private int currentFeed = 0;

    public bool hasMated = false;

    public Action OnFeedChanged;

    public GameObject giftBoxPrefab;

    [Header("Gift Spawn Point Name (scene object)")]
    public string giftSpawnPointName = "GiftSpawnPoint";

    private Transform giftSpawnPoint;

    void Start()
    {
        GameObject found = GameObject.Find(giftSpawnPointName);

        if (found != null)
        {
            giftSpawnPoint = found.transform;
        }
        else
        {
            Debug.LogWarning($"GiftSpawnPoint '{giftSpawnPointName}' not found in scene!");
        }
    }

    public bool IsReadyToMate()
    {
        return currentFeed >= mateFeedThreshold && !hasMated;
    }

    public bool CanBeFed()
    {
        return currentFeed < maxFeed && !hasMated;
    }

    public void Feed()
    {
        if (!CanBeFed()) return;

        currentFeed++;
        OnFeedChanged?.Invoke();

        if (currentFeed >= maxFeed)
        {
            SpawnGiftBox();
        }
    }

    void SpawnGiftBox()
    {
        if (giftSpawnPoint == null)
        {
            Debug.LogWarning($"{name}: GiftSpawnPoint not found!");
            return;
        }

        GameObject box = Instantiate(
            giftBoxPrefab,
            giftSpawnPoint.position,
            giftSpawnPoint.rotation
        );

        FrogGiftBox gift = box.GetComponent<FrogGiftBox>();
        if (gift != null)
        {
            gift.Init(GetComponent<FrogIdentity>());
        }

        ResetFeed();
    }

    public int GetFeedCount() => currentFeed;

    public void ResetFeed()
    {
        currentFeed = 0;
        hasMated = false;
        OnFeedChanged?.Invoke();
    }
}