using UnityEngine;
using System.Collections;

// Controls the behavior of pins in the pachinko machine
// Pins are passive obstacles that deflect the ball slightly
public class Pin : MonoBehaviour
{
    [Header("Pin Properties")]
    [SerializeField] private bool isMetal = true; // Material type affects sound
    [SerializeField] private float minImpactForce = 0.2f; // Minimum force required to make a sound
    [SerializeField] private float volumeModifier = 1.0f; // Allow per-pin volume adjustment

    // Private fields
    SFXManager sfxManager;
    // Cooldown to prevent sound spam for multiple hits
    private float lastSoundTime = 0f;
    private float soundCooldown = 0.1f;

    private void Start()
    {
        // Get reference to the SFX Manager
        sfxManager = SFXManager.Instance;

        // Validate the SFX Manager
        if (sfxManager == null)
        {
            Debug.LogWarning("Pin: SFXManager.Instance is null. SFX features will be disabled.");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Only process collisions with balls
        if (!collision.gameObject.CompareTag("Ball"))
            return;
        
        // Calculate impact force (clamped to resonable range)
        float impactForce = Mathf.Clamp(collision.relativeVelocity.magnitude, 0f, 5f);

        // Skip very small impacts
        if (impactForce < minImpactForce)
            return;
            
        // Prevent sound spam with cooldown
        if (Time.time - lastSoundTime < soundCooldown)
            return;
            
        lastSoundTime = Time.time;

        // Play pin hit sound through the SFXManager
        if (sfxManager != null)
        {
            // normalize impact for audio (0-1 range)
            float normalizedForce = impactForce / 5f;
            int materialType = isMetal ? 0 : 1;

            // Call the enhanced pin hit sound method with volume adjustment
            sfxManager.PlayPinHitSound(transform.position, normalizedForce * volumeModifier, materialType);
        }
    }
}