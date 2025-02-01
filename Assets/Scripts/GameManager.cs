using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // Import the TextMesh Pro namespace

public class GameManager : MonoBehaviour
{
    public GameObject gameOverPanel; // Reference to the Game Over Panel
    public GameObject scorePanel;
    public GameObject shopPanel;
    public GameObject pauseIcon;
    public GameObject playIcon;
    public ShopManager shopManager;

    public GameObject pauseButton;
    public GameObject bar;
    public GameObject startPanel; // Reference to the Start Panel
    public BirdScript birdScript; // Reference to the BirdScript
    public PipeSpawner pipeSpawner; // Reference to the PipeSpawner
    private bool gamePaused = false;

    private AudioSource audioSource;
    public AudioClip explosionSound;
    public AudioClip menuSound;
    public AudioClip buttonSound;

    public TMP_Text highScoreText; // TextMeshPro for displaying high score
    public TMP_Text currencyText; // TextMeshPro for displaying currency

    private int highScore = 0; // High score
    private int currency = 0;


    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        // Set shop panel off-screen at the bottom before showing it later
        RectTransform shopRect = shopPanel.GetComponent<RectTransform>();
        shopRect.anchoredPosition = new Vector2(0, -Screen.height);
        shopPanel.SetActive(false);
        playIcon.SetActive(false);
        pauseIcon.SetActive(true);
        gameOverPanel.SetActive(false);
        scorePanel.SetActive(false);

        // Load the high score from PlayerPrefs
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        currency = PlayerPrefs.GetInt("Currency", 0); // Load currency

        UpdateHighScoreText();
        UpdateCurrencyText(); 
        ShowStartPanel();
    }

    void UpdateCurrencyText()
    {
        if (currencyText != null)
        {
            currencyText.text = "" + currency; // Update the currency text
        }
    }

    public void UpdateCurrency(int newCurrency)
    {
        currency = newCurrency; // Update the currency value
        PlayerPrefs.SetInt("Currency", currency); // Save the updated currency
        PlayerPrefs.Save(); // Ensure the changes are saved immediately
        UpdateCurrencyText(); // Update the text on the screen
    }



   void ShowStartPanel()
{
    if (startPanel != null)
    {
        startPanel.SetActive(true); // Ensure the start panel is active

        // Scale all "Animated" tagged objects inside startPanel
        foreach (Transform child in startPanel.transform)
        {
            if (child.CompareTag("Animated"))
            {
                child.localScale = Vector2.one; // Start from zero
         
            }
        }

        // Play the menu music
        audioSource.clip = menuSound;
        audioSource.loop = true; 
        audioSource.Play();
    }

   
    Time.timeScale = 0f; // Pause the game
}


    void HideStartPanel()
    {
        // Find all child objects with the "Animated" tag inside startPanel
        foreach (Transform child in startPanel.transform)
        {
            if (child.CompareTag("Animated"))
            {
                LeanTween.scale(child.gameObject, Vector2.zero, 0.3f)
                    .setEase(LeanTweenType.easeInBack)
                    .setIgnoreTimeScale(true);
            }
        }

        // Ensure the entire startPanel disappears after animations
        LeanTween.delayedCall(0.35f, () => startPanel.SetActive(false));

        Time.timeScale = 1f;
    }


    public void ResumeGame()
    {
        audioSource.PlayOneShot(buttonSound);

        if (!gamePaused)
        {
            audioSource.Play();
            playIcon.SetActive(true);
            pauseIcon.SetActive(false);
            gamePaused = true;
            Time.timeScale = 0f;
        }
        else
        {
            audioSource.Stop();
            playIcon.SetActive(false);
            pauseIcon.SetActive(true);
            gamePaused = false;
            Time.timeScale = 1f;
        }
    }

    public void OpenShopPanel()
    {
        shopManager.ShowBirdShop();
        audioSource.PlayOneShot(buttonSound);

        RectTransform shopRect = shopPanel.GetComponent<RectTransform>();

        // Reset position off-screen before animation
        shopRect.anchoredPosition = new Vector2(0, -Screen.height);

        // Ensure the shop panel is active before animating
        shopPanel.SetActive(true);

        // Slide up from bottom to fully appear
        LeanTween.moveY(shopRect, 0, 0.4f)
            .setEase(LeanTweenType.easeOutBack)
            .setIgnoreTimeScale(true); // Works even when paused
    }




    public void CloseShopPanel()
    {
        audioSource.PlayOneShot(buttonSound);

        RectTransform shopRect = shopPanel.GetComponent<RectTransform>();

        // Move panel down out of view before disabling it
        LeanTween.moveY(shopRect, -Screen.height, 0.4f)
            .setEase(LeanTweenType.easeInBack)
            .setIgnoreTimeScale(true)
            .setOnComplete(() => shopPanel.SetActive(false)); // Disable after animation
    }



    public void GameOver()
    {
        bar.SetActive(false);
        birdScript.AnimateDeath();
        pauseButton.SetActive(false);
        gameOverPanel.SetActive(true); // Show the Game Over UI

        gameOverPanel.transform.localScale = Vector2.zero; // Start from zero
        LeanTween.scale(gameOverPanel, Vector2.one, 0.3f)
            .setEase(LeanTweenType.easeOutBack);

        if (pipeSpawner != null)
        {
            pipeSpawner.StopSpawning(); // Stop spawning pipes
        }

        // Check if the current score is a new high score
        if (birdScript.score > highScore)
        {
            highScore = birdScript.score;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save(); 
        }

        // Update the high score text
        UpdateHighScoreText();
    }

    public void PlayAgain()
    {
        audioSource.PlayOneShot(buttonSound);

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void StartGame()
    {
        pipeSpawner.SpawnPipe();
        if (pipeSpawner.selectedPipeId == 1)
        {
           Camera mainCamera = Camera.main;
            string hexColor = "#ccdfff";

            // Convert the hex color to a Color object
            if (ColorUtility.TryParseHtmlString(hexColor, out Color newBackgroundColor))
            {
                // Set the background color
                mainCamera.backgroundColor = newBackgroundColor;
            }
            else
            {
                Debug.LogError("Invalid hex color format! Use something like #RRGGBB or #RRGGBBAA.");
            }
        }

        scorePanel.SetActive(true);

        float delay = 0.5f; // Delay before animation starts

        // Animate all "Animated" tagged objects inside scorePanel after a delay
        foreach (Transform child in scorePanel.transform)
        {
            if (child.CompareTag("Animated"))
            {
                child.localScale = Vector2.zero; // Start from zero
                LeanTween.scale(child.gameObject, Vector2.one, 0.3f)
                    .setEase(LeanTweenType.easeOutBack)
                    .setIgnoreTimeScale(true)
                    .setDelay(delay); // Add delay before animation
            }
        }

        if (audioSource.clip == menuSound)
        {
            audioSource.Stop();
        }

        // Update the selected bird before starting the game
        if (birdScript != null)
        {
            birdScript.UpdateSelectedBird();
        }

        audioSource.PlayOneShot(buttonSound);
        HideStartPanel();
    }

    public void ExitGame()
    {
        //PlayerPrefs.DeleteAll();
        //PlayerPrefs.Save();
        audioSource.PlayOneShot(buttonSound);
        Application.Quit();
        //UnityEditor.EditorApplication.isPlaying = false;
    }

    void UpdateHighScoreText()
    {
        if (highScoreText != null)
        {
            highScoreText.text = "High Score: " + highScore;
        }
    }
}
