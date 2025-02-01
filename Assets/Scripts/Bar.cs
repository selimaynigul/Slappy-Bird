using UnityEngine;
using System.Collections;
using DentedPixel;

public class Bar : MonoBehaviour
{
    public GameObject bar; // The UI bar object
    public GameObject bg;
    private int time = 10; // Timer duration
    private bool isBonusActive = false;

    // Start is called once
    void Start()
    {
        bg.transform.localScale =  Vector2.zero; 
    }

    public void ActivateSizeBonus()
    {
        if (isBonusActive)
        {
          //  StopAllCoroutines(); // Stop previous coroutine if a new bonus is collected
        }

        isBonusActive = true;
        StartCoroutine(SizeBonusRoutine());
    }

    private IEnumerator SizeBonusRoutine()
    {
        bg.SetActive(true);
        bar.transform.localScale = Vector2.one;
        LeanTween.scale(bg, Vector2.one, 0.3f).setEase(LeanTweenType.easeOutBack);

        LeanTween.scaleX(bar, 0, time);

        yield return new WaitForSeconds(time);

        LeanTween.scale(bg, Vector2.zero, 0.3f)
            .setEase(LeanTweenType.easeInBack)
            .setOnComplete(() => bg.SetActive(false));

        isBonusActive = false;
    }
}
