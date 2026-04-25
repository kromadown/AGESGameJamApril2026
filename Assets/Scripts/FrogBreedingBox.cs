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

        Vector3 spawnPos = spawnPoint.position + Vector3.up * 0.5f;
        Instantiate(combo.resultPrefab, spawnPos, Quaternion.identity);

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

        var feeding = frog.GetComponent<FrogFeeding>();
        if (feeding != null)
            feeding.hasMated = false;

        var hoverUI = frog.GetComponent<FrogHoverUI>();
        if (hoverUI != null)
            hoverUI.HideUI();

        Vector3 spawnPos = FindSafePosition(index, frog);
        frog.transform.position = spawnPos;

        Rigidbody rb = frog.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.WakeUp();
        }
    }

    Vector3 FindSafePosition(int index, GameObject frog)
    {
        int attempts = 20;

        for (int i = 0; i < attempts; i++)
        {
            // 🔥 each frog gets its OWN sector (prevents clustering)
            float baseAngle = index == 0 ? 0f : 180f;

            float angle = baseAngle + Random.Range(-60f, 60f);

            Vector3 direction = new Vector3(
                Mathf.Cos(angle * Mathf.Deg2Rad),
                0,
                Mathf.Sin(angle * Mathf.Deg2Rad)
            );

            // 🔥 enforce MIN distance from box
            float minDistanceFromBox = boxClearanceRadius + 2.5f;
            float maxDistanceFromBox = returnRadius + 6f;

            float distance = Random.Range(minDistanceFromBox, maxDistanceFromBox);

            Vector3 candidate = transform.position + direction * distance;

            // ❌ avoid obstacles
            if (Physics.CheckSphere(candidate, 0.6f, obstacleMask))
                continue;

            // ❌ avoid spawning too close to box center
            if (Vector3.Distance(candidate, transform.position) < minDistanceFromBox)
                continue;

            // ground snap
            if (Physics.Raycast(candidate + Vector3.up * 5f, Vector3.down, out RaycastHit hit))
            {
                candidate = hit.point;
            }

            // slight lift to prevent physics jitter
            candidate.y += 0.1f;

            return candidate;
        }

        // 🔥 fallback guaranteed safe opposite sides
        Vector3 fallbackDir = (index == 0 ? Vector3.right : Vector3.left);
        return transform.position + fallbackDir * (returnRadius + 5f);
    }
}