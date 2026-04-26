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

    // =========================
    // 🍎 SPECIAL FOOD TRACKING (SINGLE TYPE ONLY)
    // =========================
    private FrogType lastSpecialFood = FrogType.None;
    private int specialStreak = 0;

    private const int specialLimit = 4;
    private FrogSpecialFoodTracker tracker;

    void Start()
    {
        GameObject found = GameObject.Find(giftSpawnPointName);

        tracker = GetComponent<FrogSpecialFoodTracker>();

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
    // 🍽 NORMAL FEEDING
    // =========================
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

    // =========================
    // 🍎 SPECIAL FOOD FEEDING (IMPORTANT RULE)
    // =========================
    public void FeedSpecial(FrogType type)
    {
        if (type == FrogType.None)
            return;

        FrogType frogType = GetComponent<FrogIdentity>().frogType;

        // =========================
        // 🎯 VALID COMBINATION RULES
        // =========================
        bool isValid =
            (frogType == FrogType.AB && (type == FrogType.A || type == FrogType.B)) ||
            (frogType == FrogType.AC && (type == FrogType.A || type == FrogType.C)) ||
            (frogType == FrogType.BC && (type == FrogType.B || type == FrogType.C));

        if (!isValid)
        {
            Debug.Log($"[Special Feed] {frogType} cannot accept {type}");
            return;
        }

        // =========================
        // 🔄 STREAK LOGIC
        // =========================
        if (type != lastSpecialFood)
        {
            lastSpecialFood = type;
            specialStreak = 0;
        }

        specialStreak++;

        Debug.Log($"[Special Feed] {frogType} got {type} = {specialStreak}/{specialLimit}");

        if (specialStreak >= specialLimit)
        {
            Debug.Log($"🔥 SPECIAL FOOD {type} TRIGGERED ON {GetComponent<FrogIdentity>().frogType}");

            FrogUnlockManager.Instance.TryUnlock(
                GetComponent<FrogIdentity>().frogType,
                type
            );

            specialStreak = 0;
        }
    }

    // =========================
    // 🎯 SPECIAL EVENT
    // =========================
    void TriggerSpecialEffect(FrogType type)
    {
        Debug.Log($"🔥 SPECIAL FOOD {type} REACHED 4 CONSECUTIVE FEEDS → SPECIAL EVENT TRIGGERED");

        if (tracker != null)
        {
            tracker.AddFood(type);
        }
        else
        {
            Debug.LogWarning("[Special Feed] No FrogSpecialFoodTracker found!");
        }
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
            gift.Init(GetComponent<FrogIdentity>());

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