using UnityEngine;
using System.Collections;

public class BreedingAnimController : MonoBehaviour
{
    public Animator breedingAnim;
    public FrogBreedingBox frogBreedingBox;

    public float breedingDuration = 2f;

    private bool isCounting = false;

    void Update()
    {
        if (frogBreedingBox.isBreeding && !isCounting)
        {
            StartCoroutine(BreedingTimer());
        }
    }

    IEnumerator BreedingTimer()
    {
        isCounting = true;

        breedingAnim.SetBool("isMating", true);

        yield return new WaitForSeconds(breedingDuration);

        breedingAnim.SetBool("isMating", false);
        frogBreedingBox.isBreeding = false;

        isCounting = false;
    }
}