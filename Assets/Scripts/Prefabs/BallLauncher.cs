using UnityEngine;
using System.Collections;

public class BallLauncher : MonoBehaviour
{
    // Ball Launcher Variables
    [SerializeField] private Transform launchPoint;
    [SerializeField] private float minLaunchForce = 5f;
    [SerializeField] private float maxLaunchForce = 15f;
    [SerializeField] private Animation launcherAnimation;
    [SerializeField] private float chargeRate = 10f;

    private int ballsRemaining;
    private float currentLaunchForce;
    private bool isCharging = false;
    private SFXManager sfxManager;

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
    }

    // Public method for external control (called by Player)
    public void StartCharging()
    {
        // Don't allow charging if no balls remain or game is over
        if (ballsRemaining <= 0 || GameManager.Instance.IsGameOver())
        {
            return;
        }

        isCharging = true;
        currentLaunchForce = minLaunchForce;
    }

    // Public method for external control (called by Player)
    public void LaunchBall()
    {
        // Don't allow launching if no balls remain or game is over
        if (ballsRemaining <= 0 || GameManager.Instance.IsGameOver())
        {
            isCharging = false;
            return;
        }

        isCharging = false;

        // Play the launcher animation
        if (launcherAnimation != null)
        {
            launcherAnimation.Play("Launch");
        }

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

    private void UpdateUI()
    {
        // Update UI to show remaining balls
        GameManager.Instance.UpdateBallsRemainingText(ballsRemaining);
    }

    private void UpdateLaunchPowerUI()
    {
        // Update power meter UI
        float powerPercentage = (currentLaunchForce - minLaunchForce) / (maxLaunchForce - minLaunchForce);
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
        currentLaunchForce = minLaunchForce;
        UpdateUI();
    }
}
