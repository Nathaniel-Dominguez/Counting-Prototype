#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Spinner))]
public class SpinnerEditor : Editor
{
    // Serialized properties
    // Rotation 
    SerializedProperty maxRotationSpeedProp;
    SerializedProperty spinRateDecayProp;

    // Physics 
    SerializedProperty spinnerFrictionProp;
    SerializedProperty ballSpinTorqueProp;
    SerializedProperty minImpactVelocityProp;
    SerializedProperty bounceForceProp;
    SerializedProperty hitImpulseMultiplierProp;

    // Visual Feedback
    SerializedProperty hitScaleDurationProp;
    SerializedProperty hitScaleFactorProp;
    SerializedProperty spinnerParticlesProp;

    private void OnEnable()
    {
        // Get seriealized properties
        maxRotationSpeedProp = serializedObject.FindProperty("maxRotationSpeed");
        spinRateDecayProp = serializedObject.FindProperty("spinRateDecay");

        spinnerFrictionProp = serializedObject.FindProperty("spinnerFriction");
        ballSpinTorqueProp = serializedObject.FindProperty("ballSpinTorque");
        minImpactVelocityProp = serializedObject.FindProperty("minImpactVelocity");
        bounceForceProp = serializedObject.FindProperty("bounceForce");
        hitImpulseMultiplierProp = serializedObject.FindProperty("hitImpulseMultiplier");

        hitScaleDurationProp = serializedObject.FindProperty("hitScaleDuration");
        hitScaleFactorProp = serializedObject.FindProperty("hitScaleFactor");
        spinnerParticlesProp = serializedObject.FindProperty("spinnerParticles");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Spinner Config", EditorStyles.boldLabel);

        // Add note about Z-axis rotation
        EditorGUILayout.HelpBox("Spinner rotates exclusively around the z axis (forward)", MessageType.Info);

        // Quick preset buttons
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Fast Spinner"))
        {
            SetupSpinnerPreset(500f, 0.3f, 0.5f, 2.0f, 3.0f, 10f);
        }
        if (GUILayout.Button("Standard"))
        {
            SetupSpinnerPreset(360f, 0.5f, 0.5f, 2.0f, 3.0f, 15f);
        }
        if (GUILayout.Button("Heavy Spinner"))
        {
            SetupSpinnerPreset(200f, 0.2f, 0.7f, 3.0f, 5.0f, 15f);
        }
        EditorGUILayout.EndHorizontal();

        // Draw properties grouped by category
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Rotation Settings", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(maxRotationSpeedProp, new GUIContent("Max Rotation Speed", "Maximum speed at which the spinner can rotate (degrees/second)"));
        EditorGUILayout.PropertyField(spinRateDecayProp, new GUIContent("Spin Rate Decay", "How quickly the spinner slows down (higher = faster slowdown)"));

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Physics Settings", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(spinnerFrictionProp, new GUIContent("Spinner Friction", "How much the spinner slows down balls on contact"));
        EditorGUILayout.PropertyField(ballSpinTorqueProp, new GUIContent("Ball Spin Torque", "Rotational force applied to balls"));
        EditorGUILayout.PropertyField(minImpactVelocityProp, new GUIContent("Min Impact Velocity", "Minimum ball velocity needed to affect the spinner"));
        EditorGUILayout.PropertyField(bounceForceProp, new GUIContent("Bounce Force", "How strongly balls bounce off the spinner"));
        EditorGUILayout.PropertyField(hitImpulseMultiplierProp, new GUIContent("Hit Impulse Multiplier", "How strongly impacts affect spinner rotation"));

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Visual Feedback", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(hitScaleDurationProp, new GUIContent("Hit Scale Duration", "Time for the scale effect when hit"));
        EditorGUILayout.PropertyField(hitScaleFactorProp, new GUIContent("Hit Scale Factor", "Size multiplier when spinner is hit"));
        EditorGUILayout.PropertyField(spinnerParticlesProp, new GUIContent("Spinner Particles", "Particles to play when spinner is hit"));

        serializedObject.ApplyModifiedProperties();
    }

    private void SetupSpinnerPreset(float maxSpeed, float decayRate, float friction, float torque, float bounce, float impulse)
    {
        maxRotationSpeedProp.floatValue = maxSpeed;
        spinRateDecayProp.floatValue = decayRate;
        spinnerFrictionProp.floatValue = friction;
        ballSpinTorqueProp.floatValue = torque;
        bounceForceProp.floatValue = bounce;
        hitImpulseMultiplierProp.floatValue = impulse;
        serializedObject.ApplyModifiedProperties();
    }

    // Add scene view visualization
    private void OnSceneGui()
    {
        Spinner spinner = (Spinner)target;
        Transform spinnerTransform = spinner.transform;

        // Draw rotation axis indicator
        Handles.color = Color.blue;
        Vector3 start = spinnerTransform.position;
        Vector3 end = start + spinnerTransform.forward * 0.5f;
        Handles.DrawLine(start, end);
        Handles.DrawSolidDisc(end, spinnerTransform.forward, 0.05f);

        // Show rotation direction based on current speed
        if (Application.isPlaying)
        {
            // Use reflection to get private field value
            System.Reflection.FieldInfo speedField = typeof(Spinner).GetField("currentRotationSpeed", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            if (speedField != null)
            {
                float speed = (float)speedField.GetValue(spinner);

                if (Mathf.Abs(speed) > 0.1f)
                {
                    Handles.color = speed > 0 ? Color.green : Color.red;
                    DrawRotationDirection(spinnerTransform, speed > 0);
                }
            }
        }
    }

    private void DrawRotationDirection(Transform transform, bool clockwise)
    {
        Vector3 center = transform.position;
        float radius = 0.4f;

        // Draw arc to indicate rotation direction
        Vector3 startDir = transform.right;
        Vector3 endDir = clockwise ? -transform.up : transform.up;

        Handles.DrawWireArc(center, transform.forward, startDir, clockwise ? -90f : 90f, radius);

        // Draw arrowhead
        Vector3 arrowPos = center + endDir * radius;
        Vector3 arrowDir = clockwise ? -transform.right : transform.right;
        Vector3 arrowLeft = arrowPos + (clockwise ? transform.up : -transform.up) * 0.1f - arrowDir * 0.1f;
        Vector3 arrowRight = arrowPos + (clockwise ? -transform.up : transform.up) * 0.1f - arrowDir * 0.1f;

        Handles.DrawLine(arrowPos, arrowLeft);
        Handles.DrawLine(arrowPos, arrowRight);
    }
}
#endif