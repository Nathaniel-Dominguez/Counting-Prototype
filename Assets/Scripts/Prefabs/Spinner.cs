using UnityEngine;
using System.Collections;

public class Spinner : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField] private float maxRotationSpeed = 360f;
    [SerializeField] private float spinRateDecay = 0.5f;
    
    [Header("Physics Settings")]
    [SerializeField] private float spinnerFriction = 0.5f;
    [SerializeField] private float ballSpinTorque = 2.0f;
    [SerializeField] private float minImpactVelocity = 0.5f;
    [SerializeField] private float hitImpulseMultiplier = 10f;
    [SerializeField] private float bounceForce = 3.0f;
    
    [Header("Visual Feedback")]
    [SerializeField] private float hitScaleDuration = 0.1f;
    [SerializeField] private float hitScaleFactor = 1.1f;

    // Private variables
    private SFXManager sfxManager;
    private float currentRotationSpeed = 0f;
    private Vector3 originalScale;
    private bool isScaling = false;
    private Quaternion initialRotation;
    private float totalRotationAngle = 0f;

    // The rotation axis is always Z (forward) for this simplified spinner
    private readonly Vector3 rotationAxis = Vector3.forward;
    private void Awake()
    {
        // Initialize variables 
        // Cache the initial state
        initialRotation = transform.rotation;
        originalScale = transform.localScale;
        
        // Configure rigidbody for Z-axis rotation only
        ConfigureRigidbody();
    }

    private void Start()
    {
        // Get reference to the SFXManager
        sfxManager = SFXManager.Instance;

        // Validate the SFX Manager
        if (sfxManager == null)
        {
            Debug.LogWarning("Spinner: SFXManager.Instance is null. Audio features will be disabled.");
        }
    }

    private void OnEnable()
    {
        // Reset rotation tracking
        initialRotation = transform.rotation; 
        totalRotationAngle = 0f;
        currentRotationSpeed = 0f;
    }

    // Update is called once per frame
    private void Update()
    {
        // Instead of transform.rotate use Quaternion Rotation. When a rotation 180 normally it flips to -180, causing the breaking of the spinner we saw
        
        // Handle rotation using quaternions to avoid Euler angle discontinuities
        if (Mathf.Abs(currentRotationSpeed) > 0.1f)
        {
            // Calculate rotation amount this frame
            float rotationAmount = currentRotationSpeed * Time.deltaTime;

            // Update our continuous rotation tracker
            totalRotationAngle += rotationAmount;

            // Apply rotation using the initial rotation as a baseline
            transform.rotation = initialRotation * Quaternion.AngleAxis(totalRotationAngle, rotationAxis);

            // Apply decay to rotation speed
            float speedReduction = currentRotationSpeed * spinRateDecay * Time.deltaTime;
            currentRotationSpeed -= speedReduction;
        }
        else if (Mathf.Abs(currentRotationSpeed) <= 0.1f && currentRotationSpeed != 0f)
        {
            currentRotationSpeed = 0f;
        }
    }

    // Configure the spinner rotation axes
    private void ConfigureRigidbody()
    {
        Rigidbody spinnerRb = GetComponent<Rigidbody>();
        if (spinnerRb != null)
        {
            // Set core properties
            spinnerRb.useGravity = false;
            spinnerRb.interpolation = RigidbodyInterpolation.Interpolate;
            spinnerRb.collisionDetectionMode = CollisionDetectionMode.Discrete;

            // Freeze all position axes
            spinnerRb.constraints = spinnerRb.constraints | 
                                    RigidbodyConstraints.FreezePosition  |
                                    RigidbodyConstraints.FreezeRotationX | 
                                    RigidbodyConstraints.FreezeRotationY;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Only interact with balls above minimum velocity
        if (!collision.gameObject.CompareTag("Ball") || collision.relativeVelocity.magnitude < minImpactVelocity)
        {
            return;
        }

        Debug.Log("Spinner: OnCollisionEnter with Ball");

        // Calculate impact point and normal
        Vector3 impactPoint = collision.contacts[0].point;
        Vector3 impactNormal = collision.contacts[0].normal;
        float impactForce = collision.relativeVelocity.magnitude;

        // Vector from spinner center to impact point
        Vector3 spinnerToImpact = impactPoint - transform.position;

        // Project this vector onto the XY plane (since we're rotating around Z)
        Vector3 impactOnPlane = new Vector3(spinnerToImpact.x, spinnerToImpact.y, 0).normalized;

        // Calculate tangential direction of impact (perpendicular to radius vector on XY plane)
        Vector3 tangent = new Vector3(-impactOnPlane.y, impactOnPlane.x, 0);

        // Calculate how much force is applied in the tangential direction
        float tangentialForce = Vector3.Dot(collision.relativeVelocity, tangent) * impactForce;

        // Apply impulse to spinner's rotation based on tangential force
        float rotationImpulse = tangentialForce * hitImpulseMultiplier;
        currentRotationSpeed += rotationImpulse;

        // Clamp to max speed
        currentRotationSpeed = Mathf.Clamp(currentRotationSpeed, -maxRotationSpeed, maxRotationSpeed);

        // Apply bounce force to the ball
        ApplyPhysicsEffects(collision);

        // Visual feedback
        if (!isScaling)
        {
            StartCoroutine(ScalePulseRoutine()); 
        }

        // Play spinner sound through the AudioManager
        if (sfxManager != null)
        {
            // Calculate normalized rotation speed for audio effects (0-1 range)
            float normalizedSpeed = Mathf.Abs(currentRotationSpeed) / maxRotationSpeed;

            // Use playSFX 
            sfxManager.PlaySpinnerSound(transform.position, normalizedSpeed);
        }
        else
        {
            Debug.LogWarning("Spinner: sfxManager is null, cannot play sound");
        }
    }

    private void ApplyPhysicsEffects(Collision collision)
    {
        Rigidbody ballRb = collision.gameObject.GetComponent<Rigidbody>();
        if (ballRb == null) return;

        // Calculate contact information
        Vector3 contactPoint = collision.contacts[0].point;
        Vector3 contactNormal = collision.contacts[0].normal;

        // Calculate direction from spinner center to contact
        Vector3 spinDirection = Vector3.Cross(rotationAxis, (contactPoint - transform.position).normalized).normalized;

        // Apply some friction to slow the ball slightly
        ballRb.linearVelocity *= (1f - spinnerFriction * Time.fixedDeltaTime);

        // Apply Torque for ball spin effect
        if (ballSpinTorque > 0)
        {
            ballRb.AddTorque(spinDirection * ballSpinTorque * Mathf.Sign(currentRotationSpeed), ForceMode.Impulse);
        }

        // Add force in direction of rotation
        float spinForce = Mathf.Abs(currentRotationSpeed) / 100f;
        ballRb.AddForce(spinDirection * spinForce * bounceForce, ForceMode.Impulse);

        // Add some bounce away from spinner
        ballRb.AddForce(contactNormal * bounceForce * 0.5f, ForceMode.Impulse);
    }

    private IEnumerator ScalePulseRoutine()
    {
        isScaling = true;

        // Scale up quickly
        float elapsed = 0f;
        float upDuration = hitScaleDuration * 0.3f;
        Vector3 targetScale = originalScale * hitScaleFactor;

        while (elapsed < upDuration)
        {
            // (t) represents time var
            float t = elapsed / upDuration;
            transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Scale down more gradually
        elapsed = 0f;
        float downDuration = hitScaleDuration * 0.7f;

        while (elapsed < downDuration)
        {
            float t = elapsed / downDuration;
            transform.localScale = Vector3.Lerp(targetScale, originalScale, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localScale = originalScale;
        isScaling = false;
    }

        // Public methods for external control if needed
    void ApplyManualSpin(float force)
    {
        currentRotationSpeed += force;
        currentRotationSpeed = Mathf.Clamp(currentRotationSpeed, -maxRotationSpeed, maxRotationSpeed);

        // Play sound when manually spinning
        if (sfxManager != null && Mathf.Abs(force) > 0.1f)
        {
            float normalizedSpeed = Mathf.Abs(currentRotationSpeed) / maxRotationSpeed;
            sfxManager.PlaySFX("Spinner", normalizedSpeed);
        }
    }

    void ResetSpin()
    {
        currentRotationSpeed = 0f;
    }

    void OnDisable()
    {
        // Ensure the spinner sound stops when disabled
        if (sfxManager != null)
        {
            sfxManager.PlaySpinnerSound(transform.position, 0f);
        }
    }
}