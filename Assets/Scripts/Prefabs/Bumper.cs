using UnityEngine;
using System.Collections;

// Controls the Behavior of bumpers in the pachinko machine
// Provides additional force to ball on collision and visual/audio feedback
public class Bumper : MonoBehaviour
{
    [Header("BumperProperties")]
    [SerializeField] private float bounceForce = 5.0f;
    [SerializeField] private float minImpactVelocity = 2.0f;
    [SerializeField] private float cooldownTime = 0.1f;

    [Header("Visual Feedback")]
    [SerializeField] private Light bumperLight;

    // Private fields
    private SFXManager sfxManager;
    private bool isActive = true;
    private float originalLightIntensity;

    private void Start()
    {
        // Get reference to the SFX Manager
        sfxManager = SFXManager.Instance;

        // Validate the SFX Manager
        if (sfxManager == null)
        {
            Debug.LogWarning("Bumper: SFXManager.Instance is null. SFX features will be disabled.");
        }

        // Store original light intensity if we have a light
        if (bumperLight != null)
        {
            originalLightIntensity = bumperLight.intensity;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Only process collisions with balls that have sufficient velocity
        if (!isActive || !collision.gameObject.CompareTag("Ball") || collision.relativeVelocity.magnitude < minImpactVelocity)
        {
            return;
        }

        // Calculate bounce direction - away from the bumper center
        Vector3 bounceDirection = (collision.transform.position - transform.position).normalized;

        // Apply force to the ball
        Rigidbody ballRb = collision.gameObject.GetComponent<Rigidbody>();
        if (ballRb != null)
        {
            // Cancel existing velocity for more predictable bounce behavior
            ballRb.linearVelocity = Vector3.zero;
            // Apply new force in the bounce direction
            ballRb.AddForce(bounceDirection * bounceForce, ForceMode.Impulse);
        }

        // Play bumper sound effect through the sfxManager = SFXManager.Instace
        if (sfxManager != null)
        {
            sfxManager.PlayBumperHitSound(collision.relativeVelocity.magnitude);
        }

        // Trigger visual feedback
        ActivateBumperFeedback();
        // Start cooldown
        StartCoroutine(CooldownRoutine());
    }

    private void ActivateBumperFeedback()
    {
        // Flash light
        if (bumperLight != null)
        {
            StartCoroutine(FlashLightRoutine());
        }
    }

    private IEnumerator CooldownRoutine()
    {
        isActive = false;
        yield return new WaitForSeconds(cooldownTime);
        isActive = true;
    }
    
    private IEnumerator FlashLightRoutine()
    {
        bumperLight.intensity = originalLightIntensity * 2; // Brighter flash
        float elapsed = 0f;
        float flashDuration = 0.2f;

        while (elapsed < flashDuration)
        {
            float intensity = Mathf.Lerp(originalLightIntensity * 2, 0, elapsed / flashDuration);
            bumperLight.intensity = intensity;
            elapsed += Time.deltaTime;
            yield return null;
        }
        bumperLight.intensity = 0;
    }
}