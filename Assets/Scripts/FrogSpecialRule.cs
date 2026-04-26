using UnityEngine;

[System.Serializable]
public class FrogSpecialRule
{
    public FrogType frogType;      // AB / AC / BC
    public FrogType foodType;      // A / B / C
    public int requiredAmount = 4;

    public GameObject targetObject;

    [HideInInspector] public bool triggered;
}