using UnityEngine;
using System.Collections;

public class BreedingAnimController : MonoBehaviour
{
    public Animator breedingAnim;
    public FrogBreedingBox frogBreedingBox;

    public GameObject heartEffect;

    public AudioSource heartSound;

    public float breedingDuration = 1.5f;

    private bool isCounting = false;

    void Start()
    {
        heartEffect.SetActive(false);
    }

    void Update()
    {
        if (frogBreedingBox.isBreeding && !isCounting)
        {
            StartCoroutine(BreedingTimer());
        }

        if (isCounting)
        {
            heartEffect.SetActive(true);
        }
        else
        {
            heartEffect.SetActive(false);
        }
    }

    

IEnumerator BreedingTimer()
    {
        if (heartSound != null)
        {
            heartSound.Play();
        }

        isCounting = true;

        breedingAnim.SetBool("isMating", true);

        yield return new WaitForSeconds(breedingDuration);

        breedingAnim.SetBool("isMating", false);
        frogBreedingBox.isBreeding = false;


        isCounting = false;
    }
}