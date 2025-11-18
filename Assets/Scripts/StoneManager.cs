using System.Collections.Generic;
using UnityEngine;

public class StoneManager : MonoBehaviour
{
    
    public float CurrentMoveSpeed => moveStonesSpeed;
    
    [Header("1. Player Tracking & Spawn Position")]
    [SerializeField] private Transform goatTransform;
    [SerializeField] private float spawnOffsetFromGoat = 20f;
    [SerializeField] private float initialPlatformStartX = 0f;
    
    [Header("2. Base Settings & Initial Spawn")]
    [SerializeField] GameObject stonePrefab;
    [SerializeField] private int initialStoneCount = 5;
    [SerializeField] private float initialStoneSpacing = 10f;
    [SerializeField] float stonesDestroyDistance = 15f; 

    [Header("3. Movement & Difficulty Scaling")]
    [SerializeField] private float baseMoveSpeed = 2f; 
    [SerializeField] private float maxSpeed = 10f; 
    [SerializeField] private float speedIncreaseRate = 0.1f; 

    [Header("4. Horizontal Gap Control (Difficulty)")]
    [SerializeField] private float baseHorizontalGap = 15f; 
    [SerializeField] private float maxHorizontalGap = 40f; 
    [SerializeField] private float gapRampUpDuration = 90f; 

    [Header("5. Vertical Height Control")]
    [SerializeField] private float minStoneY = -3f;
    [SerializeField] private float maxStoneY = 1f;
    [SerializeField] private float maxJumpHeightDifference = 0.5f;

    private float moveStonesSpeed; 
    private float currentTargetGap;
    private float gameStartTime;
    private float nextSpawnTime;
    private float lastStoneY; 
    HashSet<Transform> activeStones = new HashSet<Transform>();


    void Start()
    {
        gameStartTime = Time.time;
        moveStonesSpeed = baseMoveSpeed; 
        lastStoneY = transform.position.y;
        currentTargetGap = baseHorizontalGap;
        float startX = initialPlatformStartX;
        
        for (int i = 0; i < initialStoneCount; i++)
        {
            SpawnStone(startX + (i * initialStoneSpacing), lastStoneY);
        }

        float initialRequiredInterval = currentTargetGap / moveStonesSpeed;
        nextSpawnTime = Time.time + initialRequiredInterval + 0.5f; 
    }

    void Update()
    {
        // Follow the goat's horizontal position
        if (goatTransform != null)
        {
            Vector3 newPosition = transform.position;
            newPosition.x = goatTransform.position.x + spawnOffsetFromGoat;
            transform.position = newPosition;
        }

        // Increase stone movement speed over time
        moveStonesSpeed = Mathf.Min(maxSpeed, moveStonesSpeed + speedIncreaseRate * Time.deltaTime);
        
        float timePassed = Time.time - gameStartTime;

        // Gradually increase the target gap between stones
        float t = Mathf.Clamp01(timePassed / gapRampUpDuration); 
        currentTargetGap = Mathf.Lerp(baseHorizontalGap, maxHorizontalGap, t);

        // Determine when to spawn the next stone based on current speed and gap
        float requiredSpawnInterval = currentTargetGap / CurrentMoveSpeed;

        // Spawn stones at calculated intervals
        if (Time.time > nextSpawnTime)
        {
            nextSpawnTime = Time.time + requiredSpawnInterval;
            SpawnStone(transform.position.x); 
        }

        MoveActiveStones(); 
    }
    
    // Spawns a stone at a given x position with a random y position within constraints 
    void SpawnStone(float xPosition)
    {
        float minTargetY = lastStoneY - maxJumpHeightDifference;
        float maxTargetY = lastStoneY + maxJumpHeightDifference;

        float finalMinY = Mathf.Max(minStoneY, minTargetY);
        float finalMaxY = Mathf.Min(maxStoneY, maxTargetY);

        float randomY = Random.Range(finalMinY, finalMaxY);
        
        SpawnStone(xPosition, randomY);
    }

    void SpawnStone(float xPosition, float yPosition)
    {
        lastStoneY = yPosition; 
        // Instantiate the stone at the specified position
        Vector3 spawnPos = new Vector2(xPosition, yPosition); 
        GameObject stone = Instantiate(stonePrefab, spawnPos, Quaternion.identity);
        stone.transform.SetParent(transform); 
        activeStones.Add(stone.transform);
    }


    void MoveActiveStones()
    {
        List<Transform> stonesToRemove = new List<Transform>();

        foreach (Transform stone in activeStones) 
        {
            stone.transform.position += Vector3.left * moveStonesSpeed * Time.deltaTime;
            
            // Check if the stone is far enough behind the goat to be destroyed
            
            if(stone.transform.position.x < goatTransform.position.x - stonesDestroyDistance)
            {
                stonesToRemove.Add(stone);
            }
        }
            // Remove and destroy stones that are out of range
        foreach (Transform stone in stonesToRemove)
        {
            activeStones.Remove(stone);
            Destroy(stone.gameObject);
        }
    }
}