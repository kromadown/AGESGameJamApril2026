using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class FrogCombination
{
    public FrogType parentA;
    public FrogType parentB;

    [Header("Result")]
    public GameObject resultPrefab;

    [Header("Visual Variant")]
    public Material resultMaterial; // optional override
}

public class FrogGenetics : MonoBehaviour
{
    public static FrogGenetics Instance;

    public List<FrogCombination> combinations = new List<FrogCombination>();

    void Awake()
    {
        Instance = this;
    }

    public FrogCombination GetCombination(FrogType a, FrogType b)
    {
        foreach (var combo in combinations)
        {
            if ((combo.parentA == a && combo.parentB == b) ||
                (combo.parentA == b && combo.parentB == a))
            {
                return combo;
            }
        }
        return null;
    }
}