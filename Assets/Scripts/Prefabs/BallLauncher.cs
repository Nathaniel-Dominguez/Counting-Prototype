using UnityEngine;
using System.Collections;
using TMPro;

public class BallLauncher : MonoBehaviour
{
    // Ball Launcher Variables
    [SerializeField] private Transform launchPoint;
    [SerializeField] private float minLaunchForce = 5f;
    [SerializeField] private float maxLaunchForce = 15f;
    [SerializeField] private float chargeRate = 10f;
    [SerializeField] private AnimationCurve powerCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private float cooldownTime = 1f; // Time in seconds before the next launch can occur

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI ballsRemainingText;
    [SerializeField] private int startingBalls = 200;

    private int ballsRemaining;
    private float currentLaunchForce;
    private bool isCharging = false;
    private bool isOnCooldown = false; // Track cooldown state
    private float remainingCooldownTime = 0f; // Track remaining cooldown time
    private SFXManager sfxManager;

    // Add getter methods for min and max launch force
    public float GetCurrentLaunchForce() { return currentLaunchForce; }
    public float GetMinLaunchForce() { return minLaunchForce; }
    public float GetMaxLaunchForce() { return maxLaunchForce; }

    // Add this getter method to access cooldown time
    public bool IsCoolingDown() { return isOnCooldown; }
    public float GetCooldownTime() { return cooldownTime; }
    public float GetRemainingCooldownTime() { return remainingCooldownTime; }

    // Add this getter method to access balls remaining
    public int GetBallsRemaining()
    {
        return ballsRemaining;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        // Get reference to the SFX Manager
        sfxManager = SFXManager.Instance;

        // Validate the SFX Manager
        if (sfxManager == null)
        {
            Debug.LogWarning("BallLauncher: SFXManager.Instance is null. SFX features will be disabled.");
        }

        // Initialize with starting balls
        ResetLauncher(startingBalls);
    }

    // Update is called once per frame
    private void Update()
    {
        // Only handle the charging effect if we're currently charging
        if (isCharging)
        {
            // Increase launch force while the mouse button is held down
            currentLaunchForce = Mathf.Min(currentLaunchForce + Time.deltaTime * chargeRate, maxLaunchForce);

            // Update UI to show current power
            UpdateLaunchPowerUI();
        }
        
        // Update remaining cooldown time if on cooldown
        if (isOnCooldown && remainingCooldownTime > 0)
        {
            remainingCooldownTime -= Time.deltaTime;
            if (remainingCooldownTime <= 0)
            {
                // Ensure we don't go below zero
                remainingCooldownTime = 0;
            }
        }
    }

    // Public method for external control (called by Player)
    public void StartCharging()
    {
        // Don't allow charging if no balls remain or game is over or cooldown is active
        if (ballsRemaining <= 0 || GameManager.Instance.IsGameOver() || isOnCooldown)
        {
            return;
        }

        // Explicitly set charging state first
        isCharging = true;
        
        // Reset launch force to minimum value
        currentLaunchForce = minLaunchForce;

        // Make sure to update the UI immediately with the reset value
        UpdateLaunchPowerUI();
        
        // Force refresh the power meter text and ensure it's displayed
        GameManager.Instance.ForceRefreshPowerMeter();
        
        // Add debug logging
        Debug.Log($"BallLauncher: StartCharging - Force reset to {currentLaunchForce}, min: {minLaunchForce}, max: {maxLaunchForce}");
    }

