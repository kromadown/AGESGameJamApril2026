using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FrogLoveUI : MonoBehaviour
{
    [Header("UI")]
    public Image heart;
    public float fillDuration = 2f;
    public Vector3 offset = new Vector3(0, 2.5f, 0);

    [Header("Scale")]
    public float uiScale = 2.2f;

    private Transform frogA;
    private Transform frogB;
    private Camera cam;

    private bool started;

    public void Init(Transform a, Transform b)
    {
        frogA = a;
        frogB = b;

        transform.localScale = Vector3.one * uiScale;
    }

    void Awake()
    {
        // 🔥 GUARANTEE HEART EXISTS
        if (heart == null)
            heart = GetComponentInChildren<Image>();

        if (heart == null)
            Debug.LogError("FrogLoveUI: Heart Image is NOT assigned!");
    }

    void Start()
    {
        cam = Camera.main;

        if (heart != null)
            heart.fillAmount = 0f;
    }

    void LateUpdate()
    {
        if (frogA == null || frogB == null) return;

        Vector3 mid = (frogA.position + frogB.position) * 0.5f;

        transform.position = Vector3.Lerp(
            transform.position,
            mid + offset,
            Time.deltaTime * 10f
        );

        if (cam != null)
        {
            Vector3 dir = transform.position - cam.transform.position;
            transform.rotation = Quaternion.LookRotation(dir);
        }
    }

    public void StartLove()
    {
        if (started)
        {
            Debug.LogWarning("Love UI already started");
            return;
        }

        started = true;
        StartCoroutine(FillHeart());
    }

    IEnumerator FillHeart()
    {
        if (heart == null)
        {
            Debug.LogError("Heart Image missing - cannot fill");
            yield break;
        }

        heart.fillAmount = 0f;

        float t = 0f;

        while (t < fillDuration)
        {
            t += Time.deltaTime;
            heart.fillAmount = Mathf.Clamp01(t / fillDuration);
            yield return null;
        }

        heart.fillAmount = 1f;
    }
}