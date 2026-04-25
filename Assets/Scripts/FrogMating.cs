using UnityEngine;
using System.Collections;

public class FrogMating : MonoBehaviour
{
    public GameObject loveUIPrefab;
    public GameObject frogPrefab; // fallback ONLY if genetics missing

    private FrogFeeding feeding;
    private FrogMovement movement;
    private FrogMating partner;
    private Rigidbody rb;

    public FrogType frogType;

    private bool isMating = false;
    private bool isDone = false;

    private float arrivalTimer = 0f;
    private const float requiredArrivalTime = 0.4f;

    void Start()
    {
        feeding = GetComponent<FrogFeeding>();
        movement = GetComponent<FrogMovement>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (isMating || isDone) return;
        if (feeding == null || !feeding.IsReadyToMate()) return;

        if (partner == null)
        {
            FindPartner();
            return;
        }

        HandleArrival();
    }

    void HandleArrival()
    {
        if (partner == null || FrogManager.Instance == null) return;

        float dist = Vector3.Distance(transform.position, FrogManager.Instance.matingPoint);

        if (dist <= 1.5f)
            arrivalTimer += Time.deltaTime;
        else
            arrivalTimer = Mathf.Max(0f, arrivalTimer - Time.deltaTime * 2f);

        if (arrivalTimer >= requiredArrivalTime)
        {
            if (movement.IsGrounded && partner.movement.IsGrounded)
            {
                StartCoroutine(Mate());
            }
        }
    }

    void FindPartner()
    {
        if (FrogManager.Instance.matingInProgress) return;

        FrogMating[] frogs = FindObjectsOfType<FrogMating>();

        FrogMating closest = null;
        float best = Mathf.Infinity;

        foreach (var f in frogs)
        {
            if (f == this) continue;
            if (f.isDone || f.isMating) continue;
            if (!f.feeding.IsReadyToMate()) continue;

            float d = Vector3.Distance(transform.position, f.transform.position);

            if (d < best)
            {
                best = d;
                closest = f;
            }
        }

        if (closest != null)
        {
            partner = closest;
            closest.partner = this;

            FrogManager.Instance.matingInProgress = true;

            Vector3 mid = (transform.position + partner.transform.position) / 2f;
            FrogManager.Instance.matingPoint = mid;

            movement.moveToMatingPoint = true;
            movement.matingPoint = mid;

            partner.movement.moveToMatingPoint = true;
            partner.movement.matingPoint = mid;
        }
    }

    IEnumerator Mate()
    {
        isMating = true;
        partner.isMating = true;

        Freeze(this);
        Freeze(partner);

        GameObject ui = Instantiate(loveUIPrefab);
        ui.transform.position = FrogManager.Instance.matingPoint + Vector3.up * 2f;

        FrogLoveUI love = ui.GetComponentInChildren<FrogLoveUI>();
        if (love != null)
            love.StartLove();

        yield return new WaitForSeconds(2f);

        SpawnBaby();

        feeding.hasMated = true;
        partner.feeding.hasMated = true;

        isDone = true;
        partner.isDone = true;

        FrogManager.Instance.matingInProgress = false;

        Unfreeze(this);
        Unfreeze(partner);

        partner.partner = null;
        partner = null;
    }

    void Freeze(FrogMating frog)
    {
        Rigidbody r = frog.GetComponent<Rigidbody>();
        FrogMovement m = frog.GetComponent<FrogMovement>();

        if (r != null)
        {
            r.linearVelocity = Vector3.zero;
            r.angularVelocity = Vector3.zero;
            r.isKinematic = true;
        }

        if (m != null)
            m.isMatingLocked = true;
    }

    void Unfreeze(FrogMating frog)
    {
        if (frog == null) return;

        Rigidbody r = frog.GetComponent<Rigidbody>();
        FrogMovement m = frog.GetComponent<FrogMovement>();

        if (r != null)
        {
            r.isKinematic = false;
            r.WakeUp();
        }

        if (m != null)
        {
            m.isMatingLocked = false;
            m.moveToMatingPoint = false;
        }
    }

    void SpawnBaby()
    {
        if (FrogManager.Instance == null || FrogManager.Instance.genetics == null)
        {
            Debug.LogError("Missing FrogManager or FrogGenetics!");
            return;
        }

        // 🧠 PURE GENETICS RESULT
        var combo = FrogGenetics.Instance.GetCombination(frogType, partner.frogType);

        GameObject prefabToSpawn = frogPrefab;

        if (combo != null && combo.resultPrefab != null)
        {
            prefabToSpawn = combo.resultPrefab;
        }

        GameObject baby = Instantiate(
            prefabToSpawn,
            FrogManager.Instance.matingPoint,
            Quaternion.identity
        );

        // 🧠 APPLY VISUAL (optional override)
        FrogVisual visual = baby.GetComponent<FrogVisual>();
        if (visual != null && combo != null)
        {
            visual.ApplyMaterial(combo.resultMaterial);
        }

        Rigidbody r = baby.GetComponent<Rigidbody>();
        FrogMovement m = baby.GetComponent<FrogMovement>();

        if (r != null)
        {
            r.isKinematic = false;
            r.constraints = RigidbodyConstraints.FreezeRotation;
            r.WakeUp();
        }

        if (m != null)
        {
            m.isMatingLocked = false;
            m.moveToMatingPoint = false;
        }
    }
}