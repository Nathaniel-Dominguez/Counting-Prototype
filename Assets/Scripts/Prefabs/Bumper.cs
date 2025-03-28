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
    [SerializeField] private Animator bumperAnimator;
    [SerializeField] private Light bumperLight;
    [SerializeField] private ParticleSystem bumperParticles;

    // Private fields
    private SFXManager sfxManager;
    private bool isActive = true;
    private float originalLightIntensity;
    private Material bumperMaterial;
    private Color originalEmissionColor;
    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

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

        // Cache renderer material for emission effects
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null && renderer.material != null)
        {
            bumperMaterial = renderer.material;
            if (bumperMaterial.HasProperty(EmissionColor))
            {
                originalEmissionColor = bumperMaterial.GetColor(EmissionColor);
            }
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
            sfxManager.PlayBumperHitSound(transform.position, collision.relativeVelocity.magnitude);
        }

        // Trigger visual feedback
        ActivateBumperFeedback();
        // Start cooldown
        StartCoroutine(CooldownRoutine());
    }

    private void ActivateBumperFeedback()
    {
        // Play animation if available
        if (bumperAnimator != null)
        {
            bumperAnimator.SetTrigger("Activate");
        }

        // Play particles
        if (bumperParticles != null)
        {
            bumperParticles.Play();
        }

        // Flash light
        if (bumperLight != null)
        {
            StartCoroutine(FlashLightRoutine());
        }

        // Flash emission
        if (bumperMaterial != null && bumperMaterial.HasProperty(EmissionColor))
        {
            StartCoroutine(FlashEmissionRoutine());
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

    private IEnumerator FlashEmissionRoutine()
    {
        // Enhance emission temporarily
        bumperMaterial.EnableKeyword("_EMISSION");
        Color enhancedEmission = originalEmissionColor * 3.0f;

        bumperMaterial.SetColor(EmissionColor, enhancedEmission);

        float elapsed = 0f;
        float flashDuration = 0.3f;

        while (elapsed < flashDuration)
        {
            Color currentColor = Color.Lerp(enhancedEmission, originalEmissionColor, elapsed / flashDuration);

            bumperMaterial.SetColor(EmissionColor, currentColor);
            
            elapsed += Time.deltaTime;
            yield return null;
        }
        bumperMaterial.SetColor(EmissionColor, originalEmissionColor);
    }
}