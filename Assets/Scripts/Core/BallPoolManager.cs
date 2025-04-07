using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;

public class BallPoolManager : MonoBehaviour
{
    // The Singleton pattern is a design pattern that restricts a class to having only one instance in memory at any time, while providing a global access point to that instance
    // this is how we set it that instance "public static BallPoolManager Instance { get; private set; }"
    public static BallPoolManager Instance { get; private set; }

    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private int initialPoolSize = 20;
    [SerializeField] private bool expandIfNeeded = true;
    [SerializeField] private float poolCheckInterval = 10f; // How often to check and shuffle the pool (seconds)

    private Queue<GameObject> ballPool;
    private int consecutiveEmptyChecks = 0;
    private bool isShuffling = false;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Initialize the ball pool
        InitializePool();
        PrewarmPool();
    }
    
    private void Start()
    {
        // Start the ball pool check routine
        StartCoroutine(PeriodicPoolCheck());
    }

    private void InitializePool()
    {
        ballPool = new Queue<GameObject>();

        // Create Initial balls and add them to the pool
        for (int i = 0; i < initialPoolSize; i++)
        {
            CreateNewBall();
        }
    }

    // Preload and warm up the physics system
    public void PrewarmPool()
    {
        // Temporarily activate and then deactivate all balls to initialize physics
        for (int i = 0; i < initialPoolSize; i++)
        {
            GameObject ball = ballPool.Dequeue();

            // Activate ball
            ball.SetActive(true);

            // Get rigidbody
            Rigidbody ballRb = ball.GetComponent<Rigidbody>();
            if (ballRb != null)
            {
                // Wake up the rigidbody to initialize physics
                ballRb.WakeUp();
                // Reset velocities
                ballRb.linearVelocity = Vector3.zero;
                ballRb.angularVelocity = Vector3.zero;
            }

            // Deactivate and return to pool
            ball.SetActive(false);
            ballPool.Enqueue(ball);
        }
    }

    private GameObject CreateNewBall()
    {
        // Instantiate a new ball
        GameObject ball = Instantiate(ballPrefab);

        // Set it inactive and add components/scripts if needed
        ball.SetActive(false);

        // Add to the pool (Enqueue) adds an item to the end of the queue. 
        ballPool.Enqueue(ball);

        return ball;
    }

    public GameObject GetBall()
    {
        // Loop until we find an inactive ball or exhaust the pool
        GameObject ball = null;
        int safetyCounter = 0;
        int maxAttempts = ballPool.Count;
        
        while (safetyCounter < maxAttempts)
        {
            // If pool is empty and expansion is allowed, create a new ball
            if (ballPool.Count == 0 && expandIfNeeded)
            {
                ball = CreateNewBall();
                break;
            }
            
            // Get a ball from the pool
            if (ballPool.Count > 0)
            {
                ball = ballPool.Dequeue();
                
                // Check if the ball is already active in the scene
                if (ball != null && ball.activeInHierarchy)
                {
                    // This ball is already active, put it back at the end of the queue
                    // and try another one
                    Debug.LogWarning($"Found active ball in pool: {ball.name}. Skipping and trying another.");
                    ballPool.Enqueue(ball);
                    ball = null;
                    safetyCounter++;
                    continue;
                }
                
                // If we found a valid inactive ball, break the loop
                if (ball != null)
                {
                    break;
                }
            }
            
            safetyCounter++;
        }
        
        // If we couldn't find a valid ball and can expand, create a new one
        if (ball == null && expandIfNeeded)
        {
            Debug.LogWarning("Couldn't find a valid inactive ball, creating a new one.");
            ball = CreateNewBall();
        }

        if (ball != null)
        {
            // Reset ball state
            // Let the launcher activate it only after positioning
            Rigidbody ballRb = ball.GetComponent<Rigidbody>();
            if (ballRb != null)
            {
                // Reset the ball's velocity and angular velocity to zero
                // This is to ensure the ball is not moving when it is retrieved from the pool
                // This is important for the ball pool to work correctly
                ballRb.linearVelocity = Vector3.zero;
                ballRb.angularVelocity = Vector3.zero;
                ballRb.isKinematic = false;
            }

            // Additional check to make absolutely sure the ball is inactive before giving it out
            if (ball.activeInHierarchy)
            {
                Debug.LogError($"Ball {ball.name} is still active despite all checks. Deactivating now.");
                ball.SetActive(false);
            }

            // Don't activate here, let the launcher activate it after positioning
        }

        return ball;
    }

    public void ReturnBall(GameObject ball)
    {
        if (ball == null)
            return;
        
        // First, use the Ball component's reset method if available
        Ball ballComponent = ball.GetComponent<Ball>();
        if (ballComponent != null)
        {
            // This will handle notifying all score pockets and resetting state
            ballComponent.ResetForPool();
        }
        else
        {   
            // Legacy fallback if Ball component is not found
            // Reset and deactivate the ball
            Rigidbody ballRb = ball.GetComponent<Rigidbody>();
            if (ballRb != null)
            {
                ballRb.linearVelocity = Vector3.zero;
                ballRb.angularVelocity = Vector3.zero;
            }

            // Notify all ScorePockets that this ball is being returned to the pool
            // This helps prevent duplicate entries to score pocket dictionary when balls are recycled 
            ScorePocket[] pockets = FindObjectsByType<ScorePocket>(FindObjectsSortMode.None);
            foreach (ScorePocket pocket in pockets)
            {
                pocket.OnBallReturnedToPool(ball);
            }
        }
        
        // Make absolutely sure the ball is deactivated
        if (ball.activeInHierarchy)
        {
            ball.SetActive(false);
        }

        // Add back to the pool
        ballPool.Enqueue(ball);
    }

    // Helper method to return all active balls to the pool
    public void ReturnAllBalls()
    {
        GameObject[] activeBalls = GameObject.FindGameObjectsWithTag("Ball");
        foreach (GameObject ball in activeBalls)
        {
            if (ball.activeInHierarchy)
            {
                ReturnBall(ball);
            }
        }
    }
    
    // Shuffle the ball pool to prevent balls from getting stuck at the end
    private void ShufflePool()
    {
        if (isShuffling || ballPool.Count <= 1)
            return;
            
        isShuffling = true;
        
        // Convert queue to array, shuffle, then convert back to queue
        GameObject[] ballsArray = ballPool.ToArray();
        
        // Fisher-Yates shuffle algorithm
        for (int i = ballsArray.Length - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            GameObject temp = ballsArray[i];
            ballsArray[i] = ballsArray[randomIndex];
            ballsArray[randomIndex] = temp;
        }
        
        // Clear and refill the queue
        ballPool.Clear();
        foreach (GameObject ball in ballsArray)
        {
            ballPool.Enqueue(ball);
        }
        
        Debug.Log($"Ball pool shuffled. Pool now contains {ballPool.Count} balls.");
        isShuffling = false;
    }
    
    // Periodically check if we need to recover or shuffle balls
    private IEnumerator PeriodicPoolCheck()
    {
        while (true)
        {
            yield return new WaitForSeconds(poolCheckInterval);
            
            // Count active balls in scene
            GameObject[] activeBalls = GameObject.FindGameObjectsWithTag("Ball");
            int activeBallCount = activeBalls.Length;
            
            // Check if pool is critically low
            if (ballPool.Count == 0 || ballPool.Count < initialPoolSize * 0.25f)
            {
                consecutiveEmptyChecks++;
                Debug.LogWarning($"Ball pool is low! Pool size: {ballPool.Count}, Active balls: {activeBallCount}");
                
                // If we've had multiple checks with empty pool, recover stuck balls
                if (consecutiveEmptyChecks >= 2)
                {
                    RecoverStuckBalls();
                    consecutiveEmptyChecks = 0;
                }
            }
            else
            {
                consecutiveEmptyChecks = 0;
                
                // Periodically shuffle the pool to avoid getting stuck with the same few balls
                ShufflePool();
            }
        }
    }
    
    // Try to find and recover any stuck balls
    private void RecoverStuckBalls()
    {
        // Find all stuck balls using the StuckBallTracker
        GameObject[] allBalls = GameObject.FindGameObjectsWithTag("Ball");
        int recoveredCount = 0;
        
        foreach (GameObject ball in allBalls)
        {
            if (ball != null && ball.activeInHierarchy)
            {
                StuckBallTracker stuckTracker = ball.GetComponent<StuckBallTracker>();
                if (stuckTracker != null && stuckTracker.IsStuck())
                {
                    Debug.Log($"Recovering stuck ball: {ball.name}, stuck for {stuckTracker.GetStuckTime()} seconds");
                    ReturnBall(ball);
                    recoveredCount++;
                }
            }
        }
        
        if (recoveredCount > 0)
        {
            Debug.Log($"Recovered {recoveredCount} stuck balls");
        }
    }
}
