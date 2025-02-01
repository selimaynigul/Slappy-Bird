using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.EventSystems;


public class BirdScript : MonoBehaviour
{
    public Rigidbody2D myRigidBody;
    public Sprite[] birdSprites; // Array of bird sprites
    private SpriteRenderer spriteRenderer; // Reference to the SpriteRenderer
    private float flapStrength = 20;
    public GameManager gameOverManager; // Reference to the GameOverManager
    public AudioClip[] flapSounds; // Array to hold your audio clips
    public AudioClip[] fartSounds;
    public AudioClip smallerSound; 
    public AudioClip biggerSound;
    public AudioClip explosionSound;
    public AudioClip buildUpSound;
    public Bar bar;
    public GameObject explosionPrefab; // Reference to the explosion prefab
    private int collectibleCount = 0; // Track how many collectibles were collected
    private bool isShaking = false;
    public GameObject vomitPrefab; // Assign this in Unity (Particle System)
    public AudioClip vomitSound; // Assign a sound effect for vomiting

    private AudioSource audioSource;
    public TMP_Text scoreText; // Reference to the TextMeshPro text element
    public TMP_Text highScoreText; // Reference to display high score
    private bool birdAlive = true;

    public int score = 0; // Player's score
    private int highScore = 0; // High score
    private int currency = 0; // Total collected collectibles (money)
    private int selectedBirdId = 0;
    private float originalScale; // To store the bird's original scale
    private bool gameOver = false;

    public DizzyStarsManager starManager; // Reference to the star manager