    // Public method for external control (called by Player)
    public void LaunchBall()
    {
        // Don't allow launching if no balls remain or game is over or cooldown is active
        if (ballsRemaining <= 0 || GameManager.Instance.IsGameOver() || isOnCooldown)
        {
            isCharging = false;
            return;
        }

        isCharging = false;

        // Start cooldown
        StartCoroutine(StartCooldown());
        
        // Notify cooldown UI that we're starting cooldown
        GameManager.Instance.ResetCooldownBar();

        // Force an update of the power UI to ensure text is updated with the final value
        UpdateLaunchPowerUI();
        // Force refresh the power meter text
        GameManager.Instance.ForceRefreshPowerMeter();

        // Validate launchPoint before using it
        if (launchPoint == null)
        {
            Debug.LogError("BallLauncher: launchPoint reference is missing!");
            return;
        }

        // Get a ball from the object pool
        GameObject ball = BallPoolManager.Instance.GetBall();

        if (ball != null)
        {
            // Position the ball at the launch point
            ball.transform.position = launchPoint.position;
            ball.transform.rotation = Quaternion.identity;

            // Get the Rigidbody component and apply force in the forward direction
            Rigidbody ballRb = ball.GetComponent<Rigidbody>();
            if (ballRb != null)
            {
                ballRb.linearVelocity = Vector3.zero;
                ballRb.angularVelocity = Vector3.zero;

                // Ensure rigidbody is not kinematic
                ballRb.isKinematic = false;
            }

            ball.SetActive(true);

            // Wait for physics to register the new position
            StartCoroutine(DelayedLaunchForce(ball, currentLaunchForce));

            ballsRemaining--;
            UpdateUI();
            
            // Let GameManager know to check if game should end
            GameManager.Instance.CheckGameOver();
        }
    }

    // Cooldown coroutine
    private IEnumerator StartCooldown()
    {
        isOnCooldown = true;
        remainingCooldownTime = cooldownTime;
        Debug.Log($"BallLauncher: Cooldown started, duration: {cooldownTime}s");

        // Wait for cooldown to complete
        yield return new WaitForSeconds(cooldownTime);

        isOnCooldown = false;
        remainingCooldownTime = 0f;
        Debug.Log("BallLauncher: Cooldown ended, launcher ready");
        
        // Force a UI update to ensure power meter is ready for next charge
        UpdateLaunchPowerUI();
        GameManager.Instance.ForceRefreshPowerMeter();
        
        // Notify the cooldown UI that cooldown is complete
        GameManager.Instance.SetCooldownReady();
    }

    private void UpdateUI()
    {
        // Update UI to show remaining balls
        if (ballsRemainingText != null)
        {
            ballsRemainingText.text = "Balls: " + ballsRemaining.ToString();
        }
    }

    private void UpdateLaunchPowerUI()
    {
        // Calculate raw percentage
        float rawPercentage = (currentLaunchForce - minLaunchForce) / (maxLaunchForce - minLaunchForce);

        // Apply the curve to create a more natural feel
        float powerPercentage = powerCurve.Evaluate(rawPercentage);

        // Update the power meter UI
        GameManager.Instance.UpdatePowerMeter(powerPercentage);
    }

    // New coroutine to apply force after physics update
    private IEnumerator DelayedLaunchForce(GameObject ball, float force)
    {
        // Wait for next physics update to ensure position is registered
        yield return new WaitForFixedUpdate();

        if (ball != null && ball.activeInHierarchy)
        {
            Rigidbody ballRb = ball.GetComponent<Rigidbody>();
            if (ballRb != null)
            {
                // Apply force in forward direction of the launch point, important to remember while positioning
                ballRb.AddForce(launchPoint.forward * force, ForceMode.Impulse);

                // Play the launch sound
                if (sfxManager != null)
                {
                    // Calculate normalized launch power (0-1)
                    float normalizedPower = Mathf.InverseLerp(minLaunchForce, maxLaunchForce, currentLaunchForce);
                    sfxManager.PlayBallLaunchSound(normalizedPower);
                }
            }
        }
    }

    // Add a method to reset the launcher (useful for game restart)
    public void ResetLauncher(int numBalls)
    {
        ballsRemaining = numBalls;
        isCharging = false;
        isOnCooldown = false;
        remainingCooldownTime = 0f;
        currentLaunchForce = minLaunchForce;
        UpdateUI();
    }

    // Add a method to add balls to the launcher
    public void AddBalls(int ballsToAdd)
    {
        if (ballsToAdd <= 0) return;
        
        ballsRemaining += ballsToAdd;
        UpdateUI();
        
        Debug.Log($"BallLauncher: Added {ballsToAdd} balls. Now have {ballsRemaining} balls.");
    }
}
