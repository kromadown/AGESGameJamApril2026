using UnityEngine;
using System;

public class FrogFeeding : MonoBehaviour
{
    public int mateFeedThreshold = 3;
    public int maxFeed = 4;

    private int currentFeed = 0;

    public bool hasMated = false;

    public Action OnFeedChanged;

    public GameObject giftBoxPrefab;

    [Header("Gift Spawn Point Name (scene object)")]
    public string giftSpawnPointName = "GiftSpawnPoint";

    private Transform giftSpawnPoint;

    private FrogIdentity identity;

    // 🧠 CYCLE TRACKING
    private FrogType cycleFoodType = FrogType.None;
    private bool isPerfectCycle = true;

    void Start()
    {
        identity = GetComponent<FrogIdentity>();

        GameObject found = GameObject.Find(giftSpawnPointName);

        if (found != null)
            giftSpawnPoint = found.transform;
        else
            Debug.LogWarning($"GiftSpawnPoint '{giftSpawnPointName}' not found in scene!");
    }

    public bool IsReadyToMate()
    {
        return currentFeed >= mateFeedThreshold && !hasMated;
    }

    public bool CanBeFed()
    {
        return currentFeed < maxFeed && !hasMated;
    }

    // =========================
    // 🍽 UNIFIED FEED
    // =========================
    public void Feed(FrogType foodType)
    {
        if (!CanBeFed()) return;

        currentFeed++;
        OnFeedChanged?.Invoke();

        Debug.Log($"[Feed] {name} got {foodType} ({currentFeed}/{maxFeed})");

        TrackCycle(foodType);

        if (currentFeed >= maxFeed)
        {
            ResolveCycle();
        }
    }

    // =========================
    // 🧠 TRACK CYCLE
    // =========================
    void TrackCycle(FrogType foodType)
    {
        if (currentFeed == 1)
        {
            cycleFoodType = foodType;
            isPerfectCycle = (foodType != FrogType.None);
            return;
        }

        if (foodType != cycleFoodType || foodType == FrogType.None)
        {
            isPerfectCycle = false;
        }
    }

    // =========================
    // 🎯 RESOLVE RESULT
    // =========================
    void ResolveCycle()
    {
        bool unlocked = false;

        if (isPerfectCycle && cycleFoodType != FrogType.None)
        {
            Debug.Log($"🔥 PERFECT CYCLE: {identity.frogType} + {cycleFoodType}");

            if (FrogUnlockManager.Instance != null)
            {
                unlocked = FrogUnlockManager.Instance.TryUnlock(
                    identity.frogType,
                    cycleFoodType
                );
            }
        }

        // 🎁 fallback if no unlock
        if (!unlocked)
        {
            SpawnGiftBox();
        }

        ResetFeed();
    }

    // =========================
    // 🎁 NORMAL GIFT BOX
    // =========================
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
            gift.Init(identity);

        Debug.Log("[Feed] Gift spawned");
    }

    public int GetFeedCount() => currentFeed;

    public void ResetFeed()
    {
        currentFeed = 0;
        hasMated = false;

        cycleFoodType = FrogType.None;
        isPerfectCycle = true;

        OnFeedChanged?.Invoke();
    }
}