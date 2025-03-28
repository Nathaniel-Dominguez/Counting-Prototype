using UnityEngine;

/// <summary>
/// Tracks whether a ball is stuck (not moving) in the machine.
/// Attach this to ball prefabs to detect when they're stuck for extended periods.
/// </summary>
public class StuckBallTracker : MonoBehaviour
{
    private float stuckTime = 0f;
    private Vector3 lastPosition;
    private float positionCheckTimer = 0f;
    private float positionCheckInterval = 0.5f; // Check position every half second
    private float positionChangeThreshold = 0.01f; // Consider ball stuck if it moves less than this
    
    // Velocity below which we consider the ball potentially stuck
    private float velocityThreshold = 0.05f;

    private void Start()
    {
        lastPosition = transform.position;
    }

    private void Update()
    {
        // Position-based stuck detection
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null) return;
        
        if (rb.linearVelocity.magnitude <= velocityThreshold)
        {
            positionCheckTimer += Time.deltaTime;
            
            if (positionCheckTimer >= positionCheckInterval)
            {
                // Check if ball has moved since last check
                float distanceMoved = Vector3.Distance(transform.position, lastPosition);
                if (distanceMoved < positionChangeThreshold)
                {
                    // Ball hasn't moved much, increment stuck time
                    stuckTime += positionCheckTimer;
                    if (stuckTime > 3.0f)
                    {
                        // Optional debug log for testing
                        // Debug.Log($"Ball {gameObject.name} stuck for {stuckTime:F1} seconds");
                    }
                }
                else
                {
                    // Ball has moved, reset stuck time
                    stuckTime = 0f;
                }
                
                // Update last position and reset timer
                lastPosition = transform.position;
                positionCheckTimer = 0f;
            }
        }
        else
        {
            // Reset if ball is clearly moving
            stuckTime = 0f;
            positionCheckTimer = 0f;
            lastPosition = transform.position;
        }
    }
    
    public void IncrementStuckTime(float deltaTime)
    {
        stuckTime += deltaTime;
    }
    
    public void ResetStuckTime()
    {
        stuckTime = 0f;
    }
    
    public float GetStuckTime()
    {
        return stuckTime;
    }
    
    // Helper method to quickly check if the ball is considered stuck
    public bool IsStuck(float stuckTimeThreshold = 3.0f)
    {
        return stuckTime >= stuckTimeThreshold;
    }
} 