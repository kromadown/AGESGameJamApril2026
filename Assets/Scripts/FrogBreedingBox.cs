using UnityEngine;

public class FrogBreedingBox : MonoBehaviour
{
    public Transform spawnPoint;

    [Header("Return Settings")]
    public float returnRadius = 3.5f;

    [Header("Spawn Safety")]
    public float minSeparation = 1.2f;
    public float boxClearanceRadius = 2.0f;
    public LayerMask obstacleMask;

    private GameObject slotA;
    private GameObject slotB;

    private FrogType typeA;
    private FrogType typeB;

    public void PlaceSelectedFrog(GameObject frog)
    {
        if (frog == null) return;

        FrogIdentity id = frog.GetComponent<FrogIdentity>();
        if (id == null)
        {
            Debug.LogError("Frog is missing FrogIdentity!");
            return;
        }

        if (slotA == null)
            StoreFrog(ref slotA, ref typeA, frog, id.frogType);
        else if (slotB == null)
            StoreFrog(ref slotB, ref typeB, frog, id.frogType);
        else
            return;

        TryBreed();
    }

    void StoreFrog(ref GameObject slot, ref FrogType type, GameObject frog, FrogType frogType)
    {
        slot = frog;
        type = frogType;
        frog.SetActive(false);
    }

    void TryBreed()
    {
        if (slotA == null || slotB == null)
            return;

        var combo = FrogGenetics.Instance.GetCombination(typeA, typeB);

        if (combo == null || combo.resultPrefab == null)
        {
            ResetBox();
            return;
        }

        Instantiate(combo.resultPrefab, spawnPoint.position, Quaternion.identity);

        ResetBox();
    }

    void ResetBox()
    {
        ReturnFrog(slotA, 0);
        ReturnFrog(slotB, 1);

        slotA = null;
        slotB = null;
    }

    void ReturnFrog(GameObject frog, int index)
    {
        if (frog == null) return;

        frog.SetActive(true);

        if (FrogSelect.selectedFrog == frog)
            FrogSelect.selectedFrog = null;

        var outline = frog.GetComponent<Outline>();
        if (outline != null)
            outline.enabled = false;

        Vector3 spawnPos = FindSafePosition(index);

        frog.transform.position = spawnPos;

        Rigidbody rb = frog.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.WakeUp();
        }
    }

    Vector3 FindSafePosition(int index)
    {
        int attempts = 10;

        for (int i = 0; i < attempts; i++)
        {
            float angle = (index * 140f) + Random.Range(-30f, 30f);
            float radius = Random.Range(boxClearanceRadius, returnRadius);

            Vector3 dir = new Vector3(
                Mathf.Cos(angle * Mathf.Deg2Rad),
                0,
                Mathf.Sin(angle * Mathf.Deg2Rad)
            );

            Vector3 pos = transform.position + dir * radius;

            // ❌ avoid spawning inside box or obstacles
            if (Physics.CheckSphere(pos, 0.6f, obstacleMask))
                continue;

            // ground snap
            if (Physics.Raycast(pos + Vector3.up * 5f, Vector3.down, out RaycastHit hit))
            {
                pos = hit.point;
            }

            return pos;
        }

        // fallback (never inside box)
        return transform.position + transform.right * (returnRadius + 2f);
    }
}