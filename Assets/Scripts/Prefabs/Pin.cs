using UnityEngine;
using System.Collections;

// Controls the behavior of pins in the pachinko machine
// Pins are passive obstacles that deflect the ball slightly
public class Pin : MonoBehaviour
{
    // Private fields
    private SFXManager sfxManager;
    private float lastSoundTime = 0.1f;
    private float soundCooldown = 0.2f;

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
        
        // Calculate impact force (clamped to reasonable range)
        float impactForce = Mathf.Clamp(collision.relativeVelocity.magnitude, 0f, 5f);
            
        // Prevent sound spam with cooldown
        if (Time.time - lastSoundTime < soundCooldown)
            return;
            
        lastSoundTime = Time.time;

        // Play pin sound through SFXManager
        if (sfxManager != null)
        {
            // normalize impact for audio (0-1 range)
            float normalizedForce = impactForce / 5f;
            
            // Delegate all sound logic to SFXManager
            sfxManager.PlayPinHitSound(normalizedForce);
        }
    }
}