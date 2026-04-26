using UnityEngine;

public class FrogGiftSystem : MonoBehaviour
{
    public GameObject frogAPrefab;
    public GameObject frogBPrefab;
    public GameObject frogCPrefab;

    public GameObject frogABPrefab;
    public GameObject frogACPrefab;
    public GameObject frogBCPrefab;

    public void GiveGift(FrogIdentity giver, Vector3 spawnPos)
    {
        Debug.Log($"[GiftSystem] GiveGift called. Giver = {giver?.frogType}, Pos = {spawnPos}");

        float roll = Random.value;

        GameObject result;

        if (roll < 0.8f)
        {
            int r = Random.Range(0, 3);

            result = r switch
            {
                0 => frogAPrefab,
                1 => frogBPrefab,
                _ => frogCPrefab
            };

            Debug.Log($"[GiftSystem] Rolled COMMON: {r}");
        }
        else
        {
            result = GetPermutation(giver.frogType);
            Debug.Log($"[GiftSystem] Rolled PERMUTATION");
        }

        if (result == null)
        {
            Debug.LogError("[GiftSystem] RESULT PREFAB IS NULL — nothing spawned!");
            return;
        }

        Debug.Log($"[GiftSystem] Spawning: {result.name}");

        Instantiate(result, spawnPos, Quaternion.identity);
    }

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