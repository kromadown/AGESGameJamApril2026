using UnityEngine;

public class FoodManager : MonoBehaviour
{
    public static FoodManager Instance;

    public bool isHoldingFood = false;

    void Awake()
    {
        Instance = this;
    }

    public void SelectFood()
    {
        isHoldingFood = true;
        Debug.Log("Food selected");
    }

    public void ClearFood()
    {
        isHoldingFood = false;
    }
}