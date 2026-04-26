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
    public int maxSpecialFood = 3;

    private int foodA = 0;
    private int foodB = 0;
    private int foodC = 0;

    public void GiveGift(FrogIdentity giver, Vector3 spawnPos)
    {
        Debug.Log($"[GiftSystem] Giver = {giver?.frogType}");

        bool foodFull = IsFoodFull();

        float roll = Random.value;

        // =========================
        // 🎯 DYNAMIC TOTAL
        // =========================
        float totalWeight = 0f;

        if (!foodFull)
            totalWeight += specialFoodChance;

        totalWeight += normalFrogChance;
        totalWeight += permutationChance;

        float cumulative = 0f;

        // =========================
        // 🎁 SPECIAL FOOD (only if NOT full)
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
            Debug.Log("[GiftSystem] PERMUTATION frog spawned");
            Instantiate(result, spawnPos, Quaternion.identity);
        }
        else
        {
            Debug.LogError("[GiftSystem] Permutation result NULL");
        }
    }

    // =========================
    // 🎁 FOOD FULL CHECK
    // =========================
    bool IsFoodFull()
    {
        return foodA >= maxSpecialFood &&
               foodB >= maxSpecialFood &&
               foodC >= maxSpecialFood;
    }

    // =========================
    // 🎁 SPECIAL FOOD LOGIC
    // =========================
    bool TryGiveSpecialFood()
    {
        int attempts = 0;

        while (attempts < 10)
        {
            int r = Random.Range(0, 3);

            switch (r)
            {
                case 0:
                    if (foodA < maxSpecialFood)
                    {
                        foodA++;
                        Debug.Log($"[GiftSystem] GOT Special Food A ({foodA}/{maxSpecialFood})");
                        return true;
                    }
                    break;

                case 1:
                    if (foodB < maxSpecialFood)
                    {
                        foodB++;
                        Debug.Log($"[GiftSystem] GOT Special Food B ({foodB}/{maxSpecialFood})");
                        return true;
                    }
                    break;

                case 2:
                    if (foodC < maxSpecialFood)
                    {
                        foodC++;
                        Debug.Log($"[GiftSystem] GOT Special Food C ({foodC}/{maxSpecialFood})");
                        return true;
                    }
                    break;
            }

            attempts++;
        }

        return false;
    }

    // =========================
    // 🐸 BASE FROG
    // =========================
    void SpawnRandomBaseFrog(Vector3 pos)
    {
        int r = Random.Range(0, 3);

        GameObject result = r switch
        {
            0 => frogAPrefab,
            1 => frogBPrefab,
            _ => frogCPrefab
        };

        Debug.Log($"[GiftSystem] BASE frog spawned: {result.name}");

        Instantiate(result, pos, Quaternion.identity);
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