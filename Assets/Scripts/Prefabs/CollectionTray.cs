using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Collection tray component for pachinko game
// Handles ball drain at the bottom of the playfield
// Returns balls to the pool when they enter the tray to be used again

[RequireComponent(typeof(Collider))]
public class CollectionTray : MonoBehaviour
{
    [Header("Tray Settings")]
    [SerializeField] private string ballTag = "Ball";
    [SerializeField] private float ballRetentionTime = 0.5f;
    [Tooltip("If true, balls will immediately return to pool; if false, they'll stay in the collection tray up to max capacity")]
    [SerializeField] private bool immediateReturn = true;
    [SerializeField] private int maxCapacity = 5;
    [SerializeField] private float ballFadeOutTime = 0.3f;

    [Header("Visual Effects")]
    [SerializeField] private ParticleSystem trayParticles;
    [SerializeField] private Light trayLight;
    [SerializeField] private float lightFlashDuration = 0.3f;
    [SerializeField] private Color lightColor = new Color(0.2f, 0.8f, 0.2f); // Green? Probably change it to something bad like red
    [SerializeField] private Animator trayAnimator;
    [SerializeField] private string triggerAnimationName = "Collect";

    [Header("Physics Settings")]
    [SerializeField] private float velocityThreshold = 0.1f; // Consider ball settled when velocity is below this
    [SerializeField] private float maxSettleTime = 3f; // Maximum time to wait for settling

    private int ballsInTray = 0;
    private SFXManager sfxManager;
    private float originalLightIntensity;
    private Collider trayCollider;
    private bool isProcessing = false;
    private List<GameObject> trackedBalls = new List<GameObject>();

    private void Awake()
    {
        // Get and configure the collider
        trayCollider = GetComponent<Collider>();
        if (trayCollider != null && !trayCollider.isTrigger)
        {
            trayCollider.isTrigger = true;
            Debug.Log($"CollectionTray: Collider on {gameObject.name} set to trigger mode");
        }
    }

