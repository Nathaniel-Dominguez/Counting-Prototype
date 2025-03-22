using System.Collections.Generic;
using UnityEngine;

public class BallPoolManager : MonoBehaviour
{
    // The Singleton pattern is a design pattern that restricts a class to having only one instance in memory at any time, while providing a global access point to that instance
    // this is how we set it that instance "public static BallPoolManager Instance { get; private set; }"
    public static BallPoolManager Instance { get; private set; }

    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private int initialPoolSize = 20;
    [SerializeField] private bool expandIfNeeded = true;

    private Queue<GameObject> ballPool;

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
        // If pool is empty and expansion is allowed, create a new ball
        if (ballPool.Count == 0 && expandIfNeeded)
        {
            return CreateNewBall();
        }

        // Get a ball from the pool the (?) works as a compact if-else statement
        GameObject ball = ballPool.Count > 0 ? ballPool.Dequeue() : null;

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

            // Activate the ball
            // old code ball.SetActive(true);
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
        ball.SetActive(false);

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
}
