using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    public ScrollRect pipesScrollRect;

    public GameObject itemTemplate; // Reference to the ItemTemplate prefab
    public Transform birdsContent; // Parent object where bird items will be listed
    public Transform pipesContent; // Parent object where pipe items will be listed

    public Sprite[] birdSprites; // Array of bird sprites
    public string[] birdTitles; // Array of bird titles
    public int[] birdPrices; // Array of bird prices

    public Sprite[] pipeSprites; // Array of pipe sprites
    public string[] pipeTitles; // Array of pipe titles
    public int[] pipePrices; // Array of pipe prices

    public Button BirdsTabButton; // Button for bird shop tab
    public Button PipesTabButton; // Button for pipe shop tab

    private int selectedBirdId;
    private int selectedPipeId;

    private bool[] unlockedBirds;
    private bool[] unlockedPipes;

    private AudioSource audioSource;
    public AudioClip buttonSound;
    public AudioClip purchaseSound;
    public AudioClip errorSound;


    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // Initialize unlocked items
        unlockedBirds = new bool[birdSprites.Length];
        unlockedPipes = new bool[pipeSprites.Length];

        for (int i = 0; i < birdSprites.Length; i++)
        {
            unlockedBirds[i] = PlayerPrefs.GetInt("UnlockedBird" + i, 0) == 1 || birdPrices[i] == 0;
        }

        for (int i = 0; i < pipeSprites.Length; i++)
        {
            unlockedPipes[i] = PlayerPrefs.GetInt("UnlockedPipe" + i, 0) == 1 || pipePrices[i] == 0;
        }

        selectedBirdId = PlayerPrefs.GetInt("SelectedBird", 0);
        selectedPipeId = PlayerPrefs.GetInt("SelectedPipe", 0);

        // Set up tab buttons with background color changes
        BirdsTabButton.onClick.AddListener(() => { PlayButtonSound(); ShowBirdShop(); HighlightTab(PipesTabButton, BirdsTabButton); });
        PipesTabButton.onClick.AddListener(() => { PlayButtonSound(); ShowPipeShop(); HighlightTab(BirdsTabButton, PipesTabButton); });

        HighlightTab(BirdsTabButton, PipesTabButton); // Set initial tab highlight
    }


    private void PlayButtonSound()
    {
        if (audioSource != null && buttonSound != null)
        {
            audioSource.PlayOneShot(buttonSound);
        }
    }

    private void HighlightTab(Button selectedButton, Button unselectedButton)
    {
        // Set the selected button's background color to slightly darker grey with a bluish tint
        Image selectedButtonImage = selectedButton.GetComponent<Image>();
        if (selectedButtonImage != null)
        {
            selectedButtonImage.color = new Color(0.85f, 0.85f, 0.9f, 1f); // Slightly darker grey with a very faint blue tint
        }

        // Reset the unselected button's background color to white
        Image unselectedButtonImage = unselectedButton.GetComponent<Image>();
        if (unselectedButtonImage != null)
        {
            unselectedButtonImage.color = new Color(1f, 1f, 1f, 1f); // White
        }
    }


    public void ShowBirdShop()
    {
        pipesScrollRect.gameObject.SetActive(false); // Disable the GameObject containing the ScrollRect
        birdsContent.gameObject.SetActive(true);
        pipesContent.gameObject.SetActive(false);
        PopulateShop(birdSprites, birdTitles, birdPrices, unlockedBirds, "Bird", birdsContent);
    }

    private void ShowPipeShop()
    {
        pipesScrollRect.gameObject.SetActive(true); // Enable the GameObject containing the ScrollRect
        birdsContent.gameObject.SetActive(false);
        pipesContent.gameObject.SetActive(true);
        PopulateShop(pipeSprites, pipeTitles, pipePrices, unlockedPipes, "Pipe", pipesContent);
    }

    private void PopulateShop(Sprite[] sprites, string[] titles, int[] prices, bool[] unlockedItems, string itemType, Transform content)
    {
        // Clear existing items
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        int selectedId = itemType == "Bird" ? selectedBirdId : selectedPipeId;

        for (int i = 0; i < titles.Length; i++)
        {
            GameObject item = Instantiate(itemTemplate, content);

                // Set initial scale to zero
        item.transform.localScale = Vector3.zero;

        // Animate the scaling to full size with a slight delay for a pop-up effect
        LeanTween.scale(item, Vector3.one, 0.3f).setEase(LeanTweenType.easeOutBack).setDelay(i * 0.05f).setIgnoreTimeScale(true); 
        

            if (sprites != null) // For birds and pipes
            {
                Image itemImage = item.transform.Find("BirdImage").GetComponent<Image>();
                itemImage.sprite = sprites[i];
                itemImage.color = unlockedItems[i] ? new Color(1f, 1f, 1f, 1f) : new Color(1f, 1f, 1f, 0.5f); // Transparent if not unlocked
            }

            TMP_Text title = item.transform.Find("Title").GetComponent<TMP_Text>();
            title.text = titles[i];

            TMP_Text price = item.transform.Find("Price").GetComponent<TMP_Text>();
            price.text = unlockedItems[i] ? "Owned" : prices[i] + "";

            // Get the card background
            Image cardBackground = item.GetComponent<Image>();
            if (cardBackground != null)
            {
                // Set the card background to green if it's the selected item
                cardBackground.color = (i == selectedId) ? new Color(0f, 1f, 0f, 1f) : new Color(1f, 1f, 1f, 1f); // Green if selected, white otherwise
            }

            Button buyButton = item.transform.Find("BuyButton").GetComponent<Button>();
            TMP_Text buttonText = buyButton.transform.Find("Text (TMP)").GetComponent<TMP_Text>();

            int itemId = i;

            if (unlockedItems[i])
            {
                buttonText.text = (itemType == "Bird" && itemId == selectedBirdId) || (itemType == "Pipe" && itemId == selectedPipeId) ? "Selected" : "Select";
                buyButton.onClick.AddListener(() => SelectItem(itemType, itemId));
            }
            else
            {
                buttonText.text = "Purchase";
                buyButton.onClick.AddListener(() => PurchaseItem(itemType, itemId));
            }
        }
    }


    private void PurchaseItem(string itemType, int itemId)
    {
        int currency = PlayerPrefs.GetInt("Currency", 0);
        int price = itemType == "Bird" ? birdPrices[itemId] : pipePrices[itemId];

        if (currency >= price)
        {
            audioSource.PlayOneShot(purchaseSound);

            currency -= price;
            PlayerPrefs.SetInt("Currency", currency);

            if (itemType == "Bird")
            {
                PlayerPrefs.SetInt("UnlockedBird" + itemId, 1);
                unlockedBirds[itemId] = true;
            }
            else if (itemType == "Pipe")
            {
                PlayerPrefs.SetInt("UnlockedPipe" + itemId, 1);
                unlockedPipes[itemId] = true;
            }

            PlayerPrefs.Save();

            GameManager gameManager = Object.FindFirstObjectByType<GameManager>();
            if (gameManager != null)
            {
                gameManager.UpdateCurrency(currency);
            }

            // Update UI to reflect purchase
            Transform content = itemType == "Bird" ? birdsContent : pipesContent;
            Transform item = content.GetChild(itemId);

            TMP_Text priceText = item.Find("Price").GetComponent<TMP_Text>();
            priceText.text = "Owned";

            Button buyButton = item.Find("BuyButton").GetComponent<Button>();
            TMP_Text buttonText = buyButton.transform.Find("Text (TMP)").GetComponent<TMP_Text>();
            buttonText.text = (itemType == "Bird" && itemId == selectedBirdId) || (itemType == "Pipe" && itemId == selectedPipeId) ? "Selected" : "Select";

            Image itemImage = item.transform.Find("BirdImage").GetComponent<Image>();
            itemImage.color = new Color(1f, 1f, 1f, 1f); // Fully opaque now that it's owned

            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(() => SelectItem(itemType, itemId));

        }
        else
        {
            audioSource.PlayOneShot(errorSound);
        }
    }

    private void SelectItem(string itemType, int itemId)
    {
        if (itemType == "Bird")
        {
            selectedBirdId = itemId;
            PlayerPrefs.SetInt("SelectedBird", itemId);
        }
        else if (itemType == "Pipe")
        {
            selectedPipeId = itemId;
            PlayerPrefs.SetInt("SelectedPipe", itemId);
        }

        PlayerPrefs.Save();
        audioSource.PlayOneShot(buttonSound);

        // Refresh the UI to reflect the selected item
        Transform content = itemType == "Bird" ? birdsContent : pipesContent;
        int selectedId = itemType == "Bird" ? selectedBirdId : selectedPipeId;

        for (int i = 0; i < content.childCount; i++)
        {
            Transform item = content.GetChild(i);
            Button buyButton = item.Find("BuyButton").GetComponent<Button>();
            TMP_Text buttonText = buyButton.transform.Find("Text (TMP)").GetComponent<TMP_Text>();

            Image cardBackground = item.GetComponent<Image>();
            if (cardBackground != null)
            {
                // Update card background color based on selection
                cardBackground.color = (i == selectedId) ? new Color(0f, 1f, 0f, 1f) : new Color(1f, 1f, 1f, 1f); // Green if selected, white otherwise
            }

            // Update button text only for unlocked items
            if (itemType == "Bird" ? unlockedBirds[i] : unlockedPipes[i])
            {
                buttonText.text = (i == selectedId) ? "Selected" : "Select";
            }
            else
            {
                buttonText.text = "Purchase"; // Keep "Purchase" for locked items
            }
        }
    }

}
