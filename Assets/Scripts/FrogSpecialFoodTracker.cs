using UnityEngine;

public class FrogSpecialFoodTracker : MonoBehaviour
{
    private FrogIdentity identity;

    private int foodA;
    private int foodB;
    private int foodC;

    private const int requiredAmount = 4;

    void Awake()
    {
        identity = GetComponent<FrogIdentity>();
    }

    public void AddFood(FrogType foodType)
    {
        if (identity == null) return;

        switch (foodType)
        {
            case FrogType.A: foodA++; break;
            case FrogType.B: foodB++; break;
            case FrogType.C: foodC++; break;
        }

        Debug.Log($"[Tracker] {name} received {foodType}");

        CheckUnlock(foodType);
    }

    void CheckUnlock(FrogType foodType)
    {
        int count = GetFoodCount(foodType);

        if (count >= requiredAmount)
        {
            Debug.Log($"🔥 THRESHOLD REACHED: {identity.frogType} + {foodType}");

            FrogUnlockManager.Instance.TryUnlock(identity.frogType, foodType);

            ResetFood(foodType);
        }
    }

    int GetFoodCount(FrogType type)
    {
        return type switch
        {
            FrogType.A => foodA,
            FrogType.B => foodB,
            FrogType.C => foodC,
            _ => 0
        };
    }

    void ResetFood(FrogType type)
    {
        switch (type)
        {
            case FrogType.A: foodA = 0; break;
            case FrogType.B: foodB = 0; break;
            case FrogType.C: foodC = 0; break;
        }
    }
}