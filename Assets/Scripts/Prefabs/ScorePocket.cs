using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Score pocket component for pachinko game
// Handles the scoring, visual and audio feedback when a ball enters the pocket

[RequireComponent(typeof(Collider))]
public class ScorePocket : MonoBehaviour 
{
    public enum ScoreType
    {
        LowScore,
        MediumScore,
        HighScore,
        Jackpot
    }

    [Header("Score Settings")]
    [SerializeField] private ScoreType pocketType = ScoreType.LowScore;
    [SerializeField] private int pointValue = 100;
    [SerializeField] private string ballTag = "Ball";
    
    [Header("Settling Conditions")]
    [SerializeField] private float requiredSettleTime = 0.2f; // Time ball must stay still to count
    [SerializeField] private float velocityThreshold = 1.0f; // Ball must be below this velocity to be considered settled
    [SerializeField] private float maxSettleWaitTime = 2.0f; // Max time to wait for a ball to settle

    [Header("Visual Effects")]
    [SerializeField] private ParticleSystem scoreParticles;
    [SerializeField] private Light scoreLight;
    [SerializeField] private float lightFlashDuration = 0.5f;
    [SerializeField] private Color lightColor = Color.white; // check if this can be changed in editor, remove this comment once fixed
    [SerializeField] private float scoreDelay = 0.2f;

    [Header("Animation")]
    [SerializeField] private Animator pocketAnimator;
    [SerializeField] private string triggerAnimationName = "Activate";

    [Header("Ball Handling")]
    [Tooltip("If true, ball will immediately return to pool without animating to collection point")]
    [SerializeField] private bool returnDirectlyToPool = false;
    [Tooltip("How long to wait before returning ball to pool when using direct return")]
    [SerializeField] private float directReturnDelay = 0.5f;
    [SerializeField] private Transform collectionPoint;

    private SFXManager sfxManager;
    private float originalLightIntensity;
    private bool isProcessingBall = false;
    private Collider triggerCollider;

    // Dictionary to track balls that are currently in the pocket
    private Dictionary<GameObject, BallSettleData> ballsInPocket = new Dictionary<GameObject, BallSettleData>();

    private class BallSettleData
    {
        public float settleTimer = 0f;
        public float totalTimeInPocket = 0f;
        public bool isSettled = false;
    }

    private void Awake()
    {
        // Ensure the collider is set to trigger mode
        triggerCollider = GetComponent<Collider>();
        if (triggerCollider != null && !triggerCollider.isTrigger)
        {
            triggerCollider.isTrigger = true;
            Debug.Log($"ScorePocket: Collider on {gameObject.name} set to trigger mode");
        }

        // Set default point values if not configured with editor script
        if (pointValue <= 0)
        {
            pointValue = GetDefaultPointValue();
        }

        // Initialize ball tracking dictionary
        ballsInPocket = new Dictionary<GameObject, BallSettleData>();
    }

    private void Start()
    {
        // Get references
        sfxManager = SFXManager.Instance;
        
        // Validate the SFX Manager
        if (sfxManager == null)
        {
            Debug.LogWarning("ScorePocket: SFXManager.Instance is null. SFX features will be disabled.");
        }

        // Configure light
        if (scoreLight != null)
        {
            originalLightIntensity = scoreLight.intensity;
            scoreLight.color = lightColor;
            scoreLight.intensity = 0;
        }
    }

