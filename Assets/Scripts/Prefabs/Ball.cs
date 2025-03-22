using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Add this component to your ball prefab to track which score pocket it is in
/// and properly reset when returned to the pool
/// </summary>

[RequireComponent(typeof(Rigidbody))]
public class Ball : MonoBehaviour
{
    // List of score pockets this ball is currently inside
    private List<ScorePocket> containingPockets = new List<ScorePocket>();

    // Component references
    private Rigidbody ballRb;
    private Renderer ballRenderer;

    private void Awake() {
        ballRb = GetComponent<Rigidbody>();
        ballRenderer = GetComponent<Renderer>();

        // Initialize the list
        containingPockets = new List<ScorePocket>();
    }

    /// <summary>
    /// Register this ball with a score pocket
    /// </summary>
    public void RegisterWithPocket(ScorePocket pocket)
    {
        if (!containingPockets.Contains(pocket))
        {
            containingPockets.Add(pocket);
        }
    }

    /// <summary>
    /// Unregister this ball from a score pocket
    /// </summary>
    public void UnregisterFromPocket(ScorePocket pocket)
    {
        containingPockets.Remove(pocket);
    }

    /// <summary>
    /// Reset this ball's state when it returns to the pool
    /// </summary>
    public void ResetForPool()
    {
        // Reset physics state
        if (ballRb != null)
        {
            ballRb.linearVelocity = Vector3.zero;
            ballRb.angularVelocity = Vector3.zero;
            ballRb.isKinematic = false;
        }

        // Reset any visual effects (like fading)
        if (ballRenderer != null)
        {
            Material mat = ballRenderer.material;
            Color color = mat.color;
            color.a = 1.0f; // Reset to full opacity
            mat.color = color;
        }

        // Notify all pockets that still contain this ball
        for (int i = containingPockets.Count -1; i >= 0; i--)
        {
            ScorePocket pocket = containingPockets[i];
            if (pocket != null)
            {
                pocket.OnBallReturnedToPool(gameObject);
            }
        }

        // Clear the tracking list
        containingPockets.Clear();
    }

    /// <summary>
    /// Called when the ball is disabled (which happens when returned to pool)
    /// </summary>
    
    private void OnDisable()
    {
        // Safety cleanup in case ResetForPool wasn't called
        containingPockets.Clear();
    }
}