using UnityEngine;

public class WallGenerator : MonoBehaviour
{
    public float wallHeight = 3f;
    public float wallThickness = 0.2f;

    void Start()
    {
        // Get plane size from its scale (Unity plane = 10x10 by default)
        float planeWidth = transform.localScale.x * 10f;
        float planeLength = transform.localScale.z * 10f;

        float halfWidth = planeWidth / 2f;
        float halfLength = planeLength / 2f;

        // Front
        CreateWall(
            new Vector3(0, wallHeight / 2, halfLength),
            new Vector3(planeWidth, wallHeight, wallThickness)
        );

        // Back
        CreateWall(
            new Vector3(0, wallHeight / 2, -halfLength),
            new Vector3(planeWidth, wallHeight, wallThickness)
        );

        // Left
        CreateWall(
            new Vector3(-halfWidth, wallHeight / 2, 0),
            new Vector3(wallThickness, wallHeight, planeLength)
        );

        // Right
        CreateWall(
            new Vector3(halfWidth, wallHeight / 2, 0),
            new Vector3(wallThickness, wallHeight, planeLength)
        );
    }

    void CreateWall(Vector3 localPosition, Vector3 scale)
    {
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);

        wall.transform.parent = transform;
        wall.transform.localPosition = localPosition; // IMPORTANT: local, not world
        wall.transform.localScale = scale;
    }
}