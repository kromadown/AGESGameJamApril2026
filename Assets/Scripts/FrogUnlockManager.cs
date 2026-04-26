using UnityEngine;
using System.Collections.Generic;

public class FrogUnlockManager : MonoBehaviour
{
    public static FrogUnlockManager Instance;

    [System.Serializable]
    public class UnlockEntry
    {
        public FrogType frogType;
        public FrogType foodType;
        public GameObject targetObject;
        public bool triggered;
    }

    public List<UnlockEntry> unlocks = new List<UnlockEntry>();

    void Awake()
    {
        Instance = this;
    }

    public void TryUnlock(FrogType frogType, FrogType foodType)
    {
        foreach (var u in unlocks)
        {
            if (u.triggered) continue;

            if (u.frogType == frogType && u.foodType == foodType)
            {
                u.triggered = true;

                if (u.targetObject != null)
                {
                    u.targetObject.SetActive(true);
                    Debug.Log($"[UnlockManager] UNLOCKED {u.targetObject.name}");
                }
                else
                {
                    Debug.LogWarning("[UnlockManager] Missing target object!");
                }
            }
        }
    }
}