    private void Start()
    {
        // Get references
        sfxManager = SFXManager.Instance;

        // Validate the SFX Manager
        if (sfxManager == null)
        {
            Debug.LogWarning("CollectionTray: SFXManager.Instance is null. SFX features will be disabled.");
        }

        // Configure light
        if (trayLight != null)
        {
            originalLightIntensity = trayLight.intensity;
            trayLight.color = lightColor;
            trayLight.intensity = 0;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the colliding object is a ball
        if (other.CompareTag(ballTag) && !trackedBalls.Contains(other.gameObject))
        {
            // Play visual and audio effects
            PlayCollectionEffects();

            if (immediateReturn)
            {
                // Start ball return process
                StartCoroutine(ReturnBallToPool(other.gameObject));
            }
            else
            {
                // Add ball to tracked balls
                trackedBalls.Add(other.gameObject);

                // Add ball to tray
                StartCoroutine(ProcessBallInTray(other.gameObject));
            }

            // Check if game should end
            GameManager.Instance.CheckGameOver();
        }
    }

    private IEnumerator ReturnBallToPool(GameObject ball)
    {
        isProcessing = true;

        // Optional: Make the ball slowly fade out
        Renderer ballRenderer = ball.GetComponent<Renderer>();
        if (ballRenderer != null && ballFadeOutTime > 0)
        {
            Material ballMaterial = ballRenderer.material;
            Color originalColor = ballMaterial.color;
            float elapsed = 0f;

            while (elapsed < ballFadeOutTime)
            {
                Color fadeColor = originalColor;
                fadeColor.a = Mathf.Lerp(1f, 0f, elapsed / ballFadeOutTime);
                ballMaterial.color = fadeColor;

                elapsed += Time.deltaTime;
                yield return null;
            }
        }
        else
        {
            // If no renderer or we don't want to fade, just wait for a short time
            yield return new WaitForSeconds(0.1f);
        }

        // Return the ball to the pool
        BallPoolManager.Instance.ReturnBall(ball);

        if (trackedBalls.Contains(ball))
        {
            trackedBalls.Remove(ball);
        }

        isProcessing = false;
    }

    private IEnumerator ProcessBallInTray(GameObject ball)
    {
        // Increase ball count
        ballsInTray++;

        // Disable ball physics
        Rigidbody ballRb = ball.GetComponent<Rigidbody>();

        if (ballRb != null)
        {
            // Let the ball bounce and settle naturally
            float settleTimer = 0f;
            bool isSettled = false;
        

            // Wait for the ball to settle based on velocity or max time
            while (!isSettled && settleTimer < maxSettleTime)
            {
                // Check if ball still exists
                if (ball == null || !ball.activeInHierarchy)
                {
                    yield break;
                }

                // Consider ball settled if velocity is below threshold
                if (ballRb.linearVelocity.magnitude < velocityThreshold &&
                    ballRb.angularVelocity.magnitude < velocityThreshold)
                {
                    isSettled = true;
                }

                settleTimer += Time.deltaTime;
                yield return null;
            }
            // Set velocity of balls caught to 0
            ballRb.linearVelocity = Vector3.zero;
            ballRb.angularVelocity = Vector3.zero;

            // Now that the ball has settled, make it kinematic
            ballRb.isKinematic = true;
        }

        // If we're over capacity, start removing oldest balls
        if (ballsInTray > maxCapacity)
        {
            if (trackedBalls.Contains(ball))
            {
                trackedBalls.Remove(ball);
            }

            BallPoolManager.Instance.ReturnBall(ball);
            ballsInTray--;
            yield break;
        }

        // Position the ball somewhere in the tray (keep current X/Z, adjust Y slightly)
        Vector3 currentPos = ball.transform.position;
        Vector3 randomOffset = new Vector3(
            0f, // Keep X position
            UnityEngine.Random.Range(0.05f, 0.15f), // Slight Y adjustment
            0f // Keep Z position
        );

        // Ensure ball stays within tray bounds
        Vector3 trayBounds = Vector3.one * 0.5f; // Assuming tray is roughly 1x1 units
        if (trayCollider != null && trayCollider is BoxCollider boxCollider)
        {
            trayBounds = boxCollider.size * 0.4f; // 80% of collider size to keep inside
        }
        
        // Clamp position to stay within bounds
        float minX = transform.position.x - trayBounds.x;
        float maxX = transform.position.x + trayBounds.x;
        float minZ = transform.position.z - trayBounds.z;
        float maxZ = transform.position.z + trayBounds.z;

        float clampedX = Mathf.Clamp(currentPos.x, minX, maxX);
        float clampedZ = Mathf.Clamp(currentPos.z, minZ, maxZ);

        ball.transform.position = new Vector3(
            clampedX,
            transform.position.y + randomOffset.y,
            clampedZ
        );
        // Wait for the retention time
        yield return new WaitForSeconds(ballRetentionTime);

        // Return ball to the pool with fade effect
        if (ball != null && ball.activeInHierarchy)
        {
            StartCoroutine(ReturnBallToPool(ball));
        }
        
        ballsInTray--;
    }

    private void PlayCollectionEffects()
    {
        // Play particles
        if (trayParticles != null)
        {
            trayParticles.Play();
        }

        // Flash light
        if (trayLight != null)
        {
            StartCoroutine(FlashLight());
        }

        // Trigger animation if available
        if (trayAnimator != null)
        {
            trayAnimator.SetTrigger(triggerAnimationName);
        }

        // Play sound
        if (sfxManager != null)
        {
            sfxManager.PlaySFX("Drain", 0.4f);
        }
    }

    private IEnumerator FlashLight()
    {
        trayLight.intensity = originalLightIntensity * 1.5f; // Brighter flash

        float elapsed = 0f;
        while (elapsed < lightFlashDuration)
        {
            float intensity = Mathf.Lerp(originalLightIntensity * 1.5f, 0, elapsed / lightFlashDuration);
            trayLight.intensity = intensity;

            elapsed += Time.deltaTime;
            yield return null;
        }

        trayLight.intensity = 0;
    }

    // Public method to clear all balls from the tray
    public void ClearTray()
    {
        for (int i = trackedBalls.Count - 1; i >= 0; i--)
        {
            GameObject ball = trackedBalls[i];
            if (ball != null && ball.activeInHierarchy)
            {
                BallPoolManager.Instance.ReturnBall(ball);
            }
        }

        trackedBalls.Clear();
        ballsInTray = 0;
    }

    // Public method to get current number of balls in tray
    public int GetBallCount()
    {
        return ballsInTray;
    }

    // Public method to check if tray is currently processing a ball
    public bool IsProcessingBall()
    {
        return isProcessing;
    }

    // Visual debugging
    private void OnDrawGizmos()
    {
        // Draw a colored wireframe to represent the collection area
        Gizmos.color = new Color(0.2f, 0.8f, 0.2f, 0.3f); // Green with alpha

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
            else
            {
                // Fallback for other collider types
                Gizmos.DrawCube(transform.position, Vector3.one);
            }
        }
        else
        {
            // Fallback if no collider
            Gizmos.DrawCube(transform.position, Vector3.one);
        }

        // Draw indicator for immediate return or capacity
        if (immediateReturn)
        {
            // Draw arrow down to indicate disappearing
            Gizmos.color = Color.yellow;
            Vector3 center = transform.position;
            Vector3 down = center - Vector3.up * 0.5f;
            Gizmos.DrawLine(center, down);
            Gizmos.DrawLine(down, down + new Vector3(0.1f, 0.1f, 0));
            Gizmos.DrawLine(down, down + new Vector3(-0.1f, 0.1f, 0));
        }
        else
        {
            // Draw capacity indicator
            Gizmos.color = Color.white;

#if UNITY_EDITOR
            // Add label for capacity
            UnityEditor.Handles.Label(transform.position + Vector3.up * 0.5f, $"Capacity: {maxCapacity}");

            // Draw small balls to represent capacity
            float spacing = 0.2f;
            Vector3 startPos = transform.position + Vector3.up * 0.1f - Vector3.right * ((maxCapacity-1) * spacing * 0.5f);

            for (int i = 0; i < maxCapacity; i++)
            {
                Gizmos.DrawSphere(startPos + Vector3.right * (i * spacing), 0.05f);
            }
#endif
        }
    }
}