    void Start()
    {
        myRigidBody = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        highScore = PlayerPrefs.GetInt("HighScore", 0);
        currency = PlayerPrefs.GetInt("Currency", 0);

        originalScale = transform.localScale.x; // Store the bird's original scale


        UpdateScoreText();
        UpdateHighScoreText();

        Fly();
    }

    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Space) || IsTouchingScreen()) && birdAlive)
        {
            PlayRandomFlapSound();
            Fly();
        }
    }

    public void ApplySizeBonus(float scaleMultiplier, float duration, float animationDuration)
    {
        bar.ActivateSizeBonus();
        StartCoroutine(SizeBonusWithAnimationCoroutine(scaleMultiplier, duration, animationDuration));
    }

    private IEnumerator SizeBonusWithAnimationCoroutine(float scaleMultiplier, float duration, float animationDuration)
    {
        // Animate the bird to shrink
        audioSource.PlayOneShot(smallerSound);
        float elapsedTime = 0f;
        Vector3 originalScale = transform.localScale;
        Vector3 targetScale = originalScale * scaleMultiplier;

        // Animate shrinking
        while (elapsedTime < animationDuration)
        {
            transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsedTime / animationDuration);
            elapsedTime += Time.deltaTime;
            yield return null; // Wait until the next frame
        }
        transform.localScale = targetScale; // Ensure exact final size

        // Wait for the bonus duration
        yield return new WaitForSeconds(duration);

        // Animate the bird to grow back
        audioSource.PlayOneShot(biggerSound);
        elapsedTime = 0f;
        while (elapsedTime < animationDuration)
        {
            transform.localScale = Vector3.Lerp(targetScale, originalScale, elapsedTime / animationDuration);
            elapsedTime += Time.deltaTime;
            yield return null; // Wait until the next frame
        }
        transform.localScale = originalScale; // Ensure exact original size
    }


    public void UpdateSelectedBird()
    {
        selectedBirdId = PlayerPrefs.GetInt("SelectedBird", 0);

        if (selectedBirdId >= 0 && selectedBirdId < birdSprites.Length)
        {
            spriteRenderer.sprite = birdSprites[selectedBirdId];
            NormalizeBirdSize(); 
        }
        else
        {
            Debug.LogWarning("Invalid Bird ID. Defaulting to the first sprite.");
            spriteRenderer.sprite = birdSprites[0];
        }
    }


    bool IsTouchingScreen()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            return !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
        }
        return false;
    }


    void Fly()
    {
        myRigidBody.linearVelocity = Vector2.up * flapStrength;
    }

    void PlayRandomFlapSound()
    {
        if (selectedBirdId != 5)
        {
            int randomIndex = Random.Range(0, flapSounds.Length);
            audioSource.PlayOneShot(flapSounds[randomIndex]);
        } 
        else
        {
            int randomIndex = Random.Range(0, fartSounds.Length);
            audioSource.PlayOneShot(fartSounds[randomIndex]);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Pipe"))
        {
            if (gameOverManager != null && gameOver == false)
            {
                gameOver = true;
                birdAlive = false;
                CheckAndSaveHighScore(); 
                gameOverManager.GameOver();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ground"))
        {
            if (gameOverManager != null && gameOver == false)
            {
                gameOver = true;
                birdAlive = false;
                CheckAndSaveHighScore();
                gameOverManager.GameOver();
            }
        }
        else if (other.CompareTag("ScoreZone"))
        {
            IncrementScore(1);
        }
       
    }

    public void IncrementScore(int value)
    {
        if (birdAlive == true)
        {
            score += value;
            UpdateScoreText();
        }
    }

    public void IncrementCurrency(int value)
    {
        currency += value;
        PlayerPrefs.SetInt("Currency", currency);
        PlayerPrefs.Save(); 
    }


    void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }

    void UpdateHighScoreText()
    {
        if (highScoreText != null)
        {
            highScoreText.text = "High Score: " + highScore;
        }
    }

    void CheckAndSaveHighScore()
    {
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
            UpdateHighScoreText();
        }
    }

    private void NormalizeBirdSize()
    {
        // Define the fixed collider radius
        float fixedColliderRadius = 2.4f; // Adjust as needed for your game
        float fixedColliderDiameter = fixedColliderRadius * 2f;

        // Adjust the CircleCollider2D radius
        CircleCollider2D collider = GetComponent<CircleCollider2D>();
        if (collider != null)
        {
            collider.radius = 2.1f;
        }

        // Adjust the bird sprite size
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && spriteRenderer.sprite != null)
        {
            // Get the sprite's current size in world units
            float spriteHeight = spriteRenderer.bounds.size.y;
            float scaleFactor = fixedColliderDiameter / spriteHeight;

            // Apply scaling to the transform
            transform.localScale = new Vector3(scaleFactor, scaleFactor, 1f);
        }
    }

    public void IncreaseGravityScale()
    {
        myRigidBody.gravityScale += 0.5f;
    }

    public void AnimateDeath()
    {
        StartCoroutine(DeathAnimationCoroutine());
    }

    private IEnumerator DeathAnimationCoroutine()
    {
   
        // Spawn the explosion particle system
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }

        // Play the explosion sound
        if (audioSource != null && explosionSound != null)
        {
            audioSource.PlayOneShot(explosionSound);
        }

        // Hide or disable the bird sprite immediately
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false; // Hide the bird
        }

        // Wait for the explosion sound to finish
        if (explosionSound != null)
        {
            yield return new WaitForSeconds(explosionSound.length);
        }

        // Destroy the bird GameObject after the sound finishes
        Destroy(gameObject);
    }
    // New: Starts a continuous shake effect relative to the bird's live position
    public void StartShaking()
    {
        collectibleCount++; // Increase shake intensity

        if (!isShaking)
        {
            isShaking = true;
            StartCoroutine(ShakeEffect());
        }

        
    }

    private IEnumerator ShakeEffect()
    {
        while (isShaking) // Loop while shaking is active
        {
            float shakeAmount = Mathf.Min(collectibleCount * 0.1f, 0.3f); // Increase shake but limit max
            float shakeSpeed = 0.1f; // Speed of shake

            Vector3 shakeOffset = new Vector3(
                Random.Range(-shakeAmount, shakeAmount),
                Random.Range(-shakeAmount, shakeAmount),
                0 // Keep Z constant
            );

            transform.position += shakeOffset; // Apply shake as a **temporary offset**

            yield return new WaitForSeconds(shakeSpeed);

            transform.position -= shakeOffset; // **Reset position after each shake**
        }
    }

    // Stops shaking when necessary (e.g., game over)
    public void StopShaking()
    {
        isShaking = false;
    }

    public void ResetEffects()
    {
        collectibleCount = 0;
        myRigidBody.gravityScale = 5f;
        if (starManager != null)
        {
            starManager.RemoveAllStars();
        }
        StopShaking();
    }

    public int GetCollectibleCount()
    {
        return collectibleCount; // Returns the current count
    }

    public void PlayVomitEffect()
    {
        if (vomitPrefab != null)
        {
            GameObject vomit = Instantiate(vomitPrefab, transform.position, Quaternion.identity);
            vomit.transform.SetParent(transform); // Attach to bird
            vomit.transform.localPosition = new Vector3(0.5f, -0.2f, -5f); // Adjust position to look natural


            ParticleSystem ps = vomit.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                ps.Play(); // Play the vomit particle effect
            }

            Destroy(vomit, 1.5f); // Destroy effect after 1.5 seconds
        }

        if (audioSource != null && vomitSound != null)
        {
            audioSource.PlayOneShot(vomitSound);
        }
    }



}
