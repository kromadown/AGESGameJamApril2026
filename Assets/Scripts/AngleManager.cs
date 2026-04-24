using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AngleManager : MonoBehaviour
{
    public Transform center;
    public Button turnRight;
    public Button turnLeft;

    public float rotateDuration = 0.3f;

    private bool isRotating = false;

    void Start()
    {
        turnRight.onClick.AddListener(() => Rotate(-90));
        turnLeft.onClick.AddListener(() => Rotate(90));
    }

    void Rotate(float angle)
    {
        Debug.Log("Button clicked, angle = " + angle);

        if (!isRotating)
        {
            StartCoroutine(RotateSmooth(angle));
        }
    }

    IEnumerator RotateSmooth(float angle)
    {
        isRotating = true;

        Quaternion startRot = center.rotation;

        // 如果你要按照 Global Y 轴旋转，用这一行
        Quaternion endRot = Quaternion.Euler(0, angle, 0) * startRot;

        float time = 0;

        while (time < rotateDuration)
        {
            time += Time.deltaTime;
            float t = time / rotateDuration;

            center.rotation = Quaternion.Lerp(startRot, endRot, t);

            yield return null;
        }

        center.rotation = endRot;
        isRotating = false;
    }
}