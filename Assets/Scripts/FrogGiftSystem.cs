using UnityEngine;

public class FrogGiftSystem : MonoBehaviour
{
    [Header("Frog Prefabs")]
    public GameObject frogAPrefab;
    public GameObject frogBPrefab;
    public GameObject frogCPrefab;

    public GameObject frogABPrefab;
    public GameObject frogACPrefab;
    public GameObject frogBCPrefab;

    [Header("Drop Chances (0–1)")]
    [Range(0, 1)] public float specialFoodChance = 0.6f;
    [Range(0, 1)] public float normalFrogChance = 0.3f;
    [Range(0, 1)] public float permutationChance = 0.1f;

    [Header("Special Food Limits")]
    public int maxSpecialFood = 4;

    private int foodA = 0;
    private int foodB = 0;
    private int foodC = 0;

    [HideInInspector] public bool getSpecialFoodA = false;
    [HideInInspector] public bool getSpecialFoodB = false;
    [HideInInspector] public bool getSpecialFoodC = false;

    // =========================
    // 🎁 MAIN GIFT LOGIC
    // =========================
    public void GiveGift(FrogIdentity giver, Vector3 spawnPos)
    {
        Debug.Log($"[GiftSystem] Giver = {giver?.frogType}");

        bool foodFull = IsFoodFull();
        float roll = Random.value;

        float totalWeight =
            (!foodFull ? specialFoodChance : 0f) +
            normalFrogChance +
            permutationChance;

        float cumulative = 0f;

        // =========================
        // 🎁 SPECIAL FOOD
        // =========================
        if (!foodFull)
        {
            cumulative += specialFoodChance / totalWeight;

            if (roll < cumulative)
            {
                if (TryGiveSpecialFood())
                    return;
            }
        }

        // =========================
        // 🐸 NORMAL FROG
        // =========================
        cumulative += normalFrogChance / totalWeight;

        if (roll < cumulative)
        {
            SpawnRandomBaseFrog(spawnPos);
            return;
        }

        // =========================
        // 🧬 PERMUTATION
        // =========================
        GameObject result = GetPermutation(giver.frogType);

        if (result != null)
        {
            Instantiate(result, spawnPos, Quaternion.identity);
            Debug.Log("[GiftSystem] PERMUTATION frog spawned");
        }
    }

    // =========================
    // 🎯 UI + FOOD SYSTEM ACCESS
    // =========================
    public int GetFoodCount(FrogType type)
    {
        return type switch
        {
            FrogType.A => foodA,
            FrogType.B => foodB,
            FrogType.C => foodC,
            _ => 0
        };
    }

    public bool UseFood(FrogType type)
    {
        switch (type)
        {
            case FrogType.A:
                if (foodA <= 0) return false;
                foodA--;
                Debug.Log($"[Food] Used A → {foodA} left");
                return true;

            case FrogType.B:
                if (foodB <= 0) return false;
                foodB--;
                Debug.Log($"[Food] Used B → {foodB} left");
                return true;

            case FrogType.C:
                if (foodC <= 0) return false;
                foodC--;
                Debug.Log($"[Food] Used C → {foodC} left");
                return true;
        }

        return false;
    }

    public bool HasFood(FrogType type)
    {
        return GetFoodCount(type) > 0;
    }

    // =========================
    // 🎁 CHECK FULL INVENTORY
    // =========================
    bool IsFoodFull()
    {
        return foodA >= maxSpecialFood &&
               foodB >= maxSpecialFood &&
               foodC >= maxSpecialFood;
    }

    // =========================
    // 🎁 SPECIAL FOOD ROLL
    // =========================
    bool TryGiveSpecialFood()
    {
        for (int i = 0; i < 10; i++)
        {
            int r = Random.Range(0, 3);

            if (r == 0 && foodA < maxSpecialFood)
            {
                foodA++;
                getSpecialFoodA = true;
                Debug.Log($"[GiftSystem] Special Food A ({foodA}/{maxSpecialFood})");
                return true;
            }

            if (r == 1 && foodB < maxSpecialFood)
            {
                foodB++;
                getSpecialFoodB = true;
                Debug.Log($"[GiftSystem] Special Food B ({foodB}/{maxSpecialFood})");
                return true;
            }

            if (r == 2 && foodC < maxSpecialFood)
            {
                foodC++;
                getSpecialFoodC = true;
                Debug.Log($"[GiftSystem] Special Food C ({foodC}/{maxSpecialFood})");
                return true;
            }
        }

        return false;
    }

    // =========================
    // 🐸 BASE FROG
    // =========================
    void SpawnRandomBaseFrog(Vector3 pos)
    {
        GameObject result = Random.Range(0, 3) switch
        {
            0 => frogAPrefab,
            1 => frogBPrefab,
            _ => frogCPrefab
        };

        Instantiate(result, pos, Quaternion.identity);
        Debug.Log($"[GiftSystem] BASE frog spawned: {result.name}");
    }

    // =========================
    // 🧬 PERMUTATION
    // =========================
    GameObject GetPermutation(FrogType type)
    {
        return type switch
        {
            FrogType.A => Random.value < 0.5f ? frogABPrefab : frogACPrefab,
            FrogType.B => Random.value < 0.5f ? frogABPrefab : frogBCPrefab,
            FrogType.C => Random.value < 0.5f ? frogACPrefab : frogBCPrefab,
            _ => null
        };
    }
}