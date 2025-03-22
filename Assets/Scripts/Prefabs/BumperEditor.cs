#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

// Custom editor for the bumper Component
// Provides easy config and presets for bumper properties

[CustomEditor(typeof(Bumper))]
public class BumperEditor : Editor
{
    // Serialized properties for the bumper component
    // Bumper properties
    SerializedProperty bounceForceProperty;
    SerializedProperty minImpactVelocityProperty;
    SerializedProperty cooldownTimeProperty;

    // Visual Feedback
    SerializedProperty bumperAnimatorProperty;
    SerializedProperty bumperLightProperty;
    SerializedProperty bumperParticlesProperty;

    // Called when the editor is enabled
    private void OnEnable()
    {
        // Get references to all serialized properties
        bounceForceProperty = serializedObject.FindProperty("bounceForce");
        minImpactVelocityProperty = serializedObject.FindProperty("minImpactVelocity");
        cooldownTimeProperty = serializedObject.FindProperty("cooldownTime");

        bumperAnimatorProperty = serializedObject.FindProperty("bumperAnimator");
        bumperLightProperty = serializedObject.FindProperty("bumperLight");
        bumperParticlesProperty = serializedObject.FindProperty("bumperParticles");
    }

    public override void OnInspectorGUI() 
    {
        serializedObject.Update();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Bumper Config", EditorStyles.boldLabel);

        // Quick preset buttons
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Light Bumper"))
        {
            SetupBumperPreset(3.0f, 1.5f, 0.05f);
        }
        if (GUILayout.Button("Standard Bumper"))
        {
            SetupBumperPreset(5.0f, 2.0f, 0.1f);
        }
        if (GUILayout.Button("Heavy Bumper"))
        {
            SetupBumperPreset(8.0f, 3.0f, 0.2f);
        }
        EditorGUILayout.EndHorizontal();

        // Draw properties with categories
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Bumper Properties", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(bounceForceProperty, new GUIContent("Bounce Force", "Force applied to the ball on collision"));
        EditorGUILayout.PropertyField(minImpactVelocityProperty, new GUIContent("Min Impact Velocity", "Minimum velocity required to activate the bumper"));
        EditorGUILayout.PropertyField(cooldownTimeProperty, new GUIContent("Cooldown Time", "Time before the bumper can be activated again"));

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Visual Feedback", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(bumperAnimatorProperty);
        EditorGUILayout.PropertyField(bumperLightProperty);
        EditorGUILayout.PropertyField(bumperParticlesProperty);

        // Add helpful information
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("Bumpers provide force to deflect balls. Higher bounce force creates stronger rebounds, while minimum impact velocity prevents weak collisions from activating the bumper.", MessageType.Info);
    
        serializedObject.ApplyModifiedProperties();
    }

    private void SetupBumperPreset(float bounceForce, float minImpactVelocity, float cooldownTime)
    {
        bounceForceProperty.floatValue = bounceForce;
        minImpactVelocityProperty.floatValue = minImpactVelocity;
        cooldownTimeProperty.floatValue = cooldownTime;
        serializedObject.ApplyModifiedProperties();
    }

    // Add a scene GUI for visualizing impact direction and force
    void OnSceneGUI()
    {
        Bumper bumper = (Bumper)target;

        // Display bounce radius visualization
        Handles.color = new Color(1, 0, 0, 0.2f);
        Handles.DrawSolidDisc(bumper.transform.position, Vector3.forward, bounceForceProperty.floatValue / 5f);

        // Show force direction arrows
        Handles.color = new Color(1, 0.5f, 0, 0.8f);
        Vector3[] directions = new Vector3[]
        {
            Vector3.up,
            Vector3.down,
            Vector3.left,
            Vector3.right,
            (Vector3.up + Vector3.right).normalized,
            (Vector3.up + Vector3.left).normalized,
            (Vector3.down + Vector3.right).normalized,
            (Vector3.down + Vector3.left).normalized
        };

        foreach (Vector3 dir in directions)
        {
            Vector3 start = bumper.transform.position;
            Vector3 end = start + dir * (bounceForceProperty.floatValue / 5f);

            Handles.DrawLine(start, end);
            Handles.DrawSolidDisc(end, Vector3.forward, 0.05f);
        }
    }
}
#endif