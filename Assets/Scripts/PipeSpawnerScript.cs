using UnityEngine;

public class PipeSpawner : MonoBehaviour
{
    public GameObject[] pipePrefabs; // Array of pipe prefabs
    public GameObject collectiblePrefab; // Reference to the collectible prefab
    public GameObject easeBonusPrefab;
    public GameObject sizeBonusPrefab;

    public GameObject cloudPrefab; // Cloud prefab for background clouds

    private float spawnRate = 2.5f;
    private float heightOffset = 7f;
    private float minGap = 0.5f;
    private float maxGap = 5.0f;
    private float gapChangeInterval = 1f;
    private float currentGap;
    private float timer = 0f;
    private float gapChangeTimer = 0f;
    private float difficultyTimer = 0f;
    private float moveSpeed = 10f;
    private float speedIncrement = 2f;
    private float minSpawnRate = 0.5f;
    private float additionalGap = 18f;
    private int spawnCollectible = 0;
    private int spawnBonus = 0;
    private bool gameOver = false;
    public int selectedPipeId;

    // Cloud Spawning Variables
    private float cloudSpawnRate = 4f; // Time between cloud spawns
    private float cloudTimer = 0f;
    private float cloudMinY = -15f, cloudMaxY = 15f; // Cloud spawn height
    private float cloudMinSpeed = 1.0f, cloudMaxSpeed = 3.0f; // Cloud move speed

    void Start()
    {
        currentGap = maxGap;
        DestroyExistingPipes();
    }

    void Update()
    {
        if (!gameOver)
        {
            timer += Time.deltaTime;
            gapChangeTimer += Time.deltaTime;
            difficultyTimer += Time.deltaTime;

            if (timer >= spawnRate)
            {
                SpawnPipe();
                timer = 0f;
            }

            if (gapChangeTimer >= gapChangeInterval)
            {
                AdjustGap();
                gapChangeTimer = 0f;
            }

            if (difficultyTimer >= 7f)
            {
                IncreaseDifficulty();
                difficultyTimer = 0f;
            }
        }

        // Cloud Spawning - Uses unscaled time
        cloudTimer += Time.unscaledDeltaTime;
        if (cloudTimer >= cloudSpawnRate)
        {
            SpawnCloud();
            cloudTimer = 0f;
        }
    }


    public void SpawnPipe()
    {
        selectedPipeId = PlayerPrefs.GetInt("SelectedPipe", 0);

        if (selectedPipeId < 0 || selectedPipeId >= pipePrefabs.Length)
        {
            Debug.LogWarning("Invalid pipe ID. Defaulting to the first pipe.");
            selectedPipeId = 0;
        }

        GameObject selectedPipePrefab = pipePrefabs[selectedPipeId];

        float randomHeight = Random.Range(-heightOffset, heightOffset);
        Vector3 spawnPosition = new Vector3(transform.position.x, transform.position.y + randomHeight, transform.position.z);

        GameObject newPipe = Instantiate(selectedPipePrefab, spawnPosition, Quaternion.identity);

        Transform topPipe = newPipe.transform.Find("Top Pipe");
        Transform bottomPipe = newPipe.transform.Find("Bottom Pipe");

        if (topPipe != null && bottomPipe != null)
        {
            topPipe.localPosition = new Vector3(0f, currentGap / 2 + additionalGap, 0f);
            bottomPipe.localPosition = new Vector3(0f, -currentGap / 2 - additionalGap, 0f);
        }

        PipeMoveScript pipeMoveScript = newPipe.GetComponent<PipeMoveScript>();
        if (pipeMoveScript != null)
        {
            pipeMoveScript.moveSpeed = moveSpeed;
        }

        if (spawnCollectible == 3)
        {
            spawnCollectible = 0;
            if (Random.value > 0.5f)
            {
                SpawnCollectible(spawnPosition, -10.0f);
            }
            else
            {
                SpawnCollectible(spawnPosition, 10.0f);
            }
        }
        else
        {
            spawnCollectible += 1;
        }
    }

    void SpawnCollectible(Vector3 pipePosition, float horizontalOffset)
    {
        float randomYOffset = Random.Range(-5f, 5f);
        Vector3 collectiblePosition = new Vector3(pipePosition.x + horizontalOffset, pipePosition.y + randomYOffset, 1f);

        if (spawnBonus == 1)
        {
            spawnBonus++;
            Instantiate(easeBonusPrefab, collectiblePosition, Quaternion.identity);
        }
        else if (spawnBonus == 3)
        {
            Instantiate(sizeBonusPrefab, collectiblePosition, Quaternion.identity);
            spawnBonus = 0;
        }
        else
        {
            spawnBonus++;
            Instantiate(collectiblePrefab, collectiblePosition, Quaternion.identity);
        }
    }

    void AdjustGap()
    {
        currentGap = Random.Range(minGap, maxGap);
    }

    public void DecreaseDifficulty()
    {
        spawnRate += 0.1f;
        additionalGap += 0.5f;
        moveSpeed -= speedIncrement;
    }

    void IncreaseDifficulty()
    {
        if (spawnRate > minSpawnRate)
        {
            spawnRate -= 0.1f;
        }

        if (additionalGap > 15f)
        {
            additionalGap -= 0.5f;
        }

        moveSpeed += speedIncrement;
    }

    public void StopSpawning()
    {
        gameOver = true;
    }

    public void ContinueSpawning()
    {
        gameOver = false;
    }

    // -------------------------------------------
    // NEW: Cloud Spawning Method
    // -------------------------------------------
    private void SpawnCloud()
    {
        if (cloudPrefab == null) return;

        float randomY = Random.Range(cloudMinY, cloudMaxY); // Random Y height
        Vector3 spawnPos = new Vector3(transform.position.x + 10f, randomY, 2f);

        GameObject newCloud = Instantiate(cloudPrefab, spawnPos, Quaternion.identity);

        // Random scale for cloud
        float randomScale = Random.Range(1f, 2f); // Adjust this range as needed
        newCloud.transform.localScale = new Vector3(randomScale, randomScale, 1f);

        float randomSpeed = Random.Range(cloudMinSpeed, cloudMaxSpeed);
        CloudMovement cloudMove = newCloud.GetComponent<CloudMovement>();
        if (cloudMove != null)
        {
            cloudMove.speed = randomSpeed; // Assign movement speed
        }
    }

    private void DestroyExistingPipes()
    {
        GameObject[] existingPipes = GameObject.FindGameObjectsWithTag("Pipe");

        foreach (GameObject pipe in existingPipes)
        {
            Destroy(pipe);
        }
    }
}
