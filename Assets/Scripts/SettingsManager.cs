using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public RectTransform muteButton; // Mute button (for moving)
    public Button settingsButton; // Settings button to toggle menu
    public Button muteButtonComponent; // Mute button component (for clicking)
    public RawImage muteIcon; // Reference to the RawImage inside MuteButton
    public Texture2D muteTexture; // Muted icon (Texture)
    public Texture2D unmuteTexture; // Unmuted icon (Texture)

    private bool isSettingsOpen = false;
    private bool isMuted = false;

    private Vector2 offScreenPosition;
    private Vector2 onScreenPosition;

    private AudioSource audioSource;
    public AudioClip buttonSound;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // Save original positions
        onScreenPosition = muteButton.anchoredPosition;
        offScreenPosition = new Vector2(-200f, onScreenPosition.y); // Move off-screen to the left

        // Place muteButton off-screen at start
        muteButton.anchoredPosition = offScreenPosition;

        // Load mute state from PlayerPrefs
        isMuted = PlayerPrefs.GetInt("Muted", 0) == 1;
        UpdateMuteIcon();

        // Add button click listeners
        settingsButton.onClick.AddListener(ToggleSettings);
        muteButtonComponent.onClick.AddListener(ToggleMute);
    }

    private void ToggleSettings()
    {
        audioSource.PlayOneShot(buttonSound);
        isSettingsOpen = !isSettingsOpen;

        if (isSettingsOpen)
        {
            // Move mute button on-screen
            LeanTween.moveX(muteButton, onScreenPosition.x, 0.3f).setIgnoreTimeScale(true)
                .setEase(LeanTweenType.easeOutBack);
        }
        else
        {
            // Move mute button off-screen
            LeanTween.moveX(muteButton, offScreenPosition.x, 0.3f).setIgnoreTimeScale(true)
                .setEase(LeanTweenType.easeInBack);
        }
    }

    private void ToggleMute()
    {
        isMuted = !isMuted;
        PlayerPrefs.SetInt("Muted", isMuted ? 1 : 0);
        PlayerPrefs.Save();
        UpdateMuteIcon();
    }

    private void UpdateMuteIcon()
    {
        // Update RawImage texture instead of Sprite
        muteIcon.texture = isMuted ? muteTexture : unmuteTexture;
        AudioListener.volume = isMuted ? 0 : 1; // Mute or unmute audio
    }
}
