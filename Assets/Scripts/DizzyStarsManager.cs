using UnityEngine;
using System.Collections.Generic;

public class DizzyStarsManager : MonoBehaviour
{
    public GameObject starPrefab; // Prefab for the star
    public Transform bird; // Reference to the bird
    private int maxStars = 5; // Maximum stars that can appear
    private float radius = 1.8f; // Adjusted radius of rotation
    private float rotationSpeed = 200f; // Speed of rotation

    private List<GameObject> stars = new List<GameObject>(); // List of active stars
    private int currentStarCount = 0; // How many stars are currently active
    private float heightOffset = 1.5f; // Adjusted height to be above the bird

    // Z movement variables
    private float minZ = -0.25f;
    private float maxZ = -0.15f;
    private float zOscillationSpeed = 1f; // Speed of Z oscillation

    void Start()
    {
        UpdateStarPositions(); // Initialize star positions
    }

    void Update()
    {
        if (stars.Count > 0)
        {
            UpdateStarPositions(); // Continuously update star positions
        }
    }

    public void AddStar()
    {
        if (currentStarCount >= maxStars) return; // Limit stars to maxStars

        GameObject newStar = Instantiate(starPrefab, bird.position, Quaternion.identity);
        newStar.transform.localScale = Vector3.one * 0.1f; // Adjusted star size
        newStar.transform.SetParent(transform); // Keep hierarchy clean
        stars.Add(newStar);
        currentStarCount++;

        UpdateStarPositions();
    }

    private void UpdateStarPositions()
    {
        float angleStep = 360f / stars.Count; // Evenly distribute stars in a circle
        float currentAngle = Time.time * rotationSpeed; // Rotate over time

        for (int i = 0; i < stars.Count; i++)
        {
            float angle = currentAngle + (i * angleStep);
            float radian = angle * Mathf.Deg2Rad;

            // Sync Z movement with the angle, so stars move forward/backward as they rotate
            float zOffset = Mathf.Lerp(minZ, maxZ, (Mathf.Sin(radian * zOscillationSpeed) + 1) / 2);

            Vector3 newPosition = new Vector3(
                bird.position.x + Mathf.Cos(radian) * radius,  // X movement
                bird.position.y + heightOffset + Mathf.Sin(radian) * (radius * 0.3f),  // Y movement
                zOffset
            );

            stars[i].transform.position = newPosition;
        }
    }

    public void RemoveAllStars()
    {
        foreach (GameObject star in stars)
        {
            Destroy(star);
        }
        stars.Clear();
        currentStarCount = 0;
    }
}
