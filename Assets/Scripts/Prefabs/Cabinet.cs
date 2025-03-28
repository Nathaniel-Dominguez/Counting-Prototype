using UnityEngine;

/// <summary>
/// Handles collision detection and sound effects for the wooden cabinet.
/// Plays impact sounds when balls hit the wooden borders.
/// </summary>
public class Cabinet : MonoBehaviour
{
    [Header("Impact Settings")]
    [Tooltip("Minimum velocity magnitude required to play impact sound")]
    [SerializeField] private float minImpactVelocity = 0.5f;
    
    [Tooltip("Scale factor to convert impact velocity to sound force (higher values = louder sounds)")]
    [SerializeField] private float impactForceScale = 1.0f;
    
    [Tooltip("Cooldown time between impact sounds from the same ball (seconds)")]
    [SerializeField] private float soundCooldownTime = 0.1f;

    // Track recent collisions to prevent sound spam
    private float lastSoundTime;

    private void Start()
    {
        lastSoundTime = 0f;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the colliding object is a ball
        if (collision.gameObject.CompareTag("Ball"))
        {
            // Calculate the impact force based on relative velocity
            float impactVelocity = collision.relativeVelocity.magnitude;
            
            // Only play sound if impact is strong enough and we're not in cooldown
            if (impactVelocity >= minImpactVelocity && Time.time - lastSoundTime >= soundCooldownTime)
            {
                // Calculate impact force for sound volume/pitch variation
                float impactForce = Mathf.Clamp(impactVelocity * impactForceScale, 0f, 10f);
                
                // Play the wooden impact sound at the collision position
                if (SFXManager.Instance != null)
                {
                    Vector3 contactPoint = collision.contacts[0].point;
                    SFXManager.Instance.PlayWoodenImpactSound(contactPoint, impactForce);
                    
                    // Update last sound time
                    lastSoundTime = Time.time;
                }
            }
        }
    }
} 