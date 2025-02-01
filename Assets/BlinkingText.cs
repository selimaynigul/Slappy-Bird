using UnityEngine;
using TMPro; // Required for TextMeshPro

public class BlinkingText : MonoBehaviour
{
    public float fadeDuration = 1.0f; // Time for fade in/out
    public TextMeshProUGUI tmpText; // TextMeshPro component
    private Color textColor;
    private bool fadingIn = true; // Controls fade direction
    private float fadeTimer = 0.0f;

    void Start()
    {
        // Get the TextMeshProUGUI component
        tmpText = GetComponent<TextMeshProUGUI>();
        if (tmpText == null)
        {
            Debug.LogError("TextMeshProUGUI component is missing!");
            enabled = false;
            return;
        }

        // Get the initial text color
        textColor = tmpText.color;
    }

    void Update()
    {

        if (tmpText == null) return;

        // Update fade timer
        fadeTimer += Time.deltaTime;

        // Calculate alpha value
        float alpha = fadeTimer / fadeDuration;

        if (fadingIn)
        {
            // Fade in (alpha from 0 to 1)
            textColor.a = Mathf.Lerp(0, 1, alpha);
        }
        else
        {
            // Fade out (alpha from 1 to 0)
            textColor.a = Mathf.Lerp(1, 0, alpha);
        }

        // Apply the new color to the TextMeshPro component
        tmpText.color = textColor;

        // Switch direction when fade cycle completes
        if (fadeTimer >= fadeDuration)
        {
            fadingIn = !fadingIn; // Toggle fade direction
            fadeTimer = 0.0f;     // Reset timer
        }
    }
}
