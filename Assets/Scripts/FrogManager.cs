using UnityEngine;

public class FrogManager : MonoBehaviour
{
    public static FrogManager Instance;

    public bool matingInProgress = false;
    public Vector3 matingPoint;
    public FrogGenetics genetics;

    void Awake()
    {
        Instance = this;
    }
}