    private void Update()
    {
        // Process balls in pocket and check for settlement
        UpdateBallSettlement();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the colliding object is a ball and we're not already processing one
        if (other.CompareTag(ballTag) && !isProcessingBall)
        {
            // Only add the ball if it's not already in the dictionary
            if (!ballsInPocket.ContainsKey(other.gameObject))
            {
                // Start tracking this ball
                ballsInPocket.Add(other.gameObject, new BallSettleData());
                Debug.Log($"Ball entered pocket: {other.gameObject.name}");
            }
            else
            {
                // Ball is already being tracked - could happen with physics edge cases
                Debug.Log($"Ball already in pocket: {other.gameObject.name}");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Check if the exiting object is a ball we're tracking
        if (other.CompareTag(ballTag) && ballsInPocket.ContainsKey(other.gameObject))
        {
            // Get the ball's position and check if it's actually outside the pocket's bounds
            // This helps prevent false exits due to physics glitches

            Bounds pocketBounds = triggerCollider.bounds;
            Renderer ballRenderer = other.GetComponent<Renderer>();

            // Only remove if the ball is actually outside the collider bounds
            // Use a small margin to prevent flickering at the edges
            if (ballRenderer != null)
            {
                // Add a small margin to the bounds check (0.05 units)
                if (!pocketBounds.Intersects(ballRenderer.bounds))
                {
                    // Ball is truly outside - stop tracking it
                    ballsInPocket.Remove(other.gameObject);
                    Debug.Log($"Ball exited pocket before settling: {other.gameObject.name}");
                }
                else
                {
                    // The ball appears to be inside or partially inside despite the trigger exit
                    Debug.Log($"Ignoring false exit event for ball: {other.gameObject.name}");
                }
            }
            else
            {
                // Fallback if no renderer - use simple position check
                if (!pocketBounds.Contains(other.transform.position))
                {
                    ballsInPocket.Remove(other.gameObject);
                    Debug.Log($"Ball exited pocket before settling: {other.gameObject.name}");
                }
            }
        }
    }

    private void UpdateBallSettlement()
    {
        // Don't process anything if we're already handling a scored ball
        if (isProcessingBall)
            return;
        
        // List to keep track of balls to remove
        List<GameObject> ballsToRemove = new List<GameObject>();
        GameObject ballToScore = null;

        // Check each ball in the pocket using key value pair `kvp`
        foreach (var kvp in ballsInPocket)
        {
            GameObject ball = kvp.Key;
            BallSettleData data = kvp.Value;

            // Skip if ball is null or inactive
            if (ball == null || !ball.activeInHierarchy)
            {
                ballsToRemove.Add(ball);
                continue;
            }

            // Skip if ball is already settled
            if (data.isSettled)
            {
                ballToScore = ball;
                break;
            }

            // Verify ball is still within the pocket bounds (fixes physics glitches)
            bool isInsidePocket = IsObjectInsidePocket(ball);
            if (!isInsidePocket)
            {
                // Ball appears to be outside despite not triggering OnTriggerExit
                Debug.Log($"Ball detected outside pocket bounds: {ball.name}");
                ballsToRemove.Add(ball);
                continue;
            }

            // Check if the ball is still moving
            Rigidbody ballRb = ball.GetComponent<Rigidbody>();
            if (ballRb != null)
            {
                // Wake up rigidbody to ensure accurate velocity readings
                if (ballRb.IsSleeping())
                {
                    ballRb.WakeUp();
                }
                // Check velocities against thresholds
                bool isBelowLinearVelocityThreshold = ballRb.linearVelocity.magnitude < velocityThreshold;
                bool isBelowAngularThreshold = ballRb.angularVelocity.magnitude < velocityThreshold;

                if (isBelowLinearVelocityThreshold && isBelowAngularThreshold)
                {
                    // Ball is slow enough, increment settle timer
                    data.settleTimer += Time.deltaTime;

                    // Check if it's been settled long enough
                    if (data.settleTimer >= requiredSettleTime)
                    {
                        Debug.Log($"Ball settled in pocket: {ball.name}");
                        data.isSettled = true;
                        ballToScore = ball;
                        break;
                    }
                }
                else
                {
                    // Ball is moving too fast, reset settle timer
                    data.settleTimer = 0f;
                }
            }
            // Update total time in pocket
            data.totalTimeInPocket += Time.deltaTime;

            // Check if we've waited too long for this ball to settle
            if (data.totalTimeInPocket >= maxSettleWaitTime)
            {
                Debug.Log($"Ball reached max wait time, forcing settle: {ball.name}");
                data.isSettled = true;
                ballToScore = ball;
                break;
            }
        }

        // Clean up any null or inactive balls
        foreach (GameObject ball in ballsToRemove)
        {
            ballsInPocket.Remove(ball);
        }

        // Process a settled ball if we found one
        if (ballToScore != null)
        {
            // Remove from tracking
            ballsInPocket.Remove(ballToScore);

            // Process the ball
            ProcessScoredBall(ballToScore);
        }
    }

    private void ProcessScoredBall(GameObject ball)
    {
        // Safety check - make sure ball is still valid and active
        if (ball == null || !ball.activeInHierarchy)
        {
            isProcessingBall = false;
            return;
        }
        
        isProcessingBall = true;

        // Play particles immediately
        if (scoreParticles != null)
        {
            scoreParticles.Play();
        }

        // Flash the light
        if (scoreLight != null)
        {
            StartCoroutine(FlashLight());
        }

        // Trigger animation if available
        if (pocketAnimator != null)
        {
            pocketAnimator.SetTrigger(triggerAnimationName);
        }

        // Play appropriate sound effect
        PlayScoreSound();

        // Delay the actual scoring for visual effect
        StartCoroutine(DelayedScoring(ball));
    }

    private void PlayScoreSound()
    {
        // Use the audio manager to play the appropriate sound
        if (sfxManager != null)
        {
            switch (pocketType)
            {
                case ScoreType.LowScore:
                    sfxManager.PlaySFX("LowScore");
                    break;
                case ScoreType.MediumScore:
                    sfxManager.PlaySFX("MediumScore");
                    break;
                case ScoreType.HighScore:
                    sfxManager.PlaySFX("HighScore");
                    break;
                case ScoreType.Jackpot:
                    sfxManager.PlaySFX("Jackpot");
                    // Use existing sound for emphasis 
                    sfxManager.PlaySFX("GameOver"); 
                    break;
            }
        }
    }

    private IEnumerator FlashLight()
    {
        scoreLight.intensity = originalLightIntensity * 2; // Brighter flash

        float elapsed = 0f;
        while (elapsed < lightFlashDuration)
        {
            float intensity = Mathf.Lerp(originalLightIntensity * 2, 0, elapsed / lightFlashDuration);
            scoreLight.intensity = intensity;

            elapsed += Time.deltaTime;
            yield return null;
        }

        scoreLight.intensity = 0;
    }

    private IEnumerator DelayedScoring(GameObject ball)
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(scoreDelay);

        // Safety check - make sure ball is still valid and active
        if (ball == null || !ball.activeInHierarchy)
        {
            isProcessingBall = false;
            yield break;
        }

        // Award points
        GameManager.Instance.AddPoints(pointValue);

        // Disable the ball's physics and guide it to the collection tray
        Rigidbody ballRb = ball.GetComponent<Rigidbody>();
        if (ballRb != null)
        {
            // Make the ball kinematic to prevent it from rolling
            // ballRb.isKinematic = true;
            // Didn't like how it looked, feel free to try it or changing it a bit

            // Either directly return to pool or move to collection point (make tray prefab next, set limit to balls the tray can hold / depop)
            if (returnDirectlyToPool)
            {
                StartCoroutine(ReturnBallToPool(ball)); // remove this once we have a collection tray
            }
            else
            {
                StartCoroutine(MoveBallToCollectionTray(ball));
            }
        }
    }

    // Public method that BallPoolManager can call when returning a ball to the pool
    public void OnBallReturnedToPool(GameObject ball)
    {
        // Remove the ball from our tracking if we're tracking it
        if (ballsInPocket.ContainsKey(ball))
        {
            ballsInPocket.Remove(ball);
            Debug.Log($"Ball removed from pocket tracking due to pool return: {ball.name}");
        }
    }

    private IEnumerator ReturnBallToPool(GameObject ball)
    {
        // First remove it from tracking to prevent duplicate entries if reused quickly
        ballsInPocket.Remove(ball);
        
        // Optional: Make the ball slowly fade out
        Renderer ballRenderer = ball.GetComponent<Renderer>();
        if (ballRenderer != null)
        {
            Material ballMaterial = ballRenderer.material;
            Color originalColor = ballMaterial.color;
            float elapsed = 0f;

            while (elapsed < directReturnDelay)
            {
                Color fadeColor = originalColor;
                fadeColor.a = Mathf.Lerp(1f, 0f, elapsed / directReturnDelay);
                ballMaterial.color = fadeColor;

                elapsed += Time.deltaTime;
                yield return null;
            }
        }
        else
        {
            // If no renderer or we don't want to fade, just wait for the delay time
            yield return new WaitForSeconds(directReturnDelay);
        }

        // After the delay, return the ball to the pool and allow processing another ball
        if (ball != null && ball.activeInHierarchy)
        {
            BallPoolManager.Instance.ReturnBall(ball);
        }
        
        isProcessingBall = false;
    }

    private IEnumerator MoveBallToCollectionTray(GameObject ball)
    {
        // First remove it from tracking to prevent duplicate entries if reused quickly
        ballsInPocket.Remove(ball);
        
        // Use the collectionPoint if assigned, otherwise use the GameManager's collection tray
        Transform targetTray = collectionPoint != null ?
            collectionPoint : GameManager.Instance.GetCollectionTray();

        Vector3 startPos = ball.transform.position;
        float duration = 1.0f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            // Check if ball is still valid
            if (ball == null || !ball.activeInHierarchy)
                break;

            ball.transform.position = Vector3.Lerp(startPos, targetTray.position, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Return the ball to the pool
        if (ball != null && ball.activeInHierarchy)
        {
            BallPoolManager.Instance.ReturnBall(ball);
        }

        // Ball has reached collection tray, allow for another ball to be processed
        isProcessingBall = false;
    }

    private void OnDisable()
    {
        // Clear tracking when component is disabled
        ballsInPocket.Clear();
        isProcessingBall = false;
    }

    private int GetDefaultPointValue()
    {
        // Set default values based on type
        switch (pocketType)
        {
            case ScoreType.LowScore:
                return 100;
            case ScoreType.MediumScore:
                return 500;
            case ScoreType.HighScore:
                return 1000;
            case ScoreType.Jackpot:
                return 5000;
            default:
                return 100;
        }
    }

    // Helper method for getting the score value - useful for other GameManager tracking score 
    // Game Manager replaced counter alongside scorepocket.cs
    // It was scope creep my bad looool
    public int GetPointValue()
    {
        return pointValue;
    }

    // Helper method for getting the score pocket type - triggering jackpots modes
    public ScoreType GetPocketType()
    {
        return pocketType;
    }

    // Helper method to check if an object is inside the pocket bounds
    private bool IsObjectInsidePocket(GameObject obj)
    {
        if (triggerCollider == null || obj == null)
            return false;

        Bounds pocketBounds = triggerCollider.bounds;
        Renderer objRenderer = obj.GetComponent<Renderer>();

        if (objRenderer != null)
        {
            // Use renderer bounds to check if object is inside
            return pocketBounds.Intersects(objRenderer.bounds);
        }
        else
        {
            // Fallback to simple position check
            return pocketBounds.Contains(obj.transform.position);
        }
    }

    // Visual dugging - draw representation in scene view 
    private void OnDrawGizmos()
    {
        // Draw a colored wireframe to represent the scoring zone
        Color gizmoColor;
        switch (pocketType)
        {
            case ScoreType.LowScore:
                gizmoColor = new Color(0.2f, 0.4f, 1f); // Blue
                break;
            case ScoreType.MediumScore:
                gizmoColor = new Color(0.8f, 0.2f, 0.8f); // Purple
                break;
            case ScoreType.HighScore:
                gizmoColor = new Color(1f, 0.9f, 0.2f); // Yellow
                break;
            case ScoreType.Jackpot:
                gizmoColor = new Color(1f, 0.2f, 0.2f); // Red
                break;
            default:
                gizmoColor = Color.white;
                break;
        }

        Gizmos.color = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, 0.3f);

        // Draw different gizmos based on collider type
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            if (col is BoxCollider)
            {
                BoxCollider box = col as BoxCollider;
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawCube(box.center, box.size);
            }
            else if (col is SphereCollider)
            {
                SphereCollider sphere = col as SphereCollider;
                Gizmos.DrawSphere(transform.position + sphere.center, sphere.radius);
            }
            // Add more collider types if needed Mesh maybe?
        }
        else
        {
            // Fallback if no collider
            Gizmos.DrawSphere(transform.position, 0.5f);
        }

        // Draw line to collection point if assigned
        if (collectionPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, collectionPoint.position);
            Gizmos.DrawSphere(collectionPoint.position, 0.1f);
        }

        // Indicate direct return with a different visual
        if (returnDirectlyToPool)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, 0.6f);

            // Draw small arrow down to indicate ball disappearing
            Vector3 center = transform.position;
            Vector3 down = center - Vector3.up * 0.5f;
            Gizmos.DrawLine(center, down);
            Gizmos.DrawLine(down, down + new Vector3(0.1f, 0.1f, 0));
            Gizmos.DrawLine(down, down + new Vector3(-0.1f, 0.1f, 0));
        }
    }
}
