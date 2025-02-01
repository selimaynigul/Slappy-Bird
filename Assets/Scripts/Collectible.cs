using UnityEngine;

public class CollectibleScript : MonoBehaviour
{
    public AudioClip collectibleSound;
    public AudioClip bonusSound;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            BirdScript birdScript = other.GetComponent<BirdScript>();
            PipeSpawner pipeSpawner = Object.FindFirstObjectByType<PipeSpawner>();
            DizzyStarsManager starManager = Object.FindFirstObjectByType<DizzyStarsManager>();

            if (CompareTag("Collectible"))
            {
                if (birdScript != null)
                {
                    birdScript.IncrementCurrency(1);
                    birdScript.IncreaseGravityScale();

                    // If it's the 5th collectible, RESET stars & stop shaking
                    if (birdScript.GetCollectibleCount() >= 2)
                    {
                        if (starManager != null)
                            starManager.RemoveAllStars(); // Clear stars first

                        birdScript.PlayVomitEffect();
                        birdScript.ResetEffects(); // Reset shaking and gravity
                    }
                    else
                    {
                        //  Add star & shake only if count < 3
                        if (starManager != null)
                        {
                            starManager.AddStar();
                            starManager.AddStar();
                            starManager.AddStar();
                        }

                        birdScript.StartShaking();
                    }
                }

                if (audioSource != null && collectibleSound != null)
                {
                    audioSource.PlayOneShot(collectibleSound);
                }
            }
            else if (CompareTag("EaseBonus"))
            {
                if (audioSource != null && bonusSound != null)
                {
                    audioSource.PlayOneShot(bonusSound);
                }

                if (pipeSpawner != null)
                {
                    pipeSpawner.DecreaseDifficulty();
                }
            }
            else if (CompareTag("SizeBonus"))
            {
                if (audioSource != null && bonusSound != null)
                {
                    audioSource.PlayOneShot(bonusSound);
                }

                if (birdScript != null)
                {
                    birdScript.ApplySizeBonus(0.5f, 10f, 0.1f);
                }
            }

            // Disable collider to prevent multiple triggers
            GetComponent<Collider2D>().enabled = false;

            //  Smooth disappearing animation before destroying
            LeanTween.scale(gameObject, Vector3.zero, 0.3f)
                .setEase(LeanTweenType.easeInBack)
                .setOnComplete(() => Death());
        }
    }

    void Death()
    {
        GetComponent<SpriteRenderer>().enabled = false;
        Destroy(gameObject);
    }
}
