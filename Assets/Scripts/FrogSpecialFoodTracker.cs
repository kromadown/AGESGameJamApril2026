using UnityEngine;

public class FrogSpecialFoodTracker : MonoBehaviour
{
    public int foodACount;
    public int foodBCount;
    public int foodCCount;

    public int requiredAmount = 4;

    public void AddFood(FrogType foodType)
    {
        FrogIdentity id = GetComponent<FrogIdentity>();
        if (id == null) return;

        switch (foodType)
        {
            case FrogType.A:
                foodACount++;
                if (IsABorAC(id.frogType) && foodACount >= requiredAmount)
                    TriggerSpecial("A");
                break;

            case FrogType.B:
                foodBCount++;
                if (IsABorBC(id.frogType) && foodBCount >= requiredAmount)
                    TriggerSpecial("B");
                break;

            case FrogType.C:
                foodCCount++;
                if (IsACorBC(id.frogType) && foodCCount >= requiredAmount)
                    TriggerSpecial("C");
                break;
        }
    }

    bool IsABorAC(FrogType type)
    {
        return type == FrogType.AB || type == FrogType.AC;
    }

    bool IsABorBC(FrogType type)
    {
        return type == FrogType.AB || type == FrogType.BC;
    }

    bool IsACorBC(FrogType type)
    {
        return type == FrogType.AC || type == FrogType.BC;
    }

    void TriggerSpecial(string foodType)
    {
        Debug.Log($"[SPECIAL TRIGGER] Frog {name} activated for Food {foodType}");
    }
}